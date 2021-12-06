using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class SecondPage 
	{
		private readonly bool _isReadingOnly;
		private readonly ReadingContext _readingContext;

		public SecondPage(bool isReadingOnly, ReadingContext readingContext)
		{
			
			InitializeComponent ();
			BindingContext = readingContext;
			_readingContext = readingContext;
			_isReadingOnly = isReadingOnly;

			if (isReadingOnly)
			{
				TitleBar.Title = "Find questions";
				QuestionDraftingBox.IsVisible = false;
			}
			else
			{
				TitleBar.Title =  "Direct my question";
				navigateForwardButton.Text = "Next";
			}

		}
		
		void Question_Entered(object sender, EventArgs e)
		{
			_readingContext.DraftQuestion = ((Editor) sender).Text;
		}
		
		// If in read-only mode, initiate a question-reading page.
		// Similarly if my MP is answering.
		// If drafting, load a question-asker page, which will then 
		// lead to a question-reading page.
		//
		// TODO also doesn't do the right thing if you've previously selected
		// someone other than your MP.  In other words, it should enforce exclusivity -
		// either your MP(s) answer it, or an authority or other MP answers it.
		// At the moment this exclusivity is not enforced.
		async void OnNavigateForwardButtonClicked (object sender, EventArgs e)
		{
			var needToFindAsker = _readingContext.Filters.SelectedAnsweringMPs.IsNullOrEmpty();

			if (needToFindAsker)
			{
				var questionAskerPage = new QuestionAskerPage(_readingContext);
				await Navigation.PushAsync(questionAskerPage);
			}
			else
			{
				var readingPage = new ReadingPage(_isReadingOnly, _readingContext);
				await Navigation.PushAsync (readingPage);
			} 
		}
	
		async private void OnOtherPublicAuthorityButtonClicked(object sender, EventArgs e)
		{
			var exploringPageToSearchAuthorities
				= new ExploringPageWithSearch(new ObservableCollection<Entity>(ParliamentData.AllAuthorities),
					new ObservableCollection<Entity>(_readingContext.Filters.SelectedAuthorities),
				"Choose authorities");
			await Navigation.PushAsync(exploringPageToSearchAuthorities);
		}

		// If we already know the electorates (and hence responsible MPs), go
	    // straight to the Explorer page that lists them.
	    // If we don't, go to the page for entering address and finding them.
	    // It will pop back to here.
		async void OnAnsweredByMPButtonClicked(object sender, EventArgs e)
		{
            string message = "These are your MPs.  Select the one(s) who should answer the question";
           	var mpsExploringPage = new ExploringPage(_readingContext.ThisParticipant.MyMPs, _readingContext.Filters.SelectedAnsweringMPsMine, message);
            
            await ListMPsFindFirstIfNotAlreadyKnown(mpsExploringPage);
		}

		/*
		 * Either push the list of selectable MPs directly, or push a registration page,
		 * instructed to push the MPs selection page after.
		 */
		async Task ListMPsFindFirstIfNotAlreadyKnown(ExploringPage mpsExploringPage)
		{
			var thisParticipant = _readingContext.ThisParticipant;
			
			if (! thisParticipant.MPsKnown)
			{
				var registrationPage = new RegisterPage2(thisParticipant, false, true);
				
				await Navigation.PushAsync(registrationPage);
			}
			else
			{
				await Navigation.PushAsync(mpsExploringPage);
			}
		}

		private async void OnAnswerByOtherMPButtonClicked(object sender, EventArgs e)
		{
			var allMPsAsEntities = new ObservableCollection<Entity>(ParliamentData.MPs.AllMPs); 
			ExploringPageWithSearch mpsPage 
				= new ExploringPageWithSearch(allMPsAsEntities, _readingContext.Filters.SelectedAnsweringMPs, "Here is the complete list of MPs");
			await Navigation.PushAsync(mpsPage);
		}
	}
}

