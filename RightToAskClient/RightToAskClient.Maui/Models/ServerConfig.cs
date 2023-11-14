using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models
{
    public class ServerConfig
    {

        [JsonPropertyName("remoteServerUse")] 
        public bool remoteServerUse { get; set; } = false;

        [JsonPropertyName("url")]
        public string url { get; set; } = "";
        
        [JsonPropertyName("remoteServerPublicKey")]
        public string remoteServerPublicKey { get; set; } = "";
        
        [JsonPropertyName("localServerPublicKey")]
        public string localServerPublicKey { get; set; } = "";
    }
}