using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionBackgroundPage : ContentPage
    {
        public QuestionBackgroundPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;

            // default to false, then check if they should be true
            QuestionViewModel.Instance.CanEditBackground = false;

            // if in the question drafting stages the background is editable
            if (QuestionViewModel.Instance.IsNewQuestion)
            {
                QuestionViewModel.Instance.CanEditBackground = true;
            }
            // otherwise check to see if the user is the creator of the question to edit from the reading pages
            // this page isn't accessed out side of the question drafting flow, so these checks might not be needed.
            //else if (!string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.uid))
            //{
            //    if (!string.IsNullOrEmpty(QuestionViewModel.Instance.Question.QuestionSuggester))
            //    {
            //        if (QuestionViewModel.Instance.Question.QuestionSuggester == App.ReadingContext.ThisParticipant.RegistrationInfo.uid)
            //        {
            //            QuestionViewModel.Instance.CanEditBackground = true;
            //        }
            //    }
            //}
        }
    }
}