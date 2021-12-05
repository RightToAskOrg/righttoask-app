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
		private string stateEnum = "";
		public string display_name { get; set; } = "";
		public string public_key { get; set; }= "";
		public string state { get; set; }= "";

		public string StateEnum
		{
			get => stateEnum;
			set => stateEnum = value;
		}

		public string uid { get; set; }= "";

		private List<ElectorateWithChamber> electorates = new List<ElectorateWithChamber>();

		public List<ElectorateWithChamber> Electorates
		{
			get => electorates; 
			set => electorates = value;
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
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
