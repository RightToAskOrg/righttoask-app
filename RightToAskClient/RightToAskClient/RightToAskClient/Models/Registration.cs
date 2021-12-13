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
		// private string stateEnum = "";
		public string display_name { get; set; } = "";
		public string public_key { get; set; }= "";
		private string state { get; set; }= "";

		/* public string StateEnum
		{
			get => stateEnum;
			set => stateEnum = value;
		} */
		
		public string State
		{
			get => state;
			set => state = value;
		} 
		

		public string uid { get; set; }= "";

		public List<ElectorateWithChamber> electorates { get; set; } = new List<ElectorateWithChamber>();

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
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
