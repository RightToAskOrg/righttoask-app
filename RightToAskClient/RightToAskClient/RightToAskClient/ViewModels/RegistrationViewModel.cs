using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
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
        private Registration _registration = App.ReadingContext.ThisParticipant.RegistrationInfo;

        // The updates that have been made to this user's registration on this page
        // Note this is used only for updating an existing registration - new registrations are handled
        // with _registration.
        private ServerUser _registrationUpdates = new ServerUser();

        public void ReinitRegistrationUpdates()
        {
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
            get => _registration.SelectedStateAsIndex >= 0 ? ParliamentData.StatesAndTerritories[SelectedStateAsIndex] : "";
        }

        public int SelectedStateAsIndex
        {
            get => _registration.SelectedStateAsIndex;
            set
            {
                _registration.SelectedStateAsIndex = value;
                // At the moment, this means that there is no way that someone who has previously selected a state can
                // revert to the point where there is no state
                if (SelectedStateAsIndex != -1)
                {
                    _registrationUpdates.state = State;
                }
                OnPropertyChanged("State");
            }
        }

        // Electorates need to be updated in _registration and also in _registrationUpdates in case they are being altered
        // in an update to an existing registration.
        public ObservableCollection<ElectorateWithChamber> Electorates
        {
            get => _registration.electorates;
            set
            {
                _registration.UpdateMultipleElectoratesRemoveDuplicates(value);
                _registrationUpdates.electorates = _registration.electorates;
                OnPropertyChanged("Electorates");
            }
        }
        public Registration Registration
        {
            get => _registration;
            set => SetProperty(ref _registration, value);
        }

        private bool _showRegisterCitizenButton = false;
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

        private bool _showRegisterOrgButton = false;
        public bool ShowRegisterOrgButton
        {
            get => _showRegisterOrgButton;
            set => SetProperty(ref _showRegisterOrgButton, value);
        }

        private string _registerOrgButtonText = "";
        public string RegisterOrgButtonText
        {
            get => _registerOrgButtonText;
            set => SetProperty(ref _registerOrgButtonText, value);
        }

        private bool _showRegisterMPButton = false;
        public bool ShowRegisterMPButton
        {
            get => _showRegisterMPButton;
            set => SetProperty(ref _showRegisterMPButton, value);
        }

        private string _registerMPButtonText = "";
        public string RegisterMPButtonText
        {
            get => _registerMPButtonText;
            set => SetProperty(ref _registerMPButtonText, value);
        }

        private bool _showDoneButton = false;
        public bool ShowDoneButton
        {
            get => _showDoneButton;
            set => SetProperty(ref _showDoneButton, value);
        }

        private bool _showDMButton = false;
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

        private bool _showSeeQuestionsButton = false;
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

        private bool _showFollowButton = false;
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

        private bool _showUpdateAccountButton = false;
        public bool ShowUpdateAccountButton
        {
            get => _showUpdateAccountButton;
            set => SetProperty(ref _showUpdateAccountButton, value);
        }

        public List<string> StateList => ParliamentData.StatesAndTerritories;


        private ElectorateWithChamber? _selectedElectorateWithChamber = null;
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
                    _registrationUpdates.electorates = _registration.electorates;
                }
            }
        }

        #endregion

        // constructor
        public RegistrationViewModel()
        {
            // initialize defaults
            ReportLabelText = "";
            ShowUpdateAccountButton = App.ReadingContext.ThisParticipant.IsRegistered;

            _ = ShowTheRightButtonsAsync(_registration.display_name);
            RegisterMPButtonText = AppResources.RegisterMPAccountButtonText;
            RegisterOrgButtonText = AppResources.RegisterOrganisationAccountButtonText;
            CanEditUID = !App.ReadingContext.ThisParticipant.IsRegistered;

            // uid should still be sent in the 'update' even though it doesn't change.
            _registrationUpdates.uid = _registration.uid;

            // If this is this user's profile, show them IndividualParticipant data
            // (if there is any) and give them the option to edit (or make new).
            // Otherwise, if we're looking at someone else, just tell them it's another
            // person's profile.
            // TODO Clarify meaning of ReadingOnly - looks like it has a few different uses.
            if (!App.ReadingContext.IsReadingOnly)
            {
                SelectedStateAsIndex = Preferences.Get("StateID", -1);
                ShowUpdateAccountButton = App.ReadingContext.ThisParticipant.IsRegistered;
                Title = App.ReadingContext.ThisParticipant.IsRegistered ? AppResources.EditYourAccountTitle : AppResources.CreateAccountTitle;
                PopupLabelText = App.ReadingContext.ThisParticipant.IsRegistered ? AppResources.EditAccountPopupText : AppResources.CreateNewAccountPopupText;
            }
            else
            {
                Title = AppResources.UserProfileTitle;
                PopupLabelText = AppResources.OtherUserProfilePopupText;
            }

            // commands
            SaveButtonCommand = new Command(() =>
            {
                OnSaveButtonClicked();
            });
            UpdateAccountButtonCommand = new Command(() =>
            {
                SaveToPreferences();
                SendUpdatedUserToServer();
            });
            UpdateMPsButtonCommand = new Command(() =>
            {
                NavigateToFindMPsPage();
                // Update the electorate-updates that will be sent to the server, based on what was updated by the MP-finding page.
                _registrationUpdates.electorates = _registration.electorates;
            });
            RegisterMPButtonCommand = new Command(() =>
            {
                RegisterMPButtonText = "Registering not implemented yet";
            });
            RegisterOrgButtonCommand = new Command(() =>
            {
                RegisterOrgButtonText = "Registering not implemented yet";
            });
            FollowButtonCommand = new Command(() =>
            {
                FollowButtonText = "Following not implemented";
            });
            DMButtonCommand = new Command(() =>
            {
                DMButtonText = "DMs not implemented";
            });
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

        // commands
        public Command SaveButtonCommand { get; }
        public Command UpdateAccountButtonCommand { get; }
        public Command UpdateMPsButtonCommand { get; }
        public Command RegisterMPButtonCommand { get; }
        public Command RegisterOrgButtonCommand { get; }
        public Command FollowButtonCommand { get; }
        public Command DMButtonCommand { get; }
        public IAsyncCommand CancelButtonCommand { get; }
        public IAsyncCommand SeeQuestionsButtonCommand { get; }


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
        public async Task ShowTheRightButtonsAsync(string name)
        {
            if (App.ReadingContext.IsReadingOnly)
            {
                ShowRegisterCitizenButton = false;
                ShowRegisterOrgButton = false;
                ShowRegisterMPButton = false;
                ShowDoneButton = false;

                DMButtonText = string.Format(AppResources.DMButtonText, name);
                SeeQuestionsButtonText = string.Format(AppResources.SeeQuestionsButtonText, name);
                FollowButtonText = string.Format(AppResources.FollowButtonText, name);
            }
            else
            {
                ShowDMButton = false;
                ShowSeeQuestionsButton = false;
                ShowFollowButton = false;

                if (!App.ReadingContext.ThisParticipant.MPsKnown)
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
        }

        // This is called only for registering a new account - we need to include all necessary information, including
        // public key and uid. Electorates are optional.
        private async void OnSaveButtonClicked()
        {
            Debug.Assert(!App.ReadingContext.ThisParticipant.IsRegistered);

            _registration.public_key = App.ReadingContext.ThisParticipant.MyPublicKey();
            var regTest = _registration.IsValid().Err;
            if (string.IsNullOrEmpty(regTest))
            {
                // see if we need to push the electorates page or if we push the server account things
                if (!App.ReadingContext.ThisParticipant.MPsKnown)
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
            SaveToPreferences();

            Result<bool> httpResponse = await RTAClient.RegisterNewUser(_registration);
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            ReportLabelText = httpValidation.message;
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

        private void SaveToPreferences()
        {
            // save registration to preferences
            var registrationObjectToSave = JsonSerializer.Serialize(new ServerUser(_registration));
            Preferences.Set("RegistrationInfo", registrationObjectToSave);
            Preferences.Set("StateID", SelectedStateAsIndex); // stored as an int as used for the other page(s) state pickers
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
                Result<bool> httpResponse = await RTAClient.UpdateExistingUser(_registrationUpdates);
                var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
                ReportLabelText = httpValidation.message;
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
                    Debug.WriteLine("HttpValidationError: " + httpValidation.message);
                }
            }
            else
            {
                //await App.Current.MainPage.DisplayAlert(AppResources.NoAccountChangesDetectedTitle, AppResources.NoAccountChangesDetectedAlertText, AppResources.OKText);
                var popup = new OneButtonPopup(AppResources.NoAccountChangesDetectedAlertText, AppResources.OKText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            }
        }

        private void UpdateLocalRegistrationInfo()
        {
            App.ReadingContext.ThisParticipant.RegistrationInfo = _registration;
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            Preferences.Set("IsRegistered",
            App.ReadingContext.ThisParticipant.IsRegistered); // save the registration to preferences
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
