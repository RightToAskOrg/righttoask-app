using System.Collections.Generic;
using System.Text.Json.Serialization;

// Stores the relative weights of various search criteria, to inform the server of those priorities.

namespace RightToAskClient.Models.ServerCommsData
{
    public class SortedQuestionList 
    {
        [JsonPropertyName("token")] 
        public string token { get; set; } 
        
        [JsonPropertyName("questions")] 
        public List<ScoredIDs> questions { get; set; } 
    }
}
