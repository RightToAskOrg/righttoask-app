using System;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RightToAskClient.Helpers;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;

namespace RightToAskClient.ViewModels
{
    public class QuestionViewModel : BaseViewModel
    {
        private static QuestionViewModel? _instance;
        protected QuestionResponseRecords ResponseRecords = new QuestionResponseRecords();
        public static QuestionViewModel Instance => _instance ??= new QuestionViewModel();

        private Question _question = new Question();
        public Question Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }

        private FilterChoices _filterChoices = new FilterChoices();

        private string _newAnswer = "";
        public string NewAnswer
        {
            get => _newAnswer; 
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                
                SetProperty(ref _newAnswer, value);
                Question.AddAnswer(value);
                OnPropertyChanged();
            }
        }

        // Convenient views of things stored in the Question.
        public List<Answer> QuestionAnswers => Question.Answers;

        public string QuestionAnswerers =>  
            Extensions.JoinFilter(", ",
                string.Join(", ",Question.Filters.SelectedAnsweringMPsNotMine.Select(mp => mp.ShortestName)),
                string.Join(", ",Question.Filters.SelectedAnsweringMPsMine.Select(mp => mp.ShortestName)),
                string.Join(", ",Question.Filters.SelectedAuthorities.Select(a => a.ShortestName)));

        // The MPs or committee who are meant to ask the question
        public string QuestionAskers =>
            Extensions.JoinFilter(", ",
                string.Join(", ", Question.Filters.SelectedAskingMPsNotMine.Select(mp => mp.ShortestName)), 
                string.Join(", ", Question.Filters.SelectedAskingMPsMine.Select(mp => mp.ShortestName)), 
                string.Join(",", Question.Filters.SelectedCommittees.Select(com => com.ShortestName)));

        private string? _newHansardLink; 
        public string NewHansardLink 
        { 
            get => _newHansardLink;
            set
            {
                var urlResult = ParliamentData.StringToValidParliamentaryUrl(value);
                if (urlResult.Success)
                {
                    SetProperty(ref _newHansardLink, value);
                    Question.AddHansardLink(urlResult.Data);
                    OnPropertyChanged("HansardLink");
                }
                else
                {
                    var errorMessage = AppResources.InvalidHansardLink;
                    if (urlResult is ErrorResult<Uri> errorResult)
                    {
                        errorMessage += errorResult.Message;
                    }
                    ReportLabelText = errorMessage; 
                    OnPropertyChanged("ReportLabelText");
                }
            }
        }

        // A collection of flags describing the state of the question,
        // whether it's a new question, whether you have permission to edit it,
        // etc. The idea is that facts about its status should be set at init,
        // and decisions about what to display in the view model are derived from
        // those facts.
        
        // Whether this is a new question
        private bool _isNewQuestion;
        public bool IsNewQuestion
        {
            get => _isNewQuestion;
            set => SetProperty(ref _isNewQuestion, value);
        }

        private HowAnsweredOptions _howAnswered = HowAnsweredOptions.DontKnow; 

        public HowAnsweredOptions HowAnswered
        {
            get => _howAnswered;
            set => SetProperty(ref _howAnswered, value);
        }

        // These buttons are disabled if for some reason we're unable to read MP data.
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
        
        // TODO Can we just use this flag everywhere instead of the two above?
        public bool MPButtonsEnabled => ParliamentData.MPAndOtherData.IsInitialised;

        // private bool _canEditBackground;
        public bool CanEditBackground
        {
            get
            {
                var thisUser =  IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
                var questionWriter = _question.QuestionSuggester;
                return IsNewQuestion || 
                       (!string.IsNullOrEmpty(thisUser) && !string.IsNullOrEmpty(questionWriter) && thisUser == questionWriter);
            }
        }

        public bool OthersCanAddQuestionAnswerers
        {
            get => _question.WhoShouldAnswerTheQuestionPermissions == RTAPermissions.Others; 
            set
            {
                _question.WhoShouldAnswerTheQuestionPermissions = value ? RTAPermissions.Others : RTAPermissions.WriterOnly; 
                OnPropertyChanged();
            } 
            
        }

        public bool OthersCanAddQuestionAskers
        {
            get => _question.WhoShouldAskTheQuestionPermissions == RTAPermissions.Others; 
            set
            {
                _question.WhoShouldAskTheQuestionPermissions = value ? RTAPermissions.Others : RTAPermissions.WriterOnly; 
                OnPropertyChanged();
            }
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

        private string _saveButtonText = AppResources.SaveAnswerButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }


        public string QuestionSuggesterButtonText => QuestionViewModel.Instance.IsNewQuestion ? AppResources.EditProfileButtonText : string.Format(AppResources.ViewOtherUserProfile, QuestionViewModel.Instance.Question.QuestionSuggester);

        public void UpdateMPButtons()
        {
            OnPropertyChanged("MPButtonsEnabled"); // called by the UpdatableParliamentAndMPData class to update this variable in real time
        }

        // Set up empty question
        // Used when we're generating our own question for upload.
        public QuestionViewModel()
        {
            Question = new Question();
            ReportLabelText = "";
            Question.QuestionSuggester = IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;

            // set defaults
            IsNewQuestion = false;
            HowAnswered = HowAnsweredOptions.DontKnow;
            AnotherUserButtonText = AppResources.AnotherUserButtonText;
            NotSureWhoShouldRaiseButtonText = AppResources.NotSureButtonText;
            SelectButtonText = AppResources.SelectButtonText;
            
            // commands
            ProceedToReadingPageCommand = new AsyncCommand(async() => 
            {
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            QuestionDraftDoneCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAnswererPage)}");
            });
            // For skipping choices - just navigate forwards
            LeaveAnswererBlankButtonCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            });
            OtherPublicAuthorityButtonCommand = new AsyncCommand(async () =>
            {
                // Question.AnswerInApp = false;
                // AnswerInApp = false;
                var pageToSearchAuthorities
                    = new SelectableListPage(_filterChoices.AuthorityLists, "Choose authorities");
                await Shell.Current.Navigation.PushAsync(pageToSearchAuthorities).ContinueWith((_) => 
                {
                    MessagingCenter.Send(this, Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            // If we already know the electorates (and hence responsible MPs), go
            // straight to the Explorer page that lists them.
            // If we don't, go to the page for entering address and finding them.
            // It will pop back to here.
            AnsweredByMyMPCommand = new AsyncCommand(async () =>
            {
                // Question.AnswerInApp = true;
                // AnswerInApp = true;
                await NavigationUtils.PushMyAnsweringMPsExploringPage(
                    IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown,
                    _filterChoices.AnsweringMPsListsMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, _howAnswered == HowAnsweredOptions.InApp ?
                        Constants.GoToMetadataPageNext : Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            AnsweredByOtherMPCommandOptionB = new AsyncCommand(async () =>
            {
                // Question.AnswerInApp = false;
                // AnswerInApp = false;
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage(
                    _filterChoices.AnsweringMPsListsNotMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, _howAnswered == HowAnsweredOptions.InApp ?
                        Constants.GoToMetadataPageNext : Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            UpvoteCommand = new AsyncCommand(async () =>
            {
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered) 
                {
                    await NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText);
                }
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    return;
                }

                // upvoting a question will add it to their list
                // TODO We probably want to separate having _written_ questions from having upvoted them.
                IndividualParticipant.getInstance().HasQuestions = true;
                XamarinPreferences.shared.Set(Constants.HasQuestions, true);
                
                // This toggles the appearance immediately but doesn't record it as an upvoted question
                // unless we get a successful upload ack from the server.
                if (!Question.AlreadyUpvoted)
                {
                    Question.ToggleUpvotedStatus();
                    var upVoteSuccess = await SendUpVoteToServer(true);
                    if (upVoteSuccess)
                    {
                        ResponseRecords.AddUpvotedQuestion(Question.QuestionId);
                    }
                }
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
                var userId = Question.QuestionSuggester;
                var userToSend = await RTAClient.GetUserById(userId);
                if (userToSend.Failure)
                {
                    var errorMessage = AppResources.CouldNotFindUser;
                    if (userToSend is ErrorResult<ServerUser> errorResult)
                    {
                        errorMessage += errorResult.Message;
                    }
                    ReportLabelText = errorMessage;
                }
                // Success
                else 
                {
                    var newReg = new Registration(userToSend.Data);
                    Debug.Assert(newReg.registrationStatus == RegistrationStatus.AnotherPerson);
                    var userProfilePage = new OtherUserProfilePage(newReg);
                    await Application.Current.MainPage.Navigation.PushAsync(userProfilePage);
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
            ToHowAnsweredOptionPageCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(HowAnsweredOptionPage)}");
            });
            ToAnswererPageWithHowAnsweredSelectionCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAnswererPage)}");
            });
            ToMetadataPageCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(MetadataPage)}");
            });
            ShareCommand = new AsyncCommand(async() =>
            {
                await Share.RequestAsync(new ShareTextRequest 
                {
                    // FIXME should this be Instance.?
                    Text = Question.QuestionText,
                    Title = "Share Text"
                });
            });
            ReportCommand = new Command(() =>
            {
                Question.ToggleReportStatus();
            });
        }

        private Command? _findCommitteeCommand;
        public Command FindCommitteeCommand => _findCommitteeCommand ??= new Command(OnFindCommitteeButtonClicked);
        /*
        private Command? _answerInAppCommand;
        public Command AnswerInAppCommand => _answerInAppCommand ??= new Command(OnAnswerInAppButtonClicked);
        */

        private Command? _myMpRaiseCommand;
        public Command myMPRaiseCommand => _myMpRaiseCommand ??= new Command(OnMyMPRaiseButtonClicked);
        private Command? _otherMPRaiseCommand;
        public Command OtherMPRaiseCommand => _otherMPRaiseCommand ??= new Command(OnOtherMPRaiseButtonClicked);
        private Command? _userShouldRaiseCommand;
        public Command UserShouldRaiseCommand => _userShouldRaiseCommand ??= new Command(UserShouldRaiseButtonClicked);
        private Command? _notSureWhoShouldRaiseCommand;

        public bool IsVerifiedMpAccount => IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsVerifiedMPAccount;
        public Command NotSureWhoShouldRaiseCommand => _notSureWhoShouldRaiseCommand ??= new Command(NotSureWhoShouldRaiseButtonClicked);
        public IAsyncCommand ProceedToReadingPageCommand { get; }
        public IAsyncCommand LeaveAnswererBlankButtonCommand { get; }
        public IAsyncCommand QuestionDraftDoneCommand { get; }
        public IAsyncCommand OtherPublicAuthorityButtonCommand { get; }
        public IAsyncCommand AnsweredByMyMPCommand { get; }
        // public IAsyncCommand AnsweredByOtherMPCommand { get; }
        public IAsyncCommand AnsweredByOtherMPCommandOptionB { get; }
        public IAsyncCommand QuestionSuggesterCommand { get; }
        public IAsyncCommand BackCommand { get; }
        public Command SaveQuestionCommand { get; }
        public IAsyncCommand UpvoteCommand { get; }
        public Command EditAnswerCommand { get; }
        public IAsyncCommand OptionACommand { get; }
        public IAsyncCommand OptionBCommand { get; }
        public IAsyncCommand ToMetadataPageCommand { get; }
        public IAsyncCommand ToAnswererPageWithHowAnsweredSelectionCommand { get; }
        public IAsyncCommand ToHowAnsweredOptionPageCommand { get; }
        public IAsyncCommand ShareCommand { get; }
        public Command ReportCommand { get; }
        
        public void ClearQuestionDataAddWriter()
        {
            // set defaults
            Question = new Question();
            ReportLabelText = "";
            Question.QuestionSuggester = IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
        }

        public void ReinitQuestionUpdates()
        {
            Question.ReinitQuestionUpdates();
        }

        // methods for selecting who will raise your question
        
        // Nobody raises the question - just asking for an answer in the app.
        /*
        private async void OnAnswerInAppButtonClicked()
        {
            RaisedByOptionSelected = false;
            await Shell.Current.GoToAsync(nameof(ReadingPage));
        }
        */

        private async void OnFindCommitteeButtonClicked()
        {
            if (CommitteesAndHearingsData.CommitteesData.IsInitialised)
            {
                // RaisedByOptionSelected = true;
                await NavigationUtils.EditCommitteesClicked(_filterChoices.CommitteeLists).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, Constants.GoToMetadataPageNext); // Sends this view model
                });
            }
        }

        // Note that the non-waiting for this asyc method means that the rest of the page can keep
        // Executing. That shouldn't be a problem, though, because it is invisible and therefore unclickable.
        private async void OnMyMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                // RaisedByOptionSelected = true;
                await NavigationUtils.PushMyAskingMPsExploringPage(
                    IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown,
                    _filterChoices.AskingMPsListsMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, Constants.GoToMetadataPageNext); // Sends this view model
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

        // FIXME. We will still need this - just needs to go somewhere different.
        private async void NotSureWhoShouldRaiseButtonClicked()
        {
            // RaisedByOptionSelected = true;
            await Shell.Current.GoToAsync(nameof(MetadataPage));
        }

        // TODO: Implement SearchableListPage constructor for people.
        private void UserShouldRaiseButtonClicked()
        {
            // RaisedByOptionSelected = true;
            AnotherUserButtonText = "Not Implemented Yet";
        }

        private async void OnOtherMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsNotMineSelectableListPageAsync(_filterChoices.AskingMPsListsNotMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, Constants.GoToMetadataPageNext); // Sends this view model
                });
            }
            else
            {
                RedoButtonsForUnreadableMPData();
            }
        }

        private async void SubmitNewQuestionButton_OnClicked()
        {
            // TODO This should not be necessary any more. Perhaps turn into a debug assertion?
            if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
            {
                await NavigationUtils.DoRegistrationCheck(
                    IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                    AppResources.CancelButtonText);
            }

            if (IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
            {
                // This isn't necessary unless the person has just registered, but is necessary if they have.
                Instance.Question.QuestionSuggester = IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
                var validQuestion = Question.ValidateNewQuestion();
                if (validQuestion)
                {
                    var successfulUpload = await SendNewQuestionToServer();
                    if (successfulUpload)
                    {
                        PromptForNextStepAndClearQuestionIfNeeded();
                    }
                }                
            }
        }

        // TODO Consider permissions for question editing.
        private async void EditQuestionButton_OnClicked()
        {
            try
            {
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText).Wait();
                }
            }
            catch (Exception e)
            {
                // TODO: (unit-tests) is it ok to say "not registered" if we aren't able to check it
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.registrationStatus = RegistrationStatus.NotRegistered;
            }
            //await NavigationUtils.DoRegistrationCheck(AppResources.CancelButtonText);

            if (IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
            {
                var validQuestion = Question.ValidateUpdateQuestion();
                if (validQuestion) 
                {
                    SendQuestionEditToServer();
                }                
            }
            else
            {
                // TODO: invent a string for this
                ReportLabelText = AppResources.InvalidRegistration;
            }

        }

        // For uploading an upvote 
        // This should be called only if the person is already registered.
        // Returns true if the upload was successful; false otherwise.
        private async Task<bool> SendUpVoteToServer(bool isUp)
        {
            // This is only supposed to be called for registered participants.
            if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
            {
                throw new TriedToUploadWhileNotRegisteredException("Sending up-vote to server.");
            }

            var voteOnQuestion = new PlainTextVoteOnQuestionCommand()
            {
                question_id = Question.QuestionId,
                up = isUp
            };

            var httpResponse = await RTAClient.SendPlaintextUpvote(voteOnQuestion,
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid);
            (bool isValid, string errorMessage, string _) = RTAClient.ValidateHttpResponse(httpResponse, "Vote upload");  
            if(!isValid) 
            {
                var error =  "Error uploading vote: " + errorMessage;
                ReportLabelText = error;
                Debug.WriteLine(error);
                return false;
            }

            return true;
        }
        
        // For uploading a new question
        // This should be called only if the person is already registered.
        // Returns true if the upload was successful; false otherwise.
        private async Task<bool> SendNewQuestionToServer()
        {
            // This isn't supposed to be called for unregistered participants.
            if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
            {
                throw new TriedToUploadWhileNotRegisteredException("Sending up-vote to server.");
            }
            
            // TODO use returnedData to record questionID, version, hash
            (bool isValid, string errorMessage, string returnedData) successfulSubmission =
                await BuildSignAndUploadNewQuestion();

            if (!successfulSubmission.isValid)
            {
                ReportLabelText = "Error uploading new question: " + successfulSubmission.errorMessage;
                return false;
            }

            return true;
        }
        
        // This is only called if a question has been successfully uploaded. We clear the
        // question data and ask the user if they'd like to write another question with the same
        // filters, or clear them and go home.
        private async void PromptForNextStepAndClearQuestionIfNeeded() 
        { 
            // creating a question will add it to their list
            IndividualParticipant.getInstance().HasQuestions = true;
            XamarinPreferences.shared.Set(Constants.HasQuestions, true);

            //FIXME update version, just like for edits.

            var popup = new QuestionPublishedPopup();
            _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (GoHome)
            {
                _filterChoices.RemoveAllSelections();
                MessagingCenter.Send(this, Constants.QuestionSubmittedDeleteDraft);
                await Application.Current.MainPage.Navigation.PopToRootAsync();
                    
            }
            // Otherwise remain on the question publish page with the opportunity to write a new question.
            else 
            {
                Question.QuestionText = String.Empty;
                Question.Background = String.Empty;
            }
            
        }

        private async void SendQuestionEditToServer()
        {
            // This isn't supposed to be called for unregistered participants.
            if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
            {
                throw new TriedToUploadWhileNotRegisteredException("Sending up-vote to server.");
            }
            
            var successfulSubmission = await BuildSignAndUploadQuestionUpdates();
            
            if (!successfulSubmission.isValid)
            {
                var message = string.Format(AppResources.EditQuestionErrorText, successfulSubmission.errorMessage);
                var popup2 = new OneButtonPopup(message, AppResources.OKText);
                _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup2);
                ReportLabelText = "Error editing question: " + successfulSubmission.errorMessage;
                return;
            }
            
            // Success - reinitialize question state and make sure we've got the most up to date version.
            Question.ReinitQuestionUpdates();
            
            // FIXME at the moment, the version isn't been correctly updated.
            // TODO: Here, we'll need to ensure we've got the right version (from the server - get it returned from
            // BuildSignAndUpload... 
            var popup = new TwoButtonPopup(AppResources.QuestionEditSuccessfulPopupTitle, AppResources.QuestionEditSuccessfulPopupText, AppResources.StayOnCurrentPageButtonText, AppResources.GoHomeButtonText, false);            
            var popupResult = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);

            if (popup.HasApproved(popupResult))
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        // TODO: This is where we'll need to record the hash, version, and other server responses.
        private async Task<(bool isValid, string errorMessage, string)> BuildSignAndUploadNewQuestion()
        {
            var serverQuestion = new QuestionSendToServer(Question);

            var httpResponse = await RTAClient.RegisterNewQuestion(
                serverQuestion,
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Upload");
        }

        private async Task<(bool isValid, string errorMessage, string returnedData)> BuildSignAndUploadQuestionUpdates()
        {
            var serverQuestionUpdates = Question.Updates;
            
            // needs these two fields in the message payload for updates, but not for new questions.
            serverQuestionUpdates.question_id = Question.QuestionId;
            serverQuestionUpdates.version = Question.Version;

            var httpResponse = await RTAClient.UpdateExistingQuestion(
                serverQuestionUpdates,
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Edit");
        }
    }
}
