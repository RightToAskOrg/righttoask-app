using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class CommitteeInfo
    {
        [JsonPropertyName("jurisdiction")]
        public ParliamentData.Jurisdiction jurisdiction { get; set; }

        [JsonPropertyName("name")]
        public string name{ get; set; }

        [JsonPropertyName("url")] 
        public string url { get; set; }

        [JsonPropertyName("committee_type")] 
        public string committee_type { get; set; }
    }
}