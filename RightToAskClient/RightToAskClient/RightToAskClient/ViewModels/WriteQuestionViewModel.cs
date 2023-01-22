using System;
using System.Text.RegularExpressions;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class WriteQuestionViewModel : ReadingPageViewModel
    {
        public IAsyncCommand BackCommand { get; }
        // public AsyncCommand RefreshCommand { get; }

        private bool hasQuery = false;
        
        private string _emptyViewContent = "";
        public string EmptyViewContent
        {
            get => _emptyViewContent;
            set => SetProperty(ref _emptyViewContent, value);
        }

        private ImageSource _emptyViewIcon;
        public ImageSource EmptyViewIcon
        {
            get => _emptyViewIcon;
            set => SetProperty(ref _emptyViewIcon, value);
        }
        
        private bool _showHeader = true;
        public bool ShowHeader
        {
            get => _showHeader;
            set => SetProperty(ref _showHeader, value);
        }
        
        private string _draftQuestion = "";
        public string DraftQuestion
        {
            get => _draftQuestion;
            set => SetProperty(ref _draftQuestion, value);
        }

        // Commands
        public IAsyncCommand ProceedButtonCommand { get; }
        
        public WriteQuestionViewModel() : base(false, false)
        {
            BackCommand = new AsyncCommand(async () =>
            {
                var popup = new TwoButtonPopup(
                    AppResources.GoHomePopupTitle, 
                    AppResources.GoHomePopupText, 
                    AppResources.CancelButtonText, 
                    AppResources.GoHomeButtonText, 
                    false);
                var popupResult = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (popup.HasApproved(popupResult))
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            });
            
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions();
                if (hasQuery)
                {
                    doQuestionDisplayRefresh(questionsToDisplayList);
                }
                IsRefreshing = false;
                if (QuestionsToDisplay.Count == 0 && hasQuery)
                {
                    if (!_successRespond)
                    {
                        ShowHeader = false;
                        EmptyViewContent = AppResources.NoNetwork;
                        EmptyViewIcon = ImageSource.FromResource("RightToAskClient.Images.sentiment_very_dissatisfied.png");
                    }
                    else
                    {
                        EmptyViewContent = AppResources.NoQuestionFound;
                        EmptyViewIcon = ImageSource.FromResource("RightToAskClient.Images.auto_awesome.png");
                    }
                }
            });
            
            ProceedButtonCommand = new AsyncCommand(async () =>
            {
                OnSaveButtonClicked();
            });
        }

        public void RequestUpdate(String query, bool force = false)
        {
            DraftQuestion = query;
            ShowHeader = true;
            if (query.Length == 0)
            {
                EmptyViewContent = "";
                EmptyViewIcon = "";
                hasQuery = false;
                QuestionsToDisplay.Clear();
                return;
            }

            var endOfWord = new Regex(@"\W").IsMatch(query[query.Length - 1]+"");
            if (!endOfWord && !force)
            {
                return;
            }

            FilterChoices = new FilterChoices();
            FilterChoices.SearchKeyword = query;
            hasQuery = true;
            RefreshCommand.ExecuteAsync();
        }
        private async void OnSaveButtonClicked()
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
    }
}