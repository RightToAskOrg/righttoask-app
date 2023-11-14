using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    
    public class MPId : IEquatable<MPId>
    {
        public MPId(MP mp)
        {
            first_name = mp.first_name;
            electorate = mp.electorate;
            surname = mp.surname;
        }

        // Empty constructor for json serialisation
        public MPId()
        {
        }

        [JsonPropertyName("first_name")]
        public string first_name { get; set; }

        [JsonPropertyName("surname")]
        public string surname { get; set; }

        [JsonPropertyName("electorate")]
        public ElectorateWithChamber electorate { get; set; }

        // This allows for set-based Linq list operations such as removing duplicates.
        public bool Equals(MPId? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return first_name == other.first_name
                   && surname == other.surname
                   && electorate.chamber == other.electorate.chamber
                   && electorate.region == other.electorate.region;
        }
    }
}