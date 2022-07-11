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

        public bool Validate()
        {
            bool isValid = false;
            switch (chamber)
            {
                case ParliamentData.Chamber.Australian_House_Of_Representatives:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.ACT_Legislative_Assembly:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.Australian_Senate:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.NSW_Legislative_Assembly:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.NSW_Legislative_Council:
                    isValid = string.IsNullOrEmpty(region); // empty region
                    break;
                case ParliamentData.Chamber.Qld_Legislative_Assembly:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.SA_House_Of_Assembly:
                    isValid = string.IsNullOrEmpty(region); // empty region
                    break;
                case ParliamentData.Chamber.SA_Legislative_Council:
                    isValid = string.IsNullOrEmpty(region); // empty region
                    break;
                case ParliamentData.Chamber.Tas_House_Of_Assembly:
                    isValid = true;
                    break;
                case ParliamentData.Chamber.Tas_Legislative_Council:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.Vic_Legislative_Assembly:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.Vic_Legislative_Council:
                    isValid = !string.IsNullOrEmpty(region); // non-empty region
                    break;
                case ParliamentData.Chamber.WA_Legislative_Assembly:
                    isValid = string.IsNullOrEmpty(region);
                    break;
                case ParliamentData.Chamber.WA_Legislative_Council:
                    isValid = string.IsNullOrEmpty(region);
                    break;
                case ParliamentData.Chamber.NT_Legislative_Assembly:
                    isValid = true;
                    break;
                default:
                    break;
            }
            return isValid;
        }

        // Case-insensitive equality on Region names.
        public bool Equals(ElectorateWithChamber other)
        {
            return chamber == other.chamber 
                   && region.Equals(other.region, StringComparison.OrdinalIgnoreCase);
        }
    }
}