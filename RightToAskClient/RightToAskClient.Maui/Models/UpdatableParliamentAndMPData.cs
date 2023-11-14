using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.HttpClients;
using RightToAskClient.Maui.Models.ServerCommsData;
using RightToAskClient.Maui.ViewModels;

namespace RightToAskClient.Maui.Models
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


		private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
            };
		public string ErrorMessage { get; private set; } = "";
		
		

		// Find all the MPs representing a certain electorate.
		public List<MP> GetMPsRepresentingElectorate(ElectorateWithChamber queryElectorate)
		{
			var mps = _allMPsData.mps?.Where(mp => mp?.electorate?.chamber == queryElectorate.chamber
			                             && mp.electorate.region.Equals(queryElectorate.region,
				                             StringComparison.OrdinalIgnoreCase));
			return new List<MP>(mps ?? new MP[]{});
		}
		
		// Returns true if initialisation is successful, i.e. no errors.
		// or there are no changes since last time.
		public async Task<bool> TryInit()
		{
			// get data from local first
			var localInitResult = TryInitialisingFromStoredData();

			// then try getting data from the server.
			// TODO I believe this makes it wait a long time. Consider *not* awaiting this call.
			// Also consider whether there's a slicker way of checking (e.g. with hashes) whether our current 
			// copy of the file matches the server's.
			var serverInitResult = await TryInitialisingFromServer();
			
			if (localInitResult.Success || serverInitResult.Success)
			{
				IsInitialised = true;
				QuestionViewModel.Instance.UpdateMPButtons();
			}

			ErrorMessage = "";

			// Log / set error message. We might have local init failure, server init failure, both or neither.
			if (localInitResult.Failure)
			{
				ErrorMessage += "Failure in local initialisation of MPs.json. ";
				if (localInitResult is ErrorResult errorResult)
				{
					ErrorMessage += errorResult.Message;
				}
			}
			if (serverInitResult.Failure)
			{
				ErrorMessage += "Failure in server initialisation of MPs.json. ";
				if (serverInitResult is ErrorResult errorResult)
				{
					ErrorMessage += errorResult.Message;
				}
			}
			Debug.WriteLine(ErrorMessage);
			FilterChoices.NeedToInitAllMPsLists(this);
			
			return IsInitialised;
		}

		private JOSResult TryInitialisingFromStoredData()
		{
			var readResult = FileIO.ReadDataFromStoredJson<UpdatableParliamentAndMPDataStructure>(Constants.StoredMPDataFile, serializerOptions);
			if (readResult.Failure)
			{
				if (readResult is ErrorResult<UpdatableParliamentAndMPDataStructure> errorResult)
				{
					return new ErrorResult(errorResult.Message);
				}
				// Currently never used; here in case other error types are added later.
				return new ErrorResult("Error reading MP data from file.");
			}
			
			// readResult.Success
			_allMPsData = readResult.Data;
			return new SuccessResult();
		}


		private async Task<JOSResult> TryInitialisingFromServer()
		{
			var serverMPList = await RTAClient.GetMPsData();

			if (serverMPList is null)
			{
				return new ErrorResult("Could not reach server.");
			}

			if (serverMPList.Success)
			{
				_allMPsData = serverMPList.Data;
				return new SuccessResult();
			}

			// serverMPList.Failure
			if (serverMPList is ErrorResult<UpdatableParliamentAndMPDataStructure> errorResult)
			{
				return new ErrorResult(errorResult.Message);
			}
			// Currently not necessary; added in case other error types come later.
			return new ErrorResult("Error initialising MPs from server.");

		}



		public List<string> ListElectoratesInChamber(ParliamentData.Chamber chamber)
		{
			return new List<string>(AllMPs.Where(mp => mp?.electorate?.chamber == chamber)
				.Select(mp => mp?.electorate?.region).Where(s=> !String.IsNullOrEmpty(s)).Distinct().OrderBy(r=>r));
		}
	}
}
