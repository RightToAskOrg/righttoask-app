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

        // Note: it's possible that this would be better with an ItemTapped event instead.
        private async void Question_Selected(object sender, ItemTappedEventArgs e)
        {
            QuestionViewModel.Instance.Question = (Question)e.Item;
            QuestionViewModel.Instance.IsNewQuestion = false;
            //await Navigation.PushAsync(questionDetailPage);
            await Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
        }
    }
}