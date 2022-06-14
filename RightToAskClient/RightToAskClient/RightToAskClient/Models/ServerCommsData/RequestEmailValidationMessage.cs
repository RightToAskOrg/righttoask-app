using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class RequestEmailValidationMessage
    {
        [JsonPropertyName("why")]
        public EmailValidationReason why {get; set; }

        /// the "name" of the badge. For an MP, the [MP::badge_name], for an organization the domain name, for an account recovery...TBD. Possibly the new key?
        [JsonPropertyName("name")]
        public string name {get; set; }
    }


}
