using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
    /* A single static http client, set up to talk to Geoscape.
     */
    public static class GeoscapeClientService
    {
        
        private static JsonSerializerOptions serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false
                // I *think* this is the right thing to use for deserialising addresses
                // with possibly absent elements.
                // DefaultIgnoreCondition = JsonIgnoreCondition.Always
            };
        private static RTAHttpClient client = new RTAHttpClient(serializerOptions);
        
        public static async Task<Result<GeoscapeAddressFeatureCollection>> GetAddressDataAsync(string address)
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
            
            return await client.DoGetJSONRequest<GeoscapeAddressFeatureCollection>(Constants.GeoscapeAPIUrl + requestString);
        }

    }
}