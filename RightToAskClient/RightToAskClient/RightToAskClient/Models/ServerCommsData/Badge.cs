using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class Badge
    {
        // what a badge represents
        [JsonPropertyName("badge")] public BadgeType? badge { get; set; }

        // What the badge is about (the text on a badge)
        // For an MP, this is MP::badge_name, for an organization it is the domain.
        [JsonPropertyName("name")] public string? name { get; set; }

        public static string writeBadgeName(MP mpRepresenting, string domain)
        {
            return mpRepresenting.first_name + " " + mpRepresenting.surname + " @" + domain;
        }
    }

    public enum BadgeType
    {
        EmailDomain,
        MP,
        MPStaff,
    }
}