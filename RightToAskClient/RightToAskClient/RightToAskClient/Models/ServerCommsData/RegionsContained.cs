using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
	public class RegionsContained
	{
        [JsonPropertyName("super_region")]
		public string super_region { get; set; }
		
        [JsonPropertyName("regions")]
		public string[] regions { get; set; }
	}
}
