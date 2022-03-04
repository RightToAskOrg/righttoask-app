using System.Diagnostics;
using System.Text.Json;
using RightToAskClient.Models;
using Xamarin.Essentials;

namespace RightToAskClient
{
    public static class Constants
    {
        private static string ServerConfigFile = "server.config";
        private static ServerConfig serverConf = GetServerConfig();
        private static string BaseUrl = serverConf.remoteServerUse == "true" ? serverConf.url :
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099" : "http://localhost:8099";

        // TODO Not quite sure whether the /{0} is needed on iPhones - test.
        private static string MacExtra = DeviceInfo.Platform == DevicePlatform.Android ? "" : "/{0}";
        public static string RegUrl = BaseUrl + "/new_registration" + MacExtra;
        public static string QnUrl = BaseUrl + "/new_question" + MacExtra;
        public static string MPListUrl = BaseUrl + "/MPs.json";
        public static string UserListUrl = BaseUrl + "/get_user_list";
        public static string GeoscapeAPIUrl = "https://api.psma.com.au/beta/v2/addresses/geocoder";
        public static string StoredMPDataFile = "MPs.json";
        public static string APIKeyFileName = "GeoscapeAPIKey";
        public static string PublicServerKeyFileName = "PublicServerKey";

        // Tries to read server config, returns true and the url if there's a valid configuration file
        // specifying that that url is to be used.
        // If the config file doesn't say to use a remote server, or if it can't be read or parsed, default to localhost.
        private static ServerConfig GetServerConfig()
        {
            var serialiserOptions = new JsonSerializerOptions();
            Result<ServerConfig> readResult = FileIO.ReadDataFromStoredJson<ServerConfig>(ServerConfigFile, serialiserOptions);

            if (!readResult.Err.IsNullOrEmpty())
            {
                Debug.WriteLine("Error reading server config file: "+readResult.Err);
                return new ServerConfig{ remoteServerUse = "false", url = ""};
            }

            if (readResult.Ok.remoteServerUse == "true")
            {
                if (!readResult.Ok.url.IsNullOrEmpty())
                {
                    return readResult.Ok;
                }
            }

            return new ServerConfig{ remoteServerUse = "false", url = ""};
        }



    }
}
