using System;
using RightToAskClient.Maui.HttpClients;
using RightToAskClient.Maui.Models;
using RightToAskClient.Maui.Models.ServerCommsData;
using RightToAskClient.Maui.Resx;
using RightToAskClient.Maui.Views;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.Views.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.Input;

namespace RightToAskClient.Maui.ViewModels
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

        // private FilterChoices _filterChoices = new FilterChoices();

        private string _newAnswer = "";

        public string NewAnswer
        {
            get => _newAnswer;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                SetProperty(ref _newAnswer, value);
                Updates.NewAnswers = new List<QuestionAnswer>()
                {
                    new QuestionAnswer()
                    {
                        mp = new MPId(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.MPRegisteredAs),
                        answer = value
                    }
                };
                OnPropertyChanged();
                OnPropertyChanged("HasUpdates");
            }
        }

        public string Background
        {
            get => Question.Background;
            set
            {
                Question.Background = value;
                OnPropertyChanged();
            }
        }

        // For adding background to a question you have already published.
        private string _newBackground = "";

        public string NewBackground
        {
            get => _newBackground;
            set
            {
                Updates.NewBackground = value;
                SetProperty(ref _newBackground, value);
                OnPropertyChanged("HasUpdates");
            }
        }

        public bool IsMyQuestion
        {
            get => Question.QuestionSuggester.Equals(IndividualParticipant.getInstance().ProfileData
                .RegistrationInfo.uid);
        }

        public bool IsRaiseLayoutVisible
        {
            get => HasAskers || IsMyQuestion;
        }

        // Convenient views of things stored in the Question.
        public List<Answer> QuestionAnswers => Question.Answers;
        public List<Uri> HansardLinks => Question.HansardLink;

        public string QuestionAnswerers =>
            "";
        //TODO:
            //Extensions.JoinFilter(", ",
            //        string.Join(", ", Question.Filters.SelectedAnsweringMPsNotMine.Select(mp => mp.ShortestName)),
            //        string.Join(", ", Question.Filters.SelectedAnsweringMPsMine.Select(mp => mp.ShortestName)),
            //        string.Join(", ", Question.Filters.SelectedAuthorities.Select(a => a.ShortestName)))
            //    .NullToEmptyMessage(IsNewQuestion ? AppResources.HasNotMadeSelection : AppResources.NoSelections);

        // The MPs or committee who are meant to ask the question
        public string QuestionAskers =>
                "";
        //TODO:
        //Extensions.JoinFilter(", ",
        //            string.Join(", ", Question.Filters.SelectedAskingMPsNotMine.Select(mp => mp.ShortestName)),
        //            string.Join(", ", Question.Filters.SelectedAskingMPsMine.Select(mp => mp.ShortestName)),
        //            string.Join(",", Question.Filters.SelectedCommittees.Select(com => com.ShortestName)))
        //        .NullToEmptyMessage(IsNewQuestion ? AppResources.HasNotMadeSelection : AppResources.NoSelections);

        public bool HasAskers
        {
            get => Question.Filters.SelectedAskingMPsNotMine.Count != 0 ||
                   Question.Filters.SelectedAskingMPsMine.Count != 0 ||
                   Question.Filters.SelectedCommittees.Count != 0;
        }

        public bool HasAnswerers
        {
            get => Question.Filters.SelectedAnsweringMPsNotMine.Count != 0 ||
                   Question.Filters.SelectedAnsweringMPsMine.Count != 0 ||
                   Question.Filters.SelectedAuthorities.Count != 0;
        }

        private string? _newHansardLink;

        public string NewHansardLink
        {
            get => _newHansardLink;
            set
            {
                var urlResult = ParliamentaryURICreator.StringToValidParliamentaryUrl(value);
                if (urlResult.Success)
                {
                    SetProperty(ref _newHansardLink, value);
                    Question.AddHansardLink(urlResult.Data);

                    Updates.NewHansardLinks.Add(new HansardLink(urlResult.Data.OriginalString));
                        
                    ReportLabelText = "";
                    OnPropertyChanged("HansardLink");
                    OnPropertyChanged("HasUpdates");
                }
                else
                {
                    var errorMessage = AppResources.InvalidHansardLink;
                    if (urlResult is ErrorResult<Uri> errorResult)
                    {
                        errorMessage += ". " + errorResult.Message;
                    }

                    ReportLabelText = errorMessage;
                }
                OnPropertyChanged("ReportLabelText");
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

        // Lists the updates that have occurred since construction.
        public QuestionUpdates Updates { get; protected set; } = new QuestionUpdates();
        
        // Whether this user has updated this question. Used to 
        // determine whether the 'update' button is enabled.
        public bool HasUpdates => Updates.AnyUpdates
            || Question.WhoShouldAnswerTheQuestionPermissions != _initialWhoCanAnswerPermissions
            || Question.WhoShouldAskTheQuestionPermissions != _initialWhoCanAskPermissions; 

    private HowAnsweredOptions _howAnswered = HowAnsweredOptions.DontKnow; 

        public HowAnsweredOptions HowAnswered
        {
            get => _howAnswered;
            set => SetProperty(ref _howAnswered, value);
        }
        
        public bool IsShowPublicAuthority
        {
            get => _howAnswered != HowAnsweredOptions.InApp;
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
        
        public bool CanEditBackground
        {
            get
            {
                var thisUser =  IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
                var questionWriter = _question.QuestionSuggester;
                return IsNewQuestion || 
                       (!string.IsNullOrEmpty(thisUser) && !string.IsNullOrEmpty(questionWriter) && thisUser == questionWriter) && _question.Background.IsNullOrEmpty();
            }
        }
        

        public bool OthersCanAddQuestionAnswerers
        {
            get => _question.WhoShouldAnswerTheQuestionPermissions == RTAPermissions.Others; 
            set
            {
                _question.WhoShouldAnswerTheQuestionPermissions = value ? RTAPermissions.Others : RTAPermissions.WriterOnly;
                OnPropertyChanged();
                OnPropertyChanged("HasUpdates");
            } 
            
        }

        public bool OthersCanAddQuestionAskers
        {
            get => _question.WhoShouldAskTheQuestionPermissions == RTAPermissions.Others; 
            set
            {
                _question.WhoShouldAskTheQuestionPermissions = value ? RTAPermissions.Others : RTAPermissions.WriterOnly;
                OnPropertyChanged();
                OnPropertyChanged("HasUpdates");
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


        // net = up - down;
        // total = up + down
        public int UpVotes => (Question.TotalVotes + Question.NetVotes) / 2;
        public int DownVotes => (Question.TotalVotes - Question.NetVotes) / 2;
        
        public string QuestionSuggesterButtonText => QuestionViewModel.Instance.IsNewQuestion ? AppResources.EditProfileButtonText : string.Format(AppResources.ViewOtherUserProfile, QuestionViewModel.Instance.Question.QuestionSuggester);

        public void UpdateMPButtons()
        {
            OnPropertyChanged("MPButtonsEnabled"); // called by the UpdatableParliamentAndMPData class to update this variable in real time
        }

        // Used for keeping track of whether permissions need to be updated when editing own question.
        protected RTAPermissions _initialWhoCanAnswerPermissions=RTAPermissions.NoChange;

        protected RTAPermissions _initialWhoCanAskPermissions=RTAPermissions.NoChange;
        // constructor
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
            ProceedToReadingPageCommand = new AsyncRelayCommand (async() => 
            {
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            QuestionDraftDoneCommand = new AsyncRelayCommand (async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAnswererPage)}");
            });
            // For skipping choices - just navigate forwards
            LeaveAnswererBlankButtonCommand = new AsyncRelayCommand (async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            });
            OtherPublicAuthorityButtonCommand = new AsyncRelayCommand (async () =>
            {
                // Question.AnswerInApp = false;
                // AnswerInApp = false;
                var pageToSearchAuthorities
                    = new SelectableListPage(Instance.Question.Filters.AuthorityLists, "Choose authorities");
                await Shell.Current.Navigation.PushAsync(pageToSearchAuthorities).ContinueWith((_) => 
                {
                    MessagingCenter.Send(this, Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            // If we already know the electorates (and hence responsible MPs), go
            // straight to the Explorer page that lists them.
            // If we don't, go to the page for entering address and finding them.
            // It will pop back to here.
            AnsweredByMyMPCommand = new AsyncRelayCommand (async () =>
            {
                // Question.AnswerInApp = true;
                // AnswerInApp = true;
                await NavigationUtils.PushMyAnsweringMPsExploringPage(
                    IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown,
                    Instance.Question.Filters.AnsweringMPsListsMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, _howAnswered == HowAnsweredOptions.InApp ?
                        Constants.GoToQuestionDetailPageNext : Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            AnsweredByOtherMPCommandOptionB = new AsyncRelayCommand (async () =>
            {
                // Question.AnswerInApp = false;
                // AnswerInApp = false;
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage(
                    Instance.Question.Filters.AnsweringMPsListsNotMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, _howAnswered == HowAnsweredOptions.InApp ?
                        Constants.GoToQuestionDetailPageNext : Constants.GoToAskingPageNext); // Sends this view model
                });
            });
            UpvoteCommand = new AsyncRelayCommand (async () =>
            {
                // First check if they're registered, and offer them the chance to register if not
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    await NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText);
                }

                // If they didn't register, return
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
                if (!Question.AlreadyUpvoted && !Question.AlreadyDownvoted)
                {
                    Question.ToggleUpvotedStatus();
                    var upVoteSuccess = await SendUpVoteToServer(true);
                    if (upVoteSuccess)
                    {
                        ResponseRecords.AddUpvotedQuestion(Question.QuestionId);
                    }
                }
            });
            DownvoteCommand = new AsyncRelayCommand (async () =>
            {
                // First check if they're registered, and offer them the chance to register if not
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered) 
                {
                    await NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText);
                }
                // If they didn't register, return
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    return;
                }

                // This toggles the appearance immediately but doesn't record it as an upvoted question
                // unless we get a successful upload ack from the server.
                if (!Question.AlreadyUpvoted && !Question.AlreadyDownvoted)
                {
                    Question.ToggleDownvotedStatus();
                    // A 'false' up-vote is a down-vote
                    var downVoteSuccess = await SendUpVoteToServer(false);
                    if (downVoteSuccess)
                    {
                        ResponseRecords.AddDownvotedQuestion(Question.QuestionId);
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
            QuestionSuggesterCommand = new AsyncRelayCommand (async () =>
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
            BackCommand = new AsyncRelayCommand (async () =>
            {
                HomeButtonCommand.Execute(null); // just inherit the functionality of the home button from BaseViewModel
            });
            OptionACommand = new AsyncRelayCommand (async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            });
            OptionBCommand = new AsyncRelayCommand (async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            });
            ToHowAnsweredOptionPageCommand = new AsyncRelayCommand (async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(HowAnsweredOptionPage)}");
            });
            ToAnswererPageWithHowAnsweredSelectionCommand = new AsyncRelayCommand (async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionAnswererPage)}");
            });
            ShareCommand = new AsyncRelayCommand (async() =>
            {
                await Share.RequestAsync(new ShareTextRequest 
                {
                    // FIXME should this be Instance.?
                    Text = Question.QuestionText,
                    Title = "Share Text"
                });
            });
            ReportCommand = new AsyncRelayCommand (async () =>
            {
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    await NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText);
                }

                // If they didn't register, return
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    return;
                }
                var nextPage = new ReportQuestionPage(Question.QuestionId, ResponseRecords, new Command(() =>
                {
                    Question.AlreadyReported = true;
                }));
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            });
        }

        public bool IsAnswerInApp
        {
            get => _howAnswered == HowAnsweredOptions.InApp;
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
        public IAsyncRelayCommand  ProceedToReadingPageCommand { get; }
        public IAsyncRelayCommand  LeaveAnswererBlankButtonCommand { get; }
        public IAsyncRelayCommand  QuestionDraftDoneCommand { get; }
        public IAsyncRelayCommand  OtherPublicAuthorityButtonCommand { get; }
        public IAsyncRelayCommand  AnsweredByMyMPCommand { get; }
        // public IAsyncRelayCommand  AnsweredByOtherMPCommand { get; }
        public IAsyncRelayCommand  AnsweredByOtherMPCommandOptionB { get; }
        public IAsyncRelayCommand  QuestionSuggesterCommand { get; }
        public IAsyncRelayCommand  BackCommand { get; }
        public Command SaveQuestionCommand { get; }
        public IAsyncRelayCommand  UpvoteCommand { get; }
        public IAsyncRelayCommand  DownvoteCommand { get; }
        public Command EditAnswerCommand { get; }
        public IAsyncRelayCommand  OptionACommand { get; }
        public IAsyncRelayCommand  OptionBCommand { get; }
        
        public IAsyncRelayCommand  ToAnswererPageWithHowAnsweredSelectionCommand { get; }
        public IAsyncRelayCommand  ToHowAnsweredOptionPageCommand { get; }
        public IAsyncRelayCommand  ShareCommand { get; }
        public IAsyncRelayCommand  ReportCommand { get; }
        
        public void ClearQuestionDataAddWriter()
        {
            // set defaults
            Question = new Question();
            ReportLabelText = "";
            Question.QuestionSuggester = IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
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
                await NavigationUtils.EditCommitteesClicked(Instance.Question.Filters.CommitteeLists).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, Constants.GoToQuestionDetailPageNext); // Sends this view model
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
                    Instance.Question.Filters.AskingMPsListsMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, Constants.GoToQuestionDetailPageNext); // Sends this view model
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
            await Shell.Current.GoToAsync(nameof(QuestionDetailPage));
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
                await NavigationUtils.PushAskingMPsNotMineSelectableListPageAsync(Instance.Question.Filters.AskingMPsListsNotMine).ContinueWith((_) =>
                {
                    MessagingCenter.Send(this, Constants.GoToQuestionDetailPageNext); // Sends this view model
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

        private async void EditQuestionButton_OnClicked()
        {
            try
            {
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                { 
                    await NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText);
                }
            }
            catch (Exception e)
            {
                // TODO: (unit-tests) is it ok to say "not registered" if we aren't able to check it
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.registrationStatus = RegistrationStatus.NotRegistered;
            }

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
            (bool isValid, string errorMessage) = RTAClient.ValidateHttpResponse(httpResponse, "Vote upload");  
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
            await Application.Current.MainPage.Navigation.PushModalAsync(popup);
            if (GoHome)
            {
                Instance.Question.Filters.RemoveAllSelections();
                await Application.Current.MainPage.Navigation.PopToRootAsync();
                    
            }
            // Otherwise remain on the question publish page with the opportunity to write a new question.
            else 
            {
                Question.QuestionText = String.Empty;
                Background = String.Empty;
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
                var message = successfulSubmission.errorMessage ?? "";
                var popup2 = new OneButtonPopup(AppResources.EditQuestionErrorText, message, AppResources.OKText);
                await Application.Current.MainPage.DisplayAlert(AppResources.EditQuestionErrorText,
                    message, AppResources.OKText);
                // ReportLabelText = "Error editing question: " + successfulSubmission.errorMessage;
                return;
            }
            
            // Success - reinitialize question state and make sure we've got the most up to date version.
            Question.Version = successfulSubmission.returnedData;
            ReInitUpdatesAndErrors();
            
            // Go back to the reading page you came from.
            var popup = new OneButtonPopup(AppResources.QuestionEditSuccessfulPopupTitle, AppResources.QuestionEditSuccessfulPopupText, AppResources.GoHomeButtonText, false);            
            await App.Current.MainPage.DisplayAlert(AppResources.QuestionEditSuccessfulPopupTitle, AppResources.QuestionEditSuccessfulPopupText, AppResources.GoHomeButtonText);

            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

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
            Updates.WhoShouldAnswerPermissions =
                doPermissionUpdate(Question.WhoShouldAnswerTheQuestionPermissions, _initialWhoCanAnswerPermissions);
            Updates.WhoShouldAskPermissions =
                doPermissionUpdate(Question.WhoShouldAskTheQuestionPermissions, _initialWhoCanAskPermissions);
            
            var httpResponse = await RTAClient.UpdateExistingQuestion(
                Updates,
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid);
            return RTAClient.ValidateHttpResponse(httpResponse, "");
        }

        private RTAPermissions doPermissionUpdate(RTAPermissions current, RTAPermissions prior)
        {
            if (current == prior)
            {
                return RTAPermissions.NoChange;
            }

            return current;
        }

        protected void ReInitUpdatesAndErrors()
        {
                // Empty updatable fields
                NewAnswer = "";
                NewBackground = "";
                NewHansardLink = "";
                
                // Keep track of changes to question asking/answering permission.
                _initialWhoCanAskPermissions = _question.WhoShouldAskTheQuestionPermissions;
                _initialWhoCanAnswerPermissions = _question.WhoShouldAnswerTheQuestionPermissions;

                // Keep track of other Updates/changes
                Updates = new QuestionUpdates(Question.QuestionId, Question.Version);

                ReportLabelText = "";
        }
    }
}
