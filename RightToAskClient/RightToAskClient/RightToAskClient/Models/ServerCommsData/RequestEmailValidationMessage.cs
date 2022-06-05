using System;

namespace RightToAskClient.Models.ServerCommsData
{
    public class RequestEmailValidationMessage
    {
        EmailValidationReason why;

        /// the "name" of the badge. For an MP, the [MP::badge_name], for an organization the domain name, for an account recovery...TBD. Possibly the new key?
        string name;
    }

// This is an enum in Rust. We use multiple matches for
// paramaterised enums like in PersonID.
// TODO At the moment, only AsMP has been tested.
    public class EmailValidationReason
    {
        // if argument is true, the principal. Otherwise a staffer with access to email.
        public bool? AsMP;

        // These next two have no arguments in the Rust enum. Hoping that a null string is OK.
        public string? AsOrg;

        public string? AccountRecovery;

        // UserID,bool in Rust. I think string is OK.
        public (string, bool)? RevokeMP; // revoke a given UID. bool same meaning as AsMP.
        
        public string? RevokeOrg; // revoke a given UID
    }

    // TODO Only GainBadge currently tested.
    public class EmailValidationType
    {
        // Again, Gainbadge and AccountRecovery have no parameters - hoping empty string will work.
        public string? GainBadge;
        public string? RevokeBadge; // RevokeBadge(UserUID),
        public string? AccountRecovery;
    }
}
