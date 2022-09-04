using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using Xamarin.Essentials;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;

namespace RightToAskClient.HttpClients
{
    /* A single static http client, set up to talk to the RightToAsk server.
     */
    public static class RTAClient
    {
        
        private static (string key, string url)  serverConfig = SetUpServerConfig();
        private static string serverPublicKey = serverConfig.key;
        private static string BaseUrl = serverConfig.url;
        private static string RegUrl = BaseUrl + "/new_registration";
        private static string EditUserUrl = BaseUrl + "/edit_user";
        private static string QnUrl = BaseUrl + "/new_question";
        private static string EditQnUrl = BaseUrl + "/edit_question";
        private static string MPListUrl = BaseUrl + "/MPs.json";
        private static string CommitteeListUrl = BaseUrl + "/committees.json";
        private static string HearingsListUrl = BaseUrl + "/hearings.json";
        private static string UserListUrl = BaseUrl + "/get_user_list";
        private static string QuestionListUrl = BaseUrl + "/get_question_list";
        private static string QuestionUrl = BaseUrl + "/get_question" + "?question_id=";
        private static string UserUrl = BaseUrl + "/get_user" + "?uid=";
        private static string EmailValidationUrl = BaseUrl + "/request_email_validation";
        private static string EmailValidationPINUrl = BaseUrl + "/email_proof";
        private static string SimilarQuestionsUrl = BaseUrl + "/similar_questions";
        
        // TODO At the moment, this is not used, because we don't have a cert chain for the server Public Key.
        // Instead, the public key itself is hardcoded.
        // private static string ServerPubKeyUrl = BaseUrl + "/get_server_public_key_spki";
        
        private static JsonSerializerOptions _serializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                // This is necessary for serialisation and deserialisation of tuples.
                IncludeFields = true
            };
        
        private static readonly GenericHttpClient Client = new GenericHttpClient(_serializerOptions);

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

        public static async Task<Result<List<CommitteeInfo>>> GetCommitteeData()
        {
            var committeeList =await Client.DoGetJSONRequest<List<CommitteeInfo>>(CommitteeListUrl);
            return committeeList;
        }
        
        // TODO Correct types.
        public static async Task<Result<List<CommitteeInfo>>> GetHearingsData()
        {
            return await Client.DoGetResultRequest<List<CommitteeInfo>>(HearingsListUrl);
        }
        
        /* Currently unused, but will be used when we want to get lists of other users
         * for DMs or following
         */
        public static async Task<Result<List<string>>> GetUserList()
        {
            return await Client.DoGetJSONRequest<List<string>>(UserListUrl);
        }

        public static async Task<Result<string>> RegisterNewUser(Registration newReg)
        {
            var newUserForServer = new ServerUser(newReg);
            return await SendDataToServerVerifySignedResponse<ServerUser>(newUserForServer, "user", RegUrl);
        }

        public static async Task<Result<string>> UpdateExistingUser(ServerUser existingReg)
        {
            Debug.Assert(App.ReadingContext.ThisParticipant.IsRegistered);
            return await SignAndSendDataToServer<ServerUser>(existingReg, "user", EditUserUrl,
                AppResources.AccountUpdateSigningError);
        }


        public static async Task<Result<List<string>>> GetQuestionList()
        {
            return await Client.DoGetResultRequest<List<string>>(QuestionListUrl);
        }

        public static async Task<Result<List<ScoredIDs>>> GetSimilarQuestionIDs(QuestionSendToServer draftQuestion)
        {
            return await SendDataToServerReturnResponse<QuestionSendToServer, List<ScoredIDs>>(draftQuestion, AppResources.QuestionErrorTypeDescription, SimilarQuestionsUrl);
        }

        public static async Task<Result<QuestionReceiveFromServer>> GetQuestionById(string questionId)
        {
            string GetQuestionUrl = QuestionUrl + questionId;
            return await Client.DoGetResultRequest<QuestionReceiveFromServer>(GetQuestionUrl);
        }

        public static async Task<Result<ServerUser>> GetUserById(string userId)
        {
            string GetUserUrl = UserUrl + userId;
            return await Client.DoGetResultRequest<ServerUser>(GetUserUrl);
        }

        public static async Task<Result<string>> RegisterNewQuestion(QuestionSendToServer newQuestion)
        {
            return await SignAndSendDataToServer<QuestionSendToServer>(newQuestion, AppResources.QuestionErrorTypeDescription, QnUrl,"Error publishing New Question");
        }

        public static async Task<Result<string>> UpdateExistingQuestion(QuestionSendToServer existingQuestion)
        {
            return await SignAndSendDataToServer<QuestionSendToServer>(existingQuestion, AppResources.QuestionErrorTypeDescription, EditQnUrl, "Error editing question");
        }

        public static async Task<Result<string>> RequestEmailValidation(RequestEmailValidationMessage msg, string email)
        {
            ClientSignedUnparsed signedMsg =  App.ReadingContext.ThisParticipant.SignMessage(msg);
            RequestEmailValidationAPICall serverSend = new RequestEmailValidationAPICall()
            {
                email = email,
                message = signedMsg.message,
                signature = signedMsg.signature,
                user = signedMsg.user,
            };
            return await SendDataToServerVerifySignedResponse(serverSend, "temp error msg", EmailValidationUrl);

        }

        public static async Task<Result<string>> SendEmailValidationPIN(EmailValidationPIN msg)
        {
            return await SignAndSendDataToServer(msg, "Sending PIN", EmailValidationPINUrl, "Signing PIN");
        }

        // Sign a message (data) with this user's key, then upload to the specified url. 
        // "description" and "error string" are for reporting errors in upload and signing resp.
        private static async Task<Result<string>> SignAndSendDataToServer<T>(T data, string description, string url, string errorString)
        {
            ClientSignedUnparsed signedUserMessage = App.ReadingContext.ThisParticipant.SignMessage(data);
            if (!String.IsNullOrEmpty(signedUserMessage.signature))
            {
                return await SendDataToServerVerifySignedResponse<ClientSignedUnparsed>(signedUserMessage, description, url);
            }

            Debug.WriteLine(errorString);
            return new Result<string>() { Err = errorString};
        }

        private static async Task<Result<string>> SendDataToServerVerifySignedResponse<Tupload>(Tupload newThing,
            string typeDescr, string uri)
        {
            var serverResponse = await SendDataToServerReturnResponse<Tupload, SignedString>(newThing, typeDescr, uri);

            if (!String.IsNullOrEmpty(serverResponse.Err))
            {
                return new Result<string>() { Err = serverResponse.Err };
            }

            if (!SignatureVerificationService.VerifySignature(serverResponse.Ok, serverPublicKey)) //  
            {
                Debug.WriteLine(@"\t Error registering new " + typeDescr + ": Signature verification failed");
                return new Result<string>() { Err = "Server signature verification failed" };
            }

            return new Result<string>() { Ok = serverResponse.Ok.message };
        }

        private static async Task<Result<TReturn>> SendDataToServerDeserialiseUnsignedResponse<Tupload, TReturn>(
            Tupload newThing, string typeDescr, string uri)
        {
            return await SendDataToServerReturnResponse<Tupload, TReturn>(newThing, typeDescr, uri);
        }
        
        /*
         * Errors in the outer layer represent http errors, (e.g. 404).
         * These are logged because they represent a problem with the system.
         * Ditto signature verification.
         * Errors in the inner layer represent server errors, e.g. UID already taken.
         * These are returned to the user on the assumption that there's something they
         * can do.
         * In some cases, the server returns some information
         * in the form of a message that is then returned in Result.Ok.
         */
        private static async Task<Result<TReturn>> SendDataToServerReturnResponse<Tupload,TReturn>(Tupload newThing, string typeDescr, string uri)
        {
            var httpResponse 
                = await Client.PostGenericItemAsync<Result<TReturn>, Tupload>(newThing, uri);

            // http errors
            if (String.IsNullOrEmpty(httpResponse.Err))
            {
                // Error responses from the server
                if (String.IsNullOrEmpty(httpResponse.Ok.Err))
                {

                    return new Result<TReturn>() { Ok = httpResponse.Ok.Ok };
                }
                    
                Debug.WriteLine(@"\tError sending "+typeDescr+": "+httpResponse.Ok.Err);
                return new Result<TReturn>() { Err = httpResponse.Ok.Err };
            }

            Debug.WriteLine(@"\tError reaching server for sending " +typeDescr+": "+httpResponse.Err);
            return new Result<TReturn>() { Err = httpResponse.Err };
        }
        
        // At the moment, this simply passes the information back to the user. We might perhaps want
        // to triage errors more carefully, or explain them to users better, or distinguish user-upload
        // errors from question-upload errors.
        public static (bool isValid, string errorMessage, T returnedData) ValidateHttpResponse<T>(Result<T> response, string messageTopic) where T : new()
        {
            if (String.IsNullOrEmpty(response.Err))
            {
                return (true, "", response.Ok);
            }
            return (false, messageTopic + "Server error: " + response.Err, new T());
        }

        // overload because string doesn't satisfy T: new() 
        public static (bool isValid, string errorMessage, string returnedData) ValidateHttpResponse(Result<string> response, string messageTopic) 
        {
            if (String.IsNullOrEmpty(response.Err))
            {
                return (true, "", response.Ok);
            }
            return (false, messageTopic + "Server error: " + response.Err, "");
        }

        // Tries to read server config, returns the url if there's a valid configuration file
        // specifying that that url is to be used.
        // Otherwise use default URL for localhost.
        // Also sets public key of the server we're using, as read from server config file.
        // If the config file can't be read or parsed, set url and public key to emptylist.
        private static (string,string) SetUpServerConfig()
        {
            var serialiserOptions = new JsonSerializerOptions();
            Result<ServerConfig> readResult = FileIO.ReadDataFromStoredJson<ServerConfig>(Constants.ServerConfigFile, serialiserOptions);

            string url="";
            string key="";
            // Set url and public key to empty string if setup file can't be read.
            if (!readResult.Err.IsNullOrEmpty())
            {
                Debug.WriteLine("Error reading server config file: "+readResult.Err);
                return (key,url);
            }

            
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
            
            // Something went wrong. Leave key and url blank and write to debug.
            if (url.IsNullOrEmpty() || key.IsNullOrEmpty())
            {
                Debug.WriteLine("Server config error. Check your serverconfig.json file. "+"url = "+url+". Public key = "+key);
                return ("","");
            }

            // Success.
            // ServerConfigValidInit = true;
            return (key, url);
        }
    }
}