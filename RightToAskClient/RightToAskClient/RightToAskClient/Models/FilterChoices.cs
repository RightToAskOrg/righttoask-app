using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private SelectableList<Committee> _committeeLists
			= new SelectableList<Committee>(new List<Committee>(), new List<Committee>());
			
		public SelectableList<Committee> CommitteeLists
		{
			get => _committeeLists;
		}
		public List<Committee> SelectedCommittees
		{
			get => _committeeLists.SelectedEntities as List<Committee> ?? new List<Committee>();
		}
		
		private SelectableList<Authority> _authorityLists
			= new SelectableList<Authority>(new List<Authority>(), new List<Authority>());
			
		public SelectableList<Authority> AuthorityLists
		{
			get => _authorityLists;
		}
		public List<Authority> SelectedAuthorities
		{
			get => _authorityLists.SelectedEntities as List<Authority> ?? new List<Authority>();
		}
		
		private SelectableList<MP> _answeringMPsListMine 
			= new SelectableList<MP>(new List<MP>(), new List<MP>());
		public SelectableList<MP> AnsweringMPsListsMine
		{
			get => _answeringMPsListMine;
		}
		public List<MP> SelectedAnsweringMPsMine
		{
			get => _answeringMPsListMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				_answeringMPsListMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		private SelectableList<MP> _askingMPsListMine
			= new SelectableList<MP>(new List<MP>(), new List<MP>());
		public SelectableList<MP> AskingMPsListsMine
		{
			get => _askingMPsListMine;
		}
		public List<MP> SelectedAskingMPsMine
		{
			get => _askingMPsListMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				_askingMPsListMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}
		
		private SelectableList<MP> _answeringMPsListNotMine
			= new SelectableList<MP>(new List<MP>(), new List<MP>());
		public SelectableList<MP> AnsweringMPsListsNotMine
		{
			get => _answeringMPsListNotMine;
		}
		public List<MP> SelectedAnsweringMPsNotMine
		{
			get => _answeringMPsListNotMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				_answeringMPsListNotMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		private SelectableList<MP> _askingMPsListNotMine
			= new SelectableList<MP>(new List<MP>(), new List<MP>());
		public SelectableList<MP> AskingMPsListsNotMine
		{
			get => _askingMPsListNotMine;
		}
		public List<MP> SelectedAskingMPsNotMine
		{
			get =>  _askingMPsListNotMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				_askingMPsListNotMine.SelectedEntities = value;
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
	        _answeringMPsListNotMine = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        _askingMPsListNotMine =  new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        _authorityLists = new SelectableList<Authority>(ParliamentData.AllAuthorities, new List<Authority>());
	        _committeeLists =
		        new SelectableList<Committee>(CommitteesAndHearingsData.AllCommittees, new List<Committee>());
        }

        public void RemoveAllSelections()
        {
	        _answeringMPsListNotMine.SelectedEntities = new List<MP>();
	        _answeringMPsListMine.SelectedEntities = new List<MP>();
	        _askingMPsListNotMine.SelectedEntities = new List<MP>();
	        _askingMPsListMine.SelectedEntities = new List<MP>();
	        _authorityLists.SelectedEntities = new List<Authority>();
	        _committeeLists.SelectedEntities = new List<Committee>();
	        _questionWriterLists.SelectedEntities = new List<Person>();
	        SearchKeyword = "";
        }

        // Update the list of my MPs. Note that it's a tiny bit unclear whether we should remove
        // any selected MPs who are (now) not yours. At the moment, I have simply left it as it is,
        // which means that if a person starts drafting a question, then changes their electorate details,
        // it's still possible for the question to go to 'my MP' when that person is their previous MP.
        public void UpdateMyMPLists()
        {
	        _answeringMPsListMine.AllEntities = ParliamentData.FindAllMPsGivenElectorates(App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates.ToList());
	        _askingMPsListMine.AllEntities = _answeringMPsListMine.AllEntities;
        }

        public bool Validate()
        {
            bool isValid = false;
            bool hasInvalidData = false;
            if (SelectedAnsweringMPsMine.Any())
            {
                foreach (MP mp in SelectedAnsweringMPsMine)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAskingMPsMine.Any())
            {
                foreach (MP mp in SelectedAskingMPsMine)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAuthorities.Any())
            {
                foreach (Authority auth in SelectedAuthorities)
                {
                    if (!auth.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedCommittees.Any())
            {
                foreach (Committee com in SelectedCommittees)
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
