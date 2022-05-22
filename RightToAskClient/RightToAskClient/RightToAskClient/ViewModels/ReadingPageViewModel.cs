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
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
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

        private List<string> _questionIds = new List<string>();
        public List<string> QuestionIds
        {
            get => _questionIds;
            set => SetProperty(ref _questionIds, value);
        }

        private List<QuestionReceiveFromServer> _serverQuestions = new List<QuestionReceiveFromServer>();
        public List<QuestionReceiveFromServer> ServerQuestions
        {
            get => _serverQuestions;
            set => SetProperty(ref _serverQuestions, value);
        }

        private bool _isRefreshing = true;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private string _keyword = "";
        public string Keyword
        {
            get => _keyword;
            set
            {
                bool changed = SetProperty(ref _keyword, value);
                if (changed)
                {
                    App.ReadingContext.Filters.SearchKeyword = _keyword;
                }
            }
        }

        public ReadingPageViewModel()
        {
            PopupLabelText = AppResources.ReadingPageHeader1;
            Keyword = App.ReadingContext.Filters.SearchKeyword;

            if (!string.IsNullOrEmpty(App.ReadingContext.DraftQuestion))
            {
                DraftQuestion = App.ReadingContext.DraftQuestion;
                //DraftQuestion = QuestionViewModel.Instance.Question.QuestionText;
            }

            if (App.ReadingContext.IsReadingOnly)
            {
                ShowQuestionFrame = false;
                Heading1 = AppResources.FocusSupportInstructionReadingOnly;
                if (App.ReadingContext.TopTen)
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
            // get questions from the server
            LoadQuestions();
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
            RefreshCommand = new Command(() =>
            {
                LoadQuestions();
            });
            DraftCommand = new AsyncCommand(async () =>
            {
                //ShowQuestionFrame = true; // navigate to the draft page instead of just showing the frame on this page
                App.ReadingContext.IsReadingOnly = false;
                await Shell.Current.PopGoToAsync($"{nameof(SecondPage)}");
                //Device.BeginInvokeOnMainThread(async () =>  // needed for the UI operation to be invoked on the main thread
                //{
                //    await App.Current.MainPage.Navigation.PopToRootAsync().ContinueWith(async (_) => {

                //        await Shell.Current.GoToAsync($"{nameof(SecondPage)}");
                //    });
                //});
            });
            SearchToolbarCommand = new Command(() =>
            {
                ShowSearchFrame = !ShowSearchFrame; // just toggle it
            });
            ShowFiltersCommand = new AsyncCommand(async() =>
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
        }

        // commands
        public IAsyncCommand KeepQuestionButtonCommand { get; }
        public IAsyncCommand DiscardButtonCommand { get; }
        public Command RefreshCommand { get; }
        public IAsyncCommand DraftCommand { get; }
        public Command SearchToolbarCommand { get; }
        public Command<Question> RemoveQuestionCommand { get; }
        public IAsyncCommand ShowFiltersCommand { get; }

        // helper methods
        private async void OnSaveButtonClicked()
        {
            // Tag the new question with the authorities that have been selected.
            // ObservableCollection<Entity> questionAnswerers;

            /* var questionAnswerers = new ObservableCollection<Entity>(App.ReadingContext.Filters.SelectedAuthorities);

            foreach (var answeringMP in App.ReadingContext.Filters.SelectedAnsweringMPs)
            {
                questionAnswerers.Add(answeringMP);
            } */

            IndividualParticipant thisParticipant = App.ReadingContext.ThisParticipant;

            // Set up new question in preparation for upload. 
            // The filters are what the user has chosen through the flow.
            Question newQuestion = new Question
            {
                QuestionText = App.ReadingContext.DraftQuestion,
                QuestionSuggester = (thisParticipant.IsRegistered) ? thisParticipant.RegistrationInfo.uid : "Anonymous user",
                Filters = App.ReadingContext.Filters, 
                DownVotes = 0,
                UpVotes = 0
            };

            QuestionViewModel.Instance.Question = newQuestion;
            QuestionViewModel.Instance.IsNewQuestion = true;
            await Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
        }
        private async void OnDiscardButtonClicked()
        {
            App.ReadingContext.DraftQuestion = "";
            ShowQuestionFrame = false;

            bool goHome = await App.Current.MainPage.DisplayAlert("Draft discarded", 
               AppResources.FocusSupportReport, "Home", "Related questions");
            if (goHome)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        private async void LoadQuestions()
        {
            Result<List<string>> httpResponse = await RTAClient.GetQuestionList();
            Result<bool> resultToValidate = new Result<bool>();
            if (httpResponse is null)
            {
                return;
            }
            if (!String.IsNullOrEmpty(httpResponse.Err))
            {
                ReportLabelText = "Failed to get Question List from server.";
                return;
            }
            if (httpResponse.Ok != null)
            {
                if (httpResponse.Ok.Any())
                {
                    resultToValidate.Ok = true;
                    resultToValidate.Err = null; // or perhaps empty string
                }
            }
            else
            {
                resultToValidate.Ok = false;
                resultToValidate.Err = "Failed to get Question List from server.";
            }
            var httpValidation = RTAClient.ValidateHttpResponse(resultToValidate, "Server Signature Verification");
            ReportLabelText = httpValidation.message;
            if (httpValidation.isValid)
            {
                // reset the lists to rebuild and re-acquire questions
                QuestionsToDisplay.Clear();
                ServerQuestions.Clear();
                // set the question Ids list
                QuestionIds = httpResponse.Ok;
                // loop through the questions
                foreach (string questionId in QuestionIds)
                {
                    // pull the individual question from the server by id
                    QuestionReceiveFromServer temp;
                    try
                    {
                        var data = await RTAClient.GetQuestionById(questionId);
                        if (string.IsNullOrEmpty(data.Err))
                        {
                            temp = data.Ok;
                            if (!string.IsNullOrEmpty(temp.question_text))
                            {
                                ServerQuestions.Add(temp);
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
                foreach (QuestionReceiveFromServer serverQuestion in ServerQuestions)
                {
                    Question temp = new Question(serverQuestion);
                    
                    QuestionsToDisplay.Add(temp);
                }
                IsRefreshing = false;
            }
            // after getting the list of questions, remove the ids for dismissed questions, and set the upvoted status of liked ones
            for(int i = 0; i < App.ReadingContext.ThisParticipant.RemovedQuestionIDs.Count; i++)
            {
                Question temp = QuestionsToDisplay.ToList()
                    .Where(q => q.QuestionId == App.ReadingContext.ThisParticipant.RemovedQuestionIDs[i])
                    .FirstOrDefault();
                if(temp != null)
                {
                    QuestionsToDisplay.Remove(temp);
                }
            }
            // set previously upvoted questions
            foreach(Question q in QuestionsToDisplay)
            {
                foreach(string qID in App.ReadingContext.ThisParticipant.UpvotedQuestionIDs)
                {
                    if(q.QuestionId == qID)
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
        }
    }
}