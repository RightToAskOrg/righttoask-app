using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using RightToAskClient.Annotations;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models.ServerCommsData
{
    // These are deliberately unititialized so that the serialization will drop them if they are empty.
    public class ServerUser
    {
        // These individual-level controls can be used to override the Json serialiser settings in RTAClient.cs
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("uid")]
        public string? uid { get; set; }
        
        [JsonPropertyName("display_name")] 
        public string? display_name { get; set; }
        
        [JsonPropertyName("public_key")]
        public string? public_key { get; set; }
        
        [JsonPropertyName("state")]
        public string? state { get; set; }

        [JsonPropertyName("electorates")]
        public List<ElectorateWithChamber>? electorates { get; set; } 

        [JsonPropertyName("badges")] 
        public List<string>? badges { get; set; } 

        // This empty constructor is necessary for deserialization.
        public ServerUser()
        {
        }

        public ServerUser(Registration newReg)
        {
            // Compulsory fields
            uid = newReg.uid;
            display_name = newReg.display_name;
            public_key = newReg.public_key;
            
            // Optional fields. Add only if non-empty.
            if (newReg.Electorates.Any())
            {
                electorates = newReg.Electorates;
            }
            if (!String.IsNullOrEmpty(newReg.State))
            {
                state = newReg.State;
            }
            if (newReg.Badges.Any())
            {
                badges = newReg.Badges;
            }
        }

        public bool Validate()
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(uid)
                && !string.IsNullOrEmpty(public_key)
                && !string.IsNullOrEmpty(state))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}