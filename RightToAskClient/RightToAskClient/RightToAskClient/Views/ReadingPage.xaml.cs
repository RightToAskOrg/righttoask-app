using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    
    public static class ReadingPageExchanger
    {
        public static bool ByQuestionWriter;
    }
    public partial class ReadingPage : ContentPage
    {
        public ReadingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (ReadingPageExchanger.ByQuestionWriter)
            {
                var vm = new ReadingPageViewModel(ReadingPageExchanger.ByQuestionWriter, true);
                BindingContext = vm;
                vm.Title = AppResources.MyQuestionsTitle;
                ReadingPageExchanger.ByQuestionWriter = false;
            }
            
            var vmReadingPage = BindingContext as ReadingPageViewModel;
            FilterButton.BackgroundColor = vmReadingPage.HasFilter
                ? (Color)Application.Current.Resources["Primary"]
                : (Color)Application.Current.Resources["Purple"];
        }
        
        protected override void OnDisappearing()
        {
            // clear the selected item
            QuestionList.SelectedItem = null;
            base.OnDisappearing();
        }
        private void ClearButton_OnClicked(object sender, EventArgs e)
        {
            KeywordEntry.Text = "";
            ClearButton.IsVisible = false;
        }

        private ReadingPageViewModel? GetViewModel()
        {
            return BindingContext as ReadingPageViewModel;
        }

        private void KeywordEntry_OnCompleted(object sender, EventArgs e)
        {
            GetViewModel().RefreshCommand.ExecuteAsync();
        }

        private void KeywordEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int length = e.NewTextValue.Length;
            ClearButton.IsVisible = length > 0;
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            SearchFrame.IsVisible = !SearchFrame.IsVisible;
            if (!SearchFrame.IsVisible)
                KeywordEntry.Unfocus();
            else
                KeywordEntry.Focus();
        }
    }
}