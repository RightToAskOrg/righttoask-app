using System.Text.Json.Serialization;

// For pagination when requesting questions from the server.
namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public class QuestionListPage
    {
        [JsonPropertyName("from")] 
        public int from { get; set; } 
        
        [JsonPropertyName("to")] 
        public int to { get; set; } 
    }
}