using System.ComponentModel;
using System.Runtime.Serialization;

namespace RightToAskClient.Models
{
    public enum HowAnsweredOptions
    {
        [EnumMember(Value = "DontKnow")]
        DontKnow,
        [EnumMember(Value = "InApp")]
        InApp,
        [EnumMember(Value = "InParliament")]
        InParliament
    }
}