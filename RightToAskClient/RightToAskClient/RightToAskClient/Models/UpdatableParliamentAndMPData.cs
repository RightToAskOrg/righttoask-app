using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RightToAskClient.Annotations;
using RightToAskClient.HttpClients;

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
		private UpdatableParliamentAndMPDataStructure allMPsData = new UpdatableParliamentAndMPDataStructure();

		public List<MP> AllMPs  
		{
			get
			{
				if (!allMPsData.mps.IsNullOrEmpty())
				{
					return new List<MP>(allMPsData.mps);
				}

				return new List<MP>();
			}
		}
		
		public List<RegionsContained> FederalElectoratesByState
		{
			get
			{
				return new List<RegionsContained>(allMPsData.FederalElectoratesByState);
			}
		}
		public List<RegionsContained> VicRegions
		{
			get
			{
				return allMPsData.VicRegions;
			}
		}

		private bool isInitialised = false;
		
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
            };
		public string ErrorMessage { get; private set; } = "";
		
		

		// Find all the MPs representing a certain electorate.
		public List<MP> GetMPsRepresentingElectorate(ElectorateWithChamber queryElectorate)
		{
			var mps = allMPsData.mps.Where(mp => mp.electorate.chamber == queryElectorate.chamber
			                             && mp.electorate.region.Equals(queryElectorate.region,
				                             StringComparison.OrdinalIgnoreCase));
			return new List<MP>(mps);
		}
		
		// Returns true if initialisation is successful, i.e. no errors.
		// or there are no changes since last time.
		public async void TryInit()
		{
			if (isInitialised) return;
			Result<bool> success;
			
			success = await TryInitialisingFromServer();
			if (String.IsNullOrEmpty(success.Err))
			{
				isInitialised = true;
				return;
			}
			
			ErrorMessage = success.Err;
			Debug.WriteLine(@"\tERROR {0}", success.Err);

			success = TryInitialisingFromStoredData();
			if (String.IsNullOrEmpty(success.Err))
			{
				isInitialised = true;
				return;
			}

			ErrorMessage += success.Err;
			Debug.WriteLine(@"\tERROR {0}", success.Err);
		}

		private Result<bool> TryInitialisingFromStoredData()
		{
			var success = FileIO.ReadDataFromStoredJson<UpdatableParliamentAndMPDataStructure>(Constants.StoredMPDataFile, serializerOptions);
			if (!String.IsNullOrEmpty(success.Err))
			{
				return new Result<bool>() { Err = success.Err };
			}
			
			allMPsData = success.Ok;
			isInitialised = true;
			return new Result<bool>() { Ok = true };
		}


		private async Task<Result<bool>> TryInitialisingFromServer()
		{
			Result<UpdatableParliamentAndMPDataStructure> serverMPList = await RTAClient.GetMPsData();

			if (serverMPList is null)
			{
				return new Result<bool>() { Err = "Could not reach server."};
			}

			if (String.IsNullOrEmpty(serverMPList.Err))
			{
				allMPsData = serverMPList.Ok;
				isInitialised = true;
				return new Result<bool>() { Ok = true };
			}

			return new Result<bool>() { Err = serverMPList.Err };
		}


		public bool IsInitialised
		{
			get
			{
				return isInitialised;
			}
		} 

		public List<string> ListElectoratesInChamber(ParliamentData.Chamber chamber)
		{
			return new List<string>(AllMPs.Where(mp => mp.electorate.chamber == chamber)
				.Select(mp => mp.electorate.region));
		}
		
		public List<ElectorateWithChamber> ListElectoratePairsInChamber(ParliamentData.Chamber chamber)
		{
			return new List<ElectorateWithChamber>(AllMPs.Where(mp => mp.electorate.chamber == chamber)
				.Select(mp => new ElectorateWithChamber(chamber, mp.electorate.region)));
		}
		
		
	}
}
