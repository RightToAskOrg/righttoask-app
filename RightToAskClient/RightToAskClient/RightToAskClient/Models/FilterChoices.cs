using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class FilterChoices : INotifyPropertyChanged
	{
		private string _searchKeyword = "";
		// private ObservableCollection<MP> _selectedAnsweringMPs = new ObservableCollection<MP>();
		// private ObservableCollection<MP> _selectedAnsweringMPsMine = new ObservableCollection<MP>();
		// private ObservableCollection<MP> _selectedAskingMPs = new ObservableCollection<MP>();
		// private ObservableCollection<MP> _selectedAskingMPsMine = new ObservableCollection<MP>();
		// private ObservableCollection<Authority> _selectedAuthorities = new ObservableCollection<Authority>();
		// private ObservableCollection<string> _selectedAskingCommittee = new ObservableCollection<string>();
		//
		//private ObservableCollection<Person?> _selectedAskingUsers = new ObservableCollection<Person?>();
		private SelectableList<Person> _questionWriterLists
			= new SelectableList<Person>(new List<Person>(), new List<Person>());

		// The writer of questions, for pages where we want to see all the questions written by a certain person.
		// Needs a public setter because the list of options is taken from what the user searches for.
		public SelectableList<Person> QuestionWriterLists
		{
			get => _questionWriterLists;
			set
			{
				_questionWriterLists = value;
				OnPropertyChanged();
			}
		}
		
		// Express each FilterChoice as a pair of lists: the whole list from which things are seleced,
		// and the list of selections.

		public SelectableList<Committee> CommitteeLists { get; private set; } = new SelectableList<Committee>(new List<Committee>(), new List<Committee>());

		public List<Committee> SelectedCommittees => CommitteeLists.SelectedEntities as List<Committee> ?? new List<Committee>();

		public SelectableList<Authority> AuthorityLists { get; private set; } = new SelectableList<Authority>(new List<Authority>(), new List<Authority>());

		public List<Authority> SelectedAuthorities => AuthorityLists.SelectedEntities as List<Authority> ?? new List<Authority>();

		public SelectableList<MP> AnsweringMPsListsMine { get; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAnsweringMPsMine
		{
			get => AnsweringMPsListsMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AnsweringMPsListsMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public SelectableList<MP> AskingMPsListsMine { get; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAskingMPsMine
		{
			get => AskingMPsListsMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AskingMPsListsMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public SelectableList<MP> AnsweringMPsListsNotMine { get; private set; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAnsweringMPsNotMine
		{
			get => AnsweringMPsListsNotMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AnsweringMPsListsNotMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public SelectableList<MP> AskingMPsListsNotMine { get; private set; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAskingMPsNotMine
		{
			get =>  AskingMPsListsNotMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AskingMPsListsNotMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public string SearchKeyword
		{
			get => _searchKeyword;
			set
			{
				_searchKeyword = value;
				OnPropertyChanged();
			}
		}


		/* We have one instance of FilterChoices in the (static) reading context,
		 * for which init must be redone explicitly after details such as MPs, Committees
		 * etc are read. The other instances are related to specific questions as they are
		 * downloaded - in these cases, the Init in the constructor is all that's needed
		 * because the other information is already set up.
		 */
		public FilterChoices()
		{
			InitSelectableLists();
		}
		public event PropertyChangedEventHandler? PropertyChanged;
        
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // This is necessary on startup because of the need to update the AllMPs list.
        // It's still helpful for them to use the constructor because if this data structure
        // is initialized after the MP read-in, the constructor should suffice.
        public void InitSelectableLists()
        {
	        // Note: No init for question writers, because it needs to be initialised when the user searches for names.
	        AnsweringMPsListsNotMine = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        AskingMPsListsNotMine =  new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        AuthorityLists = new SelectableList<Authority>(ParliamentData.AllAuthorities, new List<Authority>());
	        CommitteeLists =
		        new SelectableList<Committee>(CommitteesAndHearingsData.AllCommittees, new List<Committee>());
        }

        public void RemoveAllSelections()
        {
	        AnsweringMPsListsNotMine.SelectedEntities = new List<MP>();
	        AnsweringMPsListsMine.SelectedEntities = new List<MP>();
	        AskingMPsListsNotMine.SelectedEntities = new List<MP>();
	        AskingMPsListsMine.SelectedEntities = new List<MP>();
	        AuthorityLists.SelectedEntities = new List<Authority>();
	        CommitteeLists.SelectedEntities = new List<Committee>();
	        _questionWriterLists.SelectedEntities = new List<Person>();
	        SearchKeyword = "";
        }

        // Update the list of my MPs. Note that it's a tiny bit unclear whether we should remove
        // any selected MPs who are (now) not yours. At the moment, I have simply left it as it is,
        // which means that if a person starts drafting a question, then changes their electorate details,
        // it's still possible for the question to go to 'my MP' when that person is their previous MP.
        public void UpdateMyMPLists()
        {
	        AnsweringMPsListsMine.AllEntities = ParliamentData.FindAllMPsGivenElectorates(IndividualParticipant.ProfileData.RegistrationInfo.Electorates.ToList());
	        AskingMPsListsMine.AllEntities = AnsweringMPsListsMine.AllEntities;
        }

        public bool Validate()
        {
            var isValid = false;
            var hasInvalidData = false;
            if (SelectedAnsweringMPsMine.Any())
            {
                foreach (var mp in SelectedAnsweringMPsMine)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAskingMPsMine.Any())
            {
                foreach (var mp in SelectedAskingMPsMine)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAuthorities.Any())
            {
                foreach (var auth in SelectedAuthorities)
                {
                    if (!auth.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedCommittees.Any())
            {
                foreach (var com in SelectedCommittees)
                {
                    if (string.IsNullOrEmpty(com.ShortestName))
                    {
                        hasInvalidData = true;
                    }
                }
            }
            
            isValid = !hasInvalidData;
            return isValid;
        }
    }
}
