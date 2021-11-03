using System.Text;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace RightToAskClient.CryptoUtils
{
    public class SignatureService
    {
        public byte[] SignMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            Constants.mySigner.BlockUpdate(messageBytes, 0, messageBytes.Length);

            return Constants.mySigner.GenerateSignature();
        }

        public bool VerifySignature(string message, byte[] signature, Ed25519PublicKeyParameters publicKey)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            var validator = new Ed25519Signer();
            validator.Init(false, publicKey);
            validator.BlockUpdate(messageBytes, 0, messageBytes.Length);

           return validator.VerifySignature(signature); 
        }

    }
}