using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RightToAskClient.Maui.Models
{
	public class ReadingContext : ObservableObject
	{
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
