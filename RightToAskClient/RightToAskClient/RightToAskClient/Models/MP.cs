

// This class represents a public authority,
// represented in the RightToKnow list.
namespace RightToAskClient.Models
{
    public class MP : Person
    {
        public BackgroundElectorateAndMPData.Chamber ChamberSeatedIn { get; set; }
        public string Salutation { get; set; }
        public string ElectorateRepresenting { get; set; }
    
        public override string ToString()
        {
            var StateIfNeeded = Salutation == "Senator" ? "" : ", " + (StateOrTerritory ?? "");
            // Could use String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase) to ignore case.
            return base.ToString()
                   + "\n" + (Salutation ?? "")
                   + " for " + (ElectorateRepresenting ?? "")
                   + StateIfNeeded;
        }

    }
}