using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using Xamarin.Essentials;

namespace RightToAskClient.CryptoUtils
{
    public static class ServerSignatureVerificationService
    {
        public static string ServerPubKeyUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/get_server_public_key_spki" : "http://localhost:8099/get_server_public_key_spki"; 

        //public static ECDsa signingService = makeSigningService();
        // private static string SPKI = "MCowBQYDK2VwAyEAOJ/tBn4rOrOebgbICBi3i2oflO0hqz0D8daItDZ53vI=";
        private static string SPKIRaw = "OJ/tBn4rOrOebgbICBi3i2oflO0hqz0D8daItDZ53vI=";
        // private static string SPKIInHex = "389fed067e2b3ab39e6e06c80818b78b6a1f94ed21ab3d03f1d688b43679def2";

        public static readonly Ed25519PublicKeyParameters ServerPublicKey =
            new Ed25519PublicKeyParameters(Convert.FromBase64String(SPKIRaw));

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