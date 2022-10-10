using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;

namespace RightToAskClient.HttpClients
{
    /* A single static http client, set up to talk to the RightToAsk server.
     */
    public static class RTAClient
    {
        
        private static readonly (string key, string url)  ServerConfig = SetUpServerConfig();
        private static readonly string ServerPublicKey = ServerConfig.key;
        private static readonly string BaseUrl = ServerConfig.url;
        private static readonly string RegUrl = BaseUrl + "/new_registration";
        private static readonly string EditUserUrl = BaseUrl + "/edit_user";
        private static readonly string QnUrl = BaseUrl + "/new_question";
        private static readonly string EditQnUrl = BaseUrl + "/edit_question";
        private static readonly string MPListUrl = BaseUrl + "/MPs.json";
        private static readonly string CommitteeListUrl = BaseUrl + "/committees.json";
        private static readonly string HearingsListUrl = BaseUrl + "/hearings.json";
        private static readonly string SearchUserUrl = BaseUrl + "/search_user"+ "?badges=true&search=";
        private static readonly string QuestionListUrl = BaseUrl + "/get_question_list";
        private static readonly string QuestionUrl = BaseUrl + "/get_question" + "?question_id=";
        private static readonly string GetQuestionByWriterUrl = BaseUrl + "/get_questions_created_by_user" + "?uid=";
        private static readonly string UserUrl = BaseUrl + "/get_user" + "?uid=";
        private static readonly string EmailValidationUrl = BaseUrl + "/request_email_validation";
        private static readonly string EmailValidationPinUrl = BaseUrl + "/email_proof";
        private static readonly string SimilarQuestionsUrl = BaseUrl + "/similar_questions";
        
        // TODO At the moment, this is not used, because we don't have a cert chain for the server Public Key.
        // Instead, the public key itself is hardcoded.
        // private static string ServerPubKeyUrl = BaseUrl + "/get_server_public_key_spki";
        
        private static readonly JsonSerializerOptions SerializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                // This is necessary for serialisation and deserialisation of tuples.
                IncludeFields = true
            };
        
        private static readonly GenericHttpClient Client = new GenericHttpClient(SerializerOptions);

        // This is true if the config is validly initialized, i.e. we got a consistent, readable config file with
        // a non-null public key and (if remote server was selected) a non-null remote url.
        // TODO (Issue #18) if we can't read server config (or possibly also if we can't reach the server), it's better to deactivate the buttons that require it 
        // (Just like we do for MPs when we can't access them.)
        // public static bool ServerConfigValidInit { get; private set; } = false;

        public static async Task<Result<UpdatableParliamentAndMPDataStructure>> GetMPsData()
        {
            const string errorMessage = "Could not download MP data. You can still read and submit questions, but we can't find MPs.";
            var httpResponse =await Client.DoGetJsonRequest<UpdatableParliamentAndMPDataStructure>(MPListUrl);
            
            // Note: the compiler warns this null check is unnecessary, but an exception is sometimes thrown here without this check.
            // I am confused about why this is necessary, but empirically it definitely is.
            if (httpResponse is null)
            {
                return new Result<UpdatableParliamentAndMPDataStructure>() { Err = errorMessage };
            }

            if(string.IsNullOrEmpty(httpResponse.Err))
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
            var committeeList =await Client.DoGetJsonRequest<List<CommitteeInfo>>(CommitteeListUrl);
            return committeeList;
        }
        
        // TODO Correct types.
        public static async Task<Result<List<CommitteeInfo>>> GetHearingsData()
        {
            return await Client.DoGetResultRequest<List<CommitteeInfo>>(HearingsListUrl);
        }
        
        /* Testing only. 
        public static async Task<Result<List<string>>> GetUserList()
        {
            return await Client.DoGetJSONRequest<List<string>>(UserListUrl);
        }
        */

        public static async Task<Result<List<ServerUser>>> SearchUser(string userString)
        {
            return await Client.DoGetResultRequest<List<ServerUser>>(SearchUserUrl+Uri.EscapeDataString(userString));
        }

        public static async Task<Result<string>> RegisterNewUser(Registration newReg)
        {
            var newUserForServer = new ServerUser(newReg);
            return await SendDataToServerVerifySignedResponse(newUserForServer, "user", RegUrl);
        }

        public static async Task<Result<string>> UpdateExistingUser(ServerUser existingReg)
        {
            Debug.Assert(App.ReadingContext.ThisParticipant.IsRegistered);
            return await SignAndSendDataToServer(existingReg, "user", EditUserUrl,
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
            var getQuestionUrl = QuestionUrl + Uri.EscapeDataString(questionId);
            return await Client.DoGetResultRequest<QuestionReceiveFromServer>(getQuestionUrl);
        }

        public static async Task<Result<ServerUser>> GetUserById(string userId)
        {
            var getUserUrl = UserUrl + Uri.EscapeDataString(userId);
            return await Client.DoGetResultRequest<ServerUser>(getUserUrl);
        }
        

        public static async Task<Result<List<string>>> GetQuestionsByWriterId(string userId)
        {
            var getQuestionByWriterUrl = GetQuestionByWriterUrl + Uri.EscapeDataString(userId);
            return await Client.DoGetResultRequest<List<string>>(getQuestionByWriterUrl);
        }
        public static async Task<Result<string>> RegisterNewQuestion(QuestionSendToServer newQuestion)
        {
            return await SignAndSendDataToServer(newQuestion, AppResources.QuestionErrorTypeDescription, QnUrl,"Error publishing New Question");
        }

        public static async Task<Result<string>> UpdateExistingQuestion(QuestionSendToServer existingQuestion)
        {
            return await SignAndSendDataToServer(existingQuestion, AppResources.QuestionErrorTypeDescription, EditQnUrl, "Error editing question");
        }

        public static async Task<Result<string>> RequestEmailValidation(RequestEmailValidationMessage msg, string email)
        {
            var signedMsg =  App.ReadingContext.ThisParticipant.SignMessage(msg);
            var serverSend = new RequestEmailValidationAPICall()
            {
                email = email,
                message = signedMsg.message,
                signature = signedMsg.signature,
                user = signedMsg.user,
            };
            return await SendDataToServerVerifySignedResponse(serverSend, "temp error msg", EmailValidationUrl);

        }

        public static async Task<Result<string>> SendEmailValidationPin(EmailValidationPIN msg)
        {
            return await SignAndSendDataToServer(msg, "Sending PIN", EmailValidationPinUrl, "Signing PIN");
        }

        // Sign a message (data) with this user's key, then upload to the specified url. 
        // "description" and "error string" are for reporting errors in upload and signing resp.
        private static async Task<Result<string>> SignAndSendDataToServer<T>(T data, string description, string url, string errorString)
        {
            var signedUserMessage = App.ReadingContext.ThisParticipant.SignMessage(data);
            if (!string.IsNullOrEmpty(signedUserMessage.signature))
            {
                return await SendDataToServerVerifySignedResponse(signedUserMessage, description, url);
            }

            Debug.WriteLine(errorString);
            return new Result<string>() { Err = errorString};
        }

        private static async Task<Result<string>> SendDataToServerVerifySignedResponse<TUpload>(
            TUpload newThing,
            string typeDescription, 
            string uri)
        {
            var serverResponse = await SendDataToServerReturnResponse<TUpload, SignedString>(newThing, typeDescription, uri);

            if (!string.IsNullOrEmpty(serverResponse.Err))
            {
                return new Result<string>() { Err = serverResponse.Err };
            }

            if (!SignatureVerificationService.VerifySignature(serverResponse.Ok, ServerPublicKey)) //  
            {
                Debug.WriteLine(@"\t Error registering new " + typeDescription + ": Signature verification failed");
                return new Result<string>() { Err = "Server signature verification failed" };
            }

            return new Result<string>() { Ok = serverResponse.Ok.message };
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
        private static async Task<Result<TReturn>> SendDataToServerReturnResponse<TUpload,TReturn>(
            TUpload newThing,
            string typeDescription,
            string uri)
        {
            var httpResponse 
                = await Client.PostGenericItemAsync<Result<TReturn>, TUpload>(newThing, uri);

            // http errors
            if (string.IsNullOrEmpty(httpResponse.Err))
            {
                // Error responses from the server
                if (string.IsNullOrEmpty(httpResponse.Ok.Err))
                {

                    return new Result<TReturn>() { Ok = httpResponse.Ok.Ok };
                }
                    
                Debug.WriteLine(@"\tError sending "+typeDescription+": "+httpResponse.Ok.Err);
                return new Result<TReturn>() { Err = httpResponse.Ok.Err };
            }

            Debug.WriteLine(@"\tError reaching server for sending " +typeDescription+": "+httpResponse.Err);
            return new Result<TReturn>() { Err = httpResponse.Err };
        }
        
        // At the moment, this simply passes the information back to the user. We might perhaps want
        // to triage errors more carefully, or explain them to users better, or distinguish user-upload
        // errors from question-upload errors.
        public static (bool isValid, string errorMessage, T returnedData) ValidateHttpResponse<T>(Result<T> response, string messageTopic) where T : new()
        {
            if (string.IsNullOrEmpty(response.Err))
            {
                return (true, "", response.Ok);
            }
            return (false, messageTopic + "Server error: " + response.Err, new T());
        }

        // overload because string doesn't satisfy T: new() 
        public static (bool isValid, string errorMessage, string returnedData) ValidateHttpResponse(Result<string> response, string messageTopic) 
        {
            if (string.IsNullOrEmpty(response.Err))
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
            var readResult = FileIO.ReadDataFromStoredJson<ServerConfig>(Constants.ServerConfigFile, serialiserOptions);

            var url="";
            var key="";
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