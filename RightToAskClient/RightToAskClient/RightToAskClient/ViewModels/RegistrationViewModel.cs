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
using RightToAskClient.Views.Controls;
using RightToAskClient.Views.Popups;
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
        private readonly Registration _registration = new Registration();

        private bool _isNotRegistered;

        public bool IsNotRegistered
        {
            get => _isNotRegistered;
            set => SetProperty(ref _isNotRegistered, value);
        }

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
        public string UserId

        {
            get => _registration.uid;
            // consider whether SetProperty is needed instead.
            // there may be subtle differences.
            // set => SetProperty(ref _registration.uid, )

            set
            {
                _registration.uid = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public string State =>
            _registration.StateKnown
                ? _registration.SelectedStateAsEnum.ToString()
                : "";


        private bool _stateKnown;
        private int _selectedStateAsIndex = -1;

        public int SelectedStateAsIndex
        {
            get => _selectedStateAsIndex;
            set
            {
                _selectedStateAsIndex = value;
                ParliamentData.StateEnum selectedState;
                (_stateKnown, selectedState) = _registration.UpdateStateStorePreferences(SelectedStateAsIndex);

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
                OnPropertyChanged();
            }
        }

        public string BadgesSummary => string.Join(",", _registration.Badges.Select(b => b.ToString()).ToList());

        // This is for selecting MPs if you're registering as an MP or staffer account
        private readonly SelectableList<MP> _selectableMPList = new SelectableList<MP>(new List<MP>(), new List<MP>());

        public bool IsVerifiedMPAccount =>
            _registration.Badges?.Any(b => b.badge == BadgeType.MP || b.badge == BadgeType.MPStaff) ?? false;

        public bool IsVerifiedStafferAccount => _registration.Badges?.Any(b => b.badge == BadgeType.MPStaff) ?? false;

        public string MPsRepresenting =>
            string.Join(",", _registration.Badges?.Select(b => b.name ?? "") ?? new List<string>());

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

        private string _nameLabelText = "";

        public string NameLabelText
        {
            get => _nameLabelText;
            set => SetProperty(ref _nameLabelText, value);
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


        private Accessibility.AccessibilityTrait _continueButtonAccessibilityTrait =
            Accessibility.AccessibilityTrait.Disabled;

        public Accessibility.AccessibilityTrait ContinueButtonAccessibilityTrait
        {
            get => _continueButtonAccessibilityTrait;
            set => SetProperty(ref _continueButtonAccessibilityTrait, value);
        }

        private bool _ableToContinue = false;

        public bool AbleToContinue
        {
            get => _ableToContinue;
            set
            {
                SetProperty(ref _ableToContinue, value);
                ContinueButtonAccessibilityTrait = _ableToContinue
                    ? Accessibility.AccessibilityTrait.None
                    : Accessibility.AccessibilityTrait.Disabled;
            }
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

        private bool _canEditUid = true;

        public bool CanEditUid
        {
            get => _canEditUid;
            set => SetProperty(ref _canEditUid, value);
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
        // Constructor with explicit registration info.
        public RegistrationViewModel(Registration registration) : this(false)
        {
            _registration = registration;
            ReportLabelText = "";
            switch (_registration.registrationStatus)
            {
                case RegistrationStatus.Registered:
                    IsNotRegistered = false;
                    CanEditUid = false;
                    PopupLabelText = AppResources.EditAccountPopupText;
                    Title = AppResources.EditYourAccountTitle;
                    ShowTheRightButtonsForOwnAccount();
                    break;
                case RegistrationStatus.NotRegistered:
                    IsNotRegistered = true;
                    CanEditUid = true;
                    PopupLabelText = AppResources.CreateNewAccountPopupText;
                    Title = AppResources.CreateAccountTitle;
                    ShowTheRightButtonsForOwnAccount();
                    break;
                case RegistrationStatus.AnotherPerson:
                    IsNotRegistered = true;
                    CanEditUid = false;
                    PopupLabelText = AppResources.OtherUserInfoText;
                    ShowTheRightButtonsForOtherUser(registration.display_name);
                    break;
            }

            // uid should still be sent in the 'update' even though it doesn't change.
            _registrationUpdates.uid = _registration.uid;
            _oldElectorates = _registration.Electorates;
            _selectableMPList = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
        }

        // Parameterless constructor sets defaults assuming it's a new registration for this app user, i.e ThisParticipant.
        public RegistrationViewModel() : this(false)
        {
            IsNotRegistered = true;
            _registration.registrationStatus = RegistrationStatus.NotRegistered;
        }

        // Constructor called by other constructors - sets up commands, even those that aren't used.
        // boolean input isn't used except to distinguish from default/empty constructor.

        private RegistrationViewModel(bool notUsed)
        {
            ChooseMPToRegisterButtonCommand = new AsyncCommand(async () => { SelectMPForRegistration(); });
            DoneButtonCommand = new Command(() => { OnSaveButtonClicked(); });
            UpdateAccountButtonCommand = new Command(() =>
            {
                SaveRegistrationToPreferences(_registration);
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
            DoRegistrationCommand = new AsyncCommand(async () =>
            {
                // var registerAccountPage = new RegisterAccountPage(_registration);
                // await Application.Current.MainPage.Navigation.PushAsync(registerAccountPage);
                var registerAccountFlow = new CodeOfConductPage(_registration);
                await Application.Current.MainPage.Navigation.PushAsync(registerAccountFlow);
            });
        }

        // commands
        public Command DoneButtonCommand { get; }
        public Command UpdateAccountButtonCommand { get; }
        public AsyncCommand ChooseMPToRegisterButtonCommand { get; }
        public Command UpdateMPsButtonCommand { get; }
        public Command RegisterOrgButtonCommand { get; }
        public Command FollowButtonCommand { get; }
        public Command DMButtonCommand { get; }
        public IAsyncCommand CancelButtonCommand { get; }
        public IAsyncCommand SeeQuestionsButtonCommand { get; }
        public IAsyncCommand DoRegistrationCommand { get; }

        #region Methods

        public async void NavigateToFindMPsPage()
        {
            var findMPsPage = new FindMPsPage(_registration);
            await Application.Current.MainPage.Navigation.PushAsync(findMPsPage);
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
            ShowUpdateAccountButton = _registration.IsRegistered;
            ShowRegisterMPButton = _registration.IsRegistered;
            ShowExistingMPRegistrationLabel =
                _registration.IsVerifiedMPAccount || _registration.IsVerifiedMPStafferAccount;
            ShowStafferLabel = _registration.IsVerifiedMPStafferAccount;
            ShowDMButton = false;
            ShowSeeQuestionsButton = false;
            ShowFollowButton = false;

            // TODO: move this into Registration
            if (!_registration.ElectoratesKnown)
            {
                RegisterCitizenButtonText = "Next: Find your electorates";
            }

            if (!_registration.IsRegistered)
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
            try
            {
                var userIdExists = await RTAClient.CheckUserIdExists(UserId);
                if (userIdExists)
                {
                    ReportLabelText = AppResources.UserIDAlreadyExists;
                    return;
                }
            }
            catch (Exception e)
            {
                ReportLabelText = e.Message;
                return;
            }

            ReportLabelText = "";
            NavigateToFindMPsPage();
        }

        // TODO: This may put the app into a problematic state in which the server thinks
        // it is registered, but it doesn't remember its own registration info. Alternatively,
        // it may simply be the update that has failed.
        public static void SaveRegistrationToPreferences(Registration reg)
        {
            try
            {
                var registrationObjectToSave = JsonSerializer.Serialize(new ServerUser(reg));
                XamarinPreferences.shared.Set(Constants.RegistrationInfo, registrationObjectToSave);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error storing Registration: " + e.Message);
            }
        }

        private async void SendUpdatedUserToServer()
        {
            // Shouldn't be updating a non-existent user. 
            Debug.Assert(_registration.IsRegistered);

            var hasChanges = false;
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
                Debug.Assert(_registration.IsRegistered);
                var httpResponse = await RTAClient.UpdateExistingUser(
                    _registrationUpdates,
                    _registration.uid);
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
                await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
            }
        }

        private void UpdateLocalRegistrationInfo()
        {
            XamarinPreferences.shared.Set(
                Constants.IsRegistered,
                _registration.IsRegistered);
        }


        // TODO Add email validation as from
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        private async void SelectMPForRegistration()
        {
            var pageToSearchMPs
                = new SelectableListPage(_selectableMPList, AppResources.MPSelectionText, false, true, true);

            // The user is first sent to pageToSearchMPs, and then on to pageToRegisterSelectedMP.
            // When done, they're popped all the way back here to the Account Page. 
            await Shell.Current.Navigation.PushAsync(pageToSearchMPs);

            ShowRegisterMPReportLabel = true;

            // TODO: This isn't quite right because if the registration is unsuccessful it will still show.
            ShowExistingMPRegistrationLabel = true;
        }


        private async void PromptUser(string message)
        {
            var popup = new OneButtonPopup(message, AppResources.OKText);
            _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }

        #endregion

        public void SetUserEmail(string email)
        {
            // TODO: SetUserEmail
        }

        public void ValidateUsername()
        {
            var (isValid, errMessage) = _registration.ValidateUsername();
            CheckIfAbleToContinue(null, isValid);
            ReportLabelText = errMessage;
        }

        public void ValidateName()
        {
            var (isValid, errMessage) = _registration.ValidateName();
            CheckIfAbleToContinue(isValid, null);
            NameLabelText = errMessage;
        }

        private void CheckIfAbleToContinue(bool? nameIsValid, bool? uidIsValid)
        {
            if (nameIsValid == null)
            {
                (nameIsValid, _) = _registration.ValidateName();
            }

            if (uidIsValid == null)
            {
                (uidIsValid, _) = _registration.ValidateUsername();
            }

            AbleToContinue = nameIsValid.Value && uidIsValid.Value;
        }
    }
}