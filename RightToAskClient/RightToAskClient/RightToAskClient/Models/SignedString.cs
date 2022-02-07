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

        // TODO Fix this 
        public bool verifies(Ed25519PublicKeyParameters pubKey)
        {
            byte[] messagebytes = Encoding.UTF8.GetBytes(message);
            byte[] signaturebytes = Convert.FromBase64String(signature);

            return SignatureService.VerifySignature(message, signaturebytes, pubKey);
            // PUBKEYTBD.VerifyData (messagebytes, signaturebytes, HashAlgorithmName.SHA512);
        }
    }
}