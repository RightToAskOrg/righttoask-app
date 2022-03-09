using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ReadingPageViewModel()
        {
            // main page view model doesn't exist yet -- changes haven't been merged to main branch
            //MessagingCenter.Subscribe<MainPageViewModel, bool>(this, "DraftingQuestion", (sender, arg) =>
            //{
            //    if (arg)
            //    {
            //        App.ReadingContext.IsReadingOnly = true;
            //    }
            //    MessagingCenter.Unsubscribe<MainPageViewModel, bool>(this, "DraftingQuestion");
            //});

            if (!string.IsNullOrEmpty(App.ReadingContext.DraftQuestion))
            {
                DraftQuestion = App.ReadingContext.DraftQuestion;
                //DraftQuestion = QuestionViewModel.Instance.Question.QuestionText;
            }

            if (App.ReadingContext.IsReadingOnly)
            {
                Title = AppResources.ReadQuestionsTitle;
                ShowDraftEditor = false;
                ShowDiscardButton = false;
                ShowContinueButton = false;
            }
            else
            {
                Title = AppResources.SimilarQuestionsTitle;
                ShowDraftEditor = true;
                ShowDiscardButton = true;
                ShowContinueButton = true;
            }

            KeepQuestionButtonCommand = new AsyncCommand(async () =>
            {
                OnSaveButtonClicked();
            });
            DiscardButtonCommand = new AsyncCommand(async () =>
            {
                OnDiscardButtonClicked();
            });
        }

        // commands
        public IAsyncCommand KeepQuestionButtonCommand { get; }
        public IAsyncCommand DiscardButtonCommand { get; }

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
    }
}
