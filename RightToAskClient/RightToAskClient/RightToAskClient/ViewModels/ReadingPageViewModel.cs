using RightToAskClient.Helpers;
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
using System.Threading.Tasks;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class ReadingPageViewModel : BaseViewModel
    {
        // properties
        private bool _showQuestionFrame = false;
        public bool ShowQuestionFrame
        {
            get => _showQuestionFrame;
            set => SetProperty(ref _showQuestionFrame, value);
        }

        private bool _showSearchFrame;
        public bool ShowSearchFrame
        {
            get => _showSearchFrame;
            set => SetProperty(ref _showSearchFrame, value);
        }

        private string _heading1 = string.Empty;
        public string Heading1
        {
            get => _heading1;
            set => SetProperty(ref _heading1, value);
        }
        private string _draftQuestion = "";
        public string DraftQuestion
        {
            get => _draftQuestion;
            set => SetProperty(ref _draftQuestion, value);
        }

        private Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _ = SetProperty(ref _selectedQuestion, value);
                if (_selectedQuestion != null)
                {
                    QuestionViewModel.Instance.Question = _selectedQuestion;
                    QuestionViewModel.Instance.IsNewQuestion = false;
                    _ = Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
                }
            }
        }

        private ObservableCollection<Question> _questionsToDisplay = new ObservableCollection<Question>();
        public ObservableCollection<Question> QuestionsToDisplay
        {
            get => _questionsToDisplay;
            set => SetProperty(ref _questionsToDisplay, value);
        }

        private bool _isRefreshing = true;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string Keyword
        {
            get => App.GlobalFilterChoices.SearchKeyword;
            set
            {
                App.GlobalFilterChoices.SearchKeyword = value;
                OnPropertyChanged();
            }
        }
        
        private string _writerOnlyUid = string.Empty;
        private bool _readByQuestionWriter;

        // constructor
        public ReadingPageViewModel()
        {
            Keyword = App.GlobalFilterChoices.SearchKeyword;
            
            // If we're already searching for something, show the user what.
            ShowSearchFrame = !string.IsNullOrWhiteSpace(Keyword); 


            /* I don't think we ever arrive here with a non-empty draft any more.
            if (!string.IsNullOrEmpty(App.ReadingContext.DraftQuestion))
            {
                DraftQuestion = App.ReadingContext.DraftQuestion;
                ShowQuestionFrame = true;
            }
            */

            // Reading with a draft question - prompt for upvoting similar questions
            if (ShowQuestionFrame)
            {
                Heading1 = AppResources.FocusSupportInstructionQuestionDrafting;
                Title = AppResources.SimilarQuestionsTitle;
            }
            // Reading without a draft question
            else
            {
                Heading1 = AppResources.FocusSupportInstructionReadingOnly;
                Title = AppResources.ReadQuestionsTitle;
            }
            
            PopupLabelText = AppResources.ReadingPageHeader1;
            PopupHeaderText = Heading1;
            
            if (XamarinPreferences.shared.Get(Constants.ShowFirstTimeReadingPopup, true))
            {
                InfoPopupCommand.ExecuteAsync();
                
                // Only show it once.
                XamarinPreferences.shared.Set(Constants.ShowFirstTimeReadingPopup, false);
            }
            
            KeepQuestionButtonCommand = new AsyncCommand(async () =>
            {
                OnSaveButtonClicked();
            });
            DiscardButtonCommand = new AsyncCommand(async () =>
            {
                OnDiscardButtonClicked();
            });
            // Note: There is a race condition here, in that it is possible
            // for this command to be executed multiple times simultaneously,
            // producing multiple calls to Clear and the simultaneous insertion
            // of questions from various versions of questionsToDisplay List.
            // I don't *think* this will cause a lock because QuestionsToDisplay
            // ought to be able to be cleared and added to in any order.
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions();
                QuestionsToDisplay.Clear();
                foreach (var q in questionsToDisplayList)
                {  
                    QuestionsToDisplay.Add(q);
                }
                IsRefreshing = false;
            });
            DraftCommand = new AsyncCommand(async () =>
            {
                // Check that they are registered - if not, prompt them to get an account.
                await NavigationUtils.DoRegistrationCheck(this);

                if (IndividualParticipant.getInstance().IsRegistered)
                {
                    // If this is their first question, show them the 5-step instructions.
                    var showHowToPublishPopup = XamarinPreferences.shared.Get(Constants.ShowHowToPublishPopup, true);
                    if (showHowToPublishPopup)
                    {
                        var popup = new HowToPublishPopup();
                        if (popup != null) _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);

                        // Only show it once.
                        XamarinPreferences.shared.Set(Constants.ShowHowToPublishPopup, false);
                    }
                    
                    // Now let them start drafting.
                    ShowQuestionFrame = true;
                }
            });
            SearchToolbarCommand = new Command(() =>
            {
                ShowSearchFrame = !ShowSearchFrame; // just toggle it
            });
            ShowFiltersCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync(nameof(AdvancedSearchFiltersPage));
            });
            RemoveQuestionCommand = new Command<Question>(questionToRemove =>
            {
                // store question ID for later data manipulation?
                if (!IndividualParticipant.getInstance().RemovedQuestionIDs.Contains(questionToRemove.QuestionId))
                {
                    IndividualParticipant.getInstance().RemovedQuestionIDs.Add(questionToRemove.QuestionId);
                }
                QuestionsToDisplay.Remove(questionToRemove);
            });


            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.QuestionSubmittedDeleteDraft,
                (sender) =>
            {
                    DraftQuestion = "";
                    ShowQuestionFrame = false;
            });
            MessagingCenter.Subscribe<SelectableListViewModel>(this,"ReadQuestionsWithASingleQuestionWriter", (sender) =>
            {
                var questionWriter = App.GlobalFilterChoices.QuestionWriterLists.SelectedEntities.FirstOrDefault();
                if (questionWriter is null)
                {
                    Debug.WriteLine("Error: ReadingPage for single question writer but no selection.");
                }

                _readByQuestionWriter = true;
                _writerOnlyUid = questionWriter?.RegistrationInfo.uid ?? string.Empty;
                Heading1 = AppResources.QuestionWriterReadingPageHeaderText+" "+questionWriter?.RegistrationInfo.display_name;
                
                RefreshCommand.ExecuteAsync();
            });
            
            // Get the question list for display
            if (!_readByQuestionWriter)
            {
                RefreshCommand.ExecuteAsync();
            }
        }

        // commands
        public IAsyncCommand KeepQuestionButtonCommand { get; }
        public IAsyncCommand DiscardButtonCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public IAsyncCommand DraftCommand { get; }
        public Command SearchToolbarCommand { get; }
        public Command<Question> RemoveQuestionCommand { get; }
        public IAsyncCommand ShowFiltersCommand { get; }

        // helper methods
        private async void OnSaveButtonClicked()
        {
            // Set up new question in preparation for upload. 
            // The filters are what the user has chosen through the flow.
            var newQuestion = new Question
            {
                QuestionText = DraftQuestion,
                QuestionSuggester = (IndividualParticipant.getInstance().IsRegistered)
                    ? IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid
                    : "",
                Filters = App.GlobalFilterChoices,
                DownVotesByThisUser = 0,
                UpVotesByThisUser = 0
            };

            QuestionViewModel.Instance.Question = newQuestion;
            QuestionViewModel.Instance.IsNewQuestion = true;

            // instead of going to the questionDetailsPage we now go to a Background page and then a metadata page before the details page
            await Shell.Current.GoToAsync($"{nameof(QuestionBackgroundPage)}");
        }

        private async void OnDiscardButtonClicked()
        {
            DraftQuestion = "";
            ShowQuestionFrame = false;

            var popup = new OneButtonPopup(AppResources.DraftDiscardedPopupTitle, AppResources.FocusSupportReport, AppResources.OKText);
            _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);

            ShowQuestionFrame = false;
        }

        public async Task<List<Question>> LoadQuestions()
        {
            var serverQuestions = new List<QuestionReceiveFromServer>();
            var questionsToDisplay = new List<Question>();
            var httpResponse = await GetAppropriateQuestionList();
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            if (!httpValidation.isValid)
            {
                ReportLabelText = "Failed to get Question List from server." + httpValidation.errorMessage;
                return questionsToDisplay;
            }
            
            // httpValidation isValid
            var questionIds = httpResponse.Data;
            
            // loop through the questions
            foreach (var questionId in questionIds)
            {
                // pull the individual question from the server by id
                QuestionReceiveFromServer tempQuestion;
                try
                {
                    // If retrieval is successful, add to the list of questions to be displayed.
                    var dataResult = await RTAClient.GetQuestionById(questionId);
                    if (dataResult.Success)
                    {
                        tempQuestion = dataResult.Data;
                        if (!string.IsNullOrEmpty(tempQuestion.question_text))
                        {
                            serverQuestions.Add(tempQuestion);
                        }
                    }
                    // Log retrieval failure.
                    else
                    {
                        var errorMessage = "Could not retrieve question with ID " + questionId + ". ";
                        if (dataResult is ErrorResult<QuestionReceiveFromServer> errorResult)
                        {
                            errorMessage += errorResult.Message;
                        }
                        Debug.WriteLine(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not find question: " + ex.Message);
                }
            }

            // convert the ServerQuestions to a Displayable format
            foreach (var serverQuestion in serverQuestions)
            {
                questionsToDisplay.Add(new Question(serverQuestion));
            }


            // after getting the list of questions, remove the ids for dismissed questions, and set the upvoted status of liked ones
            for (var i = 0; i < IndividualParticipant.getInstance().RemovedQuestionIDs.Count; i++)
            {
                var temp = questionsToDisplay
                    .FirstOrDefault(q => q.QuestionId == IndividualParticipant.getInstance().RemovedQuestionIDs[i]);
                if (temp != null)
                {
                    questionsToDisplay.Remove(temp);
                }
            }

            // set previously upvoted questions
            foreach (var q in questionsToDisplay)
            {
                foreach (var qId in IndividualParticipant.getInstance()
                             .UpvotedQuestionIDs
                             .Where(qId => q.QuestionId == qId))
                {
                    q.AlreadyUpvoted = true;
                }

                // set previously flagged/reported questions
                foreach (var qID in IndividualParticipant.getInstance()
                             .ReportedQuestionIDs
                             .Where(qId => q.QuestionId == qId))
                {
                    q.AlreadyReported = true;
                }
            }

            return questionsToDisplay;
        }

        // Gets the list of question IDs, using 'similarity' material
        // depending on whether this page was reached
        // by searching, drafting a question, 'what's trending' or by looking for all the questions written by a
        // given user.
        private async Task<JOSResult<List<string>>> GetAppropriateQuestionList()
        {
            var filters = App.GlobalFilterChoices;
            
            // If we're looking for all the questions written by a given user, request them.
            if (_readByQuestionWriter && !string.IsNullOrWhiteSpace(_writerOnlyUid))
            {
                var questionIDs = await RTAClient.GetQuestionsByWriterId(_writerOnlyUid);
                
                // If there's an error result, pass it back.
                if (questionIDs.Failure)
                {
                    return questionIDs;
                }

                // Success. Return question list.
                return new SuccessResult<List<string>>(questionIDs.Data);
            } 
            
            // else use the filters to search for similar questions.
            var serverSearchQuestion = new QuestionSendToServer()
            {
                question_text = DraftQuestion + " " + Keyword
            };
            
            // If there are no filters, keyword or draft question set, just ask for all questions.
            if( string.IsNullOrWhiteSpace(serverSearchQuestion.question_text) 
                && !serverSearchQuestion.TranscribeQuestionFiltersForUpload(filters))
            {
                return await RTAClient.GetQuestionList();
            }

            // Search based on filters and/or search/draft words.
            var scoredList = await RTAClient.GetSimilarQuestionIDs(serverSearchQuestion);

            // Error
            if (scoredList.Failure)
            {
                if (scoredList is ErrorResult<List<ScoredIDs>> errorResult)
                {
                    return new ErrorResult<List<string>>(errorResult.Message);
                }
                // Fallback error case - currently not reachable.
                return new ErrorResult<List<string>>("Error getting questions from server.");
            }

            // scoredList.Success
            // If we've successfully retrieved a list of scored question IDs, filter them
            // to select the ones we want
            var questionIDsOverThreshold = scoredList.Data
                .Where(q => q.score > Constants.similarityThreshold).Select(q => q.id).ToList();
            if (questionIDsOverThreshold.Any())
            {
                return new SuccessResult<List<string>>(questionIDsOverThreshold);
            }
            
            return new ErrorResult<List<string>>(AppResources.EmptyMatchingQuestionCollectionViewString);
        }
    }
}