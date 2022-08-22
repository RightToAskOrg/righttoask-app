using System;
using System.Diagnostics;
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
using RightToAskClient.Models.ServerCommsData;
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
    public static class ClientSignatureGenerationService
    {
        private static Ed25519PrivateKeyParameters? _myKeyPair ; // = await MakeMyKey();

        private static Ed25519Signer MySigner = new Ed25519Signer();

        private static bool _initSuccessful = false;
        public static bool InitSuccessful
        {
            get => _initSuccessful;
            private set => _initSuccessful = value;
        }
        /*
        static ClientSignatureGenerationService(Ed25519PrivateKeyParameters myKey)
        {
            MyKeyPair = myKey;
            _myPublicKey = myKey.GeneratePublicKey();
            MySigner.Init(true, MyKeyPair);
        } */
        
        // Constructor
        /*
        static ClientSignatureGenerationService()
        {
            
        }
        */

        public static async Task<bool> Init()
        {
            var generationResult = await MakeMyKey();
            if (!String.IsNullOrEmpty(generationResult.Err))
            {
                // Key generation unsuccessful.
                return false;
            }
            
            _myKeyPair = generationResult.Ok;
            // _myPublicKey = _myKeyPair.GeneratePublicKey();
            MySigner.Init(true, _myKeyPair);
            InitSuccessful = true;
            return true;
        }

        /*
        public static async Task<ClientSignatureGenerationService> CreateClientSignatureGenerationService()
        {
            Ed25519PrivateKeyParameters myKeyPair = await MakeMyKey();
            return new ClientSignatureGenerationService(myKeyPair);
        }
        */

        public static string MyPublicKey
        {
            get => Convert.ToBase64String(_myKeyPair?.GeneratePublicKey().GetEncoded() ?? Array.Empty<byte>());
        }
        /*
        public static string MyPublicKey()
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
        */

        private static async Task<Result<Ed25519PrivateKeyParameters>> MakeMyKey()
        {
            // First see if there's already a stored key. If so, use that.
            try
            {
                var signingKeyAsString = await SecureStorage.GetAsync("signing_key");

                // If there's an existing key, return it.
                if (!String.IsNullOrEmpty(signingKeyAsString))
                {
                    var privateKey = new Ed25519PrivateKeyParameters(Convert.FromBase64String(signingKeyAsString));
                    return new Result<Ed25519PrivateKeyParameters>()
                    {
                        Ok = privateKey
                    };
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
            // Better not to try registering a key that you haven't successfully stored for later.
            catch (Exception ex)
            {
                Debug.WriteLine("Error storing signing key" + ex.Message);
                return new Result<Ed25519PrivateKeyParameters>()
                {
                    Err = "Error storing signing key" + ex.Message
                };
            }

            if (signingKey is null)
            {
                return new Result<Ed25519PrivateKeyParameters>()
                {
                    Err = "Error generating signing key"
                };
            }
            
            // signingKey is guaranteed not to be null. All good.
            return new Result<Ed25519PrivateKeyParameters>()
            {
                Ok = signingKey
            };
        }

        public static ClientSignedUnparsed SignMessage<T>(T message, string userID)
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