using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
	public class ReadingContext : ObservableObject
	{
		private bool _dontShowFirstTimeReadingPopup;
		public bool DontShowFirstTimeReadingPopup
		{
			get => _dontShowFirstTimeReadingPopup;
			set => SetProperty(ref _dontShowFirstTimeReadingPopup, value);
		}

		public bool ShowHowToPublishPopup { get; set; } = true;

		// public event PropertyChangedEventHandler? PropertyChanged;

        public ReadingContext()
        {
	        // Also initialises signing keys etc.
	        // Consider awaiting. I don't think so, though, because there's no reason everything else should wait for it.
	        // ThisParticipant.Init();
        }
        
		// Things about this user.
		// These selections are made at registration, or at 'complete registration.'
		// public IndividualParticipant ThisParticipant { get; set; } = new IndividualParticipant();

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
