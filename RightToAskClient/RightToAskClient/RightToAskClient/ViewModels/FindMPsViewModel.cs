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
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class FindMPsViewModel : BaseViewModel
    {
        #region Properties

        private string _state = "";
        private Address _address = new Address();
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
            set => SetProperty(ref _showKnowElectoratesFrame, value);
        }
        private bool _showAddressStack = false;
        public bool ShowAddressStack
        {
            get => _showAddressStack;
            set => SetProperty(ref _showAddressStack, value);
        }
        public List<string> StatePicker => ParliamentData.StatesAndTerritories;
        private string _statePickerTitle = String.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.State) ? "Choose State or Territory" : App.ReadingContext.ThisParticipant.RegistrationInfo.State;
        public string StatePickerTitle
        {
            get => _statePickerTitle;
            set => SetProperty(ref _statePickerTitle, value);
        }
        private int _selectedState = -1;
        public int SelectedState
        {
            get
            {
                return _selectedState;
            }
            set
            {
                SetProperty(ref _selectedState, value);
                OnStatePickerSelectedIndexChanged();
            }
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
            get => ParliamentData.HouseOfRepsElectorates(_state);
        }
        
        private ObservableCollection<string> _allStateChoosableElectorates = new ObservableCollection<string>();
        public ObservableCollection<string> AllStateChoosableElectorates
        {
            get => _allStateChoosableElectorates;
        }
        
        private string _stateChoosableElectorateHeader 
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.State == ParliamentData.State.TAS
            ? string.Format("State Legislative Council Electorate: {0:F0}", App.ReadingContext.ThisParticipant.StateUpperHouseElectorate)
            : string.Format("State Legislative Assembly Electorate: {0:F0}", App.ReadingContext.ThisParticipant.StateLowerHouseElectorate) ;

        public string StateChoosableElectorateHeader
        {
            get => _stateChoosableElectorateHeader;
            set => SetProperty(ref _stateChoosableElectorateHeader, value);
            }
        
        private string _stateChoosableElectorate
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.State == ParliamentData.State.TAS
            ? App.ReadingContext.ThisParticipant.StateUpperHouseElectorate 
            : App.ReadingContext.ThisParticipant.StateLowerHouseElectorate;

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
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.State == ParliamentData.State.TAS
            ? "State Legislative Assembly Electorate: " : "State Legislative Council Electorate: ";
        
        public string StateInferredElectorateHeader
        {
            get => _stateInferredElectorateHeader;
            set => SetProperty(ref _stateInferredElectorateHeader, value);
        }
        
        private string _stateInferredElectorate  
            =  App.ReadingContext.ThisParticipant.RegistrationInfo.State == ParliamentData.State.TAS
            ? App.ReadingContext.ThisParticipant.StateLowerHouseElectorate 
            : App.ReadingContext.ThisParticipant.StateUpperHouseElectorate;
        
        public string StateInferredElectorate
        {
            get => _stateInferredElectorate;
            set => SetProperty(ref _stateInferredElectorate, value);
        }
        
        private string _federalElectoratePickerTitle = string.Format("Federal Electorate: {0:F0}", App.ReadingContext.ThisParticipant.CommonwealthElectorate);
        public string FederalElectoratePickerTitle
        {
            get => _federalElectoratePickerTitle;
            set => SetProperty(ref _federalElectoratePickerTitle, value);
        }
        private string _doneButtonText = AppResources.NextButtonText;
        public string DoneButtonText
        {
            get => _doneButtonText;
            set => SetProperty(ref _doneButtonText, value);
        }
        private bool _launchMPsSelectionPageNext = false;
        private bool _optionB = false;

        private string _mapURL = "";
        public string MapURL
        {
            get => _mapURL;
            private set => SetProperty(ref _mapURL, value);
        }
        
        /*
         * Now-deprecated option to not save your address. We can probably delete this now.
        private bool _promptAddressSave = false;
        public bool PromptAddressSave
        {
            get => _promptAddressSave;
            set => SetProperty(ref _promptAddressSave, value);
        }
        */
        private bool _postcodeIsValid = false;
        public bool PostcodeIsValid
        {
            get => _postcodeIsValid;
            set => SetProperty(ref _postcodeIsValid, value);
        }
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

            // set the state index pickers
            SelectedState = App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsIndex;
            
            // set the pickers to update their content lists here if state was already indicated elsewhere in the application
            // TODO These two conditions are equivalent - we probably only need one.
            if (SelectedState != -1 && !string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.State))
            {
                _state = ParliamentData.StatesAndTerritories[SelectedState];
                string choosableStateElectorate = (_state == ParliamentData.State.TAS )
                   ? App.ReadingContext.ThisParticipant.StateUpperHouseElectorate
                   : App.ReadingContext.ThisParticipant.StateLowerHouseElectorate;
                UpdateElectorateInferencesFromStateAndCommElectorate(App.ReadingContext.ThisParticipant.RegistrationInfo.State,
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
            // TODO Not sure we ever use this.
            MessagingCenter.Subscribe<QuestionViewModel>(this, "OptionBGoToAskingPageNext", sender =>
            {
                _optionB = true;
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, "OptionBGoToAskingPageNext");
            });
            MessagingCenter.Subscribe<QuestionViewModel>(this, "OptionBAskingNow", sender =>
            {
                _optionB = true;
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, "OptionBAskingNow");
            });

            // commands
            MPsButtonCommand = new AsyncCommand(async () =>
            {
                SelectableListPage mpsSearchableListPage;
                // We might get here via Option A from the flow options page, in which case
                //    - initialize the MP SearchableListPage with AnsweringMPsListsMine and
                //    - after that, navigate forward to a ReadingPage;
                // we might get here via Option B from the QuestionAskerPage, in which case
                //    - initialize the MP SearchableListPage with AskingMPsListsMine and
                //    - after that, navigate forward to a ReadingPage;
                if (_launchMPsSelectionPageNext)
                {
                    // Option B - our MP is asking the question
                    if (_optionB)
                    {
                        string message =
                            "These are your MPs.  Select the one(s) who should raise the question in Parliament";
                        mpsSearchableListPage = new SelectableListPage(App.ReadingContext.Filters.AskingMPsListsMine,
                            message, true);
                    }
                    // Option A - our MP should be answering the question.
                    else
                    {
                        string message = "These are your MPs.  Select the one(s) who should answer the question";
                        mpsSearchableListPage = new SelectableListPage(App.ReadingContext.Filters.AnsweringMPsListsMine,
                            message, true);
                    }

                    MessagingCenter.Send(this, "GoToReadingPage");

                    await App.Current.MainPage.Navigation.PushAsync(mpsSearchableListPage);
                    _launchMPsSelectionPageNext = false;
                    _optionB = false;
                    DoneButtonText = AppResources.DoneButtonText;
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

        // commands
        public IAsyncCommand MPsButtonCommand { get; }
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
            string state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
            App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates 
                = new ObservableCollection<ElectorateWithChamber>(ParliamentData.GetElectoratesFromGeoscapeAddress(state, addressData));
            App.ReadingContext.ThisParticipant.MPsKnown = true;
            Preferences.Set(Constants.MPsKnown, true);
        }

        // At the moment, this does nothing, since there's no notion of not 
        // saving the address.
        private void SaveAddress()
        {
            var fullAddress = JsonSerializer.Serialize(Address);
            Preferences.Set(Constants.Address, fullAddress); // save the full address
            Preferences.Set(Constants.StateID, SelectedState);
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
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates
                    // TODO Perhaps Electorates should be a List rather than an Observable Collection.
                    = new ObservableCollection<ElectorateWithChamber>(ParliamentData.FindAllRelevantElectorates(state,
                        chosenElectorate, App.ReadingContext.ThisParticipant.CommonwealthElectorate));
                (_, _, StateInferredElectorate) 
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(state, chosenElectorate, 
                        App.ReadingContext.ThisParticipant.CommonwealthElectorate);
            }
            RevealNextStepIfElectoratesKnown();
        }

        private void OnFederalElectoratePickerSelectedIndexChanged()
        {
            if (SelectedFederalElectorate >= 0 && SelectedFederalElectorate < FederalElectorates.Count && 
                !String.IsNullOrEmpty(FederalElectorates[SelectedFederalElectorate]))
            {
                
                // actually show the map in real time
                ShowMapFrame = true;
                ShowMapOfElectorate(FederalElectorates[SelectedFederalElectorate]);
                
                // For Tasmania, we need your federal electorate to infer your state Legislative Assembly electorate.
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                    // TODO Consider whether electorates should be readonly and instead have a function that updates them
                    // given this info.
                    App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates
                        = new ObservableCollection<ElectorateWithChamber>(ParliamentData.FindAllRelevantElectorates(state,
                        "", FederalElectorates[SelectedFederalElectorate]));
                if (state == ParliamentData.State.TAS)
                {
                    UpdateElectorateInferencesFromStateAndCommElectorate(state, "", FederalElectorates[SelectedFederalElectorate]);
                }
                RevealNextStepIfElectoratesKnown();
            }
        }

        private void RevealNextStepIfElectoratesKnown()
        {
            if (!String.IsNullOrEmpty(StateChoosableElectorate) || SelectedFederalElectorate != -1)
            {
                ShowFindMPsButton = true;
            }
            App.ReadingContext.ThisParticipant.MPsKnown = true;
            Preferences.Set(Constants.MPsKnown, true);
        }

        // Get the information appropriate to show to a user who is choosing their electorates, as a function 
        // of the state and (sometimes) the other electorates we now.
        // Sets the description of the state electorate they can choose, the list of available electorates they
        // can choose from, and the inferred electorates' title and contents.
        // This will often be called with only partial information (e.g. with a known state but only blank
        // electorates), in which case it fills in as much as it can and leaves the rest as empty strings.
        private void UpdateElectorateInferencesFromStateAndCommElectorate(string state, string stateElectorate, string commElectorate)
        {
            var newChoosableElectorateList =
                state == ParliamentData.State.TAS
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
            if (SelectedState != -1)
            {
                App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsIndex = SelectedState;
                
                // This will give us the right message about the upper-house electorate and a blank inferred electorate.
                _state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                UpdateElectorateInferencesFromStateAndCommElectorate(_state, "", "");
                (StateChoosableElectorateHeader, StateInferredElectorateHeader, StateInferredElectorate)
                    = ParliamentData.InferOtherChamberInfoGivenOneRegion(_state, "", "");
                ShowStateOnly = false;
            }
        }

        private void CheckPostcode()
        {
            switch (SelectedState)
            {
                // ACT
                case 0:
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
                case 1:
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
                case 2:
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
                case 3:
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
                case 4:
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
                case 5:
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
                case 6:
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
                case 7:
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
