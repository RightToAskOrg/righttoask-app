using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Models;

namespace RightToAskClient.HttpClients
{
    /* A single static http client, set up to talk to Geoscape.
     */
    public static class GeoscapeClient
    {
        private static JsonSerializerOptions _serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
                // TODO I *think* this is the right thing to use for deserialising addresses
                // with possibly absent elements.
                // DefaultIgnoreCondition = JsonIgnoreCondition.Always
            };
        
        private static GenericHttpClient _client = new GenericHttpClient(_serializerOptions);

        public static async Task<Result<GeoscapeAddressFeature>> GetFirstAddressData(string address)
        {
            var collection = await GetAddressDataAsync(address);

            if (!string.IsNullOrEmpty(collection.Err))
            {
                return new Result<GeoscapeAddressFeature> { Err = collection.Err };
            }

            var addresses = collection.Ok.AddressDataList;
            // I *think* this should never happen, because there should be a message if the
            // address list is empty, but better safe than sorry.
            if(addresses is null || addresses.Length == 0)
            {
                return new Result<GeoscapeAddressFeature> { Err = "No addresses found" };
            }

            return new Result<GeoscapeAddressFeature> { Ok = addresses[0] };
        }
        
        private static async Task<Result<GeoscapeAddressFeatureCollection>> GetAddressDataAsync(string address)
        {
            var requestString = GeoscapeAddressRequestBuilder.BuildRequest(address);

            // TODO - Possibly we should be setting client.BaseAddress rather than appending
            // the request string.
            // client.BaseAddress = new Uri(Constants.GeoscapeAPIUrl + requestString);

            if (!string.IsNullOrEmpty(GeoscapeAddressRequestBuilder.ApiKey.Err))
            {
                return new Result<GeoscapeAddressFeatureCollection>() { Err = GeoscapeAddressRequestBuilder.ApiKey.Err };
            }
            
            // The ApiKey.OK _should_ be properly initialised to "" but this isn't guaranteed (despite the
            // compiler thinking it is).
            _client.SetAuthorizationHeaders(
                new AuthenticationHeaderValue(GeoscapeAddressRequestBuilder.ApiKey.Ok ?? string.Empty));
            
            // At this point, we know we got a response, but it may say for example that
            // the address wasn't found.
            var httpResponse = await _client.DoGetJSONRequest<GeoscapeAddressFeatureCollection>(Constants.GeoscapeAPIUrl + requestString);
            return InterpretGeoscapeResponse(httpResponse);
        }

        // Geoscape-specific response interpretation.
        // TODO Find out if there's ever >1 message.
        public static Result<GeoscapeAddressFeatureCollection> InterpretGeoscapeResponse(Result<GeoscapeAddressFeatureCollection> httpResponse)
        {
            if (!string.IsNullOrEmpty(httpResponse.Err))
            {
                return httpResponse;
            }
            var errorMessages = httpResponse.Ok.Messages;
            
            // If there are no messages, the response is OK.
            if (errorMessages is null || errorMessages.Length == 0 )
            {
                return new Result<GeoscapeAddressFeatureCollection> { Ok = httpResponse.Ok };
            }
            
            // If there's an error message, prettify the "no address matched" specific error, otherwise
            // just pass the error on.
            if (errorMessages[0] == "No address matched the query.")
            {
                return new Result<GeoscapeAddressFeatureCollection> { Err = "Address not found - try again." };
            }

            return new Result<GeoscapeAddressFeatureCollection> { Err = errorMessages[0] };
        }
    }
}