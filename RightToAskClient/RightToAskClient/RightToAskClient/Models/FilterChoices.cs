using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class FilterChoices : INotifyPropertyChanged
	{
		private string _searchKeyword = "";
		// private ObservableCollection<MP> _selectedAnsweringMPs = new ObservableCollection<MP>();
		private ObservableCollection<MP> _selectedAnsweringMPsMine = new ObservableCollection<MP>();
		// private ObservableCollection<MP> _selectedAskingMPs = new ObservableCollection<MP>();
		private ObservableCollection<MP> _selectedAskingMPsMine = new ObservableCollection<MP>();
		// private ObservableCollection<Authority> _selectedAuthorities = new ObservableCollection<Authority>();
		private ObservableCollection<string> _selectedAskingCommittee = new ObservableCollection<string>();
		private ObservableCollection<Person?> _selectedAskingUsers = new ObservableCollection<Person?>();

		// Express each FilterChoice as a pair of lists: the whole list from which things are seleced,
		// and the list of selections.
		private SelectableList<Authority> _authorityLists;
			// = new SelectableList<Authority>(ParliamentData.AllAuthorities, new List<Authority>());

		public SelectableList<Authority> AuthorityLists
		{
			get => _authorityLists;
		}
		
		private SelectableList<MP> _answeringMPsListNotMine;
			// = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());

		public SelectableList<MP> OtherMPAnsweringLists
		{
			get => _answeringMPsListNotMine;
		}

		private SelectableList<MP> _askingMPsListNotMine;
			// = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());

		public SelectableList<MP> OtherMPAskingLists
		{
			get => _askingMPsListNotMine;
		}

		// TODO (for Matt): do likewise with MPs lists.
		// The 'other-MP' lists should be settable at initialization.
		// The MyMPs lists might need to wait until registration.

		public ObservableCollection<Authority> SelectedAuthorities
		{
			get => new ObservableCollection<Authority>(_authorityLists.SelectedEntities);
			set => _authorityLists.SelectedEntities = value;
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

		public ObservableCollection<MP> SelectedAskingMPsMine
		{
			get => _selectedAskingMPsMine;
			set
			{
				_selectedAskingMPsMine = value;
				OnPropertyChanged();
			}
		}
		
		public ObservableCollection<MP> SelectedAskingMPs
		{
			get =>  new ObservableCollection<MP>(_askingMPsListNotMine.SelectedEntities);
			set
			{
				_askingMPsListNotMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		

		public ObservableCollection<MP> SelectedAnsweringMPsMine
		{
			get => _selectedAnsweringMPsMine;
			set
			{
				_selectedAnsweringMPsMine = value;
				OnPropertyChanged();
			}
		}
		public ObservableCollection<MP> SelectedAnsweringMPs
		{
			get => new ObservableCollection<MP>(_answeringMPsListNotMine.SelectedEntities);
			set
			{
				_answeringMPsListNotMine.SelectedEntities = value;
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
        }
	}
}
