using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
    public class RestService : IRestService
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;

        public Result<List<string>> Items { get; private set; }

        public RestService()
        {
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<Result<List<string>>> RefreshDataAsync()
        {
            Items = new Result<List<string>>();

            Uri uri = new Uri(string.Format(Constants.UserListUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<Result<List<string>>>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        public async Task<Result<bool>> SaveTodoItemAsync(Registration item)
        {
            Uri uri = new Uri(Constants.RegUrl);
            
            try
            {
                string json = JsonSerializer.Serialize<Registration>(item, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response == null || !response.IsSuccessStatusCode)
                {
                    return new Result<bool>
                    {
                        Err = "Error connecting to server."+response?.StatusCode+response?.ReasonPhrase
                    };
                }
                
                string responseContent = await response.Content.ReadAsStringAsync();
                Result<SignedString> httpResponse =
                    JsonSerializer.Deserialize<Result<SignedString>>(responseContent, serializerOptions);

                if (httpResponse==null || !String.IsNullOrEmpty(httpResponse.Err))
                {
                    Debug.WriteLine(@"\tError saving TodoItem:"+httpResponse?.Err);
                    return new Result<bool> {Err = "Error response from server:"+httpResponse?.Err }; 
                }
                
                Debug.WriteLine(@"\tTodoItem successfully saved.");
                
                return new Result<bool>
                {
                    Ok = httpResponse.Ok.verifies(SignatureService.serverPublicKey)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return new Result<bool> { Err = "Error connecting to server."};
            }
        }
    }
}
