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
		// TODO Update this to use stored data if the server is unavailable
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
			return readDataFromStoredJSON(Constants.StoredMPDataFile, serializerOptions);
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
		
		// TODO: Refactor better into an I/O utils file.
		private Result<bool> readDataFromStoredJSON(string filename, JsonSerializerOptions jsonSerializerOptions)
		{
			List<MP>? MPList;
			
			try
			{
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				using (var sr = new StreamReader(stream))
				{
					string? MPs = sr.ReadToEnd();
					MPList = JsonSerializer.Deserialize<List<MP>>(MPs, serializerOptions);
				}
			}
			catch (IOException e)
			{
				Console.WriteLine("File could not be read: " + filename);
				Console.WriteLine(e.Message);
				return new Result<bool>() { Err = e.Message };
			}
			catch (JsonException e)
			{
				Console.WriteLine("JSON could not be deserialised: " + filename);
				Console.WriteLine(e.Message);
				return new Result<bool>() { Err = e.Message };
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + filename);
				Console.WriteLine(e.Message);
				return new Result<bool>() { Err = e.Message };
			}

			if (MPList is null)
			{
				string error = "Error: Could not deserialize MP List";
				Console.WriteLine(error);
				return new Result<bool>() { Err = error};
			}

			allMPs = MPList;
			isInitialised = true;
			return new Result<bool>() { Ok = true };
			
		}
		
	}
}
