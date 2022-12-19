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
            if (user == IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid)
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("--- valid user");
                Console.WriteLine("--- current signature: " + signature);
                var signaturebytes = Convert.FromBase64String(signature);
                Console.WriteLine("--- decoded signature");
                Ed25519PublicKeyParameters ClientPublicKey;
                try
                {
                    ClientPublicKey = new Ed25519PublicKeyParameters(Convert.FromBase64String(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.public_key));
                    Console.WriteLine("--- Got the public key");
                }
                catch (Exception e)
                {
                    Console.WriteLine("--- Throw exception: " + e);
                    Console.WriteLine(e);
                    throw new ArgumentException();
                }
                Console.WriteLine("--- about to get signature");
                var validSig = SignatureVerificationService.VerifySignature(message, signaturebytes, ClientPublicKey);
                if (validSig)
                {
                    Console.WriteLine("--- valid signature");
                    hasInvalidData = false;
                }
                else
                {
                    Console.WriteLine("--- invalid signature");
                    hasInvalidData = true;
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine("--- has invalid data");
                hasInvalidData = true;
            }
            isValid = !hasInvalidData;
            return isValid;
        }
    }
}