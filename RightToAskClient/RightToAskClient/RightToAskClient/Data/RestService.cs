using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
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
            };
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
