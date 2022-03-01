using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using RightToAskClient.Models;
using Xamarin.Essentials;

/* This class generates a local private/public Ed25519 keypair for signing.
 * Eventually, we'd prefer to store this directly in the Keystore, i.e. never to have the private key
 * accessible to the app. However, Android keystore reliability may make this impossible - we're investigating.
 * So regard this implementation as a best-effort use of available tools, with the intention of upgrading
 * before production.
 * It may be we eventually do something different for Android and iOS devices.
 * Also note I (VT) am not an expert in Xamarin secure storage, and have endeavoured to follow the instructions
 * here: https://docs.microsoft.com/en-us/xamarin/essentials/secure-storage?tabs=android
 * but the code needs a round of expert review before it is used or copied.
 * TODO: Note that I have not yet done either of the platform-specific setups recommended on that page:
 * we'll need to ask for iOS entitlements, and we'll need to turn off backup on Android.
 */
namespace RightToAskClient.CryptoUtils
{
    public static class ClientSignatureGenerationService
    {
        private static readonly AsymmetricCipherKeyPair MyKeyPair = MakeMyKey();

        private static Ed25519PublicKeyParameters? _myPublicKey  = MyKeyPair.Public as Ed25519PublicKeyParameters;

        private static readonly Ed25519Signer MySigner = MakeMySigner();
        
        public static string MyPublicKey()
        {
            if(_myPublicKey?.GetEncoded() != null)
            {
                return Convert.ToBase64String(_myPublicKey.GetEncoded());
            }
            else
            {
                Debug.WriteLine("Error generating signing key");
                return "";
            }
        }

        // TODO at the moment, this just generates a new key every time you run the app.
        // private static AsymmetricCipherKeyPair MakeMyKey()
        private static AsymmetricCipherKeyPair MakeMyKey()
        {
            var keyPairGenerator = new Ed25519KeyPairGenerator();

            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            return keyPairGenerator.GenerateKeyPair();
        }

        private static Ed25519Signer MakeMySigner()
        {
            var signer = new Ed25519Signer();
            signer.Init(true, MyKeyPair.Private);
            return signer;
        }

        public static ClientSignedUnparsed SignMessage<T>(T message, string userID)
        {
            string serializedMessage = "";
            byte[] messageBytes;
            string sig = "";
            
            try
            {
                serializedMessage = JsonSerializer.Serialize(message);
                messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
                
                MySigner.BlockUpdate(messageBytes, 0, messageBytes.Length);
                sig = Convert.ToBase64String(MySigner.GenerateSignature());
            }
            catch (JsonException e)
            {
                // TODO Deal with Json serialisation problem
            }
            catch (InvalidOperationException e)
            {
                // TODO Something went wrong with signing
            }
            catch (Exception e)
            {
                // TODO Something else went wrong
            }
                
            return new ClientSignedUnparsed()
            {
                message = serializedMessage,
                signature = sig, 
                user = userID
            };

        }
    }
}