using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    // These are deliberately unititialized so that the serialization will drop them if they are empty.
    public class ServerUser
    {
        // These individual-level controls can be used to override the Json serialiser settings in RTAClient.cs
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("uid")]
        // TODO: will uid ever be null?
        public string? uid { get; set; }

        [JsonPropertyName("display_name")] public string? display_name { get; set; }

        [JsonPropertyName("public_key")] public string? public_key { get; set; }

        [JsonPropertyName("state")] public string? state { get; set; }

        [JsonPropertyName("electorates")] public List<ElectorateWithChamber>? electorates { get; set; }

        [JsonPropertyName("badges")] public List<Badge>? badges { get; set; }

        [JsonPropertyName("sharing_electorate_info")]
        public SharingElectorateInfoOptions? sharing_electorate_info { get; set; }

        // This empty constructor is necessary for deserialization.
        public ServerUser()
        {
        }

        // TODO: use `checkPrivacyOptions` for filtering data sent to server
        public ServerUser(Registration newReg, bool checkPrivacyOptions = false)
        {
            // Compulsory fields
            uid = newReg.uid;
            display_name = newReg.display_name;
            public_key = newReg.public_key;

            // Optional fields. Add only if non-empty.
            if (newReg.Badges.Any())
            {
                badges = newReg.Badges;
            }
            
            if (!checkPrivacyOptions)
            {
                sharing_electorate_info = newReg.SharingElectorateInfoOption;
                if (newReg.Electorates.Any())
                {
                    electorates = newReg.Electorates;
                }
                if (!string.IsNullOrEmpty(newReg.State))
                {
                    state = newReg.State;
                }
            }
            else
            {
                var sharingOption = newReg.SharingElectorateInfoOption;
                var storeState = (sharingOption & SharingElectorateInfoOptions.StateOrTerritory) != 0;
                var storeFederalElect = (sharingOption & SharingElectorateInfoOptions.FederalElectorate) != 0;
                var storeStateElect = (sharingOption & SharingElectorateInfoOptions.StateElectorate) != 0;
                if (storeState && !string.IsNullOrEmpty(newReg.State))
                {
                    state = newReg.State;
                }
                if (newReg.Electorates.Any())
                {
                    electorates = new List<ElectorateWithChamber>();
                    var stateChamber = 
                        newReg.Electorates.Find(x => 
                            x.chamber == ParliamentData.Chamber.Australian_Senate);
                    var federalElectorate = 
                        newReg.Electorates.Find(x => 
                            x.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
                    var stateElectorate = 
                        newReg.Electorates.FindAll(x => 
                            (x.chamber != ParliamentData.Chamber.Australian_House_Of_Representatives &&
                             x.chamber != ParliamentData.Chamber.Australian_Senate));

                    if (storeState && stateChamber != null)
                    {
                        electorates.Add(stateChamber);
                    }
                    if (storeFederalElect && federalElectorate != null)
                    {
                        electorates.Add(federalElectorate);
                    }
                    if (storeStateElect)
                    {
                        electorates.AddRange(stateElectorate);
                    }
                }
            }
        }

        public bool Validate()
        {
            var isValid = false;
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