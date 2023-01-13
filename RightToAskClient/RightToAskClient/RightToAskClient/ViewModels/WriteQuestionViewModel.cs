using System;
using System.Text.RegularExpressions;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.ViewModels
{
    public class WriteQuestionViewModel : ReadingPageViewModel
    {
        public IAsyncCommand BackCommand { get; }
        public AsyncCommand RefreshCommand { get; }

        private bool hasQuery = false;
        
        public WriteQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions();
                if(hasQuery)
                    doQuestionDisplayRefresh(questionsToDisplayList);
                
                IsRefreshing = false;
            });
        }

        public void RequestUpdate(String query)
        {
            if (query.Length == 0)
            {
                hasQuery = false;
                QuestionsToDisplay.Clear();
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