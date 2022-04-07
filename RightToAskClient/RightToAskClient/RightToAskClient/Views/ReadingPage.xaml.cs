using System;
using System.Collections.ObjectModel;
using RightToAskClient.Controls;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class ReadingPage : ContentPage
    {
        //private FilterDisplayTableView _ttestableView;
        //private ClickableEntityListView<Authority> _clickableEntityListView;

        // default constructor required for flyout page item
        public ReadingPage()
        {
            InitializeComponent();
            HomeButton.Clicked += HomeButton_Clicked;

            //_ttestableView = new FilterDisplayTableView();
            //WholePage.Children.Insert(1, _ttestableView);
        }

        protected override void OnDisappearing()
        {
            // clear the selected item
            QuestionList.SelectedItem = null;
            base.OnDisappearing();
        }

        private async void HomeButton_Clicked(object sender, EventArgs e)
        {
            string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go home? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
            if (result == "Yes, I'm sure.")
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
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