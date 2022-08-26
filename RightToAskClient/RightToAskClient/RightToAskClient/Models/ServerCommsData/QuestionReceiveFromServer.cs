using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

// Data structure for deserialising question data received from the server.
// Note that these fields should have the same names and types as the equivalent
// fields in QuestionSendToServer, though the set of fields is not
// exactly the same.
namespace RightToAskClient.Models.ServerCommsData
{
    public class QuestionReceiveFromServer
    {
        // Question-defining fields.
        [JsonPropertyName("author")]
        public string? author { get; set; }
        
        [JsonPropertyName("question_text")]
        public string? question_text { get; set; }
        
        [JsonPropertyName("timestamp")]
        public int? timestamp { get; set; }
        
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
        // public List<(string, PersonID, MPId)>? answers { get; set; } 
        public List<(string answer, PersonID? answered_by, MPId mp) >? answers { get; set; } 
        
        // TODO This needs to be interpreted/stored carefully, because we need 'false'
        // to mean 'the user has seen it but didn't fully approve' and to distinguish this
        // from 'the user hasn't seen or hasn't rated the answer'
        // Consider whether it'd make more sense to have an enum.
        [JsonPropertyName("answer_accepted")]
        public bool? answer_accepted { get; set; }
        
        [JsonPropertyName("hansard_link")]
        public List<Uri>? hansard_link { get; set; } 
        
        [JsonPropertyName("is_followup_to")]
        public string? is_followup_to { get; set; }
        
        public bool Validate()
        {
            bool isValid = false;
            // might need to include timestamp as well
            if (!string.IsNullOrEmpty(author)
                && !string.IsNullOrEmpty(question_id)
                && !string.IsNullOrEmpty(question_text)
                && !string.IsNullOrEmpty(version))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}
