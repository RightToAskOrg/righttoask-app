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
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Annotations;
using RightToAskClient.Helpers;
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

        private string _newAnswer = "";
        public string NewAnswer
        {
            get => _newAnswer; 
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _newAnswer, value);
                    Question.AddAnswer(value);
                    OnPropertyChanged("NewAnswer");
                }
            }
        }

        public bool HasAnswer => Answers.Any();
        public List<Answer> Answers
        {
            get => Question.Answers;
        }
        
        private string? _newHansardLink; 
        public string NewHansardLink 
        { 
            get => _newHansardLink;
            set
            {
                Result<Uri> urlResult = ParliamentData.StringToValidParliamentaryUrl(value);
                if (String.IsNullOrEmpty(urlResult.Err))
                {
                    SetProperty(ref _newHansardLink, value);
                    Question.AddHansardLink(urlResult.Ok);
                    OnPropertyChanged("HansardLink");
                }
                else
                {
                    ReportLabelText = urlResult.Err ?? "";
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

        // TODO What does this mean??
        private bool _isReadingOnly;
        public bool IsReadingOnly
        {
            get => _isReadingOnly;
            set => SetProperty(ref _isReadingOnly, value);
        }

        // Whether the 'Option B' path, in which you choose both someone to answer 
        // and someone to raise the question, has been selected. This is the opposite of
        // 'AnswerInApp.' Default to true unless the user explicitly chooses to get an answer
        // in the app.
        /*
        private bool _raisedByOptionSelected = true;
        public bool RaisedByOptionSelected
        {
            get => _raisedByOptionSelected;
            set
            {
                SetProperty(ref _raisedByOptionSelected, value);
                AnswerInApp = !_raisedByOptionSelected;
            }
        }
        */

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
                string thisUser =  App.ReadingContext.ThisParticipant.RegistrationInfo.uid;
                string questionWriter = _question.QuestionSuggester;
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


        private bool _answerInApp = false;
        public bool AnswerInApp
        {
            get => _answerInApp;
            set => SetProperty(ref _answerInApp, value);
        }


        public string QuestionSuggesterButtonText => QuestionViewModel.Instance.IsNewQuestion ? AppResources.EditProfileButtonText : String.Format(AppResources.ViewOtherUserProfile, QuestionViewModel.Instance.Question.QuestionSuggester);

        public void UpdateMPButtons()
        {
            OnPropertyChanged("MPButtonsEnabled"); // called by the UpdatableParliamentAndMPData class to update this variable in real time
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
                var PageToSearchAuthorities
                    = new SelectableListPage(App.ReadingContext.Filters.AuthorityLists, "Choose authorities");
                await Shell.Current.Navigation.PushAsync(PageToSearchAuthorities).ContinueWith((_) => 
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
                await NavigationUtils.PushMyAnsweringMPsExploringPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, _howAnswered == HowAnsweredOptions.InApp ?
                        Constants.GoToMetadataPageNext : Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            /*
            AnsweredByOtherMPCommand = new AsyncCommand(async () =>
            {
                Question.AnswerInApp = true;
                AnswerInApp = true;
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, "OptionBGoToAskingPageNext"); // Sends this view model
                });
            });
            */
            AnsweredByOtherMPCommandOptionB = new AsyncCommand(async () =>
            {
                // Question.AnswerInApp = false;
                // AnswerInApp = false;
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage().ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, _howAnswered == HowAnsweredOptions.InApp ?
                        Constants.GoToMetadataPageNext : Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            UpvoteCommand = new AsyncCommand(async () =>
            {
                await NavigationUtils.DoRegistrationCheck(Instance);
                if (App.ReadingContext.ThisParticipant.IsRegistered)
                {
                    // upvoting a question will add it to their list
                    // TODO We probably want to separate having _written_ questions from having upvoted them.
                    App.ReadingContext.ThisParticipant.HasQuestions = true;
                    Preferences.Set(Constants.HasQuestions, true);
                    MessagingCenter.Send(this, Constants.HasQuestions);
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
                    /*
                     * Rather that pass the data via messaging centre, we'll just make a new page and
                     * pass it via the constructor.
                     * 
                    await Shell.Current.GoToAsync($"{nameof(OtherUserProfilePage)}").ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "OtherUser", userToSend.Ok); // Send person or send question
                    }); */
                    var newReg = new Registration(userToSend.Ok);
                    var userProfilePage = new OtherUserProfilePage(newReg);
                    await App.Current.MainPage.Navigation.PushAsync(userProfilePage);
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
        }

        private Command? _findCommitteeCommand;
        public Command FindCommitteeCommand => _findCommitteeCommand ??= new Command(OnFindCommitteeButtonClicked);
        /*
        private Command? _answerInAppCommand;
        public Command AnswerInAppCommand => _answerInAppCommand ??= new Command(OnAnswerInAppButtonClicked);
        */

        private Command? _myMpRaiseCommand;
        public Command MyMPRaiseCommand => _myMpRaiseCommand ??= new Command(OnMyMPRaiseButtonClicked);
        private Command? _otherMPRaiseCommand;
        public Command OtherMPRaiseCommand => _otherMPRaiseCommand ??= new Command(OnOtherMPRaiseButtonClicked);
        private Command? _userShouldRaiseCommand;
        public Command UserShouldRaiseCommand => _userShouldRaiseCommand ??= new Command(UserShouldRaiseButtonClicked);
        private Command? _notSureWhoShouldRaiseCommand;
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

        public void ResetInstance()
        {
            // set defaults
            Question = new Question();
            IsNewQuestion = false;
            IsReadingOnly = App.ReadingContext.IsReadingOnly; // crashes here if setting up existing test questions
            HowAnswered = HowAnsweredOptions.DontKnow;
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
                await NavigationUtils.EditCommitteesClicked().ContinueWith((_) =>
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
                await NavigationUtils.PushMyAskingMPsExploringPage().ContinueWith((_) =>
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
        private async void UserShouldRaiseButtonClicked()
        {
            // RaisedByOptionSelected = true;
            AnotherUserButtonText = "Not Implemented Yet";
        }

        private async void OnOtherMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsNotMineSelectableListPageAsync().ContinueWith((_) =>
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
            await NavigationUtils.DoRegistrationCheck(Instance);
            
            if (App.ReadingContext.ThisParticipant.IsRegistered)
            {
                // This isn't necessary unless the person has just registered, but is necessary if they have.
                Instance.Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.uid;
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
            await NavigationUtils.DoRegistrationCheck(Instance);

            if (App.ReadingContext.ThisParticipant.IsRegistered)
            {
                bool validQuestion = Question.ValidateUpdateQuestion();
                if (validQuestion) 
                {
                    sendQuestionEditToServer();
                }                
            }

        }



        // For uploading a new question
        // This should be called only if the person is already registered.
        private async void SendNewQuestionToServer()
        {
            // This isn't supposed to be called for unregistered participants.
            if (!App.ReadingContext.ThisParticipant.IsRegistered) return;

            // TODO use returnedData to record questionID, version, hash
            (bool isValid, string errorMessage, string returnedData) successfulSubmission = await BuildSignAndUploadNewQuestion();

            if (!successfulSubmission.isValid)
            {
                ReportLabelText =  "Error uploading new question: " + successfulSubmission.errorMessage;
                return;
            }
            
            // Reset the draft question only if it didn't upload correctly.
            App.ReadingContext.DraftQuestion = "";

            // creating a question will add it to their list
            App.ReadingContext.ThisParticipant.HasQuestions = true;
            Preferences.Set(Constants.HasQuestions, true);
            MessagingCenter.Send(this, Constants.HasQuestions);

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
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            }
        }

        private async void sendQuestionEditToServer()
        {
            // This isn't supposed to be called for unregistered participants.
            if (!App.ReadingContext.ThisParticipant.IsRegistered) return;
            
            (bool isValid, string errorMessage, string returnedData) successfulSubmission = await BuildSignAndUploadQuestionUpdates();
            
            if (!successfulSubmission.isValid)
            {
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
            var result = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (ApproveButtonClicked)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        // TODO: This is where we'll need to record the hash, version, and other server responses.
        private async Task<(bool isValid, string errorMessage, string)> BuildSignAndUploadNewQuestion()
        {
            var serverQuestion = new QuestionSendToServer(Question);

            Result<string> httpResponse = await RTAClient.RegisterNewQuestion(serverQuestion);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Upload");
        }

        private async Task<(bool isValid, string errorMessage, string returnedData)> BuildSignAndUploadQuestionUpdates()
        {
            var serverQuestionUpdates = Question.Updates;
            
            // needs these two fields in the message payload for updates, but not for new questions.
            serverQuestionUpdates.question_id = Question.QuestionId;
            serverQuestionUpdates.version = Question.Version;

            Result<string> httpResponse = await RTAClient.UpdateExistingQuestion(serverQuestionUpdates);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Edit");
        }
    }
}
