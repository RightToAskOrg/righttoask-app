using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace RightToAskClient.Models
{
    public class Registration : ObservableObject
    {
        // TODO: Move MP / staffer reg in here.
        public string display_name { get; set; } = "";
        public string public_key { get; set; } = "";

        public string State => _stateKnown ? SelectedStateAsEnum.ToString() : "";

        public string uid { get; set; } = "";

        private int _selectedStateAsIndex = -1;
        
        private bool _stateKnown;
        public bool StateKnown
        {
            get => _stateKnown;
            set => SetProperty(ref _stateKnown, value);

        }
        private ParliamentData.StateEnum _selectedStateAsEnum;

        public ParliamentData.StateEnum SelectedStateAsEnum
        {
            get => _selectedStateAsEnum;
            set => SetProperty(ref _selectedStateAsEnum, value);
        }
        
        private List<ElectorateWithChamber> _electorates = new List<ElectorateWithChamber>();

        // Whenever electorates are updated (by this and by AddElectorateRemoveDuplicates), we need to tell the 
        // Filters to update the list of 'my MPs'.
        public List<ElectorateWithChamber> Electorates 
        {
            get => _electorates; 
            set 
            {
                SetProperty(ref _electorates, value.ToList());
                App.GlobalFilterChoices.UpdateMyMPLists();
            } 
        }

        private List<Badge> _badges = new List<Badge>();
        
        // For the moment, this indicates (only) whether the person is registered as an MP or staffer.
        public List<Badge> Badges
        {
            get => _badges;
            set => SetProperty(ref _badges, value.ToList());
        }

        // Necesary for java serialisation & deserlialisation.
        public Registration()
        {
        }

        public Registration(ServerUser input)
        {
            display_name = input.display_name ?? "";
            public_key = input.public_key ?? "";
            var stateResult = ParliamentData.StateStringToEnum(input.state ?? "");
            if (stateResult.Success)
            {
                StateKnown = true;
                SelectedStateAsEnum = stateResult.Data;
            }
            else
            {
                StateKnown = false;
                SelectedStateAsEnum = default;
            }
            
            uid = input.uid ?? "";
            _electorates = input.electorates ?? new List<ElectorateWithChamber>();
            _badges = input.badges ?? new List<Badge>();
        }

        /* Accept a new electorate and chamber, remove any earlier ones that are inconsistent.
		 * Note: this assumes that nobody is ever represented in two different regions in the one
		 * chamber. This is true throughout Aus, but may be untrue elsewhere. Of course, each region
		 * may have multiple representatives.
		 * Inserting it at the beginning rather than adding at the end is a bit of a hack to
		 * put the Senators last (they're computed first).
		 */
        public void AddElectorateRemoveDuplicates(ElectorateWithChamber newElectorate)
        {
            // potentially remove any duplicates
            _electorates.RemoveAll(e => e.chamber == newElectorate.chamber);
            _electorates.Insert(0, newElectorate);
            OnPropertyChanged("electorates");
            App.GlobalFilterChoices.UpdateMyMPLists();
        }

        private void Electorates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public JOSResult IsValid()
        {
            var errorFields = new List<string>();

            foreach (var prop in typeof(Registration).GetProperties())
            {
                var value = prop.GetValue(this, null);
                if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    errorFields.Add(prop.Name);
                }
            }

            if (errorFields.IsNullOrEmpty() || errorFields.SequenceEqual(new List<string> { "electorates" }))
            {
                return new SuccessResult();
            }

            return new ErrorResult("Please complete " + string.Join(" and ", errorFields));
        }

        public bool Validate()
        {
            var isValid = false;
            // needs to have a uid and public key
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(public_key) && !string.IsNullOrEmpty(State))
            {
                if (Electorates.Any())
                {
                    var hasInvalidElectorate = false;
                    foreach(var e in Electorates)
                    {
                        var validElectorate = e.Validate();
                        if (!validElectorate)
                        {
                            hasInvalidElectorate = true;
                        }
                    }
                    if (hasInvalidElectorate)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        // Note that this will *not* update if the person doesn't select anything.
        public (bool success, ParliamentData.StateEnum state) UpdateStateStorePreferences(int selectedStateAsInt)
        {
            // Check that it isn't -1 (no selection) or (unexpected) a value that doesn't correspond to a valid state enum.
            var successState = default(ParliamentData.StateEnum);
            var successBool = false;

            if (Enum.IsDefined(typeof(ParliamentData.StateEnum), selectedStateAsInt))
            {
                successState =
                    (ParliamentData.StateEnum)Enum.ToObject(typeof(ParliamentData.StateEnum), selectedStateAsInt);
                successBool = true;
                IndividualParticipant.ProfileData.RegistrationInfo.StateKnown = successBool;
                IndividualParticipant.ProfileData.RegistrationInfo.SelectedStateAsEnum = successState;
                if(DeviceInfo.Platform != DevicePlatform.Unknown)
                    Preferences.Set(Constants.State, successState.ToString());
            }

            return (successBool, successState);
        }
    }
}
