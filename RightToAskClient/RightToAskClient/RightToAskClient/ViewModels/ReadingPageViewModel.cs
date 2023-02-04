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
    public class ReadingPageViewModel: BaseViewModel
    {
        public FilterChoices FilterChoices = new FilterChoices();
     
        // This is static because we want every instance of ReadingPageViewModel to have the same one:
        // if you up-vote a question in one version of the ReadingView, we need the other ReadingPages to
        // see the same change.
        private static QuestionResponseRecords _thisUsersResponsesToQuestions = new QuestionResponseRecords();
        
        // properties

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

        private Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        private ObservableCollection<QuestionDisplayCardViewModel> _questionsToDisplay = new ObservableCollection<QuestionDisplayCardViewModel>();
        public ObservableCollection<QuestionDisplayCardViewModel> QuestionsToDisplay
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
            get => FilterChoices.SearchKeyword;
            set
            {
                FilterChoices.SearchKeyword = value;
                OnPropertyChanged();
            }
        }
        
        private string _writerOnlyUid = string.Empty;
        private bool _readByQuestionWriter;
        protected bool _successRespond = true;

        private string _emptyViewLabelText;
        public string EmptyViewLabelText
        {
            get => _emptyViewLabelText;
            set => SetProperty(ref _emptyViewLabelText, value);
        }

        // Parameters for server question searches
        protected readonly Weights SearchWeights;
        public ReadingPageViewModel(): this(false, true)
        {
        }
        
        // constructor
        public ReadingPageViewModel(bool readByQuestionWriter, bool needRefresh)
        {
            _readByQuestionWriter = readByQuestionWriter;
            
            // Retrieve previous responses from Preferences, e.g. to display proper coloration on prior up-votes.
            _thisUsersResponsesToQuestions.Init();
            
            // Keyword = FilterChoices.SearchKeyword;
            
            // Default to search weights for main reading page; derived classes can alter this for other priorities.
            SearchWeights = Constants.mainReadingPageWeights;
            
            // If we're already searching for something, show the user what.
            ShowSearchFrame = !readByQuestionWriter; 

            // Reading with a draft question - prompt for upvoting similar questions
            Heading1 = AppResources.FocusSupportInstructionReadingOnly;
            Title = AppResources.ReadQuestionsTitle;

            PopupLabelText = AppResources.ReadingPageHeader1;
            PopupHeaderText = Heading1;
            
            if (XamarinPreferences.shared.Get(Constants.ShowFirstTimeReadingPopup, true))
            {
                InfoPopupCommand.ExecuteAsync();
                
                // Only show it once.
                XamarinPreferences.shared.Set(Constants.ShowFirstTimeReadingPopup, false);
            }
            // Note: There is a race condition here, in that it is possible
            // for this command to be executed multiple times simultaneously,
            // producing multiple calls to Clear and the simultaneous insertion
            // of questions from various versions of questionsToDisplay List.
            // I don't *think* this will cause a lock because QuestionsToDisplay
            // ought to be able to be cleared and added to in any order.
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions();
                doQuestionDisplayRefresh(questionsToDisplayList);
                IsRefreshing = false;
            });
            DraftCommand = new AsyncCommand(async () =>
            {
                
                // Check that they are registered - if not, prompt them to get an account.
                if (!IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                {
                    await NavigationUtils.DoRegistrationCheck(
                        IndividualParticipant.getInstance().ProfileData.RegistrationInfo,
                        AppResources.CancelButtonText);
                }

                if (IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
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
                    await Shell.Current.GoToAsync($"{nameof(WriteQuestionPage)}");
                }
            });
            SearchToolbarCommand = new Command(() =>
            {
                ShowSearchFrame = !ShowSearchFrame; // just toggle it
            });
            ShowFiltersCommand = new AsyncCommand(async () =>
            {
                var advancedSearchFiltersPage = new AdvancedSearchFiltersPage(FilterChoices);
                await Application.Current.MainPage.Navigation.PushAsync(advancedSearchFiltersPage);
            });
            RemoveQuestionCommand = new Command<QuestionDisplayCardViewModel>(questionToRemove =>
            {
                if (questionToRemove?.Question.QuestionId != null)
                {
                    _thisUsersResponsesToQuestions.AddDismissedQuestion(questionToRemove.Question.QuestionId);
                    QuestionsToDisplay.Remove(questionToRemove);
                }
                // removeQuestionAddRecord(questionToRemove);
            }); 

            // Get the question list for display
            if (needRefresh)
            {
                RefreshCommand.ExecuteAsync();
            }
            else
            {
                IsRefreshing = false;
            }
            
            // Makes the question list refresh when the Advanced Search page has updated search terms.
            MessagingCenter.Subscribe<FilterViewModel>(this, Constants.RefreshQuestionList, 
                 (sender) =>
                {
                    RefreshCommand.ExecuteAsync();
                });
        }

        // commands
        public AsyncCommand RefreshCommand { get; protected set; }
        public IAsyncCommand DraftCommand { get; }
        public Command SearchToolbarCommand { get; }
        public Command<QuestionDisplayCardViewModel> RemoveQuestionCommand { get; }
        public IAsyncCommand ShowFiltersCommand { get; }

        // Loads the questions, depending on the value of ReadByQuestionWriter.
        protected async Task<List<QuestionDisplayCardViewModel>> LoadQuestions()
        {
            var serverQuestions = new List<QuestionReceiveFromServer>();
            var questionsToDisplay = new List<QuestionDisplayCardViewModel>();
            
            JOSResult<List<string>> httpResponse;
            if (_readByQuestionWriter)
            {
                EmptyViewLabelText = AppResources.NoPostedQuestionCollectionViewString;
                httpResponse = await GetQuestionListByWriter();
            }
            else
            {
                EmptyViewLabelText = AppResources.EmptyMatchingQuestionCollectionViewString;
                httpResponse = await GetQuestionListBySearch();
            }
            
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            _successRespond = httpValidation.isValid;
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

            // convert the ServerQuestions to a Displayable format. Drop already-dismissed ones.
            foreach (var serverQuestion in serverQuestions)
            {
                if (!_thisUsersResponsesToQuestions.IsAlreadyDismissed(serverQuestion.question_id ?? ""))
                {
                    questionsToDisplay.Add(new QuestionDisplayCardViewModel(serverQuestion, _thisUsersResponsesToQuestions));
                }
            }

            return questionsToDisplay;
        }

        protected void doQuestionDisplayRefresh(List<QuestionDisplayCardViewModel> questions)
        {
            QuestionsToDisplay.Clear();
            foreach (var q in questions)
            {  
                QuestionsToDisplay.Add(q);
            }
        }
        
        // Gets the list of question IDs, using 'similarity' material
        private async Task<JOSResult<List<string>>> GetQuestionListBySearch()
        {
            var filters = FilterChoices;
            
            // use the filters to search for similar questions.
            var serverSearchQuery = new WeightedSearchRequest()
            {
                question_text = filters.SearchKeyword,
                page = new QuestionListPage()
                {
                    from = 0,
                    to = Constants.DefaultPageSize 
                },
                weights = SearchWeights,
                entity_who_should_answer_the_question = filters.TranscribeQuestionAnswerersForUpload(),
                mp_who_should_ask_the_question = filters.TranscribeQuestionAskersForUpload()
            };
            
            // Search based on filters and/or search/draft words.
            var scoredList = await RTAClient.GetSortedSimilarQuestionIDs(serverSearchQuery);

            // Error
            if (scoredList.Failure)
            {
                if (scoredList is ErrorResult<SortedQuestionList> errorResult)
                {
                    return new ErrorResult<List<string>>(errorResult.Message);
                }
                // Fallback error case - currently not reachable.
                return new ErrorResult<List<string>>("Error getting questions from server.");
            }

            // scoredList.success
            // For the moment, ignore both the token and the individual-question scores.
            return new SuccessResult<List<string>>(scoredList.Data.questions.Select(sq => sq.id).ToList());
        }
        
        private async Task<JOSResult<List<string>>> GetQuestionListByWriter()
        {
            var myUID = IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
            if (string.IsNullOrEmpty(myUID))
            {
                // TODO: Sensible error result when not registered.
                return new SuccessResult<List<string>>(new List<string>());
            }

            var questionIDs = await RTAClient.GetQuestionsByWriterId(myUID);

            // If there's an error result, pass it back.
            if (questionIDs.Failure)
            {
                return questionIDs;
            }

            // Success. Return question list.
            return new SuccessResult<List<string>>(questionIDs.Data);
        }
    }
}