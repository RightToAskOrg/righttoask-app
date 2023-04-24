using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

// Data structure for serialising question data to send to the server,
// either for new questions or for updating existing questions.
// Note that these fields should have the same names and types as the equivalent
// fields in QuestionReceiveFromServer, though the set of fields is not
// exactly the same.
namespace RightToAskClient.Models.ServerCommsData
{
    public class QuestionSendToServer
    {

        // Question-defining fields.
        // The only question-defining field we need is question_text.
        // Note that author isn't necessary because the server knows
        // which user uploaded it, and timestamp isn't sent by the client.
        [JsonPropertyName("question_text")]
        public string? question_text { get; set; }

        // bookkeeping fields
        [JsonPropertyName("question_id")]
        public string? question_id { get; set; }
        
        [JsonPropertyName("version")]
        public string? version { get; set; }
        
        // non-defining fields
        [JsonPropertyName("background")]
        public string? background { get; set; }
        
        // TODO Note that the entity that should ask the question might be a
        // committee or other RTA participant, not necessarily an MP.
        // Not clear whether we want a separate field for each of these possibilities
        // - possibly we do.
        [JsonPropertyName("mp_who_should_ask_the_question")]
        public List<PersonID>? mp_who_should_ask_the_question { get; set; }

        [JsonPropertyName("who_should_ask_the_question_permissions")]
        public RTAPermissions who_should_ask_the_question_permissions { get; set; }

        [JsonPropertyName("entity_who_should_answer_the_question")]
        public List<PersonID>? entity_who_should_answer_the_question { get; set; }

        [JsonPropertyName("who_should_answer_the_question_permissions")]
        public RTAPermissions who_should_answer_the_question_permissions { get; set; }

        [JsonPropertyName("answers")]
        // public List<Tuple<string, PersonID, MPId>> answers { get; set; } 
        public List<QuestionAnswer>? answers { get; set; } 
        
        // TODO This needs to be interpreted/stored carefully, because we need 'false'
        // to mean 'the user has seen it but didn't fully approve' and to distinguish this
        // from 'the user hasn't seen or hasn't rated the answer'
        // Consider whether it'd make more sense to have an enum.
        [JsonPropertyName("answer_accepted")]
        public bool? answer_accepted { get; set; }
        
        [JsonPropertyName("hansard_link")]
        public List<HansardLink>? hansard_link { get; set; } 
        
        [JsonPropertyName("is_followup_to")]
        public string? is_followup_to { get; set; }

        public QuestionSendToServer()
        {
            is_followup_to = "test";
        }
        
        /* This is only ever used for sending *new* questions to the server, so this constructor ignores elements of
         * question that are not sent during initial upload.
         */
        public QuestionSendToServer(Question question)
        {
            if (!string.IsNullOrEmpty(question.QuestionText))
            {
                question_text = question.QuestionText;
            }

                    
            if (!string.IsNullOrEmpty(question.Background))
            {
                background = question.Background;
            }

            entity_who_should_answer_the_question = question.Filters.TranscribeQuestionAnswerersForUpload();
            mp_who_should_ask_the_question = question.Filters.TranscribeQuestionAskersForUpload();

            who_should_answer_the_question_permissions = question.WhoShouldAnswerTheQuestionPermissions;
            who_should_ask_the_question_permissions = question.WhoShouldAskTheQuestionPermissions;

            if (!string.IsNullOrEmpty(question.IsFollowupTo))
            {
                is_followup_to = question.IsFollowupTo;
            }
        }
        
        public bool ValidateNewQuestion()
        {
            var isValid = false;
            // just needs question text for new questions
            if (!string.IsNullOrEmpty(question_text))
            {
                isValid = true;
            }
            return isValid;
        }

        public bool ValidateUpdateQuestion()
        {
            var isValid = false;
            // needs more fields to update an existing question
            if (!string.IsNullOrEmpty(question_text)
                && !string.IsNullOrEmpty(question_id)
                && !string.IsNullOrEmpty(version))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}
