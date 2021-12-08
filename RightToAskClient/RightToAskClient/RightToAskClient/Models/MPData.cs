using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
		public string ErrorMessage { get; private set; } = "";
		public List<MP> AllMPs  
		{
			get
			{
				if (!allMPs.IsNullOrEmpty())
				{
					return new List<MP>(allMPs);
				}

				return new List<MP>();
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
		// TODO Update this to use stored data if the server is unavailable
		// or there are no changes since last time.
		public async void TryInit()
		{
			if (isInitialised) return;
			
			Result<bool> success = await tryInitialising();
			if (!String.IsNullOrEmpty(success.Err))
			{
				Debug.WriteLine(@"\tERROR {0}", success.Err);
				ErrorMessage = success.Err;
			}
			else
			{
				isInitialised = true;
			}
		}



		private async Task<Result<bool>> tryInitialising()
		{
			Result<List<MP>> serverMPList = await RTAClient.GetMPsList();

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
