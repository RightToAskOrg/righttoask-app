using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.Models;

namespace RightToAskClient.Maui.HttpClients
{
	
    public static class GeoscapeAddressRequestBuilder
    {
		
        public static JOSResult<string> ApiKey { get; } = ReadGeoscapeApiKey();

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
        
        private static JOSResult<string> ReadGeoscapeApiKey()
        {
	        return FileIO.ReadFirstLineOfFileAsString(Constants.APIKeyFileName);
		}
    }
}

