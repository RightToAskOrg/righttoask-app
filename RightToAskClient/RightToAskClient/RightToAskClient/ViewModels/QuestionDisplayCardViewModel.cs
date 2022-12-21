using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class QuestionDisplayCardViewModel : QuestionViewModel
    {
        
        public IAsyncCommand QuestionDetailsCommand { get; }

        public QuestionDisplayCardViewModel(QuestionReceiveFromServer question, QuestionResponseRecords questionResponses) : base()
        {
            ResponseRecords = questionResponses;
            Question = new Question(question, questionResponses);
            QuestionDetailsCommand = new AsyncCommand(async () =>
            {
                var questionDetailPage = new QuestionDetailPage(this);
                await Application.Current.MainPage.Navigation.PushAsync(questionDetailPage);
            });
        }
    }
}