using System;
using System.Security.Cryptography;
using System.Text;

namespace RightToAskClient.Models
{
    // Signature is base64 encoded.
    public class SignedString
    {
        public string message { get; set; }
        public string signature { get; set; }

        // TODO Fix this 
        public bool verifies()
        {
            byte[] messagebytes = Encoding.UTF8.GetBytes(message);
            byte[] signaturebytes = Convert.FromBase64String(signature);

            return true;
            // PUBKEYTBD.VerifyData (messagebytes, signaturebytes, HashAlgorithmName.SHA512);
        }
    }
}