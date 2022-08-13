using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class CommitteeInfo
    {
        [JsonPropertyName("jurisdiction")]
        public ParliamentData.Chamber? jurisdiction { get; set; }

        [JsonPropertyName("name")]
        public string name{ get; set; }

        [JsonPropertyName("url")] 
        public string url { get; set; }

        [JsonPropertyName("committee_type")] 
        public ParliamentData.CommitteeType committee_type { get; set; }
    }
}