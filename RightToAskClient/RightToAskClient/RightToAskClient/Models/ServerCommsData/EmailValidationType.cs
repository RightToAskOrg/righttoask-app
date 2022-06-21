using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData

{
// TODO Only GainBadge currently tested.
    public class EmailValidationType
    {
        // Gainbadge and AccountRecovery have no parameters - hoping empty string will work.

        [JsonPropertyName("GainBadge")] 
        public string? GainBadge { get; set; }

        [JsonPropertyName("RevokeBadge")]
        public string? RevokeBadge{ get; set; } // RevokeBadge(UserUID),
        
        [JsonPropertyName("AccountRecovery")]
        public string? AccountRecovery { get; set; }
    }
}