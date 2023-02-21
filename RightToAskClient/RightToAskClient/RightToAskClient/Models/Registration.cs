using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace RightToAskClient.Models
{
    public enum RegistrationStatus
    {
        AnotherPerson,
        Registered,
        NotRegistered
    }
    
    // bitwise approach
    [Flags]
    public enum SharingElectorateInfoOptions
    {
        Nothing = 0,
        StateOrTerritory = 1,
        FederalElectorate = 2,
        StateElectorate = 4,
        FederalElectorateAndState = StateOrTerritory | FederalElectorate,
        StateElectorateAndState = StateOrTerritory | StateElectorate,
        All = StateOrTerritory | FederalElectorate | StateElectorate,
    }

    public class Registration : ObservableObject
    {
        private const int MaxDisplayNameChar = 60;

        // By default it's another person. So we need only worry about the current user registration.
        public RegistrationStatus registrationStatus { get; set; } = RegistrationStatus.AnotherPerson;
        public bool IsRegistered
        {
            get => registrationStatus == RegistrationStatus.Registered;
        }

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
        private bool _electoratesKnown = false;
        public bool ElectoratesKnown
        {
            get => _electoratesKnown;
            set
            {
                _electoratesKnown = value;
                if (value)
                {
                    _stateKnown = true;
                }
            }
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
                FilterChoices.NeedToUpdateMyMpLists(this);
            } 
        }

        private MP _MPRegisteredAs = new MP();
        public MP MPRegisteredAs { 
            get => _MPRegisteredAs;
            set
            {
                _MPRegisteredAs = value;
                // FIXME - not sure why OnPropertyChanged not compiling, but anyway perhaps we want the
                // data from Registration anyway?
                // OnPropertyChanged();
                // OnPropertyChanged("RegisteredMP");
            }
        }
        
        public bool IsVerifiedMPAccount { get; set; }
        public bool IsVerifiedMPStafferAccount { get; set; }

        private List<Badge> _badges = new List<Badge>();
        
        // For the moment, this indicates (only) whether the person is registered as an MP or staffer.
        public List<Badge> Badges
        {
            get => _badges;
            set
            {
                SetProperty(ref _badges, value.ToList());
                foreach (var badge in _badges)
                {
                    
                }
            }
        }

        private SharingElectorateInfoOptions? _sharingElectorateInfoOption;

        public SharingElectorateInfoOptions? SharingElectorateInfoOption
        {
            get => _sharingElectorateInfoOption; 
            set => SetProperty(ref _sharingElectorateInfoOption, value);
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
            FilterChoices.NeedToUpdateMyMpLists(this);
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

        public bool ValidateReadyToUse()
        {
            // if they are registered, they need valid registration info
            if (IsRegistered)
            {
                return Validate();
            }
            // if they are not registered, they could still have MPs known if they are in the process of creating their first question
            // before  they have the chance to create an account
            if (ElectoratesKnown)
            {
                return StateKnown;
            }

            return false;
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

        public (bool isValid, string validationErrMessage) ValidateName()
        {
            var isValid = true;
            var validationErrMessage = "";
            if (string.IsNullOrEmpty(display_name))
            {
                isValid = false;
                validationErrMessage = AppResources.EmptyDisplayNameMessage;
            }
            else if (display_name.Length > MaxDisplayNameChar)
            {
                isValid = false;
                validationErrMessage = String.Format(AppResources.MaxCharDisplayNameMessage, MaxDisplayNameChar);
            }
            return (isValid, validationErrMessage);
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
                _stateKnown = successBool;
                _selectedStateAsEnum = successState;
                XamarinPreferences.shared.Set(Constants.State, successState.ToString());
            }

            return (successBool, successState);
        }
    }
}
