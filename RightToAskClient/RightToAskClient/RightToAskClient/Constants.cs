using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;


namespace RightToAskClient
{
    public static class Constants
    { 
	// URL of REST service
        //public static string RestUrl = "https://YOURPROJECT.azurewebsites.net:8081/api/todoitems/{0}";

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        // public static string RestUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000/api/todoitems/{0}" : "http://localhost:5000/api/todoitems/{0}";
        public static string RegUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/new_registration" : "http://localhost:8099/new_registration/{0}";
        public static string UserListUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/get_user_list" : "http://localhost:8099/get_user_list";
        public static string ServerPubKeyUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/get_server_public_key_spki" : "http://localhost:8099/get_server_public_key_spki"; 

        //public static ECDsa signingService = makeSigningService();
        private static AsymmetricCipherKeyPair myKeyPair = MakeMyKey();
        public static Ed25519PublicKeyParameters myPublicKey  = (Ed25519PublicKeyParameters) myKeyPair.Public;
        public static Ed25519Signer mySigner = MakeMySigner();

        public const string DatabaseFilename = "TodoSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        private static AsymmetricCipherKeyPair MakeMyKey()
        {
            var keyPairGenerator = new Ed25519KeyPairGenerator();
        
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            return keyPairGenerator.GenerateKeyPair();
        }

        private static Ed25519Signer MakeMySigner()
        {
            var signer = new Ed25519Signer();
            signer.Init(true, myKeyPair.Private);
            return signer;
        }
        // TODO: change this to EDDSA to match the sigs.
        /*
        private static ECDsa makeSigningService()
        {
            ECDsa service = ECDsa.Create();

            string SPKI = "MCowBQYDK2VwAyEAOJ/tBn4rOrOebgbICBi3i2oflO0hqz0D8daItDZ53vI=";
            byte[] SPKIbytes = Convert.FromBase64String(SPKI);
            int numRead;
            service.ImportSubjectPublicKeyInfo;
            ECDsa.ImportSubjectPublicKeyInfo(SPKIbytes, numRead);

        }
        */
        
        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }
}
