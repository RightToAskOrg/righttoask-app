using System;
using System.Diagnostics;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using Xamarin.Essentials;

namespace RightToAskClient.CryptoUtils
{
    public static class ServerSignatureVerificationService
    {
        public static Ed25519PublicKeyParameters ServerPublicKey = ConvertSPKIRawToBase64String().Ok;
        
        private static Result<Ed25519PublicKeyParameters> ConvertSPKIRawToBase64String()
        {
            try
            {
                ServerPublicKey = new Ed25519PublicKeyParameters(Convert.FromBase64String(RTAClient.ServerPublicKey));
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error decoding base 64 string. Try removing \" characters from the beginning or end of your PublicServerKey or GeoscapeAPIKey file.");
                Debug.WriteLine(ex.Message);
                return new Result<Ed25519PublicKeyParameters>() { Err = "Error decoding base 64 string." + ex.Message };
            }
            return new Result<Ed25519PublicKeyParameters>() { Ok = ServerPublicKey }; 
        }

        public static bool VerifySignature(string message, byte[] signature, Ed25519PublicKeyParameters publicKey)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            var validator = new Ed25519Signer();
            validator.Init(false, publicKey);
            validator.BlockUpdate(messageBytes, 0, messageBytes.Length);

           return validator.VerifySignature(signature); 
        }

    }
}