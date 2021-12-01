using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.CryptoUtils;

namespace RightToAskClient.HttpClients
{
    /* A single static http client, set up to talk to our RightToAsk server.
     */
    public static class RTAClient
    {
        private static JsonSerializerOptions serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false
            };
        
        private static readonly GenericHttpClient client = new GenericHttpClient(serializerOptions);
        
        public static async Task<Result<List<MP>>> GetMPsList()
        {
            return await client.DoGetJSONRequest<List<MP>>(Constants.MPListUrl);
        }
        public static async Task<Result<List<string>>> GetUserList()
        {
            return await client.DoGetResultRequest<List<string>>(Constants.UserListUrl);
        }

        /*
         * Errors in the outer layer represent http errors, (e.g. 404).
         * These are logged because they represent a problem with the system.
         * Ditto signature verification.
         * Errors in the inner layer represent server errors, e.g. UID already taken.
         * These are returned to the user on the assumption that there's something they
         * can do.
         */
        public static async Task<Result<bool>> RegisterNewUser(Registration newReg)
        {
            var httpResponse 
                = await client.PostGenericItemAsync<Result<SignedString>, Registration>(newReg);

            // http errors
            if (String.IsNullOrEmpty(httpResponse.Err))
            {
                // Error responses from the server
                if (String.IsNullOrEmpty(httpResponse.Ok.Err))
                {
                    if (!httpResponse.Ok.Ok.verifies(SignatureService.ServerPublicKey))
                    {
                        Debug.WriteLine(@"\tError saving Item: Signature verification failed");
                        return new Result<bool>()
                        {
                            Err = "Server signature verification failed"
                        };
                    }

                    return new Result<bool>() { Ok = true };

                }
                    
                Debug.WriteLine(@"\tError registering new user:"+httpResponse.Ok.Err);
                return new Result<bool>() { Err = httpResponse.Ok.Err };
            }

            Debug.WriteLine(@"\tError reaching server for registering new user:"+httpResponse.Err);
            return new Result<bool>() { Err = httpResponse.Err };
        }
        public static (bool isValid, string message) validateHttpResponse(Result<bool> response, string messageTopic)
        {
            if (String.IsNullOrEmpty(response.Err))
            {
                if (response.Ok)
                {
                    return (true, messageTopic + ": Success.");
                }

                return (false, messageTopic + ": Failure.");
            }

            return (false, "Server connection error" + response.Err);
        }
    }
}