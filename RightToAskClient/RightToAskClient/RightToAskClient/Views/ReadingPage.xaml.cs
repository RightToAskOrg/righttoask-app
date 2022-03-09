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
        private FilterDisplayTableView _ttestableView;
        //private ClickableEntityListView<Authority> _clickableEntityListView;

        // default constructor required for flyout page item
        public ReadingPage()
        {
            InitializeComponent();
            BindingContext = App.ReadingContext;
            HomeButton.Clicked += HomeButton_Clicked;

            _ttestableView = new FilterDisplayTableView();
            WholePage.Children.Insert(1, _ttestableView);

            OnHideFilters();

            if (App.ReadingContext.IsReadingOnly)
            {
                TitleBar.Title = AppResources.ReadQuestionsTitle;
                QuestionDraftingBox.IsVisible = false;
                KeepButton.IsVisible = false;
                DiscardButton.IsVisible = false;
            }
            else
            {
                TitleBar.Title = AppResources.SimilarQuestionsTitle;
            }
        }

        /*
		public ReadingPage(bool isReadingOnly)
		{
			InitializeComponent();
			BindingContext = App.ReadingContext;

            _ttestableView = new FilterDisplayTableView();
            WholePage.Children.Insert(1,_ttestableView);
            //
            _clickableEntityListView = new ClickableEntityListView<Authority>
            {
	            ClickableListLabel = "What should we do with this list?",
	            ClickableListContents = App.ReadingContext.Filters.SelectedAuthorities,
	            UpdateAction = OnMoreButtonClicked
            };
            WholePage.Children.Insert(1,_clickableEntityListView);
            
            OnHideFilters();
            
			if (isReadingOnly)
			{
				TitleBar.Title = "Read Questions";
				QuestionDraftingBox.IsVisible = false;
				KeepButton.IsVisible = false;
				DiscardButton.IsVisible = false;
			}
			else
			{
				TitleBar.Title = "Similar questions";
			}
		}*/

        private async void HomeButton_Clicked(object sender, EventArgs e)
        {
            string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go home? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
            if (result == "Yes, I'm sure.")
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        private void OnShowFilters(object sender, EventArgs e)
        {
            _ttestableView.IsVisible = true;
            FilterShower.IsVisible = false;
        }

        private void OnHideFilters()
        {
            _ttestableView.IsVisible = false;
            FilterShower.IsVisible = true;
        }

        private void Questions_Scrolled(object sender, ScrolledEventArgs e)
        {
            OnHideFilters();
        }
        void Question_Entered(object sender, EventArgs e)
        {
            App.ReadingContext.DraftQuestion = ((Editor)sender).Text;
            QuestionViewModel.Instance.Question.QuestionText = ((Editor)sender).Text;
        }

        // Note: it's possible that this would be better with an ItemTapped event instead.
        private async void Question_Selected(object sender, ItemTappedEventArgs e)
        {
            QuestionViewModel.Instance.Question = (Question)e.Item;
            QuestionViewModel.Instance.IsNewQuestion = false;
            //await Navigation.PushAsync(questionDetailPage);
            await Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
        }

        async void OnDiscardButtonClicked(object sender, EventArgs e)
        {
            App.ReadingContext.DraftQuestion = "";
            DraftEditor.IsVisible = false;
            DiscardButton.IsVisible = false;
            KeepButton.IsVisible = false;

            bool goHome = await DisplayAlert("Draft discarded", "Save time and focus support by voting on a similar question", "Home", "Related questions");
            if (goHome)
            {
                await Navigation.PopToRootAsync();
                //await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
            }
        }


        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            // Tag the new question with the authorities that have been selected.
            // ObservableCollection<Entity> questionAnswerers;
            var questionAnswerers =
                new ObservableCollection<Entity>(App.ReadingContext.Filters.SelectedAuthorities);

            foreach (var answeringMP in App.ReadingContext.Filters.SelectedAnsweringMPs)
            {
                questionAnswerers.Add(answeringMP);
            }

            IndividualParticipant thisParticipant = App.ReadingContext.ThisParticipant;

            Question newQuestion = new Question
            {
                QuestionText = App.ReadingContext.DraftQuestion,
                // TODO: Enforce registration before question-suggesting.
                QuestionSuggester = Preferences.Get("DisplayName", "Anonymous user"),
                QuestionAnswerers = questionAnswerers,
                DownVotes = 0,
                UpVotes = 0
            };

            QuestionViewModel.Instance.Question = newQuestion;
            QuestionViewModel.Instance.IsNewQuestion = true;
            //await Navigation.PushAsync(questionDetailPage);
            await Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
        }
    }
}