using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.Input;

namespace RightToAskClient.ViewModels
{
    public class QuestionDisplayCardViewModel : QuestionViewModel
    {
        
        public IAsyncRelayCommand  QuestionDetailsCommand { get; }

        public QuestionDisplayCardViewModel(QuestionReceiveFromServer question, QuestionResponseRecords questionResponses) : base()
        {
            ResponseRecords = questionResponses;
            Question = new Question(question, questionResponses);

            QuestionDetailsCommand = new AsyncRelayCommand (async () =>
            {
                ReInitUpdatesAndErrors();

                var questionDetailPage = new QuestionDetailPage(this);
                await Application.Current.MainPage.Navigation.PushAsync(questionDetailPage);
            });
        }
    }
}