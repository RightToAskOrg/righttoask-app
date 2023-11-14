using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData

/* For sending to the server in order to apply for email validation
 * This data structure is exactly the same as ClientSignedUnparsed, except for the
 * addition of the unsigned data (email). At the moment, it is the only such structure,
 * but there may be others later. Consider a refactor.
 */
{
    public class RequestEmailValidationAPICall
    {
        [JsonPropertyName("email")]
        public string email { get; set; }
        
        [JsonPropertyName("message")]
        public string message { get; set; }
        
        [JsonPropertyName("signature")]
        public string signature { get; set; }
        
        [JsonPropertyName("user")]
        public string user { get; set; }
    }
}