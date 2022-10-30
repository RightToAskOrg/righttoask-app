using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
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
        private Registration _registration = App.ReadingContext.ThisParticipant.RegistrationInfo;
        public Address Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        private bool _showFindMPsButton = false;
        public bool ShowFindMPsButton
        {
            get => _showFindMPsButton;
            set => SetProperty(ref _showFindMPsButton, value);
        }
        private bool _showMapFrame = false;
        public bool ShowMapFrame
        {
            get => _showMapFrame;
            set => SetProperty(ref _showMapFrame, value);
        }
        private bool _showSkipButton = false;
        public bool ShowSkipButton
        {
            get => _showSkipButton;
            set => SetProperty(ref _showSkipButton, value);
        }
        private bool _showStateOnly = true;
        public bool ShowStateOnly
        {
            get => _showStateOnly;
            set => SetProperty(ref _showStateOnly, value);
        }
        private bool _showElectoratesFrame = false;
        public bool ShowElectoratesFrame
        {
            get => _showElectoratesFrame;
            set => SetProperty(ref _showElectoratesFrame, value);
        }
        private bool _showKnowElectoratesFrame = false;
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
                // OnPropertyChanged("FederalElectorates");
            }
        }
        private bool _showAddressStack = false;
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
        // private string _statePickerTitle = String.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.State) ? "Choose State or Territory" : App.ReadingContext.ThisParticipant.RegistrationInfo.State;
        public string StatePickerTitle
        {
            get => _registration.StateKnown
                ? _registration.SelectedStateAsEnum.ToString()
                : AppResources.ChooseStateOrTerritory;
        }
        private int _selectedStateAsInt = -1;
        public int SelectedStateAsInt
        {
            get
            {
                return _selectedStateAsInt;
            }
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
        
        public List<string> FederalElectorates
        {
            get => ParliamentData.HouseOfRepsElectorates(SelectedStateEnum.ToString());
        }
        
        private ObservableCollection<string> _allStateChoosableElectorates = new ObservableCollection<string>();
        public ObservableCollection<string> AllStateChoosableElectorates
        {
            get => _allStateChoosableElectorates;
        }
        
        private string _stateChoosableElectorateHeader 
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum == ParliamentData.StateEnum.TAS
            ? string.Format("State Legislative Council Electorate: {0:F0}", App.ReadingContext.ThisParticipant.StateUpperHouseElectorate)
            : string.Format("State Legislative Assembly Electorate: {0:F0}", App.ReadingContext.ThisParticipant.StateLowerHouseElectorate) ;

        public string StateChoosableElectorateHeader
        {
            get => _stateChoosableElectorateHeader;
            set => SetProperty(ref _stateChoosableElectorateHeader, value);
            }

        private string _stateChoosableElectorate
            = "Select: " + 
            (App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum == ParliamentData.StateEnum.TAS
                    ? App.ReadingContext.ThisParticipant.StateUpperHouseElectorate
                    : App.ReadingContext.ThisParticipant.StateLowerHouseElectorate);

        public string StateChoosableElectorate
        {
            get => _stateChoosableElectorate;
            set
            {
                SetProperty(ref _stateChoosableElectorate, value);
                // OnStateChoosableElectoratePickerSelectedIndexChanged();
            }
        }

        private string _stateInferredElectorateHeader  
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum == ParliamentData.StateEnum.TAS
            ? "State Legislative Assembly Electorate: " : "State Legislative Council Electorate: ";
        
        public string StateInferredElectorateHeader
        {
            get => _stateInferredElectorateHeader;
            set => SetProperty(ref _stateInferredElectorateHeader, value);
        }
        
        private string _stateInferredElectorate  
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum == ParliamentData.StateEnum.TAS
            ? App.ReadingContext.ThisParticipant.StateLowerHouseElectorate 
            : App.ReadingContext.ThisParticipant.StateUpperHouseElectorate;
        
        public string StateInferredElectorate
        {
            get => _stateInferredElectorate;
            set => SetProperty(ref _stateInferredElectorate, value);
        }
        
        private string _federalElectoratePickerTitle = string.Format("Select: {0:F0}", App.ReadingContext.ThisParticipant.CommonwealthElectorate);
        public string FederalElectoratePickerTitle
        {
            get => _federalElectoratePickerTitle;
            set => SetProperty(ref _federalElectoratePickerTitle, value);
        }
        
        private bool _launchMPsSelectionPageNext = false;
        
        // This indicates, when we're choosing from a list of MPs, whether they're asking the question (true) or
        // answering it (false).
        private bool _choosingAskingMP = false;

        private string _mapURL = "";
        public string MapURL
        {
            get => _mapURL;
            private set => SetProperty(ref _mapURL, value);
        }
        
        private bool _postcodeIsValid = false;
        public bool PostcodeIsValid
        {
            get => _postcodeIsValid;
            set => SetProperty(ref _postcodeIsValid, value);
        }

        private bool _stateKnown;
        #endregion

        // constructor
        public FindMPsViewModel()
        {
            PopupLabelText = AppResources.FindMPsPopupText;
            ShowSkipButton = false;
            ShowAddressStack = false;
            ShowKnowElectoratesFrame = false;
            ShowMapFrame = false;
            _launchMPsSelectionPageNext = true;

            _stateKnown = App.ReadingContext.ThisParticipant.RegistrationInfo.StateKnown;
            
            // set the pickers to update their content lists here if state was already indicated elsewhere in the application
            if(_stateKnown) 
            {
                SelectedStateEnum = App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum;
                
                // set the state index pickers
                SelectedStateAsInt = (int)SelectedStateEnum;
                
                string choosableStateElectorate = (SelectedStateEnum == ParliamentData.StateEnum.TAS )
                   ? App.ReadingContext.ThisParticipant.StateUpperHouseElectorate
                   : App.ReadingContext.ThisParticipant.StateLowerHouseElectorate;
                UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum,
                    choosableStateElectorate,
                    App.ReadingContext.ThisParticipant.CommonwealthElectorate);
            }

            if (!string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.CommonwealthElectorate))
            {
                ShowMapFrame = true;
                string electorateString = ParliamentData.ConvertGeoscapeElectorateToStandard(App.ReadingContext.ThisParticipant.RegistrationInfo.State, App.ReadingContext.ThisParticipant.CommonwealthElectorate);
                ShowMapOfElectorate(electorateString);
            }

            MessagingCenter.Subscribe<RegistrationViewModel>(this, "FromReg1", (sender) =>
            {
                _launchMPsSelectionPageNext = false;
                MessagingCenter.Unsubscribe<RegistrationViewModel>(this, "FromReg1");
            });
            MessagingCenter.Subscribe<QuestionViewModel>(this, "OptionBAskingNow", sender =>
            {
                _choosingAskingMP = true;
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, "OptionBAskingNow");
            });

            // commands
            SaveMPsButtonCommand = new AsyncCommand(async () =>
            {
                RegistrationViewModel.SaveRegistrationToPreferences(_registration);
                
                SelectableListPage mpsSearchableListPage;
                // We might get here via Option A from the flow options page, in which case
                //    - initialize the MP SearchableListPage with AnsweringMPsListsMine and
                //    - after that, navigate forward to a ReadingPage;
                // we might get here via Option B from the QuestionAskerPage, in which case
                //    - initialize the MP SearchableListPage with AskingMPsListsMine and
                //    - after that, navigate forward to a ReadingPage;
                if (_launchMPsSelectionPageNext)
                {
                    // Our MP is asking the question
                    if (_choosingAskingMP)
                    {
                        string message =
                            "These are your MPs.  Select the one(s) who should raise the question in Parliament";
                        mpsSearchableListPage = new SelectableListPage(App.ReadingContext.Filters.AskingMPsListsMine,
                            message);
                    }
                    // Our MP should be answering the question.
                    else
                    {
                        string message = "These are your MPs.  Select the one(s) who should answer the question";
                        mpsSearchableListPage = new SelectableListPage(App.ReadingContext.Filters.AnsweringMPsListsMine,
                            message);
                    }

                    // MessagingCenter.Send(this, Constants.GoToReadingPageNext);

                    await App.Current.MainPage.Navigation.PushAsync(mpsSearchableListPage);
                    _launchMPsSelectionPageNext = false;
                    _choosingAskingMP = false;
                }
                // We are here from the registration page - no need to select any MPs
                else
                {
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
                var addressPref = Preferences.Get(Constants.Address, "");
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

        private void SaveElectoratesToPreferences()
        {
            // save registration to preferences
            var registrationObjectToSave = JsonSerializer.Serialize(new ServerUser(_registration));
            Preferences.Set(Constants.RegistrationInfo, registrationObjectToSave);
        }

        // commands
        public IAsyncCommand SaveMPsButtonCommand { get; }
        public IAsyncCommand SubmitAddressButtonCommand { get; }
        public IAsyncCommand SkipButtonCommand { get; }
        public Command LookupElectoratesCommand { get; }
        public Command KnowElectoratesCommand { get; }

        // methods
        #region Methods
        
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        private async Task OnSubmitAddressButton_Clicked()
        {
            // see if we should prompt first
            CheckPostcode();

            if (!PostcodeIsValid)
            {
                var popup = new TwoButtonPopup(this, AppResources.InvalidPostcodePopupTitle, AppResources.InvalidPostcodePopupText, AppResources.CancelButtonText, AppResources.ImSureButtonText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (ApproveButtonClicked)
                {
                    PostcodeIsValid = true;
                }
            }
            if (PostcodeIsValid)
            {
                IsBusy = true; // no longer displaying the activity indicator since the webview shows that it is being updated
                Result<GeoscapeAddressFeature> httpResponse;

                string state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;

                if (String.IsNullOrEmpty(state))
                {
                    var popup = new OneButtonPopup(AppResources.SelectStateWarningText, AppResources.OKText);
                    _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    return;
                }

                Result<bool> addressValidation = _address.SeemsValid();
                if (!String.IsNullOrEmpty(addressValidation.Err))
                {
                    var popup = new OneButtonPopup(addressValidation.Err, AppResources.OKText);
                    _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    return;
                }

                httpResponse = await GeoscapeClient.GetFirstAddressData(_address + " " + state);

                if (httpResponse != null)
                {
                    if (httpResponse.Err != null)
                    {
                        ReportLabelText = httpResponse.Err;
                        // maybe display a popup if Electorates were not found
                        var popup = new OneButtonPopup(AppResources.ElectoratesNotFoundErrorText, AppResources.OKText);
                        _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                        return;
                    }

                    // Now we know everything is good.
                    GeoscapeAddressFeature? bestAddress = httpResponse.Ok;
                    // needs a federal electorate to be valid
                    if (!string.IsNullOrEmpty(bestAddress?.Properties?.CommonwealthElectorate?.ToString()))
                    {
                        AddElectorates(bestAddress);
                        ShowFindMPsButton = true;
                        ReportLabelText = "";

                        string electoratePopupTitle = "Electorates Found!";
                        string electoratePopupText = "Federal electorate: " + App.ReadingContext.ThisParticipant.CommonwealthElectorate +
                            "\n" + "State lower house electorate: " + App.ReadingContext.ThisParticipant.StateLowerHouseElectorate;
                        var electoratesFoundPopup = new OneButtonPopup(electoratePopupTitle, electoratePopupText, AppResources.OKText);
                        _ = await App.Current.MainPage.Navigation.ShowPopupAsync(electoratesFoundPopup);

                        // just save the address all the time now if it returned a valid electorate
                        SaveAddress();
                    }
                    else
                    {
                        var electoratesNotFoundPopup = new OneButtonPopup("Electorates not Found", "Please reformat the address and try again.", AppResources.OKText);
                        _ = await App.Current.MainPage.Navigation.ShowPopupAsync(electoratesNotFoundPopup);
                    }
                    ShowSkipButton = false;
                    // display the map if we stored the Federal Electorate properly
                    if (!string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.CommonwealthElectorate))
                    {
                        ShowMapFrame = true;
                        ShowKnowElectoratesFrame = false;
                        ShowAddressStack = false;
                        string electorateString = ParliamentData.ConvertGeoscapeElectorateToStandard(state, App.ReadingContext.ThisParticipant.CommonwealthElectorate);
                        ShowMapOfElectorate(electorateString);
                    }
                }
            }
        }

        private void ShowMapOfElectorate(string electorateString)
        {
            MapURL = string.Format(Constants.MapBaseURL, electorateString);
        }

        private void AddElectorates(GeoscapeAddressFeature addressData)
        {
            ParliamentData.StateEnum state = App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum;
            var electorates = ParliamentData.GetElectoratesFromGeoscapeAddress(state, addressData);
            App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates = electorates;
            
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
            Preferences.Set(Constants.Address, fullAddress); // save the full address
            Preferences.Set(Constants.State, SelectedStateEnum.ToString());
        }

        // This is the Legislative Assembly Electorate, except in Tas where it's the Legislative Council.
        // Note: this assumes the Commonwealth Electorate is correct.
        // TODO enforce that Commonwealth Electorate gets chosen first.
        private void OnStateChoosableElectoratePickerSelectedIndexChanged()
        {
            if (SelectedStateElectorateIndex >= 0 && SelectedStateElectorateIndex < AllStateChoosableElectorates.Count
                && !String.IsNullOrEmpty(AllStateChoosableElectorates[SelectedStateElectorateIndex]))
            {
                var chosenElectorate = AllStateChoosableElectorates[SelectedStateElectorateIndex];
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum;
                App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates
                    = ParliamentData.FindAllRelevantElectorates(state,
                        chosenElectorate, App.ReadingContext.ThisParticipant.CommonwealthElectorate);
                (_, _, StateInferredElectorate) 
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(SelectedStateEnum, chosenElectorate, 
                        App.ReadingContext.ThisParticipant.CommonwealthElectorate);
            }
            RevealNextStepAndCommunicateIfElectoratesKnown();
        }

        private void OnFederalElectoratePickerSelectedIndexChanged()
        {
            if (SelectedFederalElectorate >= 0 && SelectedFederalElectorate < FederalElectorates.Count && 
                !String.IsNullOrEmpty(FederalElectorates[SelectedFederalElectorate]))
            {
                
                // actually show the map in real time
                ShowMapFrame = true;
                ShowMapOfElectorate(FederalElectorates[SelectedFederalElectorate]);
                
                // TODO Consider whether electorates should be readonly and instead have a function that updates them
                // given this info.
                App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates
                        = ParliamentData.FindAllRelevantElectorates(SelectedStateEnum, "", FederalElectorates[SelectedFederalElectorate]);
                // For Tasmania, we need your federal electorate to infer your state Legislative Assembly electorate.
                if (SelectedStateEnum == ParliamentData.StateEnum.TAS)
                {
                    UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum, "", FederalElectorates[SelectedFederalElectorate]);
                }
                RevealNextStepAndCommunicateIfElectoratesKnown();
            }
        }

        private void RevealNextStepAndCommunicateIfElectoratesKnown()
        {
            if (!String.IsNullOrEmpty(StateChoosableElectorate) || SelectedFederalElectorate != -1)
            {
                ShowFindMPsButton = true;
                CommunicateElectoratesKnown();
            }
        }

        private void CommunicateElectoratesKnown()
        {
            App.ReadingContext.ThisParticipant.ElectoratesKnown = true;
            Preferences.Set(Constants.ElectoratesKnown, true);
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
            
            foreach (string electorate in newChoosableElectorateList)
            {
                AllStateChoosableElectorates.Add(electorate);   
            }

            (StateChoosableElectorateHeader, StateInferredElectorateHeader, StateInferredElectorate) 
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(state, stateElectorate, commElectorate);
        }

        private void OnStatePickerSelectedIndexChanged()
        {
            (_stateKnown, SelectedStateEnum) = App.ReadingContext.ThisParticipant.RegistrationInfo.UpdateStateStorePreferences(SelectedStateAsInt);
                
                if (_stateKnown)
                {
                // This will give us the right message about the upper-house electorate and a blank inferred electorate.
                UpdateElectorateInferencesFromStateAndCommElectorate(SelectedStateEnum, "", "");
                (StateChoosableElectorateHeader, StateInferredElectorateHeader, StateInferredElectorate)
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(SelectedStateEnum, "", "");
                ShowStateOnly = false;
            }
        }

        private void CheckPostcode()
        {
            switch (SelectedStateEnum)
            {
                case ParliamentData.StateEnum.ACT:
                    int postcode = 0; 
                    int.TryParse(Address.Postcode, out postcode);
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
    }
}
