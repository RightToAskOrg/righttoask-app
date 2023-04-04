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

        private bool _signUpFlow;
        private bool _enableFindMPsButton;
        public bool EnableFindMPsButton
        {
            get => _enableFindMPsButton;
            set => SetProperty(ref _enableFindMPsButton, value);
        }
        private bool _showMapFrame;
        public bool ShowMapFrame
        {
            get => _showMapFrame;
            set => SetProperty(ref _showMapFrame, value);
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
        public string StatePickerTitle =>
            _registration.StateKnown
                ? _registration.SelectedStateAsEnum.ToString()
                : AppResources.ChooseStateOrTerritory;

        private void OnStateSelected(int value)
        {
            OnStatePickerSelectedIndexChanged(value);
            OnPropertyChanged("FederalElectorates");
        }

        private ParliamentData.StateEnum _selectedStateEnum;
        public ParliamentData.StateEnum SelectedStateEnum
        {
            get => _selectedStateEnum;
            private set => SetProperty(ref _selectedStateEnum, value);
        }

        public List<string> FederalElectorates;

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
        }

        private string _stateInferredElectorateHeader;
        
        public string StateInferredElectorateHeader
        {
            get => _stateInferredElectorateHeader;
            set => SetProperty(ref _stateInferredElectorateHeader, value);
        }
        
        private string _stateInferredElectorate = AppResources.LegislativeCouncilText;

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

        private LabeledPickerViewModel _statePickerModel;
        public LabeledPickerViewModel StatePickerModel{
            get => _statePickerModel;
            set => SetProperty(ref _statePickerModel, value);
        }
        
        private LabeledPickerViewModel _federalPickerModel;
        public LabeledPickerViewModel FederalPickerModel{
            get => _federalPickerModel;
            set => SetProperty(ref _federalPickerModel, value);
        }
        
        private LabeledPickerViewModel _stateElectoratePickerModel;
        public LabeledPickerViewModel StateElectoratePickerModel{
            get => _stateElectoratePickerModel;
            set => SetProperty(ref _stateElectoratePickerModel, value);
        }

        #endregion

        // constructor
        public FindMPsViewModel() : this(null)
        {
            
        }

        private void InitUILabels(Registration registration)
        {
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
        }

        public FindMPsViewModel(Registration? registration = null)
        {
            if (registration != null)
            {
                _signUpFlow = true;
                _registration = registration;
            }

            InitUILabels(_registration);
            
            PopupLabelText = AppResources.FindMPsPopupText;
            ShowAddressStack = false;
            ShowKnowElectoratesFrame = false;
            ShowMapFrame = false;

            _stateKnown = _registration.StateKnown;
            var electorateString = ParliamentData.ConvertGeoscapeElectorateToStandard(
                _registration.State, 
                CommonwealthElectorate);
            ShowMapOfElectorate(electorateString);

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
            KnowElectoratesCommand.Execute(true);

            if(_registration.ElectoratesKnown)
                InitialisePickersWithElectorates();
            else
                InitialisePickers();
        }

        private void InitialisePickers()
        {
            StatePickerModel = new LabeledPickerViewModel
            {
                Items = StatePicker,
                Title = AppResources.ChooseStateOrTerritory,
            };
            StatePickerModel.OnSelectedCallback += OnStateSelected;

            FederalPickerModel = new LabeledPickerViewModel
            {
                Items = FederalElectorates,
                Title = AppResources.FederalElectoratePickerTitle,
            };
            FederalPickerModel.OnSelectedCallback += OnFederalElectoratePickerSelectedIndexChanged;

            StateElectoratePickerModel = new LabeledPickerViewModel()
            {
                Items = AllStateChoosableElectorates.ToList(),
                Title = "Legislative Assembly",
            };
            StateElectoratePickerModel.OnSelectedCallback += OnStateChoosableElectoratePickerSelectedIndexChanged;
            
        }

        private void InitialisePickersWithElectorates()
        {
            ParliamentData.StateEnum stateToSelect;
            Enum.TryParse(State, out stateToSelect);

            // initialize the selection of State picker
            StatePickerModel = new LabeledPickerViewModel
            {
                Items = StatePicker,
                Title = AppResources.ChooseStateOrTerritory,
                SelectedIndex = _stateKnown ? (int)stateToSelect : -1
            };

            // initialize the selection of Federal picker
            FederalElectorates = ParliamentData.HouseOfRepsElectorates(State);
            FederalPickerModel = new LabeledPickerViewModel
            {
                Items = FederalElectorates,
                Title = AppResources.FederalElectoratePickerTitle,
                SelectedIndex = CommonwealthElectorate.IsNullOrEmpty() ? -1 : FederalElectorates.IndexOf(CommonwealthElectorate)
            };

            var stateElectorate = (stateToSelect == ParliamentData.StateEnum.TAS)
                ? StateUpperHouseElectorate
                : StateLowerHouseElectorate;
            UpdateElectorateInferencesFromStateAndCommElectorate(stateToSelect,
                stateElectorate,
                CommonwealthElectorate);
            StateElectoratePickerModel = new LabeledPickerViewModel()
            {
                Items = AllStateChoosableElectorates.ToList(),
                Title = "Legislative Assembly",
                SelectedIndex = stateElectorate.IsNullOrEmpty()
                    ? -1
                    : AllStateChoosableElectorates.IndexOf(stateElectorate),
            };
            
            FederalPickerModel.OnSelectedCallback += OnFederalElectoratePickerSelectedIndexChanged;
            StatePickerModel.OnSelectedCallback += OnStateSelected;
            StateElectoratePickerModel.OnSelectedCallback += OnStateChoosableElectoratePickerSelectedIndexChanged;
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
            int.TryParse(Address.Postcode, out var postcode);
            PostcodeIsValid = Postcode.IsValid(SelectedStateEnum, postcode);
            
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
                EnableFindMPsButton = true;
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
            if (!string.IsNullOrEmpty(electorateString))
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
        private void OnStateChoosableElectoratePickerSelectedIndexChanged(int value)
        {
            var selectedStateElectorateIndex = StateElectoratePickerModel.SelectedIndex;
            if (selectedStateElectorateIndex >= 0 && selectedStateElectorateIndex < AllStateChoosableElectorates.Count
                && !string.IsNullOrEmpty(AllStateChoosableElectorates[selectedStateElectorateIndex]))
            {
                var chosenElectorate = AllStateChoosableElectorates[selectedStateElectorateIndex];
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

        private void OnFederalElectoratePickerSelectedIndexChanged(int value)
        {
            var selectedFederalElectorateIndex = FederalPickerModel.SelectedIndex;
            if (selectedFederalElectorateIndex >= 0 && selectedFederalElectorateIndex < FederalElectorates.Count && 
                !string.IsNullOrEmpty(FederalElectorates[selectedFederalElectorateIndex]))
            {
                
                // actually show the map in real time
                ShowMapFrame = true;
                ShowMapOfElectorate(FederalElectorates[selectedFederalElectorateIndex]);

                var stateElectorate = _registration.SelectedStateAsEnum == ParliamentData.StateEnum.TAS
                    ? StateUpperHouseElectorate : StateLowerHouseElectorate;
                _registration.Electorates
                        = ParliamentData.FindAllRelevantElectorates(SelectedStateEnum, stateElectorate, FederalElectorates[selectedFederalElectorateIndex]);
                // For Tasmania, we need your federal electorate to infer your state Legislative Assembly electorate.
                if (SelectedStateEnum == ParliamentData.StateEnum.TAS)
                {
                    UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum, stateElectorate, FederalElectorates[selectedFederalElectorateIndex]);
                }
                RevealNextStepAndCommunicateIfElectoratesKnown();
            }
        }

        private void RevealNextStepAndCommunicateIfElectoratesKnown()
        {
            var selectedFederalElectorateIndex = FederalPickerModel.SelectedIndex;
            if (!string.IsNullOrEmpty(StateChoosableElectorate) || selectedFederalElectorateIndex != -1)
            {
                EnableFindMPsButton = true;
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

        private void OnStatePickerSelectedIndexChanged(int value)
        {
            if (_registration.ElectoratesKnown)
            {
                var selectedState = (ParliamentData.StateEnum)Enum.ToObject(typeof(ParliamentData.StateEnum), value);
                if (selectedState == SelectedStateEnum)  
                    return;
            }

            (_stateKnown, SelectedStateEnum) = _registration.UpdateStateStorePreferences(StatePickerModel.SelectedIndex);
            _registration.Electorates = ParliamentData.FindAllRelevantElectorates(SelectedStateEnum, "", "");
            if (_stateKnown)
            {
                // This will give us the right message about the upper-house electorate and a blank inferred electorate.
                UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum, "", "");
                (StateChoosableElectorateHeader, StateInferredElectorateHeader, StateInferredElectorate)
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(SelectedStateEnum, "", "");

                FederalElectorates = ParliamentData.HouseOfRepsElectorates(SelectedStateEnum.ToString());
                if(FederalPickerModel != null)
                    FederalPickerModel.Items = FederalElectorates;
                if(StateElectoratePickerModel != null)
                    StateElectoratePickerModel.Items = AllStateChoosableElectorates.ToList();
                EnableFindMPsButton = true;
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
        
        private string State
        {
            get
            {
                return ParliamentData.FindOneElectorateGivenPredicate(_registration.Electorates.ToList(), 
                    chamberPair => chamberPair.chamber == ParliamentData.Chamber.Australian_Senate);
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
