using System;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class ElectorateWithChamber : IEquatable<ElectorateWithChamber>
    {
        public ParliamentData.Chamber chamber { get; set; }
        public string region { get; set; }

        public ElectorateWithChamber(ParliamentData.Chamber chamber, string region)
        {
            this.chamber = chamber;
            this.region = region;
        }

        public bool Equals(ElectorateWithChamber other)
        {
            return chamber == other.chamber && region == other.region;
        }
    }
}