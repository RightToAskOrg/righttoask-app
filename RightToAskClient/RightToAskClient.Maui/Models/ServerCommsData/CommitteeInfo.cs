using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{
    public class CommitteeInfo : IEquatable<CommitteeInfo>
    {
        public CommitteeInfo(Committee committee)
        {
            jurisdiction = committee.Jurisdiction;
            name = committee.Name;
            url = committee.Url;
            committee_type = committee.CommitteeType;
        }

        // Empty constructor for json serialisation
        public CommitteeInfo()
        {
        }
        
        [JsonPropertyName("jurisdiction")]
        public ParliamentData.Jurisdiction jurisdiction { get; set; }

        [JsonPropertyName("name")]
        public string name{ get; set; }

        [JsonPropertyName("url")] 
        public string url { get; set; }

        [JsonPropertyName("committee_type")] 
        public string committee_type { get; set; }

        // This allows for set-based Linq list operations such as removing duplicates.
        public bool Equals(CommitteeInfo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return jurisdiction == other.jurisdiction && name == other.name && url == other.url && committee_type == other.committee_type;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommitteeInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)jurisdiction;
                hashCode = (hashCode * 397) ^ name.GetHashCode();
                hashCode = (hashCode * 397) ^ url.GetHashCode();
                hashCode = (hashCode * 397) ^ committee_type.GetHashCode();
                return hashCode;
            }
        }
    }
}