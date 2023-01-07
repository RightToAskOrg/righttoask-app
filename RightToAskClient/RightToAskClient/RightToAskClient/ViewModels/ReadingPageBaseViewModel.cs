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
    public class ReadingPageBaseViewModel : BaseViewModel
    {
        public FilterChoices FilterChoices = new FilterChoices();
     
        // This is static because we want every instance of ReadingPageViewModel to have the same one:
        // if you up-vote a question in one version of the ReadingView, we need the other ReadingPages to
        // see the same change.
        protected static QuestionResponseRecords _thisUsersResponsesToQuestions = new QuestionResponseRecords();
        
        // properties
        protected bool _showQuestionFrame = false;
        public bool ShowQuestionFrame
        {
            get => _showQuestionFrame;
            set => SetProperty(ref _showQuestionFrame, value);
        }

        protected bool _showSearchFrame;
        public bool ShowSearchFrame
        {
            get => _showSearchFrame;
            set => SetProperty(ref _showSearchFrame, value);
        }

        protected string _heading1 = string.Empty;
        public string Heading1
        {
            get => _heading1;
            set => SetProperty(ref _heading1, value);
        }
        protected string _draftQuestion = "";
        public string DraftQuestion
        {
            get => _draftQuestion;
            set => SetProperty(ref _draftQuestion, value);
        }

        protected Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        protected ObservableCollection<QuestionDisplayCardViewModel> _questionsToDisplay = new ObservableCollection<QuestionDisplayCardViewModel>();
        public ObservableCollection<QuestionDisplayCardViewModel> QuestionsToDisplay
        {
            get => _questionsToDisplay;
            set => SetProperty(ref _questionsToDisplay, value);
        }

        protected bool _isRefreshing = true;
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
        
        protected string _writerOnlyUid = string.Empty;
        protected bool _readByQuestionWriter;

        // constructor
        public ReadingPageBaseViewModel()
        {
            // Retrieve previous responses from Preferences, e.g. to display proper coloration on prior up-votes.
            _thisUsersResponsesToQuestions.Init();
            
            Keyword = FilterChoices.SearchKeyword;
            
            // If we're already searching for something, show the user what.
            ShowSearchFrame = !string.IsNullOrWhiteSpace(Keyword); 


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
            RefreshCommand = new AsyncCommand(async () =>
            {
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
                    ShowQuestionFrame = true;
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


            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.QuestionSubmittedDeleteDraft,
                (sender) =>
            {
                    DraftQuestion = "";
                    ShowQuestionFrame = false;
            });
            
        }

        // commands
        public IAsyncCommand KeepQuestionButtonCommand { get; }
        public IAsyncCommand DiscardButtonCommand { get; }
        public AsyncCommand RefreshCommand { get; protected set; }
        public IAsyncCommand DraftCommand { get; }
        public Command SearchToolbarCommand { get; }
        public Command<QuestionDisplayCardViewModel> RemoveQuestionCommand { get; }
        public IAsyncCommand ShowFiltersCommand { get; }

        // helper methods
        protected async void OnSaveButtonClicked()
        {
            // Set up new question in preparation for upload. 
            // The filters are new empty filters. 
            var newQuestion = new Question()
            {
                QuestionText = DraftQuestion,
                QuestionSuggester = (IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsRegistered)
                    ? IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid
                    : "",
                Filters = new FilterChoices()
            };

            QuestionViewModel.Instance.Question = newQuestion;
            QuestionViewModel.Instance.IsNewQuestion = true;

            // instead of going to the questionDetailsPage we now go to a Background page and then a metadata page before the details page
            await Shell.Current.GoToAsync($"{nameof(QuestionBackgroundPage)}");
        }

        protected async void OnDiscardButtonClicked()
        {
            DraftQuestion = "";
            ShowQuestionFrame = false;

            var popup = new OneButtonPopup(AppResources.DraftDiscardedPopupTitle, AppResources.FocusSupportReport, AppResources.OKText);
            _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);

            ShowQuestionFrame = false;
        }

        // Loads the questions, given a parameter specifying the function for retrieving the correct list of them.
        protected async Task<List<QuestionDisplayCardViewModel>> LoadQuestions(
            Task<JOSResult<List<string>>> questionListGettingFunction)
        {
            var serverQuestions = new List<QuestionReceiveFromServer>();
            var questionsToDisplay = new List<QuestionDisplayCardViewModel>();
            var httpResponse = await questionListGettingFunction;
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
    }
}
