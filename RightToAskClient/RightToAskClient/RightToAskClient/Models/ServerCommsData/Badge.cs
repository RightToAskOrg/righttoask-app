using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class Badge
    {
        // what a badge represents
        [JsonPropertyName("badge")]
        public BadgeType? badge {get; set;}

        // What the badge is about (the text on a badge)
        // For an MP, this is MP::badge_name, for an organization it is the domain.
        [JsonPropertyName("name")] 
        public string? name { get; set; }
    }

    public enum BadgeType
    {
        EmailDomain,
        MP,
        MPStaff,
    }
}