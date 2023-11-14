using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Maui.Models;
using RightToAskClient.Maui.Resx;

namespace RightToAskClient.Maui.Helpers
{
	public static class ParliamentaryURICreator
	{
		public static List<string> ValidParliamentaryDomains =
			new List<string>(((Enum.GetValues(typeof(ParliamentData.Chamber)) as IEnumerable<ParliamentData.Chamber>))
				.Select(c => ParliamentData.GetDomain(c)).Distinct());

		public static JOSResult<Uri> StringToValidParliamentaryUrl(string value)
		{
			if (!Uri.TryCreate(value, UriKind.Absolute, out Uri outUri))
			{
				// Not a valid url.
				return new ErrorResult<Uri>(AppResources.LinkNotWellFormedErrorText);
			}

			if (outUri.Scheme != Uri.UriSchemeHttps && outUri.Scheme != Uri.UriSchemeHttp)
			{
				// must start with http:// or https:// 
				return new ErrorResult<Uri>(AppResources.LinkHttpErrorText);
			}

			foreach (string domain in ValidParliamentaryDomains)
			{
				// Valid url, ends with one of the right domains
				if (outUri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
				{
					return new SuccessResult<Uri>(outUri);
				}
			}

			// Valid url, but not one of the allowed ones.
			return new ErrorResult<Uri>(AppResources.ParliamentaryURLErrorText);
		}
	}
} 