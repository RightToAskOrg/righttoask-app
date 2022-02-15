using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RightToAskClient.Annotations;

namespace RightToAskClient.Models
{
	public class Registration : INotifyPropertyChanged
	{
		// private string stateEnum = "";
		public string display_name { get; set; } = "";
		public string public_key { get; set; }= "";
		private string state { get; set; }= "";

		public string State
		{
			get => state;
			set => state = value;
		} 
		
		public string uid { get; set; }= "";

		public List<ElectorateWithChamber> electorates { get; private set; } = new List<ElectorateWithChamber>();

		/* Accept a new electorate and chamber, remove any earlier ones that are inconsistent.
		 * Note: this assumes that nobody is ever represented in two different regions in the one
		 * chamber. This is true throughout Aus, but may be untrue elsewhere. Of course, each region
		 * may have multiple representatives.
		 * Inserting it at the beginning rather than adding at the end is a bit of a hack to
		 * put the Senators last (they're computed first).
		 */
		public void AddElectorateRemoveDuplicates(ElectorateWithChamber newElectorate)
		{
				electorates.RemoveAll(e => e.chamber == newElectorate.chamber);
				electorates.Insert(0,newElectorate);
		}

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

		
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
