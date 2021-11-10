using System;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class MainPage : ContentPage
	{
		private ReadingContext readingContext;
		public MainPage ()
		{
			InitializeComponent();

			// TODO Possibly this should go in OnStart instead
			readingContext = new ReadingContext(); 
		}

		async void OnTop10NowButtonClicked(object sender, EventArgs e)
		{
			readingContext.TopTen = true;

			var readingPage = new ReadingPage (true, readingContext);
			await Navigation.PushAsync (readingPage);
		}
		
		private void OnExpiringSoonButtonClicked(object sender, EventArgs e)
		{
			OnTop10NowButtonClicked(sender, e);
		}

		// If either 'enter' is pressed after a keyword change, or the 
		// 'search by keyword' button is pressed, launch the reading page.
		// Otherwise, if only the keyword is changed, update it but don't
		// launch a new page.
		async void OnReadByKeywordFieldCompleted(object sender, EventArgs e)
		{
			readingContext.Filters.SearchKeyword = ((SearchBar)sender).Text;
			launchKeywordReadingPage();
		}
		
		private void OnKeywordChanged(object sender, TextChangedEventArgs e)
		{
			readingContext.Filters.SearchKeyword = e.NewTextValue;
		}

		async void launchKeywordReadingPage()
		{
			var readingPage = new ReadingPage(true, readingContext);
			await Navigation.PushAsync(readingPage);
		}
		
		async void OnNavigateButtonClicked (object sender, EventArgs e)
		{
			var secondPage = new SecondPage (false, readingContext);
			await Navigation.PushAsync (secondPage);
		}
		
		async void OnReadButtonClicked(object sender, EventArgs e)
		{
			var secondPage = new SecondPage (true, readingContext);
			await Navigation.PushAsync(secondPage);
		}

	}
}
