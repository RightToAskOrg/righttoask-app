using System;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class MainPage 
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
			App.ReadingContext.TopTen = true;
			//await Navigation.PushAsync (readingPage);
			await Shell.Current.GoToAsync($"//{nameof(ReadingPage)}");
		}
		
		private void OnExpiringSoonButtonClicked(object sender, EventArgs e)
		{
			OnTop10NowButtonClicked(sender, e);
		}

		// If either 'enter' is pressed after a keyword change, or the 
		// 'search by keyword' button is pressed, launch the reading page.
		// Otherwise, if only the keyword is changed, update it but don't
		// launch a new page.
		void OnReadByKeywordFieldCompleted(object sender, EventArgs e)
		{
			_readingContext.Filters.SearchKeyword = ((SearchBar)sender).Text;
			LaunchKeywordReadingPage();
		}
		
		private void OnKeywordChanged(object sender, TextChangedEventArgs e)
		{
			_readingContext.Filters.SearchKeyword = e.NewTextValue;
		}

		async void LaunchKeywordReadingPage()
		{
			App.ReadingContext.IsReadingOnly = true;
			//await Navigation.PushAsync(readingPage);
			await Shell.Current.GoToAsync($"//{nameof(ReadingPage)}");// send true for isReadingOnly
		}
		
		async void OnNavigateButtonClicked (object sender, EventArgs e)
		{
			App.ReadingContext.IsReadingOnly = false;
			//await Navigation.PushAsync (secondPage);
			await Shell.Current.GoToAsync($"{nameof(SecondPage)}"); // send false for isReadingOnly
		}
		
		async void OnReadButtonClicked(object sender, EventArgs e)
		{
			App.ReadingContext.IsReadingOnly = true;
			//await Navigation.PushAsync(secondPage);
			await Shell.Current.GoToAsync($"{nameof(SecondPage)}"); // send true for isReadingOnly
		}

	}
}
