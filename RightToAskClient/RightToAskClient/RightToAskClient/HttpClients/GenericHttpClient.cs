using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;

/* This makes a small wrapper around the HttpClient class.
 * The intention is that a static instance of this class should
 * be used for each different server.
 * Usage is loosely based on this tutorial:
 * https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
 * Note that it does suffer from the DNS-update problem described here:
 * https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
 * but this seems an acceptable price to pay for simplicity. It does mean that
 * if the DNS settings change, clients will have to be restarted.
 * */
namespace RightToAskClient.HttpClients
{
    public class GenericHttpClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        // TODO Not sure if this is the right way to do this.
        public void SetAuthorizationHeaders(AuthenticationHeaderValue authHeader)
        {
            _client.DefaultRequestHeaders.Authorization = authHeader;
        }

        public GenericHttpClient(JsonSerializerOptions serializerOptions)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(3);
            this._serializerOptions = serializerOptions;
            
        }
        
        
        public async Task<JOSResult<T>> DoGetJsonRequest<T>(string uriString)
        {
            var uri = new Uri(uriString);
            try
            {
                var deserialisedResponse = await _client.GetFromJsonAsync<T>(uri, _serializerOptions);

                if (deserialisedResponse is null)
                {
                    return new ErrorResult<T>("Error getting json from server.");
                };

                return new SuccessResult<T>(deserialisedResponse);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new ErrorResult<T> ("Error connecting to server." + ex.Message);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new ErrorResult<T> ("JSON deserialisation error." + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new ErrorResult<T> ("Error connecting to server." + ex.Message);
            }
        }
        
        /* Unwinds the double layer of Result<> when the server itself responds with a ServerResult<T> data structure.
         * TODO - I think this could simply say "if result is null [generate new error]; return result.
         * TODO** even better, could this simply be 'return await DoGetJsonRequeset...' ?
         */
        public async Task<JOSResult<T>> DoGetResultRequest<T>(string uriString)
        {
            var result = await DoGetJsonRequest<ServerResult<T>>(uriString);
            
            // If we failed to contact the server at all
            if (result.Failure)
            {
                return new ErrorResult<T>("Couldn't contact the server");
            }
            
            // We contacted the server successfully. Check whether it gave a success or failure response.
            // result.Success is true.
            var serverResult = result.Data;
            
            // If the server returned an error, pass it on.
            if (!string.IsNullOrEmpty(serverResult.Err))
            {
                return new ErrorResult<T>(serverResult.Err ?? "");
            }
            
            // Success - return the data.
            return new SuccessResult<T>(result.Data.Ok);
        }

        // Tin is the type of the thing we post, which is also the input type of this function.
        // TResponse is the type of the server's response, which we assume to be incorporated
        // into a ServerResult. We check for errors and return.
        public async Task<JOSResult<TResponse>> PostGenericItemAsync<TResponse, TIn>(TIn item, string requestedUri)
        {
            var uri = new Uri(requestedUri);
            
            try
            {
                var json = JsonSerializer.Serialize(item, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new ErrorResult<TResponse>( "Error connecting to server." + response?.StatusCode + response?.ReasonPhrase );
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                var httpResponse =
                    JsonSerializer.Deserialize<ServerResult<TResponse>>(responseContent, _serializerOptions);

                if (httpResponse is null)
                {
                    Debug.WriteLine(@"\tError saving Item:");
                    return new ErrorResult<TResponse> ("Null response from server:"); 
                }

                Debug.WriteLine(@"\tItem successfully saved on server.");

                return httpResponse.ToJOSResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new ErrorResult<TResponse>(ex.Message);
            }
        }
        
        // For the case when we do not expect any data returned from the server. Use a string because it can be null.
        // If there is no error, produce an empty JOS Success Result, otherwise a type-less Error result with the
        // same message. Note that this will throw an exception
        // if there actually is some data.
        // FIXME This thows an exception because the call to PostGenericItemAsync<string,TIn>
        // tries to build a <string> JOSResult with no data.
        public async Task<JOSResult> PostGenericItemAsync<TIn>(TIn item, string requestedUri)
        {
            var result = await PostGenericItemAsync<string, TIn>(item, requestedUri);

            if (result is ErrorResult<string> e)
            {
                return new ErrorResult(e.Message);
            }

            return new SuccessResult();
        }
    }
}