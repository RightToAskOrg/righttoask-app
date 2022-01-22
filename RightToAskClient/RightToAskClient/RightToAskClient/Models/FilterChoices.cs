using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class FilterChoices : INotifyPropertyChanged
	{
		private string _searchKeyword;
		private ObservableCollection<MP> _selectedAnsweringMPs;
		private ObservableCollection<MP> _selectedAnsweringMPsMine;
		private ObservableCollection<MP> _selectedAskingMPs;
		private ObservableCollection<MP> _selectedAskingMPsMine;
		private ObservableCollection<Authority> _selectedAuthorities;
		private ObservableCollection<string> _selectedAskingCommittee;
		private ObservableCollection<Entity> _selectedAskingUsers;

		public FilterChoices()
		{
			_selectedAnsweringMPs = new ObservableCollection<MP>();
			_selectedAskingMPs = new ObservableCollection<MP>();
			_selectedAnsweringMPsMine = new ObservableCollection<MP>();
			_selectedAskingMPsMine = new ObservableCollection<MP>();
			_selectedAuthorities = new ObservableCollection<Authority>();
			_selectedAskingCommittee = new ObservableCollection<string>();
			_selectedAskingUsers = new ObservableCollection<Entity>();
			
		}

		public ObservableCollection<Authority> SelectedAuthorities
		{
			get { return _selectedAuthorities; }
			set
			{
				_selectedAuthorities = value;
				OnPropertyChanged("SelectedAuthorities");
			}
		}

		public string SearchKeyword
		{
			get { return _searchKeyword; }
			set
			{
				_searchKeyword = value;
				OnPropertyChanged("SearchKeyword");
			}
		}

		public ObservableCollection<MP> SelectedAskingMPsMine
		{
			get { return _selectedAskingMPsMine; }
			set
			{
				_selectedAskingMPsMine = value;
				OnPropertyChanged("SelectedAskingMPsMine");
			}
		}
		
		public ObservableCollection<MP> SelectedAskingMPs
		{
			get { return _selectedAskingMPs; }
			set
			{
				_selectedAskingMPs = value;
				OnPropertyChanged("SelectedAskingMPs");
			}
		}

		

		public ObservableCollection<MP> SelectedAnsweringMPsMine
		{
			get { return _selectedAnsweringMPsMine; }
			set
			{
				_selectedAnsweringMPsMine = value;
				OnPropertyChanged("SelectedAnsweringMPsMine");
			}
		}
		public ObservableCollection<MP> SelectedAnsweringMPs
		{
			get { return _selectedAnsweringMPs; }
			set
			{
				_selectedAnsweringMPs = value;
				OnPropertyChanged("SelectedAnsweringMPs");
			}
		}

		public ObservableCollection<string> SelectedAskingCommittee
		{
			get { return _selectedAskingCommittee; }
			set
			{
				_selectedAskingCommittee = value;
				OnPropertyChanged("SelectedAskingCommittee");
			}
		}

		public ObservableCollection<Entity> SelectedAskingUsers
		{
			get { return _selectedAskingUsers; }
			set
			{
				_selectedAskingUsers = value;
				OnPropertyChanged("SelectedAskingUsers");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
        
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
	}
}
