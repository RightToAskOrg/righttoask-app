using Xamarin.Essentials;

namespace RightToAskClient
{
    public static class Constants
    { 
        public static string RegUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/new_registration" : "http://localhost:8099/new_registration/{0}";
        // TODO Not quite sure whether the /{0} is needed on iPhones - test.
        public static string QnUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/new_question" : "http://localhost:8099/new_question/{0}";
        public static string MPListUrl= DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/MPs.json" : "http://localhost:8099/MPs.json";
        public static string UserListUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/get_user_list" : "http://localhost:8099/get_user_list";
        public static string GeoscapeAPIUrl = "https://api.psma.com.au/beta/v2/addresses/geocoder";
        public static string StoredMPDataFile = "MPs.json";
		public static string APIKeyFileName = "GeoscapeAPIKey";
        public static string PublicServerKeyFileName = "PublicServerKey";

        public static string FakePublicKey = "123";

    }
}
