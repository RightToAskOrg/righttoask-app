using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
	public class UpdatableParliamentAndMPDataStructure
	{
        [JsonPropertyName("mps")]
        public MP[]? mps { get; set; }

        [JsonPropertyName("federal_electorates_by_state")]
        public RegionsContained[]? FederalElectoratesByState { get; set; }

        [JsonPropertyName("vic_districts")] 
        public RegionsContained[]? VicRegions { get; set; }
	}
}