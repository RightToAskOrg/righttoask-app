using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public class EmailValidationPIN
    {
        [JsonPropertyName("hash")]
        public string hash { get; set; }

        [JsonPropertyName("code")]
        public int code { get; set; }
    }
}