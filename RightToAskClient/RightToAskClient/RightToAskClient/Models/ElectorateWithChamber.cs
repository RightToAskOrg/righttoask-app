namespace RightToAskClient.Models
{
    public class ElectorateWithChamber
    {
        public ParliamentData.Chamber chamber { get; set; }
        public string region { get; set; } = "";

        public ElectorateWithChamber(ParliamentData.Chamber chamber, string region)
        {
            this.chamber = chamber;
            this.region = region;
        }
    }
}