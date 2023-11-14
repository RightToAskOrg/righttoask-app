using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public class ScoredIDs
    {
        [JsonPropertyName("id")] 
        public string id { get; set; } = "";

        [JsonPropertyName("score")] 
        public float score { get; set; } = 0;
    }
}
