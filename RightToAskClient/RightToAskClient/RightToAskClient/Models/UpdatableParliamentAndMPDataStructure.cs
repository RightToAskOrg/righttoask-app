	using System.Collections.Generic;
	using System.Text.Json.Serialization;
	using RightToAskClient.Models;

namespace RightToAskClient.Models
{
	public class UpdatableParliamentAndMPDataStructure
	{
        [JsonPropertyName("mps")]
        public MP[] mps { get; set; }

        [JsonPropertyName("federal_electorates_by_state")]
        public RegionsContained[] FederalElectoratesByState { get; set; }

        [JsonPropertyName("vic_districts")]
        public List<RegionsContained> VicRegions = new List<RegionsContained>();
	}
}