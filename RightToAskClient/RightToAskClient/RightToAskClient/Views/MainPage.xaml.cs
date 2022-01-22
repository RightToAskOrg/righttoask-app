using System;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class MainPage : ContentPage
	{
		private ReadingContext _readingContext;
		public MainPage ()
		{
			InitializeComponent();

			// TODO Possibly this should go in OnStart instead
			_readingContext = new ReadingContext(); 
		}

		async void OnTop10NowButtonClicked(object sender, EventArgs e)
		{
			_readingContext.TopTen = true;

			var readingPage = new ReadingPage (true, _readingContext);
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
			_readingContext.Filters.SearchKeyword = ((SearchBar)sender).Text;
			OnLaunchKeywordReadingPage();
		}
		
		private void OnKeywordChanged(object sender, TextChangedEventArgs e)
		{
			_readingContext.Filters.SearchKeyword = e.NewTextValue;
		}

		async void OnLaunchKeywordReadingPage()
		{
			var readingPage = new ReadingPage(true, _readingContext);
			await Navigation.PushAsync(readingPage);
		}
		
		async void OnNavigateButtonClicked (object sender, EventArgs e)
		{
			var secondPage = new SecondPage (false, _readingContext);
			await Navigation.PushAsync (secondPage);
		}
		
		async void OnReadButtonClicked(object sender, EventArgs e)
		{
			var secondPage = new SecondPage (true, _readingContext);
			await Navigation.PushAsync(secondPage);
		}

	}
}
