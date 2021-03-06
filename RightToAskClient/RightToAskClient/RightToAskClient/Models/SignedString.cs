using System;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using RightToAskClient.CryptoUtils;

namespace RightToAskClient.Models
{
    // Signature is base64 encoded.
    public class SignedString
    {
        public string message { get; set; } = "";
        public string signature { get; set; } = "";

        public bool verifies(Ed25519PublicKeyParameters pubKey)
        {
            byte[] signaturebytes = Convert.FromBase64String(signature);

            return ServerSignatureVerificationService.VerifySignature(message, signaturebytes, pubKey);
        }
    }
}