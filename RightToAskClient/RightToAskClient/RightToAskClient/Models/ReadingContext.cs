using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
	public class ReadingContext : ObservableObject
	{
		public bool ShowHowToPublishPopup { get; set; } = true;

		private FilterChoices _filters = new FilterChoices();

		public FilterChoices Filters
		{
			get => _filters;
			set
			{
				_filters = value;
				OnPropertyChanged();
			}
		} 
	}
}
