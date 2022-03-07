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
        }

        // I'm not actually sure what triggers the 'send' event here, and hence not sure
        // which of these two functions should be doing the saving.
        private void Answer_Entered(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.LinkOrAnswer = ((Editor)sender).Text;
        }

        private void Background_Entered(object sender, EventArgs e)
        {
            // Do nothing.
        }

        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage.Navigation.PopToRootAsync();
            //_ = Shell.Current.GoToAsync(nameof(MainPage));
            return true;
        }
    }
}