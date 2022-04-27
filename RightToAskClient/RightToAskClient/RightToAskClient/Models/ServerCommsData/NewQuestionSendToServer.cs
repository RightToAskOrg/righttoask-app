using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class NewQuestionSendToServer
    {

        [JsonPropertyName("question_text")]
        public string? question_text { get; set; }
        //[JsonPropertyName("question_writer")]
        //public string question_writer { get; set; }
        //[JsonPropertyName("upload_timestamp")]
        //public DateTime upload_timestamp { get; set; }

        // bookkeeping fields
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("question_id")]
        public string? question_id { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
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

        // TODO: Check whether this data structure lines up with the 'QuestionAnswer' type 
        // - possibly we want to implement that explicitly.
        [JsonPropertyName("answers")]
        public List<Tuple<string, Person, MP>>? answers { get; set; } 
        
        // TODO This needs to be interpreted/stored carefully, because we need 'false'
        // to mean 'the user has seen it but didn't fully approve' and to distinguish this
        // from 'the user hasn't seen or hasn't rated the answer'
        // Consider whether it'd make more sense to have an enum.
        [JsonPropertyName("answer_accepted")]
        public bool? answer_accepted { get; set; }
        
        [JsonPropertyName("hansard_link")]
        public List<Uri> hansard_link { get; set; } 
        
        [JsonPropertyName("is_followup_to")]
        public string? is_followup_to { get; set; } 
        
        //[JsonPropertyName("keywords")]
        //public List<string> keywords { get; set; }
        //[JsonPropertyName("category")]
        //public List<string> category { get; set; }
        //[JsonPropertyName("expiry_date")]
        //public DateTime expiry_date { get; set; }

        
        public NewQuestionSendToServer () {}
        public NewQuestionSendToServer(Question question)
        {
            if (!String.IsNullOrEmpty(question.QuestionText))
            {
                question_text = question.QuestionText;
            }

            if (!String.IsNullOrEmpty(question.Background))
            {
                background = question.Background;
            }

            //TODO: Add other structures.
            // we *don't* need QuestionID or version, but it's 
            //possibly clearer to have them anyway.
        }

    }
}
