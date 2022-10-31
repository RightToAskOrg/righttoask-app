using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using RightToAskClient.Resx;

namespace RightToAskClient.Models.ServerCommsData
{
    public class Badge
    {
        // what a badge represents
        [JsonPropertyName("badge")] public BadgeType? badge { get; set; }

        // What the badge is about (the text on a badge)
        // For an MP, this is MP::badge_name, for an organization it is the domain.
        [JsonPropertyName("name")] public string? name { get; set; }

        // Express badge in exactly the format expected by the server.
        public static string WriteBadgeName(MP mpRepresenting, string domain)
        {
            return mpRepresenting.first_name + " " + mpRepresenting.surname + " @" + domain;
        }

        public override string ToString()
        {
            return badge + " " + AppResources.For + " " + name;
        }
    }

    public enum BadgeType
    {
		[EnumMember(Value = "EmailDomain")]
        EmailDomain,
        
		[EnumMember(Value = "MP")]
        MP,
        
		[EnumMember(Value = "MPStaff")]
        MPStaff,
    }
}