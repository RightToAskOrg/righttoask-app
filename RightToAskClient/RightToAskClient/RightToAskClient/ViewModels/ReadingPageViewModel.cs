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
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class ReadingPageViewModel : BaseViewModel
    {
        // properties
        private bool _showContinueButton = true;
        public bool ShowContinueButton
        {
            get => _showContinueButton;
            set => SetProperty(ref _showContinueButton, value);
        }

        private bool _showDiscardButton = true;
        public bool ShowDiscardButton
        {
            get => _showDiscardButton;
            set => SetProperty(ref _showDiscardButton, value);
        }

        private bool _showDraftEditor = true;
        public bool ShowDraftEditor
        {
            get => _showDraftEditor;
            set => SetProperty(ref _showDraftEditor, value);
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

        private List<NewQuestionServerReceive> _serverQuestions = new List<NewQuestionServerReceive>();
        public List<NewQuestionServerReceive> ServerQuestions
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

        public ReadingPageViewModel()
        {
            if (!string.IsNullOrEmpty(App.ReadingContext.DraftQuestion))
            {
                DraftQuestion = App.ReadingContext.DraftQuestion;
                //DraftQuestion = QuestionViewModel.Instance.Question.QuestionText;
            }

            if (App.ReadingContext.IsReadingOnly)
            {
                ShowDraftEditor = false;
                ShowDiscardButton = false;
                ShowContinueButton = false;
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
                Title = AppResources.SimilarQuestionsTitle;
                ShowDraftEditor = true;
                ShowDiscardButton = true;
                ShowContinueButton = true;
            }
            // get questions from the server
            LoadQuestions();

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
        }

        // commands
        public IAsyncCommand KeepQuestionButtonCommand { get; }
        public IAsyncCommand DiscardButtonCommand { get; }
        public Command RefreshCommand { get; }

        // helper methods
        private async void OnSaveButtonClicked()
        {
            // Tag the new question with the authorities that have been selected.
            // ObservableCollection<Entity> questionAnswerers;
            var questionAnswerers = new ObservableCollection<Entity>(App.ReadingContext.Filters.SelectedAuthorities);

            foreach (var answeringMP in App.ReadingContext.Filters.SelectedAnsweringMPs)
            {
                questionAnswerers.Add(answeringMP);
            }

            IndividualParticipant thisParticipant = App.ReadingContext.ThisParticipant;

            Question newQuestion = new Question
            {
                QuestionText = App.ReadingContext.DraftQuestion,
                // TODO: Enforce registration before question-suggesting.
                QuestionSuggester = (thisParticipant.IsRegistered) ? thisParticipant.RegistrationInfo.display_name : "Anonymous user",
                QuestionAnswerers = questionAnswerers,
                DownVotes = 0,
                UpVotes = 0
            };

            QuestionViewModel.Instance.Question = newQuestion;
            QuestionViewModel.Instance.IsNewQuestion = true;
            //await Navigation.PushAsync(questionDetailPage);
            await Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
        }
        private async void OnDiscardButtonClicked()
        {
            App.ReadingContext.DraftQuestion = "";
            ShowDraftEditor = false;
            ShowDiscardButton = false;
            ShowContinueButton = false;

            bool goHome = await App.Current.MainPage.DisplayAlert("Draft discarded", 
                "Save time and focus support by voting on a similar question", 
                "Home", "Related questions");
            if (goHome)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        private async void LoadQuestions()
        {
            Result<List<string>> httpResponse = await RTAClient.GetQuestionList();
            Result<bool> resultToValidate = new Result<bool>();
            if (httpResponse.Ok.Any())
            {
                resultToValidate.Ok = true;
                resultToValidate.Err = null; // or perhaps empty string
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
                    NewQuestionServerReceive temp;
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
                foreach (NewQuestionServerReceive serverQuestion in ServerQuestions)
                {
                    Question temp = new Question()
                    {
                        QuestionId = serverQuestion.question_id,
                        QuestionText = serverQuestion.question_text,
                        QuestionSuggester = serverQuestion.author,
                        Version = serverQuestion.version,
                        //Background = serverQuestion.background, // doesn't exist on the server response object yet
                        //UploadTimestamp = serverQuestion.timestamp,
                        //ExpiryDate = serverQuestion.last_modified,
                        //QuestionAsker = serverQuestion.who_should_ask_the_question_permissions,
                        //QuestionAnswerers = serverQuestion.who_should_answer_the_question_permissions,
                    };
                    QuestionsToDisplay.Add(temp);
                }
                IsRefreshing = false;
            }
        }
    }
}
