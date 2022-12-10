using RightToAskClient.Models;
using RightToAskClient.Views;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class QuestionDisplayCardViewModel : QuestionViewModel
    {
        
        public Command QuestionDetailsCommand { get; }

        public QuestionDisplayCardViewModel(Question question) : base()
        {
            //?? Question = question;
            QuestionDetailsCommand = new Command(() =>
            {
                Instance.Question = question;
                Instance.IsNewQuestion = false;
                _ = Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
            });
        }
    }
}