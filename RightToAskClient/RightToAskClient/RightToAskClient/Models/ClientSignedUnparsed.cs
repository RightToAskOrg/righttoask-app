using RightToAskClient.CryptoUtils;
using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models
{
    public class ClientSignedUnparsed
    {
        [JsonPropertyName("message")]
        public string message { get; set; } = "";

        [JsonPropertyName("signature")]
        public string signature { get; set; } = "";

        [JsonPropertyName("user")]
        public string user { get; set; } = "";

        public bool Validate()
        {
            bool isValid = false;
            bool hasInvalidData = false;
            // if user is me, check for valid signature
            if (user == App.ReadingContext.ThisParticipant.RegistrationInfo.uid)
            {
                byte[] signaturebytes = Convert.FromBase64String(signature);
                bool validSig = ServerSignatureVerificationService.VerifySignature(message, signaturebytes, ServerSignatureVerificationService.ServerPublicKey);
                if (validSig)
                {
                    hasInvalidData = false;
                }
                else
                {
                    hasInvalidData = true;
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                hasInvalidData = true;
            }
            isValid = !hasInvalidData;
            return isValid;
        }
    }
}