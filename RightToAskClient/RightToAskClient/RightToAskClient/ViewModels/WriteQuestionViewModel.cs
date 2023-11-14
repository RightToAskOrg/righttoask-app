using System;
using System.Text.RegularExpressions;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using RightToAskClient.Views.Popups;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.Input;
namespace RightToAskClient.ViewModels
{
    public class WriteQuestionViewModel : ReadingPageViewModel
    {
        public IAsyncRelayCommand  BackCommand { get; }
        // public AsyncRelayCommand  RefreshCommand { get; }

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
        public IAsyncRelayCommand  ProceedButtonCommand { get; }
        
        public WriteQuestionViewModel() : base(false, false)
        {
            BackCommand = new AsyncRelayCommand (async () =>
            {
                //var popup = new TwoButtonPopup(
                //    AppResources.GoHomePopupTitle, 
                //    AppResources.GoHomePopupText, 
                //    AppResources.CancelButtonText, 
                //    AppResources.GoHomeButtonText, 
                //    false);
                var popupResult = await App.Current.MainPage.DisplayPromptAsync(AppResources.GoHomePopupTitle,
                    AppResources.GoHomePopupText,
                    AppResources.GoHomeButtonText,
                    AppResources.CancelButtonText);
                if (popupResult == AppResources.GoHomeButtonText)
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            });
            
            RefreshCommand = new AsyncRelayCommand (async () =>
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
            
            ProceedButtonCommand = new AsyncRelayCommand (async () =>
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
            RefreshCommand.ExecuteAsync(null);
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