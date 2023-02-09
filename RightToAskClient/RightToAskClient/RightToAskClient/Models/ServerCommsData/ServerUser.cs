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

            if (!checkPrivacyOptions)
            {
                sharing_electorate_info = newReg.SharingElectorateInfoOption;
            }

            // Optional fields. Add only if non-empty.
            if (newReg.Electorates.Any())
            {
                if (checkPrivacyOptions)
                {
                    electorates = new List<ElectorateWithChamber>();
                    ElectorateWithChamber? state = null;
                    ElectorateWithChamber? federalElectorate = null;
                    var stateElectorate = new List<ElectorateWithChamber>();
                    foreach (var electorate in newReg.Electorates)
                    {
                        switch (electorate.chamber)
                        {
                            case ParliamentData.Chamber.Australian_Senate:
                                state = electorate;
                                break;
                            case ParliamentData.Chamber.Australian_House_Of_Representatives:
                                federalElectorate = electorate;
                                break;
                            default:
                                stateElectorate.Add(electorate);
                                break;
                        }
                    }
                    
                    switch (newReg.SharingElectorateInfoOption)
                    {
                        case SharingElectorateInfoOptions.Nothing:
                            break;
                        case SharingElectorateInfoOptions.StateOrTerritory:
                            if (state != null)
                            {
                                electorates.Add(state);
                            }
                            break;
                        
                        case SharingElectorateInfoOptions.FederalElectorateAndState:
                            if (state != null)
                            {
                                electorates.Add(state);
                            }
                            if (federalElectorate != null)
                            {
                                electorates.Add(federalElectorate);
                            }
                            break;
                        
                        case SharingElectorateInfoOptions.StateElectorateAndState:
                            if (state != null)
                            {
                                electorates.Add(state);
                            }
                            electorates.AddRange(stateElectorate);
                            break;
                        
                        case SharingElectorateInfoOptions.All:
                            if (state != null)
                            {
                                electorates.Add(state);
                            }
                            if (federalElectorate != null)
                            {
                                electorates.Add(federalElectorate);
                            }
                            electorates.AddRange(stateElectorate);
                            break;
                    }
                }
                else
                {
                    electorates = newReg.Electorates;
                }
            }

            if (!string.IsNullOrEmpty(newReg.State))
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