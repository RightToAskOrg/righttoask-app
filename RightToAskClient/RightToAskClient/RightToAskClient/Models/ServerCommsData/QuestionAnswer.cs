using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models.ServerCommsData
{
    public class QuestionAnswer
    {

        // Should be omitted on sending - filled in by server.
        [JsonPropertyName("answered_by")] 
        public string? answered_by { get; set; } // uid.

        [JsonPropertyName("mp")] 
        public MPId mp { get; set; }

        [JsonPropertyName("answer")] 
        public string answer { get; set; }
    }
}