using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class Registration1ViewModel : BaseViewModel
    {
        #region Properties
        private Registration _registration = new Registration();
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

        private ElectorateWithChamber? _selectedElectorateWithChamber = null;
        public ElectorateWithChamber? SelectedElectorateWithChamber
        {
            get => _selectedElectorateWithChamber;
            set
            {
                _ = SetProperty(ref _selectedElectorateWithChamber, value);
                if (_selectedElectorateWithChamber != null)
                {
                    ElectorateSelected();
                }
            }
        }
        private int _selectedState = -1;
        public int SelectedState
        {
            get => _selectedState;
            set
            {
                SetProperty(ref _selectedState, value);
                if (SelectedState != -1)
                {
                    Registration.State = ParliamentData.StatesAndTerritories[SelectedState];
                    App.ReadingContext.ThisParticipant.RegistrationInfo.State = Registration.State;
                    App.ReadingContext.ThisParticipant.UpdateChambers(Registration.State);
                }
            }
        }
        #endregion

        // constructor
        public Registration1ViewModel()
        {
            // initialize defaults
            Registration = App.ReadingContext.ThisParticipant.RegistrationInfo ?? new Registration()
            {
                uid = "This is a test user",
                display_name = "testing user",
                public_key = "123",
                State = "VIC"
            };
            ShowTheRightButtonsAsync(Registration.display_name);
            Title = App.ReadingContext.IsReadingOnly ? AppResources.UserProfileTitle : AppResources.CreateAccountTitle;
            RegisterMPButtonText = AppResources.RegisterMPAccountButtonText;
            RegisterOrgButtonText = AppResources.RegisterOrganisationAccountButtonText;

            // get account info from preferences
            Registration.display_name =  Preferences.Get("DisplayName", Registration.display_name);
            Registration.uid = Preferences.Get("UID", Registration.uid);
            SelectedState = Preferences.Get("State", -1);

            // commands
            SaveButtonCommand = new Command(() =>
            {
                OnSaveButtonClicked();
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
        public Command RegisterMPButtonCommand { get; }
        public Command RegisterOrgButtonCommand { get; }
        public Command FollowButtonCommand { get; }
        public Command DMButtonCommand { get; }
        public IAsyncCommand CancelButtonCommand { get; }
        public IAsyncCommand SeeQuestionsButtonCommand { get; }

        #region Methods
        // TODO Make this put up the electorate-finding page.
        public async void ElectorateSelected()
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage2)}").ContinueWith((_) =>
            {
                MessagingCenter.Send(this, "FromReg1"); // sending Registration1ViewModel
            });
        }

        // Show and label different buttons according to whether we're registering
        // as a new user, or viewing someone else's profile.
        public void ShowTheRightButtonsAsync(string name)
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
                    if (App.ReadingContext.ThisParticipant.MPsKnown)
                    {
                        _ = Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Electorates already selected",
                            "You have already selected your electorates - you can change them if you like",
                            "OK");
                    }
                    ShowRegisterCitizenButton = false;
                }
            }
        }

        private async void OnSaveButtonClicked()
        {
            var newRegistration = Registration;
            newRegistration.public_key = ClientSignatureGenerationService.MyPublicKey();
            var regTest = newRegistration.IsValid().Err;
            if (string.IsNullOrEmpty(regTest))
            {
                // see if we need to push the electorates page or if we push the server account things
                if (!App.ReadingContext.ThisParticipant.MPsKnown)
                {
                    await Shell.Current.GoToAsync($"{nameof(RegisterPage2)}").ContinueWith((_) => 
                    {
                        MessagingCenter.Send(this, "FromReg1"); // sending Registration1ViewModel
                    });
                }
                else
                {
                    // save to preferences
                    Preferences.Set("DisplayName", Registration.display_name);
                    Preferences.Set("UID", Registration.uid);
                    Preferences.Set("State", SelectedState); // stored as an int as used for the other page(s) state pickers
                    Result<bool> httpResponse = await RTAClient.RegisterNewUser(newRegistration);
                    var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
                    ReportLabelText = httpValidation.message;
                    if (httpValidation.isValid)
                    {
                        App.ReadingContext.ThisParticipant.RegistrationInfo = newRegistration;
                        App.ReadingContext.ThisParticipant.IsRegistered = true;
                        Preferences.Set("IsRegistered", App.ReadingContext.ThisParticipant.IsRegistered); // save the registration to preferences
                        // pop back to the QuestionDetailsPage after the account is created
                        await App.Current.MainPage.Navigation.PopAsync();
                    }
                }
            }
            else
            {
                if(regTest != null)
                {
                    PromptUser(regTest);
                }
            }
        }

        private async void PromptUser(string message)
        {
            await App.Current.MainPage.DisplayAlert("Registration incomplete", message, "OK");
        }
        #endregion
    }
}
