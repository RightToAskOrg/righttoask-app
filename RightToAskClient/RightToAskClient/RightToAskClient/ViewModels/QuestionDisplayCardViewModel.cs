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
            
            // Keep track of changes to question asking/answering permission.
            _initialWhoCanAskPermissions = question.who_should_ask_the_question_permissions;
            _initialWhoCanAnswerPermissions = question.who_should_answer_the_question_permissions;
            
            QuestionDetailsCommand = new AsyncCommand(async () =>
            {
                var questionDetailPage = new QuestionDetailPage(this);
                await Application.Current.MainPage.Navigation.PushAsync(questionDetailPage);
            });
        }
    }
}