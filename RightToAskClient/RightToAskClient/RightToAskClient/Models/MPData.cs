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
	public class MPData 
	{
		private List<MP> allMPs = new List<MP>();
		private bool isInitialised = false;
		
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
            };
		public string ErrorMessage { get; private set; } = "";
		
		
		public ObservableCollection<MP> AllMPs  
		{
			get
			{
				if (!allMPs.IsNullOrEmpty())
				{
					return new ObservableCollection<MP>(allMPs);
				}

				return new ObservableCollection<MP>();
			}
		}

		// Find all the MPs representing a certain electorate.
		public List<MP> GetMPsRepresentingElectorate(ElectorateWithChamber queryElectorate)
		{
			var mps = allMPs.Where(mp => mp.electorate.chamber == queryElectorate.chamber
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
			var success = FileIO.readDataFromStoredJSON<List<MP>>(Constants.StoredMPDataFile, serializerOptions);
			if (!String.IsNullOrEmpty(success.Err))
			{
				return new Result<bool>() { Err = success.Err };
			}
			
			allMPs = success.Ok;
			isInitialised = true;
			return new Result<bool>() { Ok = true };
		}


		private async Task<Result<bool>> TryInitialisingFromServer()
		{
			Result<List<MP>> serverMPList = await RTAClient.GetMPsList();

			if (serverMPList is null)
			{
				return new Result<bool>() { Err = "Could not reach server."};
			}

			if (String.IsNullOrEmpty(serverMPList.Err))
			{
				allMPs = serverMPList.Ok;
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
