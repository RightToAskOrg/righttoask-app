using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public enum CensorshipReason
    {
        NotAQuestion,
        ThreateningViolence,
        IncludesPrivateInformation,
        IncitesHatredOrDiscrimination,
        EncouragesHarm,
        TargetedHarassment,
        DefamatoryInsinuation,
        Illegal,
        Impersonation,
        Spam
    }
    
    public class ReportQuestionCommand
    {
        [JsonPropertyName("question_id")] 
        public string question_id { get; set; } 

        [JsonPropertyName("reason")] 
        public CensorshipReason reason { get; set; }
    }
}