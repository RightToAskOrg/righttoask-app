

// This class represents a public authority,
// represented in the RightToKnow list.

using System;

namespace RightToAskClient.Models
{
    public class MP : Entity
    {
        public string first_name { get; }
        // public ParliamentData.Chamber ChamberSeatedIn { get; set; }

        public string surname { get; }

        public ElectorateWithChamber electorate { get; set; }

        // TODO consider making this a specific appropriate type
        public string email { get; } = "";
        
        public string role { get; }= "";
        
        public string party { get; }= "";

        public override string ShortestName
        {
            get { return first_name + " " + surname; }
        }

        private string salutation = "";

        public MP(string first_name, string surname, ElectorateWithChamber electorate,
            string email = "", string role = "", string party = "" )
        {
            this.surname = surname;
            this.first_name = first_name;
            this.electorate = electorate;
            this.email = email;
            this.role = role;
            this.party = party;
        }

        // public string ElectorateRepresenting { get; set; } = "";

        public override string GetName()
        {
            return first_name + " " + surname;
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