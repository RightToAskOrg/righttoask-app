using System;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class MPId 
    {
        public MPId(MP mp)
        {
            first_name = mp.first_name;
            electorate = mp.electorate;
            surname = mp.surname;
        }

        public MPId()
        {
        }

        [JsonPropertyName("first_name")]
        public string first_name { get; set; }

        [JsonPropertyName("surname")]
        public string surname { get; set; }

        [JsonPropertyName("electorate")]
        public ElectorateWithChamber electorate { get; set; }
    }
}