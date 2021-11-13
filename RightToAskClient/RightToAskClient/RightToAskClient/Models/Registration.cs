using System;
using System.Collections.Generic;
using System.Reflection;

namespace RightToAskClient.Models
{
	public class Registration
	{
		public string display_name { get; set; }
		public string public_key { get; set; }
		public string state { get; set; }
		public string uid { get; set; }

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

			if (errorFields.IsNullOrEmpty())
			{
				return new Result<bool>() { Ok = true };
			}
			return new Result<bool>()
			{
				Err ="Please complete "+String.Join(" and ",errorFields)
			};
		}
	}
}
