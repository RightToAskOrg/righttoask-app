

// This class represents a public authority,
// represented in the RightToKnow list.
namespace RightToAskClient.Models
{
    public class MP : Person
    {
        public string first_name { get; }
        // public ParliamentData.Chamber ChamberSeatedIn { get; set; }

        public string surname { get; }

        public ElectorateWithChamber electorate { get; set; }

        // TODO consider making this a specific appropriate type
        public string email { get; }
        
        public string role { get; }
        
        public string party { get; }

        private string salutation = ""; 
        
        // public string ElectorateRepresenting { get; set; } = "";
    
        public override string ToString()
        {
            ParliamentData.Chamber cham = electorate.chamber;
            salutation = cham.Salutation(); 
            var StateIfNeeded = salutation == "Senator" ? "" : ", " + (registrationInfo.state);
            // Could use String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase) to ignore case.
            return base.ToString()
                   + "\n" + salutation
                   + " for " + electorate.region
                   + StateIfNeeded;
        }

    }
}