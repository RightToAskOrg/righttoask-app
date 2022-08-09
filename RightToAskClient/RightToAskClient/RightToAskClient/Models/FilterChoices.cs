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
		private ObservableCollection<string> _selectedAskingCommittee = new ObservableCollection<string>();
		private ObservableCollection<Person?> _selectedAskingUsers = new ObservableCollection<Person?>();

		// Express each FilterChoice as a pair of lists: the whole list from which things are seleced,
		// and the list of selections.
		private SelectableList<Authority> _authorityLists
			= new SelectableList<Authority>(new List<Authority>(), new List<Authority>());
			
		public SelectableList<Authority> AuthorityLists
		{
			get => _authorityLists;
		}
		public List<Authority> SelectedAuthorities
		{
			get => _authorityLists.SelectedEntities as List<Authority> ?? new List<Authority>();
			set => _authorityLists.SelectedEntities = value;
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

        public ObservableCollection<string> SelectedAskingCommittee
        {
            get => _selectedAskingCommittee;
            set
            {
                _selectedAskingCommittee = value;
                OnPropertyChanged();
            }
        }

		public ObservableCollection<Person?> SelectedAskingUsers
		{
			get => _selectedAskingUsers;
			set
			{
				_selectedAskingUsers = value;
				OnPropertyChanged();
			}
		}

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
        // TODO add code to update all Committees too.
        public void InitSelectableLists()
        {
	        _answeringMPsListNotMine = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        _askingMPsListNotMine =  new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        _authorityLists = new SelectableList<Authority>(ParliamentData.AllAuthorities, new List<Authority>());
	        // Doing this at init causes a nullpointer exception because App isn't loaded yet.
	        // UpdateMyMPLists();
        }

        public void RemoveAllSelections()
        {
	        _answeringMPsListNotMine.SelectedEntities = new List<MP>();
	        _answeringMPsListMine.SelectedEntities = new List<MP>();
	        _askingMPsListNotMine.SelectedEntities = new List<MP>();
	        _askingMPsListMine.SelectedEntities = new List<MP>();
	        _authorityLists.SelectedEntities = new List<Authority>();
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
            //if (SelectedAnsweringMPs.Any())
            //{
            //    foreach (MP mp in SelectedAnsweringMPs)
            //    {
            //        if (!mp.Validate())
            //        {
            //            hasInvalidData = true;
            //        }
            //    }
            //}
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
            //if (SelectedAskingMPs.Any())
            //{
            //    foreach (MP mp in SelectedAskingMPs)
            //    {
            //        if (!mp.Validate())
            //        {
            //            hasInvalidData = true;
            //        }
            //    }
            //}
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
            if (SelectedAskingCommittee.Any())
            {
                foreach (string com in SelectedAskingCommittee)
                {
                    if (string.IsNullOrEmpty(com))
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAskingUsers != null)
            {
                if (SelectedAskingUsers.Any())
                {
                    for (int i = 0; i < SelectedAskingUsers.Count-1; i++)
                    {
                        if (SelectedAskingUsers[i] != null)
                        {
                            bool temp = SelectedAskingUsers[i].Validate();
                            if (SelectedAskingUsers[i].Validate())
                            {
                                hasInvalidData = true;
                            }
                        }
                    }
                }
            }
            isValid = !hasInvalidData;
            return isValid;
        }
    }
}
