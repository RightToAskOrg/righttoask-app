using RightToAskClient.Models.ServerCommsData;
using Xamarin.Essentials;

namespace RightToAskClient
{
    public static class Constants
    {
        public static readonly string ServerConfigFile = "serverconfig.json";
        public static readonly string DefaultLocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android
                        ? "http://10.0.2.2:8099" : "http://localhost:8099";
        // public static readonly string GeoscapeAPIUrl = "https://api.psma.com.au/beta/v2/addresses/geocoder";
        public static readonly string GeoscapeAPIUrl = "https://api.psma.com.au/v2/addresses/geocoder";
        public static readonly string StoredMPDataFile = "MPs.json";
        public static readonly string StoredCommitteeDataFile = "committees.json";
        public static readonly string StoredHearingsDataFile = "hearings.json";
        public static readonly string APIKeyFileName = "GeoscapeAPIKey";
        public static readonly string MapBaseURL = "https://www.abc.net.au/res/sites/news-projects/interactive-electorateboundaries-2/5.0.0/?kml=/dat/news/elections/federal/2022/guide/kml/{0}.kml";
        
        // Preferences storage strings
        public static readonly string IsRegistered = "IsRegistered";
        public static readonly string RegistrationInfo = "RegistrationInfo";
        public static readonly string State = "State";
        public static readonly string ShowFirstTimeReadingPopup = "ShowFirstTimeReadingPopup";
        public static readonly string ShowHowToPublishPopup = "ShowHowToPublishPopup";
        public static readonly string HasQuestions = "HasQuestions";
        public static readonly string ElectoratesKnown = "ElectoratesKnown";
        public static readonly string IsVerifiedMPStafferAccount = "IsVerifiedMPStafferAccount";
        public static readonly string IsVerifiedMPAccount = "IsVerifiedMPAccount";
        public static readonly string MPRegisteredAs= "MPRegisteredAs";
        public static readonly string Address = "Address";
        public static readonly string DownvotedQuestions = "DownvotedQuestions";
        public static readonly string UpvotedQuestions = "UpvotedQuestions";
        public static readonly string DismissedQuestions = "DismissedQuestions";
        public static readonly string ReportedQuestions = "ReportedQuestions";
        
        // Default settings for sorted search
        public static readonly int DefaultPageSize = 20;

        // These weights are designed so that metadata (i.e. directions for who should
        // raise or answer it) dominate when present. Otherwise, search text similarity
        // is most important.
        public static readonly Weights mainReadingPageWeights = new Weights()
        {
            // Text similarity
            text = 6,
            
            // Directions
            metadata = 20,
            
            net_votes = 2,
            total_votes = 1,
            recentness = 1,
            recentness_timescale = 3600
        };
        
        // Main reading page
        public static readonly int ReadingPageMetadataWeight = 20;
        public static readonly int ReadingNetVotesWeight = 5;
        public static readonly int ReadingPageTotalVotesWeight = 5;
        public static readonly int ReadingPageRecentnessWeight = 10;
        public static readonly int ReadingPageRecentnessTimescale = 3600;
        public static readonly int ReadingPageTextSimilarityWeight = 1;
        
        
        // Messaging center strings
        public static readonly string GoToReadingPageNext = "GoToReadingPageNext";
        public static readonly string GoToMetadataPageNext = "GoToMetadataPageNext";
        public static readonly string GoToAskingPageNext = "GoToAskingPageNext";
        public static readonly string GoBackToAdvancedSearchPage = "GoBackToAdvancedSearchPage";
        public static readonly string QuestionSubmittedDeleteDraft = "QuestionSubmittedDeleteDraft";
        public static readonly string InitCommitteeLists = "InitCommitteeLists";
        public static readonly string InitAllMPsLists = "InitAllMPsLists";
        public static readonly string NeedToUpdateMyMpLists= "NeedToUpdateMyMpLists";
        public static readonly string UpdateFilters = "UpdateFilters";
        public static readonly string RefreshQuestionList = "RefreshQuestionList";
        

    }
}
