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
            var isValid = false;
            var hasInvalidData = false;
            // if user is me, check for valid signature
            if (user == IndividualParticipant.ProfileData.RegistrationInfo.uid)
            {
                var signaturebytes = Convert.FromBase64String(signature);
                Ed25519PublicKeyParameters ClientPublicKey;
                try
                {
                    ClientPublicKey = new Ed25519PublicKeyParameters(Convert.FromBase64String(IndividualParticipant.ProfileData.RegistrationInfo.public_key));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new ArgumentException();
                }
                
                var validSig = SignatureVerificationService.VerifySignature(message, signaturebytes, ClientPublicKey);
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