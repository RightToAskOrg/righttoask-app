using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
	public class ReadingContext : ObservableObject
	{
		// Used for determining if the user is in the process of trying to draft a question, or if they are browsing on the reading page.
		// The reading page can be accessed from various ways from the main page (with some filters already filled in)
		// Or from within the question drafting flow for finding a similar question to upvote instead of continuing with the drafted question for submission.
		private bool _isReadingOnly;
		public bool IsReadingOnly 
		{ 
			get => _isReadingOnly;
			set
			{
				_isReadingOnly = value;
				OnPropertyChanged();
			}
		}

		private bool _dontShowFirstTimeReadingPopup = false;
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
	        ThisParticipant.Init();
        }
        
		// Things about this user.
		// These selections are made at registration, or at 'complete registration.'
		public IndividualParticipant ThisParticipant { get; set; } = new IndividualParticipant();

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
		public string DraftQuestion { get; set; } = "";
		
		// Whether this is a 'trending now' search.
		public bool TrendingNow { get; set; }

		public string GoDirectCommittee { get; set; } = "";

		public string GoDirectMP { get; set; } = "";

		// Existing things about the world.
		public ObservableCollection<Question> ExistingQuestions { get; set; } = new ObservableCollection<Question>();
	}
}
