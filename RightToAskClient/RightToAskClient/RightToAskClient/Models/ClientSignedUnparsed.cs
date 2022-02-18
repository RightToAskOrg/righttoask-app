

using System.Text.Json.Serialization;

namespace RightToAskClient.Models
{
    public class ClientSignedUnparsed
    {
        [JsonPropertyName("message")]
        public string message { get; set; }

        [JsonPropertyName("signature")]
        public string signature { get; set; }

        [JsonPropertyName("user")]
        public string user { get; set; }
    }
}