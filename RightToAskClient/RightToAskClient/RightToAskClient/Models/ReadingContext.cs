using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class ReadingContext : INotifyPropertyChanged
	{
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
		
		// At the moment, this simply populates the reading context with a
		// hardcoded set of "existing" questions.
	//	private void InitializeDefaultQuestions()
	//	{
	//		ExistingQuestions.Add(
	//			new Question
	//			{
	//				QuestionText = "What is the error rate of the Senate Scanning solution?",
	//				QuestionSuggester = "Alice",
	//				QuestionAsker = "",
	//				DownVotes = 1,
	//				UpVotes = 2
	//			});
	//		ExistingQuestions.Add(
	//			new Question
	//			{
	//				QuestionText = "What is the monthly payment to AWS for COVIDSafe?",
	//				QuestionSuggester = "Bob",
	//				QuestionAsker = "",
	//				DownVotes = 3,
	//				UpVotes = 1
	//			});
	//		ExistingQuestions.Add(
	//			new Question
	//			{
	//				QuestionText =
	//					"Why did the ABC decide against an opt-in consent model for data sharing with Facebook and Google?",
	//				QuestionSuggester = "Chloe",
	//				QuestionAsker = "",
	//				DownVotes = 1,
	//				UpVotes = 2
	//			});
	//		ExistingQuestions.Add(
	//			new Question
	//			{
	//				QuestionText =
	//					"What is the government's position on the right of school children to strike for climate?",
	//				QuestionSuggester = "Darius",
	//				DownVotes = 1,
	//				UpVotes = 2
	//			});
	//}


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
