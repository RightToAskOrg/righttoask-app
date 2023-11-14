using System.Text.Json.Serialization;

// Stores the relative weights of various search criteria, to inform the server of those priorities.

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public class Weights
    {
        [JsonPropertyName("metadata")] 
        public int metadata { get; set; } 
        
        [JsonPropertyName("net_votes")] 
        public int net_votes { get; set; } 
        
        [JsonPropertyName("recentness")] 
        public int recentness { get; set; } 
        
        [JsonPropertyName("recentness_timescale")] 
        public int recentness_timescale { get; set; } 
        
        [JsonPropertyName("text")] 
        public int text { get; set; } 
        
        [JsonPropertyName("total_votes")] 
        public int total_votes { get; set; } 
    }
}