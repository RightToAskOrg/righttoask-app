using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using RightToAskClient.Models;
using Xamarin.Essentials;

/* This class generates a local private/public Ed25519 keypair for signing.
 * Eventually, we'd prefer to store this directly in the Keystore, i.e. never to have the private key
 * accessible to the app. However, Android keystore reliability may make this unreliable for some phones
 * - we're investigating.
 * So regard this implementation as a best-effort use of available tools, with the intention of upgrading
 * before production.
 * It may be we eventually do something different for Android and iOS devices.
 * Also note I (VT) am not an expert in Xamarin secure storage, and have endeavoured to follow the instructions
 * here: https://docs.microsoft.com/en-us/xamarin/essentials/secure-storage
 * but the code needs a round of expert review before it is used or copied.
 * TODO: Note that I have not yet done either of the platform-specific setups recommended on that page:
 * we'll need to ask for iOS entitlements, and we'll need to turn off backup on Android.
 *
 * Use of private initialiser to deal with asynchrony is based on suggestion here:
 * https://endjin.com/blog/2020/08/fully-initialize-types-in-constructor-csharp-nullable-async-factory-pattern
 */
namespace RightToAskClient.CryptoUtils
{
    public class ClientSignatureGenerationService
    {
        // private static readonly AsymmetricCipherKeyPair MyKeyPair = await MakeMyKey();

        // private static Ed25519PublicKeyParameters? _myPublicKey  = MyKeyPair.Public as Ed25519PublicKeyParameters;

        //private static Ed25519PrivateKeyParameters MyKeyPair; // = await MakeMyKey();

        private Ed25519PrivateKeyParameters MyKeyPair; // = await MakeMyKey();

        private Ed25519PublicKeyParameters _myPublicKey;

        private static Ed25519Signer MySigner;

        private ClientSignatureGenerationService(Ed25519PrivateKeyParameters myKey)
        {
            MyKeyPair = myKey;
            _myPublicKey = myKey.GeneratePublicKey();
            MySigner = MakeMySigner();
        }

        public static async Task<ClientSignatureGenerationService> CreateClientSignatureGenerationService()
        {
            Ed25519PrivateKeyParameters myKeyPair = await MakeMyKey();
            return new ClientSignatureGenerationService(myKeyPair);
        }

        public string MyPublicKey()
        {
            if (_myPublicKey.GetEncoded() != null)
            {
                return Convert.ToBase64String(_myPublicKey.GetEncoded());
            }
            else
            {
                Debug.WriteLine("Error generating signing key");
                return "";
            }
        }

        private static async Task<Ed25519PrivateKeyParameters> MakeMyKey()
        {
            // First see if there's already a stored key. If so, use that.
            try
            {
                var signingKeyAsString = await SecureStorage.GetAsync("signing_key");

                // If there's an existing key, return it.
                if (!String.IsNullOrEmpty(signingKeyAsString))
                {
                    var privateKey = new Ed25519PrivateKeyParameters(Convert.FromBase64String(signingKeyAsString));
                    return privateKey;
                }
            }
            // If there's an exception, write a debug message and continue through generating a new one.
            // TODO Note it's not 100% clear that this is the right thing to do if there was an exception
            // attempting to retrieve one. It's possible we want a more careful examination of whether we
            // expect a key to be there.
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            // If there isn't already a stored key, generate a new one 
            var keyPairGenerator = new Ed25519KeyPairGenerator();

            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            Ed25519PrivateKeyParameters? signingKey = keyPairGenerator.GenerateKeyPair().Private as Ed25519PrivateKeyParameters;

            // and store it. 
            try
            {
                var encodedSigningKey = signingKey?.GetEncoded() ?? Array.Empty<byte>();
                string keyAsString = Convert.ToBase64String(encodedSigningKey);
                if (!String.IsNullOrEmpty(keyAsString))
                {
                    await SecureStorage.SetAsync("signing_key", keyAsString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error storing signing key" + ex.Message);
            }

            return signingKey;
        }

        private Ed25519Signer MakeMySigner()
        {
            var signer = new Ed25519Signer();
            signer.Init(true, MyKeyPair);
            return signer;
        }

        public ClientSignedUnparsed SignMessage<T>(T message, string userID)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            
            string serializedMessage = "";
            byte[] messageBytes;
            string sig = "";

            try
            {
                serializedMessage = JsonSerializer.Serialize(message, serializerOptions);
                // serializedMessage = JsonSerializer.Serialize(message);
                messageBytes = Encoding.UTF8.GetBytes(serializedMessage);

                MySigner.BlockUpdate(messageBytes, 0, messageBytes.Length);
                sig = Convert.ToBase64String(MySigner.GenerateSignature());
            }
            catch (JsonException e)
            {
                // TODO Deal with Json serialisation problem
                Debug.WriteLine("Json Exception: " + e.Message);
            }
            catch (InvalidOperationException e)
            {
                // TODO Something went wrong with signing
                Debug.WriteLine("Invalid Operation Exception: " + e.Message);
            }
            catch (Exception e)
            {
                // TODO Something else went wrong
                Debug.WriteLine("Generic Exception: " + e.Message);
            }

            return new ClientSignedUnparsed()
            {
                message = serializedMessage,
                signature = sig,
                user = userID
            };
        }

        public ClientSignedUnparsed SignMessageWithOptions<T>(T message, string userID)
        {
            string serializedMessage = "";
            byte[] messageBytes;
            string sig = "";

            // json serializer options
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                serializedMessage = JsonSerializer.Serialize(message, options);
                messageBytes = Encoding.UTF8.GetBytes(serializedMessage);

                MySigner.BlockUpdate(messageBytes, 0, messageBytes.Length);
                sig = Convert.ToBase64String(MySigner.GenerateSignature());
            }
            catch (JsonException e)
            {
                // TODO Deal with Json serialisation problem
                Debug.WriteLine("Json Exception: " + e.Message);
            }
            catch (InvalidOperationException e)
            {
                // TODO Something went wrong with signing
                Debug.WriteLine("Invalid Operation Exception: " + e.Message);
            }
            catch (Exception e)
            {
                // TODO Something else went wrong
                Debug.WriteLine("Generic Exception: " + e.Message);
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