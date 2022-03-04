using System.Text.Json.Serialization;

namespace RightToAskClient.Models
{
    public class ServerConfig
    {

        [JsonPropertyName("remoteServerUse")] 
        public string remoteServerUse { get; set; } = "";

        [JsonPropertyName("url")]
        public string url { get; set; } = "";
    }
}