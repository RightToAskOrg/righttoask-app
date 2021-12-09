using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class FilterChoices : INotifyPropertyChanged
	{
		private string searchKeyword;
		private ObservableCollection<MP> selectedAnsweringMPs;
		private ObservableCollection<MP> selectedAnsweringMPsMine;
		private ObservableCollection<MP> selectedAskingMPs;
		private ObservableCollection<MP> selectedAskingMPsMine;
		private ObservableCollection<Authority> selectedAuthorities;
		private ObservableCollection<string> selectedAskingCommittee;
		private ObservableCollection<Entity> selectedAskingUsers;

		public FilterChoices()
		{
			selectedAnsweringMPs = new ObservableCollection<MP>();
			selectedAskingMPs = new ObservableCollection<MP>();
			selectedAnsweringMPsMine = new ObservableCollection<MP>();
			selectedAskingMPsMine = new ObservableCollection<MP>();
			selectedAuthorities = new ObservableCollection<Authority>();
			selectedAskingCommittee = new ObservableCollection<string>();
			selectedAskingUsers = new ObservableCollection<Entity>();
			
		}

		public ObservableCollection<Authority> SelectedAuthorities
		{
			get { return selectedAuthorities; }
			set
			{
				selectedAuthorities = value;
				OnPropertyChanged("SelectedAuthorities");
			}
		}

		public string SearchKeyword
		{
			get { return searchKeyword; }
			set
			{
				searchKeyword = value;
				OnPropertyChanged("SearchKeyword");
			}
		}

		public ObservableCollection<MP> SelectedAskingMPsMine
		{
			get { return selectedAskingMPsMine; }
			set
			{
				selectedAskingMPsMine = value;
				OnPropertyChanged("SelectedAskingMPsMine");
			}
		}
		
		public ObservableCollection<MP> SelectedAskingMPs
		{
			get { return selectedAskingMPs; }
			set
			{
				selectedAskingMPs = value;
				OnPropertyChanged("SelectedAskingMPs");
			}
		}

		

		public ObservableCollection<MP> SelectedAnsweringMPsMine
		{
			get { return selectedAnsweringMPsMine; }
			set
			{
				selectedAnsweringMPsMine = value;
				OnPropertyChanged("SelectedAnsweringMPsMine");
			}
		}
		public ObservableCollection<MP> SelectedAnsweringMPs
		{
			get { return selectedAnsweringMPs; }
			set
			{
				selectedAnsweringMPs = value;
				OnPropertyChanged("SelectedAnsweringMPs");
			}
		}

		public ObservableCollection<string> SelectedAskingCommittee
		{
			get { return selectedAskingCommittee; }
			set
			{
				selectedAskingCommittee = value;
				OnPropertyChanged("SelectedAskingCommittee");
			}
		}

		public ObservableCollection<Entity> SelectedAskingUsers
		{
			get { return selectedAskingUsers; }
			set
			{
				selectedAskingUsers = value;
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
