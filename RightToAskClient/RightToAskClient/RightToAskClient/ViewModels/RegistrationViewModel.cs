using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        #region Properties

        // The complete information about this user's current registration, including any updates that have been made
        // on this page.
        private Registration _registration = new Registration(); 

        // The updates that have been made to this user's registration on this page
        // Note this is used only for updating an existing registration - new registrations are handled
        // with _registration.
        private ServerUser _registrationUpdates = new ServerUser();
        private List<ElectorateWithChamber> _oldElectorates;

        public void ReinitRegistrationUpdates()
        {
            // Note this has to be a new copy, because we want to compare it with an updated electorate list.
            _oldElectorates = new List<ElectorateWithChamber>(_registration.Electorates);
            _registrationUpdates = new ServerUser() { uid = _registration.uid };
        }
        // UserID, DisplayName, State, SelectedStateAsInt and Electorates are all just reflections of their 
        // corresponding data in _registration.
        //
        // Note that there is no need to update _registrationUpdates because UID is only set when the registration
        // is initialized.
        public string UserID

        {
            get => _registration.uid;
            // consider whether SetProperty is needed instead.
            // there may be subtle differences.
            // set => SetProperty(ref _registration.uid, )

            set
            {
                _registration.uid = value;
                OnPropertyChanged("UserID");
            }
        }

        // Update both _registration and also _registrationUpdates, because the latter may be used if we are updating
        // the display name of an existing registration.
        public string DisplayName
        {
            get => _registration.display_name;
            set
            {
                _registration.display_name = value;
                _registrationUpdates.display_name = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string State
        {
            get => _registration.StateKnown
                ? _registration.SelectedStateAsEnum.ToString()
                : "";
        }


        private bool _stateKnown;
        private int _selectedStateAsIndex = -1;
        public int SelectedStateAsIndex
        {
            get => _selectedStateAsIndex;
            set
            {
                _selectedStateAsIndex = value;
                ParliamentData.StateEnum selectedState;
                (_stateKnown, selectedState) =  App.ReadingContext.ThisParticipant.RegistrationInfo.UpdateStateStorePreferences(SelectedStateAsIndex);
                
                // Include new state in updates. At the moment, this means that there is no way that
                // someone who has previously selected a state can
                // revert to the point where there is no state
                if (_stateKnown)
                {
                    _registrationUpdates.state = selectedState.ToString();
                } 
                OnPropertyChanged("State");
            }
        }

        // Electorates need to be updated in _registration and also in _registrationUpdates in case they are being altered
        // in an update to an existing registration.
        public List<ElectorateWithChamber> Electorates
        {
            get => _registration.Electorates;
            set
            {
                _registration.Electorates = value;
                _registrationUpdates.electorates = _registration.Electorates;
                OnPropertyChanged("Electorates");
            }
        }

        public string BadgesSummary
        {
            get => String.Join(",", _registration.Badges.Select(b => b.ToString()).ToList());
        }
        
        // This is for selecting MPs if you're registering as an MP or staffer account
        private SelectableList<MP> _selectableMPList = new SelectableList<MP>(new List<MP>(), new List<MP>());
        
        public bool IsVerifiedMPAccount
        {
            get => _registration?.Badges?.Any(b =>  b.badge == BadgeType.MP || b.badge == BadgeType.MPStaff) ?? false;
        }

        public bool IsVerifiedStafferAccount
        {
            get => _registration?.Badges?.Any(b => b.badge == BadgeType.MPStaff) ?? false;
        }

        public string MPsRepresenting
        {
            get => String.Join(",",_registration?.Badges?.Select(b => b.name ?? "") ?? new List<string>());
        }
        
        // public MP RegisteredMP { get; }
        public bool ShowStafferLabel { get; set; }
        public bool ShowExistingMPRegistrationLabel { get; set; }
        
        private bool _showRegisterMPReportLabel;
        public bool ShowRegisterMPReportLabel
        {
            get => _showRegisterMPReportLabel;
            set => SetProperty(ref _showRegisterMPReportLabel, value);
        } 
        
        private bool _showRegisterCitizenButton;
        public bool ShowRegisterCitizenButton
        {
            get => _showRegisterCitizenButton;
            set => SetProperty(ref _showRegisterCitizenButton, value);
        }

        private string _registerCitizenButtonText = "";
        public string RegisterCitizenButtonText
        {
            get => _registerCitizenButtonText;
            set => SetProperty(ref _registerCitizenButtonText, value);
        }

        private bool _showRegisterOrgButton;
        public bool ShowRegisterOrgButton
        {
            get => _showRegisterOrgButton;
            set => SetProperty(ref _showRegisterOrgButton, value);
        }

        public string RegisterOrgButtonText => AppResources.RegisterOrganisationAccountButtonText;

        private bool _showRegisterMPButton;
        public bool ShowRegisterMPButton
        {
            get => _showRegisterMPButton;
            set => SetProperty(ref _showRegisterMPButton, value);
        }

        public string RegisterMPButtonText => AppResources.RegisterMPAccountButtonText;
        
        private bool _showDoneButton;
        public bool ShowDoneButton
        {
            get => _showDoneButton;
            set => SetProperty(ref _showDoneButton, value);
        }

        private bool _showDMButton;
        public bool ShowDMButton
        {
            get => _showDMButton;
            set => SetProperty(ref _showDMButton, value);
        }

        private string _dmButtonText = "";
        public string DMButtonText
        {
            get => _dmButtonText;
            set => SetProperty(ref _dmButtonText, value);
        }

        private bool _showSeeQuestionsButton;
        public bool ShowSeeQuestionsButton
        {
            get => _showSeeQuestionsButton;
            set => SetProperty(ref _showSeeQuestionsButton, value);
        }

        private string _seeQuestionsButtonText = "";
        public string SeeQuestionsButtonText
        {
            get => _seeQuestionsButtonText;
            set => SetProperty(ref _seeQuestionsButtonText, value);
        }

        private bool _showFollowButton;
        public bool ShowFollowButton
        {
            get => _showFollowButton;
            set => SetProperty(ref _showFollowButton, value);
        }

        private string _followButtonText = "";
        public string FollowButtonText
        {
            get => _followButtonText;
            set => SetProperty(ref _followButtonText, value);
        }

        private bool _canEditUID = true;
        public bool CanEditUID
        {
            get => _canEditUID;
            set => SetProperty(ref _canEditUID, value);
        }

        private bool _showUpdateAccountButton;
        public bool ShowUpdateAccountButton
        {
            get => _showUpdateAccountButton;
            set => SetProperty(ref _showUpdateAccountButton, value);
        }

        public List<string> StateList => ParliamentData.StatesAndTerritories;


        private ElectorateWithChamber? _selectedElectorateWithChamber;
        public ElectorateWithChamber? SelectedElectorateWithChamber
        {
            get => _selectedElectorateWithChamber;
            set
            {
                _ = SetProperty(ref _selectedElectorateWithChamber, value);
                if (_selectedElectorateWithChamber != null)
                {
                    NavigateToFindMPsPage();
                    // Update the electorate-updates that will be sent to the server, based on what was updated by the MP-finding page.
                    _registrationUpdates.electorates = _registration.Electorates;
                }
            }
        }

        #endregion

        // Constructors
        // Constructor with explicit registration info assumes it's someone else's registration info and initialises accordingly.
        public RegistrationViewModel(Registration reg) : this(false)
        {
            _registration = reg;
            ReportLabelText = "";
            CanEditUID = false;
            PopupLabelText = AppResources.OtherUserInfoText;
            
            // First do default command init, to make sure nothing is null.
            // SetDefaultCommands();
            ShowTheRightButtonsForOtherUser(reg.display_name);
        }
        
        // Parameterless constructor sets defaults assuming it's the registration for this app user, i.e ThisParticipant.
        public RegistrationViewModel() : this(false)
        {
            // initialize defaults
            var me = App.ReadingContext.ThisParticipant;
            Title = me.IsRegistered ? AppResources.EditYourAccountTitle : AppResources.CreateAccountTitle;
            // Title = App.ReadingContext.ThisParticipant.IsRegistered ? AppResources.EditYourAccountTitle : AppResources.CreateAccountTitle;
            ReportLabelText = "";
            _registration = me.RegistrationInfo; 
            
            ShowTheRightButtonsForOwnAccount();
            CanEditUID = !me.IsRegistered;

            // uid should still be sent in the 'update' even though it doesn't change.
            _registrationUpdates.uid = _registration.uid;
            _oldElectorates = _registration.Electorates;

            PopupLabelText = App.ReadingContext.ThisParticipant.IsRegistered ? AppResources.EditAccountPopupText : AppResources.CreateNewAccountPopupText;
                
            _selectableMPList = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());

            // SetDefaultCommands();
        }
        
        // Constructor called by other constructors - sets up commands, even those that aren't used.
        // boolean input isn't used except to distinguish from default/empty constructor.
        
            private RegistrationViewModel(bool notUsed)
            {
                ChooseMPToRegisterButtonCommand = new AsyncCommand(async () =>
                {
                    SelectMPForRegistration();
                });
                DoneButtonCommand = new Command(() => { OnSaveButtonClicked(); });
                UpdateAccountButtonCommand = new Command(() =>
                {
                    SaveRegistrationToPreferences();
                    SendUpdatedUserToServer();
                });
                UpdateMPsButtonCommand = new Command(() =>
                {
                    // We need this because we don't necessarily know that the electorates 
                    // will change just because we go to the find-new-electorates page.
                    _oldElectorates = new List<ElectorateWithChamber>(_registration.Electorates);

                    NavigateToFindMPsPage();
                });
                FollowButtonCommand = new Command(() => { FollowButtonText = "Following not implemented"; });
                DMButtonCommand = new Command(() => { DMButtonText = "DMs not implemented"; });
                CancelButtonCommand = new AsyncCommand(async () =>
                {
                    //await Navigation.PopAsync();
                    await Shell.Current.GoToAsync("..");
                });
                // At the moment, this pushes a brand new question-reading page,
                // which is meant to have only questions from this person, but
                // at the moment just has everything.
                // 
                // Think a bit harder about how people will navigate or understand this:
                // Will they expect to be adding a new stack layer, or popping off old ones?
                SeeQuestionsButtonCommand = new AsyncCommand(async () =>
                {
                    await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
                });
            }

            internal static RegistrationViewModel OnCreateInstance(bool notUsed)
            {
                return new RegistrationViewModel(notUsed);
            }

            // commands
        public Command DoneButtonCommand { get; private set;}
        public Command UpdateAccountButtonCommand { get; private set;}
        public AsyncCommand ChooseMPToRegisterButtonCommand { get; private set; }
        public Command UpdateMPsButtonCommand { get; private set; }
        public Command RegisterOrgButtonCommand { get; }
        public Command FollowButtonCommand { get; private set; }
        public Command DMButtonCommand { get; private set; }
        public IAsyncCommand CancelButtonCommand { get; private set; }
        public IAsyncCommand SeeQuestionsButtonCommand { get; private set; }


        #region Methods
        public async void NavigateToFindMPsPage()
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage2)}").ContinueWith((_) =>
            {
                MessagingCenter.Send(this, "FromReg1"); // sending Registration1ViewModel
            });

        }

        // Show and label different buttons according to whether we're registering
        // as a new user, editing our own existing profile, 
        // or viewing someone else's profile.
        public void ShowTheRightButtonsForOtherUser(string name)
        {
            ShowRegisterCitizenButton = false;
            ShowRegisterOrgButton = false;
            ShowRegisterMPButton = false;
            ShowDoneButton = false;

            DMButtonText = string.Format(AppResources.DMButtonText, name);
            SeeQuestionsButtonText = string.Format(AppResources.SeeQuestionsButtonText, name);
            FollowButtonText = string.Format(AppResources.FollowButtonText, name);
        }

        public void ShowTheRightButtonsForOwnAccount()
        {
            var me = App.ReadingContext.ThisParticipant;
            ShowUpdateAccountButton = me.IsRegistered;
            ShowRegisterMPButton = me.IsRegistered;
            ShowExistingMPRegistrationLabel = me.IsVerifiedMPAccount || me.IsVerifiedMPStafferAccount;
            ShowStafferLabel = me.IsVerifiedMPStafferAccount;
            ShowDMButton = false;
            ShowSeeQuestionsButton = false;
            ShowFollowButton = false;

            if (!App.ReadingContext.ThisParticipant.ElectoratesKnown)
            {
                RegisterCitizenButtonText = "Next: Find your electorates";
            }

            if (!App.ReadingContext.ThisParticipant.IsRegistered)
            {
                ShowRegisterCitizenButton = true;
                ShowRegisterOrgButton = true;
                ShowRegisterMPButton = true;
            }
            else
            {
                ShowRegisterCitizenButton = false;
            }
        }

        // This is called only for registering a new account - we need to include all necessary information, including
        // public key and uid. Electorates are optional.
        private async void OnSaveButtonClicked()
        {
            SaveRegistrationToPreferences();
            Debug.Assert(!App.ReadingContext.ThisParticipant.IsRegistered);

            _registration.public_key = ClientSignatureGenerationService.MyPublicKey; 
            var regTest = _registration.IsValid().Err;
            if (string.IsNullOrEmpty(regTest))
            {
                // see if we need to push the electorates page or if we push the server account things
                if (!App.ReadingContext.ThisParticipant.ElectoratesKnown)
                {
                    // if MPs have not been found for this user yet, prompt to find them
                    var popup = new TwoButtonPopup(this, AppResources.MPsPopupTitle, AppResources.MPsPopupText, AppResources.SkipButtonText, AppResources.YesButtonText);
                    _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    if (ApproveButtonClicked)
                    {
                        NavigateToFindMPsPage();
                    }
                    else
                    {
                        await SendNewUserToServer();
                    }
                }
                else
                {
                    await SendNewUserToServer();
                }
            }
            else
            {
                if (regTest != null)
                {
                    PromptUser(regTest);
                }
            }
        }

        private async Task SendNewUserToServer()
        {

            var httpResponse = await RTAClient.RegisterNewUser(_registration);
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            ReportLabelText = httpValidation.errorMessage;
            if (httpValidation.isValid)
            {
                UpdateLocalRegistrationInfo();
                     
                // if the response seemed successful, put it in more common terms for the user.
                if (ReportLabelText.Contains("Success"))
                {
                    ReportLabelText = AppResources.AccountCreationSuccessResponseText;
                }
                // Now we're registered, we can't change our UID - we can only update the other fields.
                ShowUpdateAccountButton = true;
                CanEditUID = false;
                Title = AppResources.EditYourAccountTitle;
                PopupLabelText = AppResources.EditAccountPopupText;
                // pop back to the QuestionDetailsPage after the account is created
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }

        private void SaveRegistrationToPreferences()
        {
            // save registration to preferences
            var registrationObjectToSave = JsonSerializer.Serialize(new ServerUser(_registration));
            Preferences.Set(Constants.RegistrationInfo, registrationObjectToSave);
            // Preferences.Set(Constants.State, _registration.SelectedStateAsEnum.ToString()); 
        }

        private async void SendUpdatedUserToServer()
        {
            // Shouldn't be updating a non-existent user. 
            Debug.Assert(App.ReadingContext.ThisParticipant.IsRegistered);

            bool hasChanges = false;
            if (_registrationUpdates.uid == null)
            {
                return;
            }
            
            // Check whether the state has been updated (in the FindMPs page).
            // If it has, update the display on this page and add the new state
            // to _registrationUpdates.
            if (_registration.StateKnown && (int)_registration.SelectedStateAsEnum != SelectedStateAsIndex)
            {
                _registrationUpdates.state = _registration.SelectedStateAsEnum.ToString();
                SelectedStateAsIndex = (int)_registration.SelectedStateAsEnum;
                OnPropertyChanged(State);
            }
            // Update the electorate-updates that will be sent to the server,
            // based on what was updated by the MP-finding page, if it is actually changed.
            // This will update both _registrationUpdates and _registration.
            if (!_oldElectorates.HasSameElements(_registration.Electorates))
            {
                Electorates = _registration.Electorates;
            }
            
            // if display name, state, electorates, or badges were changed, send the update
            if (_registrationUpdates.display_name != null
                || _registrationUpdates.state != null
                || _registrationUpdates.electorates != null
                || _registrationUpdates.badges != null)
            {
                hasChanges = true;
            }

            // displays an alert if no changes were found on the user's account via the _registrationUpdates object
            if (hasChanges)
            {
                var httpResponse = await RTAClient.UpdateExistingUser(_registrationUpdates);
                var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
                ReportLabelText = httpValidation.errorMessage;
                if (httpValidation.isValid)
                {
                    // if the response seemed successful, put it in more common terms for the user.
                    if (ReportLabelText.Contains("Success"))
                    {
                        ReportLabelText = AppResources.AccountUpdateSuccessResponseText;
                    }
                    UpdateLocalRegistrationInfo();
                }
                else
                {
                    // IsRegistered flags in both Readingcontext and Preferences default to false.
                    Debug.WriteLine("HttpValidationError: " + httpValidation.errorMessage);
                }
            }
            else
            {
                var popup = new OneButtonPopup(AppResources.NoAccountChangesDetectedAlertText, AppResources.OKText);
                await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            }
        }

        private void UpdateLocalRegistrationInfo()
        {
            App.ReadingContext.ThisParticipant.RegistrationInfo = _registration;
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            // save the registration to preferences
            Preferences.Set(Constants.IsRegistered, App.ReadingContext.ThisParticipant.IsRegistered); 
        }



        // TODO Add email validation as from
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        private async void SelectMPForRegistration()
        {
            var pageToSearchMPs
                    = new SelectableListPage(_selectableMPList, AppResources.MPSelectionText , false, true, true);

            // The user is first sent to pageToSearchMPs, and then on to pageToRegisterSelectedMP.
            // When done, they're popped all the way back here to the Account Page. 
            await Shell.Current.Navigation.PushAsync(pageToSearchMPs).ContinueWith(async (_) => 
            {
                MessagingCenter.Send(this, "RegMPAccount", _selectableMPList);
                // var pageToRegisterSelectedMP = new MPRegistrationVerificationPage(_selectableMPList);
                // await App.Current.MainPage.Navigation.PushAsync(pageToRegisterSelectedMP);
            }); 
            ShowRegisterMPReportLabel = true;

            // TODO: This isn't quite right because if the registration is unsuccessful it will still show.
            ShowExistingMPRegistrationLabel = true;
        }


        private async void PromptUser(string message)
        {
            //await App.Current.MainPage.DisplayAlert("Registration incomplete", message, "OK");
            var popup = new OneButtonPopup(message, AppResources.OKText);
            _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }
        #endregion
    }
}
