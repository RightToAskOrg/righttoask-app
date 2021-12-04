using System;
using System.Collections;
using System.Linq;
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
        private static JsonSerializerOptions serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false
                // TODO I *think* this is the right thing to use for deserialising addresses
                // with possibly absent elements.
                // DefaultIgnoreCondition = JsonIgnoreCondition.Always
            };
        
        private static GenericHttpClient client = new GenericHttpClient(serializerOptions);

        public static async Task<Result<GeoscapeAddressFeature>> GetFirstAddressData(string address)
        {
            var collection = await GetAddressDataAsync(address);

            if (!String.IsNullOrEmpty(collection.Err))
            {
                return new Result<GeoscapeAddressFeature> { Err = collection.Err };
            }

            var addresses = collection.Ok.AddressDataList;
            // I *think* this should never happen, because there should be a message if the
            // address list is empty, but better safe than sorry.
            if(addresses.Length == 0)
            {
                return new Result<GeoscapeAddressFeature> { Err = "No addresses found" };
            }

            return new Result<GeoscapeAddressFeature> { Ok = addresses[0] };
        }
        
        private static async Task<Result<GeoscapeAddressFeatureCollection>> GetAddressDataAsync(string address)
        {
            string requestString = GeoscapeAddressRequestBuilder.BuildRequest(address);

            // TODO - Possibly we should be setting client.BaseAddress rather than appending
            // the request string.
            // client.BaseAddress = new Uri(Constants.GeoscapeAPIUrl + requestString);
            // ***TODO Check that there is an OK.

            if (!String.IsNullOrEmpty(GeoscapeAddressRequestBuilder.ApiKey.Err))
            {
                return new Result<GeoscapeAddressFeatureCollection>() { Err = GeoscapeAddressRequestBuilder.ApiKey.Err };
            }
            
            client.SetAuthorizationHeaders(
                new AuthenticationHeaderValue(GeoscapeAddressRequestBuilder.ApiKey.Ok));
            
            // At this point, we know we got a response, but it may say for example that
            // the address wasn't found.
            Result<GeoscapeAddressFeatureCollection> httpResponse = await client.DoGetJSONRequest<GeoscapeAddressFeatureCollection>(Constants.GeoscapeAPIUrl + requestString);
            return InterpretGeoscapeResponse(httpResponse.Ok);
        }

        // Geoscape-specific response interpretation.
        // TODO Find out if there's ever >1 message.
        public static Result<GeoscapeAddressFeatureCollection> InterpretGeoscapeResponse(GeoscapeAddressFeatureCollection httpResponseOk)
        {
            if (httpResponseOk.Messages.Length > 0)
            {
                if (httpResponseOk.Messages[0] == "No address matched the query.")
                {
                    return new Result<GeoscapeAddressFeatureCollection> { Err = "Address not found - try again." };
                }

                return new Result<GeoscapeAddressFeatureCollection> { Err = httpResponseOk.Messages[0] };
            }

            return new Result<GeoscapeAddressFeatureCollection> { Ok = httpResponseOk };
        }
    }
}