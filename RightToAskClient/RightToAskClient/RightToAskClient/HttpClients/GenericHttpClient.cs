using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.Models;

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
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public Result<List<string>> Items 
        { 
            get; 
            private set; 
        } = new Result<List<string>>();

        // TODO Not sure if this is the right way to do this.
        public void SetAuthorizationHeaders(AuthenticationHeaderValue authHeader)
        {
            _client.DefaultRequestHeaders.Authorization = authHeader;
        }

        public GenericHttpClient(JsonSerializerOptions serializerOptions)
        {
            _client = new HttpClient();
            this._serializerOptions = serializerOptions;
            
        }
        
        
        // TODO This is just a copy paste of DoGetRequest, with small changes
        // for automated JSON serialising
        public async Task<Result<T>> DoGetJSONRequest<T>(string uriString)
        {
            Uri uri = new Uri(uriString);
            try
            {
                T deserialisedResponse = await _client.GetFromJsonAsync<T>(uri, _serializerOptions);

                if (deserialisedResponse is null)
                {
                    return new Result<T>
                    {
                        Err = "Error getting json from server."
                    };
                }

                // TODO - there may need to be specific error handling for each server. For example, 
                // Geoscape returns a special value "Enumeration yielded no results" in Results.Empty 
                // when the address didn't match anything.
                // Actually this may be better dealt with by whatever receives this info.

                return new Result<T>
                {
                    Ok = deserialisedResponse
                };
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<T>
                    { Err = "Error connecting to server." + ex.Message };
            }
            catch (JsonException ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<T>
                    { Err = "JSON deserialisation error." + ex.Message };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<T>
                    { Err = "Error connecting to server." + ex.Message };
            }
        }
        
        // Note: This shouldn't be necessary any more - just unify with
        // DoGetJsonRequest
        public async Task<Result<T>> DoGetResultRequest<T>(string uriString)
        {
            Uri uri = new Uri(uriString);
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);

                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<T>
                    {
                        Err = "Error connecting to server." + response?.StatusCode + response?.ReasonPhrase
                    };
                }

                string content = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonSerializer.Deserialize<Result<T>>(content, _serializerOptions);

                if (deserialisedResponse is null)
                {
                    return new Result<T>
                    {
                        Err = "Error deserialising server response."
                    };
                }

                // TODO - there may need to be specific error handling for each server. For example, 
                // Geoscape returns a special value "Enumeration yielded no results" in Results.Empty 
                // when the address didn't match anything.

                return deserialisedResponse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<T>()
                    { Err = "Error connecting to server." + ex.Message };
            }
        }

        // TODO Unused - Remove?
        public async Task<Result<List<string>>> RefreshDataAsync()
        {

            Uri uri = new Uri(Constants.UserListUrl);
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                
                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<List<string>>
                    {
                        Err = "Error connecting to server."+response?.StatusCode+response?.ReasonPhrase
                    };
                }
                
                string content = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonSerializer.Deserialize<Result<List<string>>>(content, _serializerOptions);

                if (deserialisedResponse is null)
                {
                    return new Result<List<string>>
                    {
                        Err = "Error deserialising server response."
                    };
                }
                
                Items = deserialisedResponse; 
                return Items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<List<string>>()
                    { Err = "Error connecting to server." + ex.Message };
            }
        }

        // Tin is the type of the thing we post, which is also the input type of this function.
        // TResponse is the type of the server's response, which we return.
        public async Task<Result<TResponse>> PostGenericItemAsync<TResponse, TIn>(TIn item)
        {
            Uri uri = new Uri(Constants.RegUrl);
            
            try
            {
                string json = JsonSerializer.Serialize(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(uri, content);

                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<TResponse>
                    {
                        Err = "Error connecting to server."+response?.StatusCode+response?.ReasonPhrase
                    };
                }
                
                string responseContent = await response.Content.ReadAsStringAsync();
                TResponse httpResponse =
                    JsonSerializer.Deserialize<TResponse>(responseContent, _serializerOptions);

                if (httpResponse is null)
                {
                    Debug.WriteLine(@"\tError saving Item:");
                    return new Result<TResponse> {Err = "Null response from server:"}; 
                }

                Debug.WriteLine(@"\tItem successfully saved on server.");
                
                return new Result<TResponse>
                {
                    Ok = httpResponse
                };

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<TResponse> { Err = "Error connecting to server."+ex.Message};
            }
        }
    }
}