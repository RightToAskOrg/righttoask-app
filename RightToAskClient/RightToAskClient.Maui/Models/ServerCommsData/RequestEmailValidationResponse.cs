using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    // For the server, this is an enum, which is either EmailSent(token) or AlreadyValidated(Option(token))
    // However, C# doesn't have parameterised enums, so we can't easily express that one or the other is non-empty.
    public class RequestEmailValidationResponse
    {
        [JsonPropertyName("EmailSent")] 
        public string EmailSent { get; set; } = "";
        
        [JsonPropertyName("AlreadyValidated")]
        public string AlreadyValidated { get; set; } = "";

        // Note that AlreadyValidated may not send a non-empty string. So we just test for EmailSent and otherwise
        // assume AlreadyValidated.
        public bool IsEmailSent => !String.IsNullOrEmpty(EmailSent);

        // Must be one or the other, not both.
        public bool IsValid => String.IsNullOrEmpty(EmailSent) || String.IsNullOrEmpty(AlreadyValidated);
    }
}