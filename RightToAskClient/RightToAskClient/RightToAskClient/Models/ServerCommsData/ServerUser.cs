using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models.ServerCommsData
{
    public class ServerUser
    {
        [JsonPropertyName("uid")]
        public string uid { get; set; } = "";
        
        [JsonPropertyName("display_name")]
        public string display_name { get; set; } = "";
        
        [JsonPropertyName("public_key")]
        public string public_key { get; set; } = "";
        
        [JsonPropertyName("state")]
        public string state { get; set; } = "";

        [JsonPropertyName("electorates")]
        public ObservableCollection<ElectorateWithChamber> electorates { get; set; } =
            new ObservableCollection<ElectorateWithChamber>();

        [JsonPropertyName("badges")] 
        public List<string> badges { get; set; } = new List<string>();

        // This empty constructor is necessary for deserialization.
        public ServerUser()
        {
        }

        public ServerUser(Registration newReg)
        {
            electorates = newReg.electorates;
            uid = newReg.uid;
            display_name = newReg.display_name;
            public_key = newReg.public_key;
            state = newReg.State;
            //TODO: add this when Registration has it.
            // badges = newReg.badges;

        }
    }
}