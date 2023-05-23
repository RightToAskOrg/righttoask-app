using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    // For the server, this is an enum, which is either EmailSent(token) or AlreadyValidated(Option(token))
    // However, C# doesn't have parameterised enums, so we can't easily express that one or the other is non-empty.
    public class RequestEmailValidationResponse
    {
        [JsonPropertyName("EmailSent")]
        public string EmailSent { get; set; }
        
        [JsonPropertyName("AlreadyValidated")]
        public string AlreadyValidated { get; set; }
    }
}