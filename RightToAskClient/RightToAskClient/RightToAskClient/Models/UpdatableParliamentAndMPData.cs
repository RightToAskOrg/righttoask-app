using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.ViewModels;

namespace RightToAskClient.Models
{
	/* This class initialises all the data about MPs and Parliamentary structures that we
	 * get from the server, on the basis that it might change. It also tries initialising from
	 * a file if the server is unreachable.
	 * This means the MPs and their information, plus info about electorate names and which
	 * states or regions they're in.
	 * Currently, we only bother with Vic sub-regions, because although WA (at the time of
	 * writing) still has them, they are planning to abolish them.
	 * */
	public class UpdatableParliamentAndMPData
	{
		private UpdatableParliamentAndMPDataStructure _allMPsData
			= new UpdatableParliamentAndMPDataStructure()
			{
				mps = new MP[] { },
				FederalElectoratesByState = new RegionsContained[] { }
			};

		public bool IsInitialised { get; private set; }

		public List<MP> AllMPs  
		{
			get
			{
				if (!_allMPsData.mps.IsNullOrEmpty())
				{
					return new List<MP>(_allMPsData.mps ?? new MP[]{});
				}

				return new List<MP>();
			}
		}
		
		public List<RegionsContained> FederalElectoratesByState
		{
			get
			{
				return new List<RegionsContained>(_allMPsData.FederalElectoratesByState ?? new RegionsContained[]{});
			}
		}
		public List<RegionsContained> VicRegions => new List<RegionsContained>(_allMPsData.VicRegions ?? Array.Empty<RegionsContained>());


		private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
            };
		public string ErrorMessage { get; private set; } = "";
		
		

		// Find all the MPs representing a certain electorate.
		public List<MP> GetMPsRepresentingElectorate(ElectorateWithChamber queryElectorate)
		{
			var mps = _allMPsData.mps?.Where(mp => mp.electorate.chamber == queryElectorate.chamber
			                             && mp.electorate.region.Equals(queryElectorate.region,
				                             StringComparison.OrdinalIgnoreCase));
			return new List<MP>(mps ?? new MP[]{});
		}
		
		// Returns true if initialisation is successful, i.e. no errors.
		// or there are no changes since last time.
		// TODO This isn't very elegantly structured - redo so the init of other data structures
		// isn't repeated.
		public async Task<bool> TryInit()
		{
			if (IsInitialised) return true;
			Result<bool> success;

			// get data from local first
			success = TryInitialisingFromStoredData();
			if (string.IsNullOrEmpty(success.Err))
			{
				IsInitialised = true;
				QuestionViewModel.Instance.UpdateMPButtons(); 
				// App.ReadingContext.Filters.InitSelectableLists();
				//return;
			}

			// TODO I believe this makes it wait a long time. Consider *not* awaiting this call.
			success = await TryInitialisingFromServer();
			if (string.IsNullOrEmpty(success.Err))
			{
				IsInitialised = true;
				QuestionViewModel.Instance.UpdateMPButtons();
				// App.ReadingContext.Filters.InitSelectableLists();
				return true;
			}
			
			ErrorMessage = success.Err ?? "";
			Debug.WriteLine(@"\tERROR {0}", success.Err);

			success = TryInitialisingFromStoredData();
			if (string.IsNullOrEmpty(success.Err))
			{
				IsInitialised = true;
				QuestionViewModel.Instance.UpdateMPButtons();
				// App.ReadingContext.Filters.InitSelectableLists();
				return true;
			}

			ErrorMessage += success.Err;
			Debug.WriteLine(@"\tERROR {0}", success.Err);
			return false;
		}

		private Result<bool> TryInitialisingFromStoredData()
		{
			var success = FileIO.ReadDataFromStoredJson<UpdatableParliamentAndMPDataStructure>(Constants.StoredMPDataFile, serializerOptions);
			if (!string.IsNullOrEmpty(success.Err))
			{
				return new Result<bool>() { Err = success.Err };
			}
			
			_allMPsData = success.Ok;
			IsInitialised = true;
			// TODO this seem to be called twice. Prob don't need both.
			QuestionViewModel.Instance.UpdateMPButtons();
			return new Result<bool>() { Ok = true };
		}


		private async Task<Result<bool>> TryInitialisingFromServer()
		{
			var serverMPList = await RTAClient.GetMPsData();

			if (serverMPList is null)
			{
				return new Result<bool>() { Err = "Could not reach server."};
			}

			if (string.IsNullOrEmpty(serverMPList.Err))
			{
				_allMPsData = serverMPList.Ok;
				IsInitialised = true;
				QuestionViewModel.Instance.UpdateMPButtons();
				return new Result<bool>() { Ok = true };
			}

			return new Result<bool>() { Err = serverMPList.Err };
		}



		public List<string> ListElectoratesInChamber(ParliamentData.Chamber chamber)
		{
			return new List<string>(AllMPs.Where(mp => mp.electorate.chamber == chamber)
				.Select(mp => mp.electorate.region).Distinct().OrderBy(r=>r));
		}
	}
}
