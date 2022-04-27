using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class NewQuestionReceiveFromServer
    {
        [JsonPropertyName("author")]
        public string? author { get; set; }
        [JsonPropertyName("question_text")]
        public string? question_text { get; set; }
        [JsonPropertyName("background")]
        public string? background { get; set; }
        [JsonPropertyName("timestamp")]
        public int? timestamp { get; set; }
        
        // TODO Note that the entity that should ask the question might be a
        // committee or other RTA participant, not necessarily an MP.
        // Not clear whether we want a separate field for each of these possibilities
        // - possibly we do.
        [JsonPropertyName("mp_who_should_ask_the_question")]
        public List<Entity>? mp_who_should_ask_the_question { get; set; }
        
        [JsonPropertyName("who_should_ask_the_question_permissions")]
        public RTAPermissions? who_should_ask_the_question_permissions { get; set; }
        
        [JsonPropertyName("entity_who_should_answer_the_quetion")]
        public List<Entity>? entity_who_should_answer_the_question { get; set; }
        
        [JsonPropertyName("who_should_answer_the_question_permissions")]
        public RTAPermissions? who_should_answer_the_question_permissions { get; set; }
        [JsonPropertyName("question_id")]
        public string? question_id { get; set; }
        [JsonPropertyName("version")]
        public string? version { get; set; }
        [JsonPropertyName("last_modified")]
        public int? last_modified { get; set; }
    }
}
