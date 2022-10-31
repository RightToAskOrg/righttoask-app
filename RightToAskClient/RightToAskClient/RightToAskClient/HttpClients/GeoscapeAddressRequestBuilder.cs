using RightToAskClient.Helpers;
using RightToAskClient.Models;

namespace RightToAskClient.HttpClients
{
	
    public static class GeoscapeAddressRequestBuilder
    {
		
        public static Result<string> ApiKey { get; } = ReadGeoscapeApiKey();

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
        
        private static Result<string> ReadGeoscapeApiKey()
        {
	        return FileIO.ReadFirstLineOfFileAsString(Constants.APIKeyFileName);
		}
    }
}

