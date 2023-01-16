using System;
using System.Text.RegularExpressions;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class WriteQuestionViewModel : ReadingPageViewModel
    {
        public IAsyncCommand BackCommand { get; }
        public AsyncCommand RefreshCommand { get; }

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
        
        private bool _showReturnHomeButton = false;
        public bool ShowReturnHomeButton
        {
            get => _showReturnHomeButton;
            set => SetProperty(ref _showReturnHomeButton, value);
        }
        
        private string _headerContent = AppResources.SimilarQuestionsInstructionText;
        public string HeaderContent
        {
            get => _headerContent;
            set => SetProperty(ref _headerContent, value);
        }
        
        public WriteQuestionViewModel() : base(false, false)
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions();
                if (hasQuery)
                {
                    doQuestionDisplayRefresh(questionsToDisplayList);
                    HeaderContent = AppResources.SimilarQuestionsFound;
                }
                ShowReturnHomeButton = questionsToDisplayList.Count > 0;
                IsRefreshing = false;
                if (QuestionsToDisplay.Count == 0)
                {
                    EmptyViewContent = AppResources.EmptyMatchingQuestionCollectionViewString;
                    EmptyViewIcon = ImageSource.FromResource("RightToAskClient.Images.auto_awesome.png");
                }
            });
        }

        public void RequestUpdate(String query)
        {
            if (query.Length == 0)
            {
                EmptyViewContent = "";
                EmptyViewIcon = "";
                hasQuery = false;
                QuestionsToDisplay.Clear();
                ShowReturnHomeButton = false;
                HeaderContent = AppResources.SimilarQuestionsInstructionText;
                return;
            }

            var endOfWord = new Regex(@"\W").IsMatch(query[query.Length - 1]+"");
            if (!endOfWord)
            {
                return;
            }

            FilterChoices = new FilterChoices();
            FilterChoices.SearchKeyword = query;
            hasQuery = true;
            RefreshCommand.ExecuteAsync();
            
        }
    }
}