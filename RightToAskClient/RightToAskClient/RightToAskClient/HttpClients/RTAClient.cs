using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.CryptoUtils;
using Xamarin.Essentials;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.HttpClients
{
    /* A single static http client, set up to talk to the RightToAsk server.
     */
    public static class RTAClient
    {
        
        private static string BaseUrl = SetUpServerConfig();

        private static string RegUrl = BaseUrl + "/new_registration";
        private static string EditUserUrl = BaseUrl + "/edit_user";
        private static string QnUrl = BaseUrl + "/new_question";
        private static string MPListUrl = BaseUrl + "/MPs.json";
        private static string UserListUrl = BaseUrl + "/get_user_list";
        private static string QuestionListUrl = BaseUrl + "/get_question_list";
        private static string QuestionUrl = BaseUrl + "/get_question";
        // TODO At the moment, this is not used, because we don't have a cert chain for the server Public Key.
        // Instead, the public key itself is hardcoded.
        // private static string ServerPubKeyUrl = BaseUrl + "/get_server_public_key_spki";
        private static JsonSerializerOptions _serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = false,
            };
        
        private static readonly GenericHttpClient Client = new GenericHttpClient(_serializerOptions);
        public static string? ServerPublicKey
        {
            get;
            private set;
        }

        // This is true if the config is validly initialized, i.e. we got a consistent, readable config file with
        // a non-null public key and (if remote server was selected) a non-null remote url.
        // TODO (Issue #18) if we can't read server config (or possibly also if we can't reach the server), it's better to deactivate the buttons that require it 
        // (Just like we do for MPs when we can't access them.)
        // public static bool ServerConfigValidInit { get; private set; } = false;

        public static async Task<Result<UpdatableParliamentAndMPDataStructure>> GetMPsData()
        {
            string errorMessage = "Could not download MP data. You can still read and submit questions, but we can't find MPs.";
            Result<UpdatableParliamentAndMPDataStructure>? httpResponse =await Client.DoGetJSONRequest<UpdatableParliamentAndMPDataStructure>(MPListUrl);
            
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
            return await Client.DoGetResultRequest<List<string>>(UserListUrl);
        }

        public static async Task<Result<bool>> RegisterNewUser(Registration newReg)
        {
            return await SendDataToServer<Registration>(newReg, "user", RegUrl);
        }

        public static async Task<Result<bool>> UpdateExistingUser(ClientSignedUnparsed existingReg)
        {
            return await SendDataToServer<ClientSignedUnparsed>(existingReg, "user", EditUserUrl);
        }

        public static async Task<Result<List<string>>> GetQuestionList()
        {
            return await Client.DoGetResultRequest<List<string>>(QuestionListUrl);
        }

        public static async Task<Result<NewQuestionServerReceive>> GetQuestionById(string questionId)
        {
            string GetQuestionUrl = QuestionUrl + "?question_id=" + questionId;
            return await Client.DoGetResultRequest<NewQuestionServerReceive>(GetQuestionUrl);
        }

        public static async Task<Result<bool>> RegisterNewQuestion(ClientSignedUnparsed newQuestion)
        {
            return await SendDataToServer<ClientSignedUnparsed>(newQuestion, "question", QnUrl);
        }

        /*
         * Errors in the outer layer represent http errors, (e.g. 404).
         * These are logged because they represent a problem with the system.
         * Ditto signature verification.
         * Errors in the inner layer represent server errors, e.g. UID already taken.
         * These are returned to the user on the assumption that there's something they
         * can do.
         */
        private static async Task<Result<bool>> SendDataToServer<T>(T newThing, string typeDescr, string uri)
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
        
        // At the moment, this simply passes the information back to the user. We might perhaps want
        // to triage errors more carefully, or explain them to users better, or distinguish user-upload
        // errors from question-upload errors.
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

            return (false, "Server error" + response.Err);
        }

        // maybe overload this to get both a response boolean and data back
        public static (bool isValid, string message) ValidateHttpResponse(Result<(bool, List<string>)> response, string messageTopic)
        {
            if (String.IsNullOrEmpty(response.Err))
            {
                if (response.Ok.Item1)
                {
                    return (true, messageTopic + ": Success.");
                }
                return (false, messageTopic + ": Failure.");
            }
            return (false, "Server error" + response.Err);
        }

        // Tries to read server config, returns the url if there's a valid configuration file
        // specifying that that url is to be used.
        // Otherwise use default URL for localhost.
        // Also sets public key of the server we're using, as read from server config file.
        // If the config file can't be read or parsed, set url and public key to emptylist.
        private static string SetUpServerConfig()
        {
            var serialiserOptions = new JsonSerializerOptions();
            Result<ServerConfig> readResult = FileIO.ReadDataFromStoredJson<ServerConfig>(Constants.ServerConfigFile, serialiserOptions);

            // Set url and public key to empty string if setup file can't be read.
            if (!readResult.Err.IsNullOrEmpty())
            {
                Debug.WriteLine("Error reading server config file: "+readResult.Err);
                ServerPublicKey = "";
                return "";
            }

            string url;
            string key;
            
            // Remote server use.
            if (readResult.Ok.remoteServerUse)
            {
                key = readResult.Ok.remoteServerPublicKey;
                url = readResult.Ok.url;
            } else // Local server use; special url for Android simulator.
            {
                key = readResult.Ok.localServerPublicKey; 
                url = Constants.DefaultLocalhostUrl;
            }
            
            // Something went wrong. Leave ServerInit false and write to debug.
            if (url.IsNullOrEmpty() || key.IsNullOrEmpty())
            {
                Debug.WriteLine("Server config error. Check your serverconfig.json file. "+"url = "+url+". Public key = "+key);
                return "";
            }

            // Success.
            ServerPublicKey = key;
            // ServerConfigValidInit = true;
            return url;
        }
    }
}