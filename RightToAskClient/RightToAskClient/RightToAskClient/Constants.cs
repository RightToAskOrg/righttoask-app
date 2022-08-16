using System.Diagnostics;
using System.Text.Json;
using RightToAskClient.Models;
using Xamarin.Essentials;

namespace RightToAskClient
{
    public static class Constants
    {
        public static string ServerConfigFile = "serverconfig.json";
        public static string DefaultLocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android
                        ? "http://10.0.2.2:8099" : "http://localhost:8099";
        // public static string GeoscapeAPIUrl = "https://api.psma.com.au/beta/v2/addresses/geocoder";
        public static string GeoscapeAPIUrl = "https://api.psma.com.au/v2/addresses/geocoder";
        public static string StoredMPDataFile = "MPs.json";
        public static string StoredCommitteeDataFile = "committees.json";
        public static string StoredHearingsDataFile = "hearings.json";
        public static string APIKeyFileName = "GeoscapeAPIKey";
        public static string MapBaseURL = "https://www.abc.net.au/res/sites/news-projects/interactive-electorateboundaries-2/5.0.0/?kml=/dat/news/elections/federal/2022/guide/kml/{0}.kml";
        
        // Preferences storage strings
        public static string IsRegistered = "IsRegistered";
        public static string RegistrationInfo = "RegistrationInfo";
        public static string StateID = "StateID";
        public static string DontShowFirstTimeReadingPopup = "DontShowFirstTimeReadingPopup";
        public static string ShowHowToPublishPopup = "ShowHowToPublishPopup";
        public static string HasQuestions = "HasQuestions";
        public static string MPsKnown = "MPsKnown";
        public static string IsVerifiedMPStafferAccount = "IsVerifiedMPStafferAccount";
        public static string IsVerifiedMPAccount = "IsVerifiedMPAccount";
        public static string MPRegisteredAs= "MPRegisteredAs";
        public static string Address = "Address";
        
        // Special numbers
        public static float similarityThreshold = 2.5F;
    }
}
