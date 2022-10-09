using Org.BouncyCastle.Crypto.Parameters;
using RightToAskClient.CryptoUtils;
using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
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
                var ClientPublicKey = new Ed25519PublicKeyParameters(Convert.FromBase64String(App.ReadingContext.ThisParticipant.RegistrationInfo.public_key));
                bool validSig = SignatureVerificationService.VerifySignature(message, signaturebytes, ClientPublicKey);
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