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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class ReadingPageViewModel : BaseViewModel
    {
        // properties
        private bool _showQuestionFrame = true;
        public bool ShowQuestionFrame
        {
            get => _showQuestionFrame;
            set => SetProperty(ref _showQuestionFrame, value);
        }

        private bool _showSearchFrame = false;
        public bool ShowSearchFrame
        {
            get => _showSearchFrame;
            set => SetProperty(ref _showSearchFrame, value);
        }

        private bool _dontShowFirstTimeReadingPopup = false;
        public bool DontShowFirstTimeReadingPopup
        {
            get => _dontShowFirstTimeReadingPopup;
            set
            {
                var changed = SetProperty(ref _dontShowFirstTimeReadingPopup, value);
                if (changed)
                {
                    Preferences.Set(Constants.DontShowFirstTimeReadingPopup, value);
                    App.ReadingContext.DontShowFirstTimeReadingPopup = value;
                }
            }
        }

        private string heading1 = string.Empty;
        public string Heading1
        {
            get => heading1;
            set => SetProperty(ref heading1, value);
        }
        private string _draftQuestion = "";
        public string DraftQuestion
        {
            get => _draftQuestion;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SetProperty(ref _draftQuestion, value);
                    App.ReadingContext.DraftQuestion = value;
                }
            }
        }

        private Question? _selectedQuestion = null;
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

        public ObservableCollection<Question> ExistingQuestions => App.ReadingContext.ExistingQuestions;
        private ObservableCollection<Question> _questionsToDisplay = new ObservableCollection<Question>();
        public ObservableCollection<Question> QuestionsToDisplay
        {
            get => _questionsToDisplay;
            set => SetProperty(ref _questionsToDisplay, value);
        }

        /*
        private List<string> _questionIds = new List<string>();
        private List<string> QuestionIds
        {
            get => _questionIds;
            set => SetProperty(ref _questionIds, value);
        }
        */

        /*
        private List<QuestionReceiveFromServer> _serverQuestions = new List<QuestionReceiveFromServer>();
        public List<QuestionReceiveFromServer> ServerQuestions
        {
            get => _serverQuestions;
            set => SetProperty(ref _serverQuestions, value);
        }
        */

        private bool _isRefreshing = true;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string Keyword
        {
            get => App.ReadingContext.Filters.SearchKeyword;
            set
            {
                App.ReadingContext.Filters.SearchKeyword = value;
                OnPropertyChanged();
            }
        }

        // constructor
        public ReadingPageViewModel()
        {
            PopupLabelText = AppResources.ReadingPageHeader1;
            Keyword = App.ReadingContext.Filters.SearchKeyword;

            if (!string.IsNullOrEmpty(App.ReadingContext.DraftQuestion))
            {
                DraftQuestion = App.ReadingContext.DraftQuestion;
            }

            if (App.ReadingContext.IsReadingOnly)
            {
                ShowQuestionFrame = false;
                Heading1 = AppResources.FocusSupportInstructionReadingOnly;
                if (App.ReadingContext.TrendingNow)
                {
                    Title = AppResources.RecentQuestionsTitle;
                }
                else
                {
                    Title = AppResources.ReadQuestionsTitle;
                }
            }
            else
            {
                Heading1 = AppResources.FocusSupportInstructionQuestionDrafting;
                Title = AppResources.SimilarQuestionsTitle;
                ShowQuestionFrame = true;
            }
            if (!App.ReadingContext.DontShowFirstTimeReadingPopup)
            {
                var popup = new ReadingPagePopup(this);
                _ = App.Current.MainPage.Navigation.ShowPopupAsync(popup);
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
                var questionsToDisplayList = await LoadQuestions() ?? new List<Question>();
                QuestionsToDisplay.Clear();
                foreach (Question q in questionsToDisplayList)
                {  
                    QuestionsToDisplay.Add(q);
                }
                IsRefreshing = false;
            });
            DraftCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.IsReadingOnly = false;
                await Shell.Current.PopGoToAsync($"{nameof(SecondPage)}");
            });
            SearchToolbarCommand = new Command(() =>
            {
                ShowSearchFrame = !ShowSearchFrame; // just toggle it
            });
            ShowFiltersCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync(nameof(AdvancedSearchFiltersPage));
            });
            RemoveQuestionCommand = new Command<Question>((Question questionToRemove) =>
            {
                // store question ID for later data manipulation?
                if (!App.ReadingContext.ThisParticipant.RemovedQuestionIDs.Contains(questionToRemove.QuestionId))
                {
                    App.ReadingContext.ThisParticipant.RemovedQuestionIDs.Add(questionToRemove.QuestionId);
                }
                QuestionsToDisplay.Remove(questionToRemove);
            });

            // Get the question list for display
            RefreshCommand.ExecuteAsync();
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
            IndividualParticipant thisParticipant = App.ReadingContext.ThisParticipant;

            // Set up new question in preparation for upload. 
            // The filters are what the user has chosen through the flow.
            Question newQuestion = new Question
            {
                QuestionText = App.ReadingContext.DraftQuestion,
                QuestionSuggester = (thisParticipant.IsRegistered)
                    ? thisParticipant.RegistrationInfo.uid
                    : "",
                Filters = App.ReadingContext.Filters,
                DownVotes = 0,
                UpVotes = 0
            };

            QuestionViewModel.Instance.Question = newQuestion;
            QuestionViewModel.Instance.IsNewQuestion = true;

            // instead of going to the questionDetailsPage we now go to a Background page and then a metadata page before the details page
            await Shell.Current.GoToAsync($"{nameof(QuestionBackgroundPage)}");
        }

        private async void OnDiscardButtonClicked()
        {
            App.ReadingContext.DraftQuestion = "";
            ShowQuestionFrame = false;

            var popup = new TwoButtonPopup(QuestionViewModel.Instance, AppResources.DraftDiscardedPopupTitle, AppResources.FocusSupportReport, AppResources.RelatedQuestionsButtonText, AppResources.GoHomeButtonText);
            _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (ApproveButtonClicked)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        public async Task<List<Question>> LoadQuestions()
        {
            List<QuestionReceiveFromServer> serverQuestions = new List<QuestionReceiveFromServer>();
            List<Question> questionsToDisplay = new List<Question>();
            Result<List<string>> httpResponse = await GetAppropriateQuestionList();
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            if (!httpValidation.isValid)
            {
                ReportLabelText = "Failed to get Question List from server." + httpValidation.errorMessage;
                return questionsToDisplay;
            }
            
            // httpValidation isValid
            var questionIds = httpResponse.Ok;
            
            // loop through the questions
            foreach (string questionId in questionIds)
            {
                // pull the individual question from the server by id
                QuestionReceiveFromServer tempQuestion;
                try
                {
                    var data = await RTAClient.GetQuestionById(questionId);
                    if (string.IsNullOrEmpty(data.Err))
                    {
                        tempQuestion = data.Ok;
                        if (!string.IsNullOrEmpty(tempQuestion.question_text))
                        {
                            serverQuestions.Add(tempQuestion);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Could not find question: " + data.Err);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not find question: " + ex.Message);
                }
            }

            // convert the ServerQuestions to a Displayable format
            foreach (QuestionReceiveFromServer serverQuestion in serverQuestions)
            {
                questionsToDisplay.Add(new Question(serverQuestion));
            }


            // after getting the list of questions, remove the ids for dismissed questions, and set the upvoted status of liked ones
            for (int i = 0; i < App.ReadingContext.ThisParticipant.RemovedQuestionIDs.Count; i++)
            {
                Question? temp = questionsToDisplay
                    .FirstOrDefault(q => q.QuestionId == App.ReadingContext.ThisParticipant.RemovedQuestionIDs[i]);
                if (temp != null)
                {
                    questionsToDisplay.Remove(temp);
                }
            }

            // set previously upvoted questions
            foreach (Question q in questionsToDisplay)
            {
                foreach (string qID in App.ReadingContext.ThisParticipant.UpvotedQuestionIDs)
                {
                    if (q.QuestionId == qID)
                    {
                        q.AlreadyUpvoted = true;
                    }
                }

                // set previously flagged/reported questions
                foreach (string qID in App.ReadingContext.ThisParticipant.ReportedQuestionIDs)
                {
                    if (q.QuestionId == qID)
                    {
                        q.AlreadyReported = true;
                    }
                }
            }

            return questionsToDisplay;
        }

        // Gets the list of question IDs, using 'similarity' material
        // depending on whether this page was reached
        // by searching, drafting a question, 'what's trending' etc.
        private async Task<Result<List<string>>> GetAppropriateQuestionList()
        {
            // Ask for questions similar to the Draft question text and/or the keyword.
            if (!String.IsNullOrWhiteSpace(DraftQuestion + Keyword))
            {
                var serverSearchQuestion = new QuestionSendToServer()
                {
                    question_text = DraftQuestion + " " + Keyword
                };
                Result<List<ScoredIDs>> scoredList = await RTAClient.GetSimilarQuestionIDs(serverSearchQuestion);

                // Error
                if (!String.IsNullOrEmpty(scoredList?.Err))
                {
                    return new Result<List<string>>()
                    {
                        Err = scoredList.Err
                    };
                }

                // If we've successfully retrieved a list of scored question IDs, filter them
                // to select the ones we want
                List<string> questionIDsOverThreshold = scoredList.Ok.Where(q => q.score > Constants.similarityThreshold)
                    .Select(q => q.id).ToList();

                if (questionIDsOverThreshold.Any())
                {
                    return new Result<List<string>>()
                    {
                        Ok = questionIDsOverThreshold
                    };
                }
                else
                {
                    return new Result<List<string>>()
                    {
                        Err = AppResources.EmptyMatchingQuestionCollectionViewString
                    };
                }
            }
            
            // If we're just looking at what's trending, show everything
            // Expect App.ReadingContext.TrendingNow, but not necessarily because, for example,
            // you might have drafted a blank question
            return await RTAClient.GetQuestionList();
        }
    }
}