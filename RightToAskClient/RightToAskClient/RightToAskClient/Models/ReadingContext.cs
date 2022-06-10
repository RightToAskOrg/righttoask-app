using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

		public event PropertyChangedEventHandler? PropertyChanged;

        public ReadingContext()
        {
	        //InitializeDefaultQuestions(); // Needed to remove pre-existing questions to prevent a crash with the new server question models setup
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
		// Things about this user.
		// These selections are made at registration, or at 'complete registration.'
		public IndividualParticipant ThisParticipant { get; set; } = new IndividualParticipant();

		// Things about the current search, draft question or other action.
		// TODO: Possibly this needs to use getValue and setValue to that property changes
		// cause UI updates.
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
		
		// Whether this is a 'top ten' search.
		public bool TopTen { get; set; }

		public string GoDirectCommittee { get; set; } = "";

		public string GoDirectMP { get; set; } = "";

		// Existing things about the world.
		public ObservableCollection<Question> ExistingQuestions { get; set; } = new ObservableCollection<Question>();
		
		// TODO This ToString doesn't really properly convey the state of
		// the ReadingContext, e.g. doesn't reflect registering or knowing your
		// MPs.
		// And it probably isn't necessary to write out all the unselected things.
		
		public override string ToString ()
		{
			return "Keyword: " + Filters.SearchKeyword + '\n' +
			       "TopTen: " + TopTen + '\n' +
			       "Direct Committee: " + GoDirectCommittee + '\n' +
			       "Direct MP: " + GoDirectMP + '\n' +
			       "Question: " + DraftQuestion + '\n' ;
		}

	}
}
