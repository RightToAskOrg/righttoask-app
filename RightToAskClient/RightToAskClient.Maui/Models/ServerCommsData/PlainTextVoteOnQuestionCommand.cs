using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public class PlainTextVoteOnQuestionCommand
    {
        
        [JsonPropertyName("question_id")] 
        public string question_id { get; set; } 

        [JsonPropertyName("up")] 
        public bool up { get; set; }
    }
}