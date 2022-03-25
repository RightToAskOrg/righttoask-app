using System;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class QuestionDetailPage : ContentPage
    {
        public QuestionDetailPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            // default to false, then check if they should be true
            QuestionViewModel.Instance.CanEditBackground = false;
            QuestionViewModel.Instance.CanEditQuestion = false;
            if (!string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.uid))
            {
                if (!string.IsNullOrEmpty(QuestionViewModel.Instance.Question.QuestionSuggester))
                {
                    if (QuestionViewModel.Instance.Question.QuestionSuggester == App.ReadingContext.ThisParticipant.RegistrationInfo.uid)
                    {
                        QuestionViewModel.Instance.CanEditBackground = true;
                        QuestionViewModel.Instance.CanEditQuestion = true;
                    }
                }
            }
        }

        // I'm not actually sure what triggers the 'send' event here, and hence not sure
        // which of these two functions should be doing the saving.
        private void Answer_Entered(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.LinkOrAnswer = ((Editor)sender).Text;
        }

        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage.Navigation.PopToRootAsync();
            //_ = Shell.Current.GoToAsync(nameof(MainPage));
            return true;
        }
    }
}