using RightToAskClient.CryptoUtils;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Annotations;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;

namespace RightToAskClient.ViewModels
{
    public class QuestionViewModel : BaseViewModel
    {
        private static QuestionViewModel? _instance;
        public static QuestionViewModel Instance => _instance ??= new QuestionViewModel();

        private Question _question = new Question();
        public Question Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }

        private QuestionSendToServer _serverQuestionUpdates = new QuestionSendToServer();
        public QuestionSendToServer ServerQuestionUpdates
        {
            get => _serverQuestionUpdates;
            set => SetProperty(ref _serverQuestionUpdates, value);
        }

        private bool _isNewQuestion;
        public bool IsNewQuestion
        {
            get => _isNewQuestion;
            set => SetProperty(ref _isNewQuestion, value);
        }

        private bool _isReadingOnly;
        public bool IsReadingOnly
        {
            get => _isReadingOnly;
            set => SetProperty(ref _isReadingOnly, value);
        }

        private bool _raisedByOptionSelected;
        public bool RaisedByOptionSelected
        {
            get => _raisedByOptionSelected;
            set
            {
                SetProperty(ref _raisedByOptionSelected, value);
                if (_raisedByOptionSelected)
                {
                    Question.AnswerInApp = false;
                    AnswerInApp = false;
                }
            }
        }

        private bool _displayFindCommitteeButton;
        public bool DisplayFindCommitteeButton
        {
            get => _displayFindCommitteeButton;
            set => SetProperty(ref _displayFindCommitteeButton, value);
        }

        private bool _displaySenatesEstimatesSection;
        public bool DisplaySenateEstimatesSection
        {
            get => _displaySenatesEstimatesSection;
            set => SetProperty(ref _displaySenatesEstimatesSection, value);
        }

        private string _senateEstimatesAppearanceText = "";
        public string SenateEstimatesAppearanceText
        {
            get => _senateEstimatesAppearanceText;
            set => SetProperty(ref _senateEstimatesAppearanceText, value);
        }

        private bool _enableMyMPShouldRaiseButton;
        public bool EnableMyMPShouldRaiseButton
        {
            get => _enableMyMPShouldRaiseButton;
            set => SetProperty(ref _enableMyMPShouldRaiseButton, value);
        }

        private bool _enableAnotherMPShouldRaiseButton;
        public bool EnableAnotherMPShouldRaiseButton
        {
            get => _enableAnotherMPShouldRaiseButton;
            set => SetProperty(ref _enableAnotherMPShouldRaiseButton, value);
        }

        private bool _goHome;
        public bool GoHome
        {
            get => _goHome;
            set => SetProperty(ref _goHome, value);
        }

        private bool _showReportLabel;
        public bool ShowReportLabel
        {
            get => _showReportLabel;
            set => SetProperty(ref _showReportLabel, value);
        }

        private string _anotherUserButtonText = "";
        public string AnotherUserButtonText
        {
            get => _anotherUserButtonText;
            set => SetProperty(ref _anotherUserButtonText, value);
        }

        private string _notSureWhoShouldRaiseButtonText = "";
        public string NotSureWhoShouldRaiseButtonText
        {
            get => _notSureWhoShouldRaiseButtonText;
            set => SetProperty(ref _notSureWhoShouldRaiseButtonText, value);
        }

        private string _selectButtonText = "";
        public string SelectButtonText
        {
            get => _selectButtonText;
            set => SetProperty(ref _selectButtonText, value);
        }

        private string _upvoteButtonText = AppResources.UpvoteButtonText;
        public string UpvoteButtonText
        {
            get => _upvoteButtonText;
            set => SetProperty(ref _upvoteButtonText, value);
        }

        private string _saveButtonText = AppResources.SaveAnswerButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }

        private bool _canEditBackground;
        public bool CanEditBackground
        {
            get => _canEditBackground;
            set => SetProperty(ref _canEditBackground, value);
        }

        private bool _canEditQuestion;
        public bool CanEditQuestion
        {
            get => _canEditQuestion;
            set => SetProperty(ref _canEditQuestion, value);
        }

        private bool _answerInApp = false;
        public bool AnswerInApp
        {
            get => _answerInApp;
            set => SetProperty(ref _answerInApp, value);
        }

        public bool ShowEditQuestionButton => CanEditQuestion && !IsNewQuestion;

        public string QuestionSuggesterButtonText => QuestionViewModel.Instance.IsNewQuestion ? AppResources.EditProfileButtonText : String.Format(AppResources.ViewOtherUserProfile, QuestionViewModel.Instance.Question.QuestionSuggester);

        public bool MPButtonsEnabled => ParliamentData.MPAndOtherData.IsInitialised;
        public void UpdateMPButtons()
        {
            OnPropertyChanged("MPButtonsEnabled"); // called by the UpdatableParliamentAndMPData class to update this variable in real time
        }
        public bool NeedToFindAsker => App.ReadingContext.Filters.SelectedAnsweringMPsNotMine.IsNullOrEmpty();

        private RTAPermissions _whoShouldAnswerItPermissions = RTAPermissions.NoChange;

        public RTAPermissions WhoShouldAnswerItPermissions
        {
            get => _whoShouldAnswerItPermissions;
            set
            {
                // It should only be set when it changes, not set to 'no change'
                Debug.Assert(value != RTAPermissions.NoChange);

                _whoShouldAnswerItPermissions = value;
                //Question.OthersCanAddAnswerers = value == RTAPermissions.Others;
                //_serverQuestionUpdates.who_should_answer_the_question_permissions = value;
            }
        }

        private RTAPermissions _whoShouldAskItPermissions = RTAPermissions.NoChange;
        public RTAPermissions WhoShouldAskItPermissions
        {
            get => _whoShouldAskItPermissions;
            set
            {
                // It should only be set when it changes, not set to 'no change'
                Debug.Assert(value != RTAPermissions.NoChange);
                
                _whoShouldAskItPermissions = value;
                //Question.OthersCanAddAskers = value == RTAPermissions.Others;
                //_serverQuestionUpdates.who_should_ask_the_question_permissions = value;
            }
        }

        public QuestionViewModel()
        {
            // set defaults
            ResetInstance();

            // commands
            ProceedToReadingPageCommand = new AsyncCommand(async() => 
            {
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            // If in read-only mode, initiate a question-reading page.
            // Similarly if my MP is answering.
            // If drafting, load a question-asker page, which will then 
            // lead to a question-reading page.
            //
            // TODO also doesn't do the right thing if you've previously selected
            // someone other than your MP.  In other words, it should enforce exclusivity -
            // either your MP(s) answer it, or an authority or other MP answers it.
            // At the moment this exclusivity is not enforced.
            NavigateForwardCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(FlowOptionPage)}");
                //if (NeedToFindAsker)
                //{
                //    await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
                //}
                //else
                //{
                //    App.ReadingContext.IsReadingOnly = IsReadingOnly;
                //    await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
                //}
            });
            OtherPublicAuthorityButtonCommand = new AsyncCommand(async () =>
            {
                Question.AnswerInApp = false;
                AnswerInApp = false;
                // var selectableList = new SelectableList<Authority>(ParliamentData.AllAuthorities, App.ReadingContext.Filters.SelectedAuthorities); 
                var PageToSearchAuthorities
                    = new SelectableListPage(App.ReadingContext.Filters.AuthorityLists, "Choose authorities");
                await Shell.Current.Navigation.PushAsync(PageToSearchAuthorities).ContinueWith((_) => 
                {
                    MessagingCenter.Send(this, "OptionBGoToAskingPageNext"); // Sends this view model
                });
            });
            // If we already know the electorates (and hence responsible MPs), go
            // straight to the Explorer page that lists them.
            // If we don't, go to the page for entering address and finding them.
            // It will pop back to here.
            AnsweredByMyMPCommand = new AsyncCommand(async () =>
            {
                Question.AnswerInApp = true;
                AnswerInApp = true;
                await NavigationUtils.PushMyAnsweringMPsExploringPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, "GoToReadingPage"); // Sends this view model
                });
            });
            AnsweredByOtherMPCommand = new AsyncCommand(async () =>
            {
                Question.AnswerInApp = true;
                AnswerInApp = true;
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, "GoToReadingPage"); // Sends this view model
                });
            });
            AnsweredByOtherMPCommandOptionB = new AsyncCommand(async () =>
            {
                Question.AnswerInApp = false;
                AnswerInApp = false;
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, "OptionBGoToAskingPageNext"); // Sends this view model
                });
            });
            SelectCommitteeButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.Filters.SelectedAskingCommittee.Add("Senate Estimates tomorrow");
                SelectButtonText = AppResources.SelectedButtonText;
                // then navigate to the reading page
                await Shell.Current.GoToAsync(nameof(ReadingPage));
            });
            UpvoteCommand = new AsyncCommand(async () =>
            {
                await DoRegistrationCheck();
                if (App.ReadingContext.ThisParticipant.IsRegistered)
                {
                    // upvoting a question will add it to their list
                    App.ReadingContext.ThisParticipant.HasQuestions = true;
                    Preferences.Set("HasQuestions", true);
                    if (Question.AlreadyUpvoted)
                    {
                        Question.UpVotes--;
                        Question.AlreadyUpvoted = false;
                        UpvoteButtonText = AppResources.UpvoteButtonText;
                    }
                    else
                    {
                        Question.UpVotes++;
                        Question.AlreadyUpvoted = true;
                        UpvoteButtonText = AppResources.UpvotedButtonText;
                    }
                }
            });
            SaveAnswerCommand = new Command(() =>
            {
                SaveButtonText = "Answer saving not implemented";
            });
            SaveQuestionCommand = new Command(() =>
            {
                SubmitNewQuestionButton_OnClicked();
            });
            EditAnswerCommand = new Command(() =>
            {
                EditQuestionButton_OnClicked();
            });
            QuestionSuggesterCommand = new AsyncCommand(async () =>
            {
                string userId = Question.QuestionSuggester;
                var userToSend = await RTAClient.GetUserById(userId);
                if (userToSend.Err != null)
                {
                    ReportLabelText = "Could not find user on the server: " + userToSend.Err;
                }
                if (userToSend.Ok != null)
                {
                    await Shell.Current.GoToAsync($"{nameof(OtherUserProfilePage)}").ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "OtherUser", userToSend.Ok); // Send person or send question
                    });
                }
            });
            BackCommand = new AsyncCommand(async () =>
            {
                HomeButtonCommand.Execute(null); // just inherit the functionality of the home button from BaseViewModel
            });
            OptionACommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            });
            OptionBCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            });
            ToMetadataPageCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(MetadataPage)}");
            });
        }

        private Command? _findCommitteeCommand;
        public Command FindCommitteeCommand => _findCommitteeCommand ??= new Command(OnFindCommitteeButtonClicked);
        private Command? _myMpRaiseCommand;
        public Command MyMPRaiseCommand => _myMpRaiseCommand ??= new Command(OnMyMPRaiseButtonClicked);
        private Command? _otherMPRaiseCommand;
        public Command OtherMPRaiseCommand => _otherMPRaiseCommand ??= new Command(OnOtherMPRaiseButtonClicked);
        private Command? _userShouldRaiseCommand;
        public Command UserShouldRaiseCommand => _userShouldRaiseCommand ??= new Command(UserShouldRaiseButtonClicked);
        private Command? _notSureWhoShouldRaiseCommand;
        public Command NotSureWhoShouldRaiseCommand => _notSureWhoShouldRaiseCommand ??= new Command(NotSureWhoShouldRaiseButtonClicked);
        public IAsyncCommand ProceedToReadingPageCommand { get; }
        public IAsyncCommand NavigateForwardCommand { get; }
        public IAsyncCommand OtherPublicAuthorityButtonCommand { get; }
        public IAsyncCommand AnsweredByMyMPCommand { get; }
        public IAsyncCommand AnsweredByOtherMPCommand { get; }
        public IAsyncCommand AnsweredByOtherMPCommandOptionB { get; }
        public IAsyncCommand QuestionSuggesterCommand { get; }
        public IAsyncCommand BackCommand { get; }
        public Command SaveQuestionCommand { get; }
        public IAsyncCommand SelectCommitteeButtonCommand { get; }
        public IAsyncCommand UpvoteCommand { get; }
        public Command SaveAnswerCommand { get; }
        public Command EditAnswerCommand { get; }
        public IAsyncCommand OptionACommand { get; }
        public IAsyncCommand OptionBCommand { get; }
        public IAsyncCommand ToMetadataPageCommand { get; }
        public IAsyncCommand ToDetailsPageCommand { get; }
        public Command AnsweringMPsFilterCommand { get; }
        public Command AskingMPsFilterCommand { get; }

        public void ResetInstance()
        {
            // set defaults
            Question = new Question();
            IsNewQuestion = false;
            IsReadingOnly = App.ReadingContext.IsReadingOnly; // crashes here if setting up existing test questions
            RaisedByOptionSelected = false;
            DisplayFindCommitteeButton = true;
            DisplaySenateEstimatesSection = false;
            SenateEstimatesAppearanceText = "";
            AnotherUserButtonText = AppResources.AnotherUserButtonText;
            NotSureWhoShouldRaiseButtonText = AppResources.NotSureButtonText;
            SelectButtonText = AppResources.SelectButtonText;
            ReportLabelText = "";
            // TODO not sure whether either of these is the right default - suggest not setting either?
            App.ReadingContext.DraftQuestion = Question.QuestionText;
            Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.uid;
        }

        public void ReinitQuestionUpdates()
        {
            ServerQuestionUpdates = new QuestionSendToServer();
            Question.ReinitQuestionUpdates();
        }

        // methods for selecting who will raise your question
        private void OnFindCommitteeButtonClicked()
        {
            RaisedByOptionSelected = true;
            DisplayFindCommitteeButton = false;
            DisplaySenateEstimatesSection = true;
            SenateEstimatesAppearanceText =
                String.Join(" ", App.ReadingContext.Filters.SelectedAuthorities)
                    + " is appearing at Senate Estimates tomorrow";
        }

        // Note that the non-waiting for this asyc method means that the rest of the page can keep
        // Executing. That shouldn't be a problem, though, because it is invisible and therefore unclickable.
        private async void OnMyMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                RaisedByOptionSelected = true;
                await NavigationUtils.PushMyAskingMPsExploringPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, "GoToReadingPage"); // Sends this view model
                    MessagingCenter.Send(this, "OptionBAskingNow");
                });
            }
            else
            {
                RedoButtonsForUnreadableMPData();
            }
        }

        private void RedoButtonsForUnreadableMPData()
        {
            EnableMyMPShouldRaiseButton = false;
            EnableAnotherMPShouldRaiseButton = false;
            ShowReportLabel = true;
            ReportLabelText = ParliamentData.MPAndOtherData.ErrorMessage;
        }

        private async void NotSureWhoShouldRaiseButtonClicked()
        {
            RaisedByOptionSelected = true;
            NotSureWhoShouldRaiseButtonText = "Not implemented yet";
            await Shell.Current.GoToAsync(nameof(ReadingPage));
        }

        // TODO: Implement an ExporingPage constructor for people.
        private async void UserShouldRaiseButtonClicked()
        {
            RaisedByOptionSelected = true;
            AnotherUserButtonText = "Not Implemented Yet";
            await Shell.Current.GoToAsync(nameof(ReadingPage));
        }

        private async void OnOtherMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsNotMineSelectableListPageAsync().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, "GoToReadingPage"); // Sends this view model
                });
            }
            else
            {
                RedoButtonsForUnreadableMPData();
            }
        }

        // If we already know the electorates (and hence responsible MPs), go
        // straight to the Explorer page that lists them.
        // If we don't, go to the page for entering address and finding them.
        // It will pop back to here.
        private async void OnAnsweredByMPButtonClicked(object sender, EventArgs e)
        {
            await NavigationUtils.PushMyAnsweringMPsExploringPage();
        }

        private async void OnAnswerByOtherMPButtonClicked(object sender, EventArgs e)
        {
            await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage();
        }

        private async void SubmitNewQuestionButton_OnClicked()
        {
            await DoRegistrationCheck();
            
            if (App.ReadingContext.ThisParticipant.IsRegistered)
            {
                // This isn't necessary unless the person has just registered, but is necessary if they have.
                QuestionViewModel.Instance.Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.uid;
                bool validQuestion = Question.ValidateNewQuestion();
                if (validQuestion)
                {
                    SendNewQuestionToServer();
                }                
            }
        }
        
        // TODO Consider permissions for question editing.
        private async void EditQuestionButton_OnClicked()
        {
            await DoRegistrationCheck();

            if (App.ReadingContext.ThisParticipant.IsRegistered)
            {
                bool validQuestion = Question.ValidateUpdateQuestion();
                if (validQuestion) 
                {
                    sendQuestionEditToServer();
                }                
            }

        }

        private async Task DoRegistrationCheck()
        {
            if (!App.ReadingContext.ThisParticipant.IsRegistered)
            {
                //string message = AppResources.CreateAccountPopUpText;
                //bool registerNow
                //    = await App.Current.MainPage.DisplayAlert(AppResources.MakeAccountQuestionText, message, AppResources.OKText, AppResources.NotNowAnswerText);
                var popup = new TwoButtonPopup(QuestionViewModel.Instance, AppResources.MakeAccountQuestionText, AppResources.CreateAccountPopUpText, AppResources.CancelButtonText, AppResources.OKText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (ApproveButtonClicked)
                {
                    await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
                }
            }
        }

        // For uploading a new question
        // This should be called only if the person is already registered.
        private async void SendNewQuestionToServer()
        {
            // This isn't supposed to be called for unregistered participants.
            if (!App.ReadingContext.ThisParticipant.IsRegistered) return;

            // Insert the question into our own list even if it doesn't upload.
            App.ReadingContext.ExistingQuestions.Insert(0, Question);

            (bool isValid, string errorMessage) successfulSubmission = await BuildSignAndUploadNewQuestion();

            if (!successfulSubmission.isValid)
            {
                ReportLabelText =  "Error uploading new question: " + successfulSubmission.errorMessage;
                return;
            }
            
            // Reset the draft question only if it didn't upload correctly.
            App.ReadingContext.DraftQuestion = "";

            // creating a question will add it to their list
            App.ReadingContext.ThisParticipant.HasQuestions = true;
            Preferences.Set("HasQuestions", true);

            //FIXME update version, just like for edits.

            var popup = new QuestionPublishedPopup();
            _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (GoHome)
            {
                App.ReadingContext.Filters.RemoveAllSelections();
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
            else // Pop back to readingpage. TODO: fix the context so that it doesn't think you're drafting
                // a question.  Possibly the right thing to do is pop everything and then push a reading page.
            {
                //await Navigation.PopAsync();
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            }
        }

        private async void sendQuestionEditToServer()
        {
            // This isn't supposed to be called for unregistered participants.
            if (!App.ReadingContext.ThisParticipant.IsRegistered) return;
            
            (bool isValid, string errorMessage) successfulSubmission = await BuildSignAndUploadQuestionUpdates();
            
            if (!successfulSubmission.isValid)
            {
                // await App.Current.MainPage.DisplayAlert("Error editing question", successfulSubmission.errorMessage, "OK");
                string message = string.Format(AppResources.EditQuestionErrorText, successfulSubmission.errorMessage);
                var popup2 = new OneButtonPopup(message, AppResources.OKText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup2);
                ReportLabelText = "Error editing question: " + successfulSubmission.errorMessage;
                return;
            }
            
            // Success - reinitialize question state and make sure we've got the most up to date version.
            Question.ReinitQuestionUpdates();
            
            // FIXME at the moment, the version isn't been correctly updated.
            // TODO: Here, we'll need to ensure we've got the right version (from the server - get it returned from
            // BuildSignAndUpload... 
            var popup = new TwoButtonPopup(QuestionViewModel.Instance, AppResources.QuestionEditSuccessfulPopupTitle, AppResources.QuestionEditSuccessfulPopupText, AppResources.StayOnCurrentPageButtonText, AppResources.GoHomeButtonText);
            _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (ApproveButtonClicked)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        private async Task<(bool isValid, string message)> BuildSignAndUploadNewQuestion()
        {
            var serverQuestion = new QuestionSendToServer(Question);
            setQuestionEditPermissions(serverQuestion);
            TranscribeQuestionFiltersForUpload(serverQuestion);

            Result<bool> httpResponse = await RTAClient.RegisterNewQuestion(serverQuestion);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Upload");
        }

        private async Task<(bool isValid, string message)> BuildSignAndUploadQuestionUpdates()
        {
            var serverQuestionUpdates = Question.Updates;
            setQuestionEditPermissions(serverQuestionUpdates);
            
            // needs these two fields in the message payload for updates, but not for new questions.
            serverQuestionUpdates.question_id = Question.QuestionId;
            serverQuestionUpdates.version = Question.Version;

            Result<bool> httpResponse = await RTAClient.UpdateExistingQuestion(serverQuestionUpdates);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Edit");
        }
       
        private void setQuestionEditPermissions(QuestionSendToServer serverQuestionUpdates)
        {
            serverQuestionUpdates.who_should_answer_the_question_permissions = _whoShouldAnswerItPermissions;
            serverQuestionUpdates.who_should_ask_the_question_permissions = _whoShouldAskItPermissions;
        }
        
        // Interprets the current filters into the right form for server upload.
        // This clearly doesn't work for *updates* - it simply reports the current settings
        // regardless of whether they have been altered.
        private void TranscribeQuestionFiltersForUpload(QuestionSendToServer currentQuestionForUpload)
        {
            // We take the (duplicate-removing) union of selected MPs, because at the moment the UI doesn't remove 
            // your MPs from the 'other MPs' list and the user may have selected the same MP in both categories.
            var MPAnswerers = Question.Filters.SelectedAnsweringMPsNotMine.Union(Question.Filters.SelectedAnsweringMPsMine);
            var MPanswerersServerData = MPAnswerers.Select(mp => new PersonID(new MPId(mp)));
            
            // Add authorities, guaranteed not to be duplicates.
            List<PersonID> answerers = MPanswerersServerData.
                Concat(Question.Filters.SelectedAuthorities.Select(a => new PersonID(a))).ToList();
            currentQuestionForUpload.entity_who_should_answer_the_question = answerers;

            // Entities who should raise the question - currently just MPs.
            // TODO add committees, other users, etc.
            var MPAskers = Question.Filters.SelectedAskingMPsNotMine.Union(Question.Filters.SelectedAskingMPsMine);
            var MPAskersServerData = MPAskers.Select(mp => new PersonID(new MPId(mp)));

            List<PersonID> askers = MPAskersServerData.ToList();
            currentQuestionForUpload.mp_who_should_ask_the_question = askers;
        }
    }
}
