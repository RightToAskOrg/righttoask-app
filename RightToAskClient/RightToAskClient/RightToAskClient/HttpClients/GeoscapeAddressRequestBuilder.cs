using System;
using System.IO;
using System.Reflection;
using RightToAskClient.Models;

namespace RightToAskClient.HttpClients
{
	
    public static class GeoscapeAddressRequestBuilder
    {
		private static string aPIKeyFileName = "GeoscapeAPIKey";
		
        public static Result<string> ApiKey { get; } = ReadGeoscapeAPIKey();

        static GeoscapeAddressRequestBuilder()
        {
            
        }

        // TODO: It's probably not good practice to make
        // up my own query string. Figure out how to use
        // something like https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0
        // to build this elegantly.
        public static string BuildRequest(string address)
        {
	        return "?" + "address=" +address + 
	                               "&additionalProperties=commonwealthElectorate,stateElectorate,localGovernmentArea"; 
        }
        
        // TODO Note this is a bit of a copy of ParliamentData:ReadDataFromCSV.
        // Consider refactoring into an IO Utils file.
        private static Result<string> ReadGeoscapeAPIKey()
        {
			try
			{
				string key;
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + aPIKeyFileName);
				using (var sr = new StreamReader(stream))
				{
					key = sr.ReadLine() ?? string.Empty;
					if (!String.IsNullOrEmpty(key))
					{
						return new Result<string>() { Ok = key };
					}

				}
				
			}
			catch (IOException e)
			{
				Console.WriteLine("MP file could not be read: " + aPIKeyFileName);
				Console.WriteLine(e.Message);
			}
			return new Result<string>() { Err = "Error reading Geoscape API Key." };
		}
    }
}

