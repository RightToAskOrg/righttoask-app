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
    // TODO*** Refactor this so that the Geoscape services and the RightToAsk services use different clients.
    // Check whether getAsync is reentrant.
    public class RestService : IRestService
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;

        public Result<List<string>> Items 
        { 
            get; 
            private set; 
        } = new Result<List<string>>();

        public RestService()
        {
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false 
                // I *think* this is the right thing to use for deserialising addresses
                // with possibly absent elements.
                // DefaultIgnoreCondition = JsonIgnoreCondition.Always
            };
        }
        
        
        // TODO This is just a copy paste of DoGetRequest, with small changes
        // for automated JSON serialising
        public async Task<Result<T>> DoGetJSONRequest<T>(string uriString)
        {
            Uri uri = new Uri(uriString);
            try
            {
                T deserialisedResponse = await client.GetFromJsonAsync<T>(uri, serializerOptions);

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
        // TODO At the moment this is just a copy-paste of the Result version.
        // Re-unify them into one function.
        public async Task<Result<T>> DoGetRequest<T>(string uriString)
        {
            Uri uri = new Uri(uriString);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);

                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<T>
                    {
                        Err = "Error connecting to server." + response?.StatusCode + response?.ReasonPhrase
                    };
                }

                string content = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonSerializer.Deserialize<T>(content, serializerOptions);

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

                // TODO - this is the *only* step that differs depending on whether T is itself
                // a result type. Can we use type introspection to just check it?
                return new Result<T>
                {
                    Ok = deserialisedResponse
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<T>
                    { Err = "Error connecting to server." + ex.Message };
            }
        }
        public async Task<Result<T>> DoGetResultRequest<T>(string uriString)
        {
            Uri uri = new Uri(uriString);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);

                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<T>
                    {
                        Err = "Error connecting to server." + response?.StatusCode + response?.ReasonPhrase
                    };
                }

                string content = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonSerializer.Deserialize<Result<T>>(content, serializerOptions);

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

        public async Task<Result<GeoscapeAddressFeatureCollection>> GetGeoscapeAddressData(string address)
        {
            string requestString = GeoscapeAddressRequestBuilder.BuildRequest(address);

            // TODO - Possibly we shold be setting client.BaseAddress rather than appending
            // the request string.
            // client.BaseAddress = new Uri(Constants.GeoscapeAPIUrl + requestString);
            // ***TODO Check that there is an OK.

            if (!String.IsNullOrEmpty(GeoscapeAddressRequestBuilder.ApiKey.Err))
            {
                return new Result<GeoscapeAddressFeatureCollection>() { Err = GeoscapeAddressRequestBuilder.ApiKey.Err };
            }
            
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(GeoscapeAddressRequestBuilder.ApiKey.Ok);
            
            return await DoGetJSONRequest<GeoscapeAddressFeatureCollection>(Constants.GeoscapeAPIUrl + requestString);
        }

        public async Task<Result<List<string>>> GetUserList()
        {
            // client.BaseAddress = new Uri(Constants.UserListUrl);
            return await DoGetResultRequest<List<string>>(Constants.UserListUrl);
        }

        public async Task<Result<List<string>>> RefreshDataAsync()
        {

            Uri uri = new Uri(Constants.UserListUrl);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                
                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<List<string>>
                    {
                        Err = "Error connecting to server."+response?.StatusCode+response?.ReasonPhrase
                    };
                }
                
                string content = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonSerializer.Deserialize<Result<List<string>>>(content, serializerOptions);

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

        
        public async Task<Result<bool>> SaveTodoItemAsync(Registration item)
        {
            Uri uri = new Uri(Constants.RegUrl);
            
            try
            {
                string json = JsonSerializer.Serialize(item, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response is null || !response.IsSuccessStatusCode)
                {
                    return new Result<bool>
                    {
                        Err = "Error connecting to server."+response?.StatusCode+response?.ReasonPhrase
                    };
                }
                
                string responseContent = await response.Content.ReadAsStringAsync();
                Result<SignedString>? httpResponse =
                    JsonSerializer.Deserialize<Result<SignedString>>(responseContent, serializerOptions);

                if (httpResponse is null || !String.IsNullOrEmpty(httpResponse.Err))
                {
                    Debug.WriteLine(@"\tError saving TodoItem:"+httpResponse?.Err);
                    return new Result<bool> {Err = "Error response from server:"+httpResponse?.Err }; 
                }
                
                Debug.WriteLine(@"\tTodoItem successfully saved.");
                
                return new Result<bool>
                {
                    Ok = httpResponse.Ok.verifies(SignatureService.ServerPublicKey)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<bool> { Err = "Error connecting to server."+ex.Message};
            }
        }

    }
}
