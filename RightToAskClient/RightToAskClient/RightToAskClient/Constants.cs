﻿using Xamarin.Essentials;
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
        public static string MPListUrl= DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/MPs.json" : "http://localhost:8099/MPs.json";
        public static string UserListUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/get_user_list" : "http://localhost:8099/get_user_list";
        public static string GeoscapeAPIUrl = "https://api.psma.com.au/beta/v2/addresses/geocoder";
        public static string StoredMPDataFile = "MPs.json";
		public static string aPIKeyFileName = "GeoscapeAPIKey";

        public static string FakePublicKey = "123";

    }
}
