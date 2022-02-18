using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static JsonSerializerOptions _serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
            };
        
        private static readonly GenericHttpClient Client = new GenericHttpClient(_serializerOptions);
        
        public static async Task<Result<UpdatableParliamentAndMPDataStructure>> GetMPsData()
        {
            string errorMessage = "Could not download MP data. You can still read and submit questions, but we can't find MPs.";
            Result<UpdatableParliamentAndMPDataStructure>? httpResponse =await Client.DoGetJSONRequest<UpdatableParliamentAndMPDataStructure>(Constants.MPListUrl);
            
            // Note: the compiler warns this null check is unnecessary, but an exception is sometimes thrown here without this check.
            // I am confused about why this is necessary, but empirically it definitely is.
            if (httpResponse is null)
            {
                return new Result<UpdatableParliamentAndMPDataStructure>() { Err = errorMessage };
            }

            if(String.IsNullOrEmpty(httpResponse.Err))
            {
                return httpResponse;
            }
            
            Debug.WriteLine("Error downloading MP data: "+httpResponse.Err);
            return new Result<UpdatableParliamentAndMPDataStructure>()
            {
                Err = errorMessage 
            };
              
            
        }
        
        /* Currently unused, but will be used when we want to get lists of other users
         * for DMs or following
         */
        public static async Task<Result<List<string>>> GetUserList()
        {
            return await Client.DoGetResultRequest<List<string>>(Constants.UserListUrl);
        }

        public static async Task<Result<bool>> RegisterNewUser(Registration newReg)
        {
            return await RegisterNewThing<Registration>(newReg, "user", Constants.RegUrl);
        }

        public static async Task<Result<bool>> RegisterNewQuestion(ClientSignedUnparsed newQuestion)
        {
            return await RegisterNewThing<ClientSignedUnparsed>(newQuestion, "question", Constants.QnUrl);
        }

        /*
         * Errors in the outer layer represent http errors, (e.g. 404).
         * These are logged because they represent a problem with the system.
         * Ditto signature verification.
         * Errors in the inner layer represent server errors, e.g. UID already taken.
         * These are returned to the user on the assumption that there's something they
         * can do.
         */
        public static async Task<Result<bool>> RegisterNewThing<T>(T newThing, string typeDescr, string uri)
        {
            var httpResponse 
                = await Client.PostGenericItemAsync<Result<SignedString>, T>(newThing, uri);

            // http errors
            if (String.IsNullOrEmpty(httpResponse.Err))
            {
                // Error responses from the server
                if (String.IsNullOrEmpty(httpResponse.Ok.Err))
                {
                    if (!httpResponse.Ok.Ok.verifies(ServerSignatureVerificationService.ServerPublicKey))
                    {
                        Debug.WriteLine(@"\t Error registering new "+typeDescr+": Signature verification failed");
                        return new Result<bool>()
                        {
                            Err = "Server signature verification failed"
                        };
                    }

                    return new Result<bool>() { Ok = true };

                }
                    
                Debug.WriteLine(@"\tError registering new "+typeDescr+": "+httpResponse.Ok.Err);
                return new Result<bool>() { Err = httpResponse.Ok.Err };
            }

            Debug.WriteLine(@"\tError reaching server for registering new " +typeDescr+": "+httpResponse.Err);
            return new Result<bool>() { Err = httpResponse.Err };
        }
        
        public static (bool isValid, string message) ValidateHttpResponse(Result<bool> response, string messageTopic)
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