using System.Text.Json.Serialization;

namespace RightToAskClient.Maui.Models.ServerCommsData
{

    public class HansardLink
    {
        public HansardLink(string url)
        {
            this.url = url;
        }

        // Empty constructor for json serialisation
        public HansardLink()
        {
        }

        [JsonPropertyName("url")] 
        public string url { get; set; }

    }
}