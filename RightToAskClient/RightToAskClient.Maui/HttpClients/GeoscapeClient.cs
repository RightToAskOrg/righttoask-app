using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Maui.Models;

namespace RightToAskClient.Maui.HttpClients
{
    /* A single static http client, set up to talk to Geoscape.
     */
    public static class GeoscapeClient
    {
        private static readonly JsonSerializerOptions SerializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
                // TODO I *think* this is the right thing to use for deserialising addresses
                // with possibly absent elements.
                // DefaultIgnoreCondition = JsonIgnoreCondition.Always
            };
        
        private static readonly GenericHttpClient Client = new GenericHttpClient(SerializerOptions);

        public static async Task<JOSResult<GeoscapeAddressFeature>> GetFirstAddressData(string address)
        {
            var collection = await GetAddressDataAsync(address);

            if (collection.Failure)
            {
                var errorMessage = "Couldn't get first address.";
                if (collection is ErrorResult<GeoscapeAddressFeatureCollection> errorResult)
                {
                    errorMessage = errorResult.Message;
                }
                return new ErrorResult<GeoscapeAddressFeature>(errorMessage);
            }

            // collection.Success
            var addresses = collection.Data.AddressDataList;
            
            // I *think* this should never happen, because there should be a message if the
            // address list is empty, but better safe than sorry.
            if(addresses is null || addresses.Length == 0)
            {
                return new ErrorResult<GeoscapeAddressFeature>("No addresses found");
            }

            return new SuccessResult<GeoscapeAddressFeature>(addresses[0]);
        }
        
        private static async Task<JOSResult<GeoscapeAddressFeatureCollection>> GetAddressDataAsync(string address)
        {
            var requestString = GeoscapeAddressRequestBuilder.BuildRequest(address);

            // TODO - Possibly we should be setting client.BaseAddress rather than appending
            // the request string.
            // client.BaseAddress = new Uri(Constants.GeoscapeAPIUrl + requestString);

            var apiKeyResult = GeoscapeAddressRequestBuilder.ApiKey;
            if (apiKeyResult.Failure)
            {
                if (apiKeyResult is ErrorResult<string> error)
                {
                    return new ErrorResult<GeoscapeAddressFeatureCollection>(error.Message);
                }

                // At the moment, there are no other errors, but this is put here in case someone adds one later.
                return new ErrorResult<GeoscapeAddressFeatureCollection>("No API Key read.");
            }
            
            // apiKeyResult.Success
            // At this point, we know we got a response, but it may say for example that
            // the address wasn't found.
            Client.SetAuthorizationHeaders(new AuthenticationHeaderValue(GeoscapeAddressRequestBuilder.ApiKey.Data));
            
            var httpResponse = await Client.DoGetJsonRequest<GeoscapeAddressFeatureCollection>(Constants.GeoscapeAPIUrl + requestString);
            return InterpretGeoscapeResponse(httpResponse);
        }

        // Geoscape-specific response interpretation.
        // TODO Find out if there's ever >1 message.
        private static JOSResult<GeoscapeAddressFeatureCollection> InterpretGeoscapeResponse(JOSResult<GeoscapeAddressFeatureCollection> httpResponse)
        {
            if (httpResponse.Failure)
            {
                return httpResponse;
            }
            
            // Successful http response, but there still might be errors from Geoscape.
            var errorMessages = httpResponse.Data.Messages;
            
            // If there are no messages, the response is OK. Success.
            if (errorMessages is null || errorMessages.Length == 0 )
            {
                return new SuccessResult<GeoscapeAddressFeatureCollection>(httpResponse.Data);
            }
            
            // If there's an error message, prettify the "no address matched" specific error, otherwise
            // just pass the error on.
            if (errorMessages[0] == "No address matched the query.")
            {
                return new ErrorResult<GeoscapeAddressFeatureCollection>("Address not found - try again.");
            }

            return new ErrorResult<GeoscapeAddressFeatureCollection> (errorMessages[0]);
        }
    }
}