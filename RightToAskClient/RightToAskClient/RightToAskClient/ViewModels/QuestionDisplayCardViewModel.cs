using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Views;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class QuestionDisplayCardViewModel : QuestionViewModel
    {
        
        public Command QuestionDetailsCommand { get; }

        public QuestionDisplayCardViewModel(QuestionReceiveFromServer question, QuestionResponseRecords questionResponses) : base()
        {
            ResponseRecords = questionResponses;
            Question = new Question(question, questionResponses);
            QuestionDetailsCommand = new Command(() =>
            {
                Instance.Question = Question;
                Instance.IsNewQuestion = false;
                _ = Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
            });
        }
    }
}