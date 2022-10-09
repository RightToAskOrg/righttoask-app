using System;
using System.Diagnostics;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.CryptoUtils
{
    public static class SignatureVerificationService
    {

        public static bool VerifySignature(string message, byte[] signature, Ed25519PublicKeyParameters publicKey)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            var validator = new Ed25519Signer();
            validator.Init(false, publicKey);
            validator.BlockUpdate(messageBytes, 0, messageBytes.Length);

           return validator.VerifySignature(signature); 
        }

        public static bool VerifySignature(SignedString signedString, string publicKey)
        {
            var keyResult = ConvertSPKIRawToBase64String(publicKey);
            if (!String.IsNullOrEmpty(keyResult.Err))
            {
                return false;
            }
            
            byte[] signaturebytes = Convert.FromBase64String(signedString.signature);
            return VerifySignature(signedString.message, signaturebytes, keyResult.Ok);
        }
        private static Result<Ed25519PublicKeyParameters> ConvertSPKIRawToBase64String(string keyAsString)
        {
            Ed25519PublicKeyParameters key;
            try
            {
                key = new Ed25519PublicKeyParameters(Convert.FromBase64String(keyAsString));
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error decoding base 64 string. Try removing \" characters from the beginning or end of your PublicServerKey or GeoscapeAPIKey file.");
                Debug.WriteLine(ex.Message);
                return new Result<Ed25519PublicKeyParameters>() { Err = "Error decoding base 64 string." + ex.Message };
            }
            return new Result<Ed25519PublicKeyParameters>() { Ok = key }; 
        } 
    }
}