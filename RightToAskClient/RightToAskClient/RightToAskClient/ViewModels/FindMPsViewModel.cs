using System;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.Helpers;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class FindMPsViewModel : BaseViewModel
    {
        #region Properties

        private Address _address = new Address();
        private readonly Registration _registration = IndividualParticipant.getInstance().ProfileData.RegistrationInfo;
        public Address Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private bool _signUpFlow = false;
        private bool _showFindMPsButton;
        public bool ShowFindMPsButton
        {
            get => _showFindMPsButton;
            set => SetProperty(ref _showFindMPsButton, value);
        }
        private bool _showMapFrame;
        public bool ShowMapFrame
        {
            get => _showMapFrame;
            set => SetProperty(ref _showMapFrame, value);
        }
        private bool _showStateOnly = true;
        public bool ShowStateOnly
        {
            get => _showStateOnly;
            set => SetProperty(ref _showStateOnly, value);
        }
        private bool _showElectoratesFrame;
        public bool ShowElectoratesFrame
        {
            get => _showElectoratesFrame;
            set => SetProperty(ref _showElectoratesFrame, value);
        }
        private bool _showKnowElectoratesFrame;
        public bool ShowKnowElectoratesFrame
        {
            get => _showKnowElectoratesFrame;
            set
            {
                // Make sure all the data in the 'know electorates' frame is refreshed.
                SetProperty(ref _showKnowElectoratesFrame, value); 
                OnPropertyChanged("StatePickerTitle"); 
                OnPropertyChanged("FederalElectorate");
                OnPropertyChanged("StateChoosableElectorate");
                OnPropertyChanged("StateInferredElectorate");
            }
        }
        private bool _showAddressStack;
        public bool ShowAddressStack
        {
            get => _showAddressStack;
            set
            {
                // Make sure the state data in the address frame is refreshed.
                SetProperty(ref _showAddressStack, value);
                OnPropertyChanged("StatePickerTitle");
            }
        }
        public List<string> StatePicker => ParliamentData.StateStrings;
        // private string _statePickerTitle = String.IsNullOrEmpty(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.State) ? "Choose State or Territory" : IndividualParticipant.getInstance().ProfileData.RegistrationInfo.State;
        public string StatePickerTitle =>
            _registration.StateKnown
                ? _registration.SelectedStateAsEnum.ToString()
                : AppResources.ChooseStateOrTerritory;

        private int _selectedStateAsInt = -1;
        public int SelectedStateAsInt
        {
            get => _selectedStateAsInt;
            set
            {
                SetProperty(ref _selectedStateAsInt, value);
                OnStatePickerSelectedIndexChanged();
                OnPropertyChanged("FederalElectorates");
            }
        }

        private ParliamentData.StateEnum _selectedStateEnum;
        public ParliamentData.StateEnum SelectedStateEnum
        {
            get => _selectedStateEnum;
            private set => SetProperty(ref _selectedStateEnum, value);
        }
        
        // The index of the selected state electorate in the (current)
        // list of state electorates. We never actually use this value - 
        // it's just a hack to detect when the picker selection changes.
        private int _selectedStateElectorateIndex = -1;
        public int SelectedStateElectorateIndex
        {
            get => _selectedStateElectorateIndex;
            set
            {
                SetProperty(ref _selectedStateElectorateIndex, value);
                OnStateChoosableElectoratePickerSelectedIndexChanged();
            }
        }
        
        private int _selectedFederalElectorate = -1;
        public int SelectedFederalElectorate
        {
            get => _selectedFederalElectorate;
            set
            {
                SetProperty(ref _selectedFederalElectorate, value);
                OnFederalElectoratePickerSelectedIndexChanged();
            }
        }
        
        public List<string> FederalElectorates => ParliamentData.HouseOfRepsElectorates(SelectedStateEnum.ToString());

        public ObservableCollection<string> AllStateChoosableElectorates { get; } = new ObservableCollection<string>();

        private string _stateChoosableElectorateHeader ;

        public string StateChoosableElectorateHeader
        {
            get => _stateChoosableElectorateHeader;
            set => SetProperty(ref _stateChoosableElectorateHeader, value);
            }

        private string _stateChoosableElectorate;

        public string StateChoosableElectorate
        {
            get => _stateChoosableElectorate;
            set => SetProperty(ref _stateChoosableElectorate, value);
            // OnStateChoosableElectoratePickerSelectedIndexChanged();
        }

        private string _stateInferredElectorateHeader;
        
        public string StateInferredElectorateHeader
        {
            get => _stateInferredElectorateHeader;
            set => SetProperty(ref _stateInferredElectorateHeader, value);
        }
        
        private string _stateInferredElectorate;
        
        public string StateInferredElectorate
        {
            get => _stateInferredElectorate;
            set => SetProperty(ref _stateInferredElectorate, value);
        }
        
        private string _federalElectoratePickerTitle;
        public string FederalElectoratePickerTitle
        {
            get => _federalElectoratePickerTitle;
            set => SetProperty(ref _federalElectoratePickerTitle, value);
        }

        private string _mapURL = "";
        public string MapURL
        {
            get => _mapURL;
            private set => SetProperty(ref _mapURL, value);
        }
        
        private bool _postcodeIsValid;
        public bool PostcodeIsValid
        {
            get => _postcodeIsValid;
            set => SetProperty(ref _postcodeIsValid, value);
        }

        private bool _stateKnown;
        #endregion

        // constructor
        public FindMPsViewModel() : this(null)
        {
            
        }
        public FindMPsViewModel(Registration? registration = null)
        {
            if (registration != null)
            {
                _signUpFlow = true;
                _registration = registration;
            }

            if (_registration.SelectedStateAsEnum == ParliamentData.StateEnum.TAS)
            {
                StateChoosableElectorateHeader = $"State Legislative Council Electorate: {StateUpperHouseElectorate:F0}";
                StateChoosableElectorate = "Select: " + StateUpperHouseElectorate;
                StateInferredElectorateHeader = "State Legislative Assembly Electorate: ";
                StateInferredElectorate = StateLowerHouseElectorate;
            }
            else
            {
                StateChoosableElectorateHeader = $"State Legislative Assembly Electorate: {StateLowerHouseElectorate:F0}";
                StateChoosableElectorate = "Select: " + StateLowerHouseElectorate;
                StateInferredElectorateHeader = "State Legislative Council Electorate: ";
                StateInferredElectorate =  StateUpperHouseElectorate;
            }
            FederalElectoratePickerTitle = $"Select: {CommonwealthElectorate:F0}";
            
            PopupLabelText = AppResources.FindMPsPopupText;
            ShowAddressStack = false;
            ShowKnowElectoratesFrame = false;
            ShowMapFrame = false;

            _stateKnown = _registration.StateKnown;
            
            // set the pickers to update their content lists here if state was already indicated elsewhere in the application
            if(_stateKnown) 
            {
                SelectedStateEnum = _registration.SelectedStateAsEnum;
                
                // set the state index pickers
                SelectedStateAsInt = (int)SelectedStateEnum;
                
                var choosableStateElectorate = (SelectedStateEnum == ParliamentData.StateEnum.TAS )
                   ? StateUpperHouseElectorate
                   : StateLowerHouseElectorate;
                UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum,
                    choosableStateElectorate,
                    CommonwealthElectorate);
            }

            if (!string.IsNullOrEmpty(CommonwealthElectorate))
            {
                ShowMapFrame = true;
                var electorateString = ParliamentData.ConvertGeoscapeElectorateToStandard(
                    _registration.State, 
                    CommonwealthElectorate);
                ShowMapOfElectorate(electorateString);
            }

            // commands
            SaveMPsButtonCommand = new AsyncCommand(async () =>
            {
                if (_signUpFlow)
                {
                    var sharingElectoratePage = new SharingElectorateInfoPage(_registration);
                    await Application.Current.MainPage.Navigation.PushAsync(sharingElectoratePage);
                }
                else
                {
                    RegistrationViewModel.SaveRegistrationToPreferences(_registration);
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            });
            SubmitAddressButtonCommand = new AsyncCommand(async () =>
            {
                await OnSubmitAddressButton_Clicked();
            });
            // TODO Think about what should happen if the person has made 
            // some choices, then clicks 'skip'.  At the moment, it retains 
            // the choices they made and pops the page.
            SkipButtonCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
            LookupElectoratesCommand = new Command(() =>
            {
                ShowAddressStack = true;
                ShowKnowElectoratesFrame = false;
                ShowElectoratesFrame = false;

                // set the address data if we have it
                var addressPref = XamarinPreferences.shared.Get(Constants.Address, "");
                if (!string.IsNullOrEmpty(addressPref))
                {
                    var addressObj = JsonSerializer.Deserialize<Address>(addressPref);
                    Address = addressObj ?? new Address();
                }
            });
            KnowElectoratesCommand = new Command(() =>
            {
                ShowKnowElectoratesFrame = true;
                ShowAddressStack = false;
                ShowElectoratesFrame = false;
            });

        }

        // commands
        public IAsyncCommand SaveMPsButtonCommand { get; }
        public IAsyncCommand SubmitAddressButtonCommand { get; }
        public IAsyncCommand SkipButtonCommand { get; }
        public Command LookupElectoratesCommand { get; }
        public Command KnowElectoratesCommand { get; }

        // methods
        #region Methods

        private async Task ShowOneButtonPopup(string? title, string message, string buttonText)
        {
            try
            {
                OneButtonPopup popup;
                if (title != null)
                {
                    popup = new OneButtonPopup(title,message, buttonText);
                }
                else
                {
                    popup = new OneButtonPopup(message, buttonText);
                } 
                _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task<bool> ShowTwoButtonPopup(string popupTitle, string popupText, string cancelMessage, string approveMessage)
        {
            try
            {
                var popup = new TwoButtonPopup(
                    AppResources.InvalidPostcodePopupTitle, 
                    AppResources.InvalidPostcodePopupText, 
                    AppResources.CancelButtonText, 
                    AppResources.ImSureButtonText, 
                    false);
                var popupResult = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                return popup.HasApproved(popupResult);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        private async Task OnSubmitAddressButton_Clicked()
        {
            // see if we should prompt first
            CheckPostcode();
            if (!PostcodeIsValid)
            {
                var popupResult = await ShowTwoButtonPopup(
                    AppResources.InvalidPostcodePopupTitle,
                    AppResources.InvalidPostcodePopupText,
                    AppResources.CancelButtonText, 
                    AppResources.ImSureButtonText);
                if (popupResult)
                {
                    PostcodeIsValid = true;
                }
                else
                {
                    return;
                }
            }

            var state = _registration.State;
            if (string.IsNullOrEmpty(state))
            {
                await ShowOneButtonPopup(null, AppResources.SelectStateWarningText, AppResources.OKText);
                return;
            }

            var addressValidation = _address.SeemsValid();
            if (addressValidation.Failure)
            {
                var errorMessage = AppResources.InvalidAddress;
                if (addressValidation is ErrorResult<bool> errorResult)
                {
                    errorMessage += errorResult.Message;
                }
                await ShowOneButtonPopup(null, errorMessage, AppResources.OKText);
                return;
            }

            var httpResponse = await GeoscapeClient.GetFirstAddressData(_address + " " + state);
            if (httpResponse == null)
            {
                return;
            }

            // Display a popup if Electorates were not found
            if (httpResponse.Failure)
            {
                ReportLabelText = ((httpResponse is ErrorResult<GeoscapeAddressFeature> errorResult)
                    ? ReportLabelText = errorResult.Message
                    : ReportLabelText = AppResources.ErrorFindingAddress);
                await ShowOneButtonPopup(null, AppResources.ElectoratesNotFoundErrorText, AppResources.OKText);
                return;
            }

            var bestAddress = httpResponse.Data;
            var electoratePopupTitle = "Electorates not Found";
            var electoratePopupText = "Please reformat the address and try again.";
            // needs a federal electorate to be valid
            if (!string.IsNullOrEmpty(bestAddress.Properties?.CommonwealthElectorate?.ToString()))
            {
                AddElectorates(bestAddress);
                ShowFindMPsButton = true;
                ReportLabelText = "";

                electoratePopupTitle = "Electorates Found!";
                electoratePopupText = ("Federal electorate: " +
                                       CommonwealthElectorate +
                                       "\nState lower house electorate: " + 
                                       StateLowerHouseElectorate);

                // just save the address all the time now if it returned a valid electorate
                SaveAddress();
            }
            await ShowOneButtonPopup(electoratePopupTitle, electoratePopupText, AppResources.OKText);
            
            // display the map if we stored the Federal Electorate properly
            if (!string.IsNullOrEmpty(CommonwealthElectorate))
            {
                ShowMapFrame = true;
                ShowKnowElectoratesFrame = false;
                ShowAddressStack = false;
                var electorateString = ParliamentData.ConvertGeoscapeElectorateToStandard(state, CommonwealthElectorate);
                ShowMapOfElectorate(electorateString);
            }
        }

        private void ShowMapOfElectorate(string electorateString)
        {
            MapURL = string.Format(Constants.MapBaseURL, electorateString);
        }

        private void AddElectorates(GeoscapeAddressFeature addressData)
        {
            var state = _registration.SelectedStateAsEnum;
            var electorates = ParliamentData.GetElectoratesFromGeoscapeAddress(state, addressData);
            _registration.Electorates = electorates;
            
            // There really shouldn't be any scenario in which there aren't any electorates here, unless something goes
            // wrong extracting Electorate strings from the Geoscape address.
            if (electorates.Any())
            {
                CommunicateElectoratesKnown();        
            }
        }

        // At the moment, this does nothing, since there's no notion of not 
        // saving the address.
        private void SaveAddress()
        {
            var fullAddress = JsonSerializer.Serialize(Address);
            XamarinPreferences.shared.Set(Constants.Address, fullAddress); // save the full address
            XamarinPreferences.shared.Set(Constants.State, SelectedStateEnum.ToString());
        }

        // This is the Legislative Assembly Electorate, except in Tas where it's the Legislative Council.
        // Note: this assumes the Commonwealth Electorate is correct.
        // TODO enforce that Commonwealth Electorate gets chosen first.
        private void OnStateChoosableElectoratePickerSelectedIndexChanged()
        {
            if (SelectedStateElectorateIndex >= 0 && SelectedStateElectorateIndex < AllStateChoosableElectorates.Count
                && !string.IsNullOrEmpty(AllStateChoosableElectorates[SelectedStateElectorateIndex]))
            {
                var chosenElectorate = AllStateChoosableElectorates[SelectedStateElectorateIndex];
                var state = _registration.SelectedStateAsEnum;
                _registration.Electorates
                    = ParliamentData.FindAllRelevantElectorates(state,
                        chosenElectorate, CommonwealthElectorate);
                (_, _, StateInferredElectorate) 
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(SelectedStateEnum, chosenElectorate, 
                        CommonwealthElectorate);
            }
            RevealNextStepAndCommunicateIfElectoratesKnown();
        }

        private void OnFederalElectoratePickerSelectedIndexChanged()
        {
            if (SelectedFederalElectorate >= 0 && SelectedFederalElectorate < FederalElectorates.Count && 
                !string.IsNullOrEmpty(FederalElectorates[SelectedFederalElectorate]))
            {
                
                // actually show the map in real time
                ShowMapFrame = true;
                ShowMapOfElectorate(FederalElectorates[SelectedFederalElectorate]);

                var stateElectorate = _registration.SelectedStateAsEnum == ParliamentData.StateEnum.TAS
                    ? StateUpperHouseElectorate : StateLowerHouseElectorate;
                // TODO Consider whether electorates should be readonly and instead have a function that updates them
                // given this info.
                _registration.Electorates
                        = ParliamentData.FindAllRelevantElectorates(SelectedStateEnum, stateElectorate, FederalElectorates[SelectedFederalElectorate]);
                // For Tasmania, we need your federal electorate to infer your state Legislative Assembly electorate.
                if (SelectedStateEnum == ParliamentData.StateEnum.TAS)
                {
                    UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum, stateElectorate, FederalElectorates[SelectedFederalElectorate]);
                }
                RevealNextStepAndCommunicateIfElectoratesKnown();
            }
        }

        private void RevealNextStepAndCommunicateIfElectoratesKnown()
        {
            if (!string.IsNullOrEmpty(StateChoosableElectorate) || SelectedFederalElectorate != -1)
            {
                ShowFindMPsButton = true;
                CommunicateElectoratesKnown();
            }
        }

        private void CommunicateElectoratesKnown()
        {
            _registration.ElectoratesKnown = true;
            XamarinPreferences.shared.Set(Constants.ElectoratesKnown, true);
            MessagingCenter.Send(this, Constants.ElectoratesKnown);
        }

        // Get the information appropriate to show to a user who is choosing their electorates, as a function 
        // of the state and (sometimes) the other electorates we know.
        // Sets the description of the state electorate they can choose, the list of available electorates they
        // can choose from, and the inferred electorates' title and contents.
        // This will often be called with only partial information (e.g. with a known state but only blank
        // electorates), in which case it fills in as much as it can and leaves the rest as empty strings.
        private void UpdateElectorateInferencesFromStateAndCommElectorate(ParliamentData.StateEnum state, string stateElectorate, string commElectorate)
        {
            var newChoosableElectorateList =
                state == ParliamentData.StateEnum.TAS
                    ? ParliamentData.ListElectoratesInStateUpperHouse(state)
                    : ParliamentData.ListElectoratesInStateLowerHouse(state);

            AllStateChoosableElectorates.Clear();   
            
            foreach (var electorate in newChoosableElectorateList)
            {
                AllStateChoosableElectorates.Add(electorate);   
            }

            (StateChoosableElectorateHeader, StateInferredElectorateHeader, StateInferredElectorate) 
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(state, stateElectorate, commElectorate);
        }

        private void OnStatePickerSelectedIndexChanged()
        {
            (_stateKnown, SelectedStateEnum) = _registration.UpdateStateStorePreferences(SelectedStateAsInt);
            _registration.Electorates = ParliamentData.FindAllRelevantElectorates(SelectedStateEnum, "", "");
                
            if (_stateKnown)
            {
                // This will give us the right message about the upper-house electorate and a blank inferred electorate.
                UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum, "", "");
                (StateChoosableElectorateHeader, StateInferredElectorateHeader, StateInferredElectorate)
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(SelectedStateEnum, "", "");
                ShowStateOnly = false;
                ShowFindMPsButton = true;
            }
        }

        private void CheckPostcode()
        {
            // TODO: reduce complexity (need to be testablr - currently, SelectedStateEnum isn't accessible)
            switch (SelectedStateEnum)
            {
                case ParliamentData.StateEnum.ACT:
                    int.TryParse(Address.Postcode, out var postcode);
                    if((postcode >= 2600 && postcode <= 2618) 
                        || (postcode >= 2900 && postcode <= 2920))
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // NSW
                case ParliamentData.StateEnum.NSW:
                    int.TryParse(Address.Postcode, out postcode);
                    if ((postcode >= 2000 && postcode <= 2599)
                        || (postcode >= 2619 && postcode <= 2898)
                        || (postcode >= 2921 && postcode <= 2999))
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // NT
                case ParliamentData.StateEnum.NT:
                    int.TryParse(Address.Postcode, out postcode);
                    if (postcode >= 0800 && postcode <= 0899)
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // QLD
                case ParliamentData.StateEnum.QLD:
                    int.TryParse(Address.Postcode, out postcode);
                    if (postcode >= 4000 && postcode <= 4999)
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // SA
                case ParliamentData.StateEnum.SA:
                    int.TryParse(Address.Postcode, out postcode);
                    if (postcode >= 5000 && postcode <= 5799)
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // TAS
                case ParliamentData.StateEnum.TAS:
                    int.TryParse(Address.Postcode, out postcode);
                    if (postcode >= 7000 && postcode <= 7799)
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // VIC
                case ParliamentData.StateEnum.VIC:
                    int.TryParse(Address.Postcode, out postcode);
                    if (postcode >= 3000 && postcode <= 3999)
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                // WA
                case ParliamentData.StateEnum.WA:
                    int.TryParse(Address.Postcode, out postcode);
                    if (postcode >= 6000 && postcode <= 6797)
                    {
                        PostcodeIsValid = true;
                    }
                    else
                    {
                        PostcodeIsValid = false;
                    }
                    break;
                default:
                    PostcodeIsValid = false;
                    break;
            }
        }
        #endregion
        
        
        
        /* Many states don't have an upper house, so this just returns ""
         */
        private string StateUpperHouseElectorate
        {
            get
            {
                return ParliamentData.FindOneElectorateGivenPredicate(_registration.Electorates.ToList(), 
                    c => ParliamentData.IsUpperHouseChamber(c.chamber));
            }
        }
		

        private string CommonwealthElectorate
        {
            get
            {
                return ParliamentData.FindOneElectorateGivenPredicate(_registration.Electorates.ToList(), 
                    chamberPair => chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
            }
        }

        private string StateLowerHouseElectorate
        {
            get
            {
                return ParliamentData.FindOneElectorateGivenPredicate(_registration.Electorates.ToList(),
                    chamberPair => ParliamentData.IsLowerHouseChamber(chamberPair.chamber)); 
            }
        }
    }
}
