using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class ReadingPage : ContentPage
    {
        public ReadingPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            // clear the selected item
            QuestionList.SelectedItem = null;
            base.OnDisappearing();
        }
    }
}