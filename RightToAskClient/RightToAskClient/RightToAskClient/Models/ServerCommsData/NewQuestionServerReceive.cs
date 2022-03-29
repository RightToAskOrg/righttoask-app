using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class NewQuestionServerReceive
    {
        [JsonPropertyName("author")]
        public string? author { get; set; }
        [JsonPropertyName("question_text")]
        public string? question_text { get; set; }
        [JsonPropertyName("background")]
        public string? background { get; set; }
        [JsonPropertyName("timestamp")]
        public int? timestamp { get; set; }
        [JsonPropertyName("who_should_ask_the_question_permissions")]
        public string? who_should_ask_the_question_permissions { get; set; }
        [JsonPropertyName("who_should_answer_the_question_permissions")]
        public string? who_should_answer_the_question_permissions { get; set; }
        [JsonPropertyName("question_id")]
        public string? question_id { get; set; }
        [JsonPropertyName("version")]
        public string? version { get; set; }
        [JsonPropertyName("last_modified")]
        public int? last_modified { get; set; }
    }
}
