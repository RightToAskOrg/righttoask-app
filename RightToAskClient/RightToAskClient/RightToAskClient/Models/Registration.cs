using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RightToAskClient.Annotations;
using RightToAskClient.Models;

namespace RightToAskClient.Models
{
	public class Registration : INotifyPropertyChanged
	{
		public string display_name { get; set; } = "";
		public string public_key { get; set; }= "";
		public string state { get; set; }= "";
		public string stateEnum { get; set; } = "";
		public string uid { get; set; }= "";

		public List<ElectorateWithChamber> electorates { get; } =
			new List<ElectorateWithChamber>();

		public Result<bool> IsValid()
		{
			List<string> errorFields = new List<string>();
			
			foreach(PropertyInfo prop in typeof(Registration).GetProperties())
			{
				var value = prop.GetValue(this, null);
				if (value is null || String.IsNullOrWhiteSpace(value.ToString()))
				{
					errorFields.Add(prop.Name);
				}
			}

			if (errorFields.IsNullOrEmpty() || errorFields.SequenceEqual(new List<string>{"electorates"}))
			{
				return new Result<bool>() { Ok = true };
			}
			return new Result<bool>()
			{
				Err ="Please complete "+String.Join(" and ",errorFields)
			};
		}

		// TODO: Do some validity checking to ensure that you're not adding inconsistent
		// data, e.g. a second electorate for a given chamber, or a state different from
		// the expected state.
		public void AddElectorate(ParliamentData.Chamber chamberToAdd, string regionToAdd)
		{
			electorates.Add(new ElectorateWithChamber(chamberToAdd, regionToAdd));
		}

		// TODO at the moment it just assumes everyone is in Vic.	
		// TODO: Error/validity checking to make sure it's a valid electorate in that state's lower
		// House.
		public void AddStateLowerHouseElectorate(string state, string regionToAdd)
		{
			// TODO: Note this is not the most robust way of doing this - it'd be better to structure the data
			// sensibly so we knew what chambers were in which states.
			
			/* Redo error checking. 
			if (chamberToAdd.Count == 0)
			{
				Debug.WriteLine(@"\tERROR {0}", "Couldn't find a lower house chamber for " + state);
				return;
			}
			*/

			AddElectorate(ParliamentData.Chamber.Vic_Legislative_Assembly, regionToAdd);
		}
		

		// TODO this would be a lot easier if electorates was a dictionary instead of a list of pairs
		public string CommonwealthElectorate()
		{
			var houseOfRepsElectoratePair = electorates.Find(chamberPair =>
				chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
			if (houseOfRepsElectoratePair is null)
			{
				return "";
			}

			return houseOfRepsElectoratePair.region;
		}

		public string StateLowerHouseElectorate()
		{
			var electoratePair = electorates.Find(chamberPair =>
				ParliamentData.IsLowerHouseChamber(chamberPair.chamber));
			if (electoratePair is null)
			{
				return "";
			}

			return electoratePair.region;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
