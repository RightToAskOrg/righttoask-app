

// This class represents a public authority,
// represented in the RightToKnow list.

using System;
using System.Text.Json.Serialization;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class MP : Entity, IEquatable<MP> 

    {
        [JsonPropertyName("first_name")]
        public string first_name { get; set; }
        // public ParliamentData.Chamber ChamberSeatedIn { get; set; }

        [JsonPropertyName("surname")] 
        public string surname { get; set; } = "";


        [JsonPropertyName("electorate")] 
        public ElectorateWithChamber electorate { get; set; }

        // TODO consider making this a specific appropriate type
        [JsonPropertyName("email")]
        public string email { get; set; } = "";
        
        
        [JsonPropertyName("role")]
        public string role { get; set; }= "";
        
        [JsonPropertyName("party")]
        public string party { get; set; }= "";

        public override string ShortestName
        {
            get { return first_name + " " + surname; }
        }

        private string salutation = "";

        // The compiler thinks this is unused, but it's necessary for json deserialisation.
        public MP()
        {
        }

        public MP(string _firstname, string _lastname, ElectorateWithChamber _electorateWithChamber, string _email, string _role, string _party)
        {
            first_name = _firstname;
            surname = _lastname;
            electorate = _electorateWithChamber;
            email = _email;
            role = _role;
            party = _party;
        }

        // TODO Consider adding lookup of other attributes such as ministerial roles from MP.json.
        public MP(MPId serverMP)
        {
            first_name = serverMP.first_name;
            surname = serverMP.surname;
            electorate = serverMP.electorate;
        }

        // public string ElectorateRepresenting { get; set; } = "";

        public override string GetName()
        {
            return first_name + " " + surname;
        }

        // Note that this is *not* complete equality of the whole data structure. 
        // In particular, it omits to check roles, parties and email, on the assumption
        // that these things may change and we still want to consider it to be the same MP.
        public bool Equals(MP other)
        {
            return surname == other.surname
                   && first_name == other.first_name
                   && electorate.Equals(other.electorate);
        }

        public override string ToString()
        {
            ParliamentData.Chamber cham = electorate.chamber;
            salutation = cham.Salutation(); 
            var StateIfNeeded = salutation == "Senator" ? "" : ", " + (electorate.region);
            var RoleIfNeeded = String.IsNullOrWhiteSpace(role) ? "" : "\n" + role;
            // Could use String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase) to ignore case.
            return first_name + " " + surname
                   + "\n" + salutation
                   + " for " + electorate.region
                   + StateIfNeeded
                   + RoleIfNeeded; 
        }

        public bool Validate()
        {
            // TODO: 

            bool isValid = false;
            if (!string.IsNullOrEmpty(surname) && electorate != null && !string.IsNullOrEmpty(electorate.region))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}