using System;
using System.Collections.ObjectModel;
using RightToAskClient.Controls;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class ReadingPage : ContentPage
	{
		private string _draftQuestion;
		private readonly ReadingContext _readingContext;
		private FilterDisplayTableView _ttestableView;
		private ClickableEntityListView<Authority> _clickableEntityListView;

		public ReadingPage(bool isReadingOnly, ReadingContext readingContext)
		{
			InitializeComponent();
			_readingContext = readingContext;
			BindingContext = readingContext;

            _ttestableView = new FilterDisplayTableView(readingContext);
            WholePage.Children.Insert(1,_ttestableView);
            //
            /*
            _clickableEntityListView = new ClickableEntityListView<Authority>
            {
	            ClickableListLabel = "What should we do with this list?",
	            ClickableListContents = _readingContext.Filters.SelectedAuthorities,
	            UpdateAction = OnMoreButtonClicked
            };
            WholePage.Children.Insert(1,_clickableEntityListView);
            */
            
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
			_draftQuestion = ((Editor) sender).Text;
			((ReadingContext) BindingContext).DraftQuestion = _draftQuestion;
		}

		// Note: it's possible that this would be better with an ItemTapped event instead.
		private async void Question_Selected(object sender, ItemTappedEventArgs e)
		{
			var questionDetailPage 
				= new QuestionDetailPage(false, (Question) e.Item, _readingContext);
			await Navigation.PushAsync(questionDetailPage);
		}

		async void OnDiscardButtonClicked(object sender, EventArgs e)
		{
            _readingContext.DraftQuestion = null;
            DraftEditor.IsVisible = false;
            DiscardButton.IsVisible = false; 
			KeepButton.IsVisible = false;
			
            bool goHome = await DisplayAlert("Draft discarded", "Save time and focus support by voting on a similar question", "Home", "Related questions");
            if (goHome)
            {
                await Navigation.PopToRootAsync();
            }

		}


		async void OnSaveButtonClicked(object sender, EventArgs e)
		{
			// Tag the new question with the authorities that have been selected.
			// ObservableCollection<Entity> questionAnswerers;
			var questionAnswerers =
				new ObservableCollection<Entity>(_readingContext.Filters.SelectedAuthorities);

			foreach (var answeringMP in _readingContext.Filters.SelectedAnsweringMPs)
			{
				questionAnswerers.Add(answeringMP);	
			}

			IndividualParticipant thisParticipant = _readingContext.ThisParticipant;
			
			Question newQuestion = new Question
			{
				QuestionText = _readingContext.DraftQuestion,
				// TODO: Enforce registration before question-suggesting.
				QuestionSuggester 
					= (thisParticipant.IsRegistered) ? thisParticipant.RegistrationInfo.display_name : "Anonymous user",
				QuestionAnswerers = questionAnswerers,
				DownVotes = 0,
				UpVotes = 0
			};


			var questionDetailPage = new QuestionDetailPage(true, newQuestion, _readingContext);
			await Navigation.PushAsync(questionDetailPage);
		}
	}
}