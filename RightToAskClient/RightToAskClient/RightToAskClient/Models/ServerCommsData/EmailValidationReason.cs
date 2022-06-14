using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
// This is an enum in Rust. We use multiple matches for
// paramaterised enums like in PersonID.
// TODO At the moment, only AsMP has been tested.
    public class EmailValidationReason
    {
        // if argument is true, the principal. Otherwise a staffer with access to email.
        [JsonPropertyName("AsMP")] 
        public bool? AsMP;

        // These next two have no arguments in the Rust enum. Hoping that a null string is OK.
        [JsonPropertyName("AsOrg")] 
        public string? AsOrg;

        [JsonPropertyName("AccountRecovery")] 
        public string? AccountRecovery;

        // UserID,bool in Rust. I think string is OK.
        [JsonPropertyName("RevokeMP")] 
        public (string, bool)? RevokeMP; // revoke a given UID. bool same meaning as AsMP.

        [JsonPropertyName("RevokeOrg")] 
        public string? RevokeOrg; // revoke a given UID
    }
}