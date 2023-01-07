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
        private static readonly string PlaintextVoteQnUrl = BaseUrl + "/plaintext_vote_question";
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
        private static readonly string WeightedSearchQuestionsUrl = BaseUrl + "/get_similar_questions";
        
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

        public static async Task<JOSResult<UpdatableParliamentAndMPDataStructure>> GetMPsData()
        {
            const string errorMessage = "Could not download MP data. You can still read and submit questions, but we can't find MPs.";
            var httpResponse =await Client.DoGetJsonRequest<UpdatableParliamentAndMPDataStructure>(MPListUrl);
            
            // Note: the compiler warns this null check is unnecessary, but an exception is sometimes thrown here without this check.
            // I am confused about why this is necessary, but empirically it definitely is.
            if (httpResponse is null)
            {
                return new ErrorResult<UpdatableParliamentAndMPDataStructure>(errorMessage);
            }

            // We got a response, but it was an error 
            if (httpResponse is ErrorResult<UpdatableParliamentAndMPDataStructure> error)
            {
                Debug.WriteLine("Error downloading MP data: " + error.Message);
            }

            // May be an error or a success result.
            return httpResponse;
            
        }

        public static async Task<JOSResult<List<CommitteeInfo>>> GetCommitteeData()
        {
            var committeeList =await Client.DoGetJsonRequest<List<CommitteeInfo>>(CommitteeListUrl);
            return committeeList;
        }
        
        // TODO Correct types.
        public static async Task<JOSResult<List<CommitteeInfo>>> GetHearingsData()
        {
            return await Client.DoGetResultRequest<List<CommitteeInfo>>(HearingsListUrl);
        }
        
        /* Testing only. 
        public static async Task<Result<List<string>>> GetUserList()
        {
            return await Client.DoGetJSONRequest<List<string>>(UserListUrl);
        }
        */

        public static async Task<JOSResult<List<ServerUser>>> SearchUser(string userString)
        {
            return await Client.DoGetResultRequest<List<ServerUser>>(SearchUserUrl+Uri.EscapeDataString(userString));
        }

        public static async Task<JOSResult<string>> RegisterNewUser(Registration newReg)
        {
            var newUserForServer = new ServerUser(newReg);
            return await SendDataToServerVerifySignedResponse(newUserForServer, "user", RegUrl);
        }

        public static async Task<JOSResult<string>> UpdateExistingUser(ServerUser existingReg, string uid)
        {
            return await SignAndSendDataToServer(existingReg, "user", EditUserUrl, AppResources.AccountUpdateSigningError, uid);
        }


        public static async Task<JOSResult<List<string>>> GetQuestionList()
        {
            return await Client.DoGetResultRequest<List<string>>(QuestionListUrl);
        }

        public static async Task<JOSResult<List<ScoredIDs>>> GetSimilarQuestionIDs(QuestionSendToServer draftQuestion)
        {
            return await SendDataToServerReturnResponse<QuestionSendToServer, List<ScoredIDs>>(draftQuestion, AppResources.QuestionErrorTypeDescription, SimilarQuestionsUrl);
        }
        
        public static async Task<JOSResult<SortedQuestionList>> GetSortedSimilarQuestionIDs(WeightedSearchRequest request)
        {
            return await SendDataToServerReturnResponse<WeightedSearchRequest, SortedQuestionList>(request, AppResources.QuestionErrorTypeDescription, WeightedSearchQuestionsUrl);
        }

        public static async Task<JOSResult<QuestionReceiveFromServer>> GetQuestionById(string questionId)
        {
            var getQuestionUrl = QuestionUrl + Uri.EscapeDataString(questionId);
            return await Client.DoGetResultRequest<QuestionReceiveFromServer>(getQuestionUrl);
        }

        public static async Task<JOSResult<ServerUser>> GetUserById(string userId)
        {
            var getUserUrl = UserUrl + Uri.EscapeDataString(userId);
            return await Client.DoGetResultRequest<ServerUser>(getUserUrl);
        }
        

        public static async Task<JOSResult<List<string>>> GetQuestionsByWriterId(string userId)
        {
            var getQuestionByWriterUrl = GetQuestionByWriterUrl + Uri.EscapeDataString(userId);
            return await Client.DoGetResultRequest<List<string>>(getQuestionByWriterUrl);
        }
        public static async Task<JOSResult<string>> RegisterNewQuestion(QuestionSendToServer newQuestion, string uid)
        {
            return await SignAndSendDataToServer(newQuestion, AppResources.QuestionErrorTypeDescription, QnUrl,"Error publishing New Question", uid);
        }

        public static async Task<JOSResult<string>> UpdateExistingQuestion(QuestionSendToServer existingQuestion, string uid)
        {
            return await SignAndSendDataToServer(existingQuestion, AppResources.QuestionErrorTypeDescription, EditQnUrl, "Error editing question", uid);
        }
        
        public static async Task<JOSResult<string>> SendPlaintextUpvote(PlainTextVoteOnQuestionCommand voteOnQuestion, string uid)
        {
            return await SignAndSendDataToServer(voteOnQuestion, AppResources.QuestionErrorTypeDescription, PlaintextVoteQnUrl, "Error voting on question", uid);
        }

        public static async Task<JOSResult<string>> RequestEmailValidation(ClientSignedUnparsed signedMsg, string email)
        {
            var serverSend = new RequestEmailValidationAPICall()
            {
                email = email,
                message = signedMsg.message,
                signature = signedMsg.signature,
                user = signedMsg.user,
            };
            return await SendDataToServerVerifySignedResponse(serverSend, "temp error msg", EmailValidationUrl);

        }

        public static async Task<JOSResult<string>> SendEmailValidationPin(EmailValidationPIN msg, string uid)
        {
            return await SignAndSendDataToServer(msg, "Sending PIN", EmailValidationPinUrl, "Signing PIN", uid);
        }

        // Sign a message (data) with this user's key, then upload to the specified url. 
        // "description" and "error string" are for reporting errors in upload and signing resp.
        private static async Task<JOSResult<string>> SignAndSendDataToServer<T>(T data, string description, string url, string errorString, string uid)
        {
            var signedUserMessage = ClientSignatureGenerationService.SignMessage(data, uid);
            if (!string.IsNullOrEmpty(signedUserMessage.signature))
            {
                return await SendDataToServerVerifySignedResponse(signedUserMessage, description, url);
            }

            Debug.WriteLine(errorString);
            return new ErrorResult<string>(errorString);
        }

        private static async Task<JOSResult<string>> SendDataToServerVerifySignedResponse<TUpload>(
            TUpload newThing,
            string typeDescription, 
            string uri)
        {
            var serverResponse = await SendDataToServerReturnResponse<TUpload, SignedString>(newThing, typeDescription, uri);

            if (serverResponse.Failure)
            {
                if (serverResponse is ErrorResult<SignedString> errorResult)
                {
                    return new ErrorResult<string>(errorResult.Message);
                }
                // Other general error result, just in case.
                return new ErrorResult<string>("Error sending data to server at " + uri);
            }

            // serverResponse.Success. We got a valid server response - now verify the signature.
            if (!SignatureVerificationService.VerifySignature(serverResponse.Data, ServerPublicKey)) //  
            {
                Debug.WriteLine(@"\t Error registering new " + typeDescription + ": Signature verification failed");
                return new ErrorResult<string>("Server signature verification failed");
            }

            return new SuccessResult<string>(serverResponse.Data.message);
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
        private static async Task<JOSResult<TReturn>> SendDataToServerReturnResponse<TUpload,TReturn>(
            TUpload newThing,
            string typeDescription,
            string uri)
        {
            var httpResponse 
                = await Client.PostGenericItemAsync<ServerResult<TReturn>, TUpload>(newThing, uri);

            // http errors
            if (string.IsNullOrEmpty(httpResponse.Err))
            {
                // Error responses from the server
                if (string.IsNullOrEmpty(httpResponse.Ok.Err))
                {
                    return new SuccessResult<TReturn>(httpResponse.Ok.Ok);
                }
                    
                Debug.WriteLine(@"\tError sending "+typeDescription+": "+httpResponse.Ok.Err);
                return new ErrorResult<TReturn>(httpResponse.Ok.Err ?? "");
            }

            Debug.WriteLine(@"\tError reaching server for sending " +typeDescription+": "+httpResponse.Err);
            return new ErrorResult<TReturn>(httpResponse.Err ?? "");
        }
        
        // At the moment, this simply passes the information back to the user. We might perhaps want
        // to triage errors more carefully, or explain them to users better, or distinguish user-upload
        // errors from question-upload errors.
        // This may also be much more elegantly implementable using other errors in the JOSResult type, rather than
        // the triple now being returned.
        public static (bool isValid, string errorMessage, T returnedData) ValidateHttpResponse<T>(JOSResult<T> response, string messageTopic) where T : new()
        {
            if (response.Success)
            {
                return (true, "", response.Data);
            }
            
            // response.Failure
            var errorMessage = messageTopic + "Server error: ";
            if (response is ErrorResult<T> errorResult)
            {
                errorMessage += errorResult.Message;
            }
            return (false, errorMessage, new T());
        }

        // overload because string doesn't satisfy T: new() 
        
        public static (bool isValid, string errorMessage, string returnedData) ValidateHttpResponse(JOSResult<string> response, string messageTopic) 
        {
            if (response.Success)
            {
                return (true, "", response.Data);
            }
            
            // response.Failure
            var errorMessage = messageTopic + "Server error: ";
            if (response is ErrorResult<string> errorResult)
            {
                errorMessage += errorResult.Message;
            }
            return (false, errorMessage, "");
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
            if (readResult.Failure)
            {
                var errorMessage = "";
                if (readResult is ErrorResult<ServerConfig> errorResult)
                {
                    errorMessage = errorResult.Message;
                }

                Debug.WriteLine("Error reading server config file: " + errorMessage);
                return (key, url);
            }

            // readResult.Success            
            var configData = readResult.Data;
            
            // Remote server use.
            if (configData.remoteServerUse)
            {
                key = configData.remoteServerPublicKey;
                url = configData.url;
            } else // Local server use; special url for Android simulator.
            {
                key = configData.localServerPublicKey; 
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