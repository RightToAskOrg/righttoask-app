using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class FindMPsViewModel : BaseViewModel
    {
        #region Properties
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
        private bool _showSkipButton = false;
        public bool ShowSkipButton
        {
            get => _showSkipButton;
            set => SetProperty(ref _showSkipButton, value);
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
                if (!string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.State))
                {
                    UpdateElectoratePickerSources(App.ReadingContext.ThisParticipant.RegistrationInfo.State);
                }
                return _selectedState;
            }
            set
            {
                SetProperty(ref _selectedState, value);
                OnStatePickerSelectedIndexChanged();
            }
        }
        private int _selectedStateLAElectorate = -1;
        public int SelectedStateLAElectorate
        {
            get => _selectedStateLAElectorate;
            set
            {
                SetProperty(ref _selectedStateLAElectorate, value);
                OnStateLAElectoratePickerSelectedIndexChanged();
            }
        }
        private int _selectedStateLCElectorate = -1;
        public int SelectedStateLCElectorate
        {
            get => _selectedStateLCElectorate;
            set
            {
                SetProperty(ref _selectedStateLCElectorate, value);
                OnStateLCElectoratePickerSelectedIndexChanged();
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
        private List<string> _allFederalElectorates = new List<string>();
        public List<string> AllFederalElectorates
        {
            get => _allFederalElectorates;
            set => SetProperty(ref _allFederalElectorates, value);
        }
        private List<string> _allStateLAElectorates = new List<string>();
        public List<string> AllStateLAElectorates
        {
            get => _allStateLAElectorates;
            set => SetProperty(ref _allStateLAElectorates, value);
        }
        private List<string> _allStateLCElectorates = new List<string>();
        public List<string> AllStateLCElectorates
        {
            get => _allStateLCElectorates;
            set => SetProperty(ref _allStateLCElectorates, value);
        }
        private string _stateLAElectoratePickerTitle = string.Format("State Legislative Assembly Electorate: {0:F0}", App.ReadingContext.ThisParticipant.StateLowerHouseElectorate);
        public string StateLAElectoratePickerTitle
        {
            get => _stateLAElectoratePickerTitle;
            set => SetProperty(ref _stateLAElectoratePickerTitle, value);
        }
        private string _stateLCElectoratePickerTitle = string.Format("State Legislative Council Electorate: {0:F0}", App.ReadingContext.ThisParticipant.StateUpperHouseElectorate);
        public string StateLCElectoratePickerTitle
        {
            get => _stateLCElectoratePickerTitle;
            set => SetProperty(ref _stateLCElectoratePickerTitle, value);
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
        private bool _launchMPsSelectionPageNext;
        #endregion

        // constructor
        public FindMPsViewModel()
        {
            ShowSkipButton = false;
            ShowAddressStack = false;
            ShowKnowElectoratesFrame = false;
            _launchMPsSelectionPageNext = true;

            // commands
            MPsButtonCommand = new AsyncCommand(async () =>
            {
                if (_launchMPsSelectionPageNext)
                {
                    string message = "These are your MPs.  Select the one(s) who should answer the question";
                    var mpsExploringPage = new ExploringPage(App.ReadingContext.ThisParticipant.GroupedMPs, App.ReadingContext.Filters.SelectedAskingMPs, message);
                    await App.Current.MainPage.Navigation.PushAsync(mpsExploringPage);
                    _launchMPsSelectionPageNext = false;
                    DoneButtonText = AppResources.DoneButtonText;
                }
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
            });
            KnowElectoratesCommand = new Command(() =>
            {
                ShowKnowElectoratesFrame = true;
                ShowAddressStack = false;
            });

            // set the pickers to update their content lists here if state was already indicated elsewhere in the application
            if (SelectedState != -1 && !string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.State))
            {
                UpdateElectoratePickerSources(App.ReadingContext.ThisParticipant.RegistrationInfo.State);
            }
        }

        // commands
        public IAsyncCommand MPsButtonCommand { get; }
        public IAsyncCommand SubmitAddressButtonCommand { get; }
        public IAsyncCommand SkipButtonCommand { get; }
        public Command LookupElectoratesCommand { get; }
        public Command KnowElectoratesCommand { get; }

        // methods
        #region Methods
        // At the moment this just chooses random electorates. 
        // TODO: We probably want this to give the person a chance to go back and fix it if wrong.
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        private async Task OnSubmitAddressButton_Clicked()
        {
            Result<GeoscapeAddressFeature> httpResponse;

            string state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;

            if (String.IsNullOrEmpty(state))
            {
                await App.Current.MainPage.DisplayAlert(AppResources.SelectStateWarningText, "", "OK");
                return;
            }

            Result<bool> addressValidation = _address.SeemsValid();
            if (!String.IsNullOrEmpty(addressValidation.Err))
            {
                await App.Current.MainPage.DisplayAlert(addressValidation.Err, "", "OK");
                return;
            }

            httpResponse = await GeoscapeClient.GetFirstAddressData(_address + " " + state);

            if (httpResponse != null)
            {
                if (httpResponse.Err != null)
                {
                    ReportLabelText = httpResponse.Err;
                    return;
                }

                // Now we know everything is good.
                var bestAddress = httpResponse.Ok;
                AddElectorates(bestAddress);
                ShowFindMPsButton = true;
                ReportLabelText = "";

                bool saveThisAddress = await App.Current.MainPage.DisplayAlert("Electorates found!",
                    // "State Assembly Electorate: "+thisParticipant.SelectedLAStateElectorate+"\n"
                    // +"State Legislative Council Electorate: "+thisParticipant.SelectedLCStateElectorate+"\n"
                    "Federal electorate: " + App.ReadingContext.ThisParticipant.CommonwealthElectorate + "\n" +
                    "State lower house electorate: " + App.ReadingContext.ThisParticipant.StateLowerHouseElectorate + "\n" +
                    "Do you want to save your address on this device? Right To Ask will not learn your address.",
                    "OK - Save address on this device", "No thanks");
                if (saveThisAddress)
                {
                    SaveAddress();
                }
                ShowSkipButton = false;
            }
        }

        private void AddElectorates(GeoscapeAddressFeature addressData)
        {
            App.ReadingContext.ThisParticipant.AddElectoratesFromGeoscapeAddress(addressData);
            App.ReadingContext.ThisParticipant.MPsKnown = true;
        }

        // TODO: At the moment, this does nothing, since there's no notion of not 
        // saving the address.
        private void SaveAddress()
        {
        }

        private void OnStateLCElectoratePickerSelectedIndexChanged()
        {
            if (!String.IsNullOrEmpty(AllStateLCElectorates[SelectedStateLCElectorate]))
            {
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                App.ReadingContext.ThisParticipant.AddStateUpperHouseElectorate(state, AllStateLCElectorates[SelectedStateLCElectorate]);
                RevealNextStepIfElectoratesKnown();
            }
        }

        private void OnStateLAElectoratePickerSelectedIndexChanged()
        {
            if (!String.IsNullOrEmpty(AllStateLAElectorates[SelectedStateLAElectorate]))
            {
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                App.ReadingContext.ThisParticipant.AddStateElectoratesGivenOneRegion(state, AllStateLAElectorates[SelectedStateLAElectorate]);
                RevealNextStepIfElectoratesKnown();
            }
        }

        private void OnFederalElectoratePickerSelectedIndexChanged()
        {
            if (!String.IsNullOrEmpty(AllFederalElectorates[SelectedFederalElectorate]))
            {
                App.ReadingContext.ThisParticipant.AddHouseOfRepsElectorate(AllFederalElectorates[SelectedFederalElectorate]);
                RevealNextStepIfElectoratesKnown();
            }
        }

        private void RevealNextStepIfElectoratesKnown()
        {
            if (SelectedStateLAElectorate != -1 && SelectedFederalElectorate != -1)
            {
                ShowFindMPsButton = true;
            }
            App.ReadingContext.ThisParticipant.MPsKnown = true;
        }

        private void UpdateElectoratePickerSources(string state)
        {
            AllFederalElectorates = ParliamentData.ListElectoratesInHouseOfReps(state);
            AllStateLAElectorates = ParliamentData.ListElectoratesInStateLowerHouse(state);

            if (ParliamentData.HasUpperHouse(state))
            {
                AllStateLCElectorates = ParliamentData.ListElectoratesInStateUpperHouse(state);
            }
            else
            {
                AllStateLCElectorates = new List<string>();
                StateLCElectoratePickerTitle = string.Format(AppResources.NoUpperHousePickerTitle, state);
            }
        }

        void OnStatePickerSelectedIndexChanged()
        {
            if (SelectedState != -1)
            {
                App.ReadingContext.ThisParticipant.RegistrationInfo.State = ParliamentData.StatesAndTerritories[SelectedState];
                App.ReadingContext.ThisParticipant.AddSenatorsFromState(App.ReadingContext.ThisParticipant.RegistrationInfo.State);
                UpdateElectoratePickerSources(App.ReadingContext.ThisParticipant.RegistrationInfo.State);
            }
        }
        #endregion
    }
}
