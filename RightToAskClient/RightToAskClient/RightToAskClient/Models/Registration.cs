using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RightToAskClient.Models
{
	public class Registration
	{
		public string display_name { get; set; } = "";
		public string public_key { get; set; }= "";
		public string state { get; set; }= "";
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
		public void AddElectorate(BackgroundElectorateAndMPData.Chamber chamberToAdd, string regionToAdd)
		{
			electorates.Add(new ElectorateWithChamber()
			{
				chamber = chamberToAdd,
				region = regionToAdd
			});
		}

		// TODO this would be a lot easier if electorates was a dictionary instead of a list of pairs
		public string CommonwealthElectorate()
		{
			var houseOfRepsElectoratePair = electorates.Find(chamberPair =>
				chamberPair.chamber == BackgroundElectorateAndMPData.Chamber.Australian_House_Of_Representatives);
			if (houseOfRepsElectoratePair is null)
			{
				return "";
			}

			return houseOfRepsElectoratePair.region;
		}
	}
}
