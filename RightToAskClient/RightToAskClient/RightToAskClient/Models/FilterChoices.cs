using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class FilterChoices : INotifyPropertyChanged
	{
		private string searchKeyword;
		private ObservableCollection<Entity> selectedAnsweringMPs;
		private ObservableCollection<Entity> selectedAnsweringMPsMine;
		private ObservableCollection<Entity> selectedAskingMPs;
		private ObservableCollection<Entity> selectedAskingMPsMine;
		private ObservableCollection<Entity> selectedAuthorities;
		private ObservableCollection<string> selectedAskingCommittee;

		public FilterChoices()
		{
			selectedAnsweringMPs = new ObservableCollection<Entity>();
			selectedAskingMPs = new ObservableCollection<Entity>();
			selectedAnsweringMPsMine = new ObservableCollection<Entity>();
			selectedAskingMPsMine = new ObservableCollection<Entity>();
			selectedAuthorities = new ObservableCollection<Entity>();
			selectedAskingCommittee = new ObservableCollection<string>();
			
		}

		public ObservableCollection<Entity> SelectedAuthorities
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

		public ObservableCollection<Entity> SelectedAskingMPsMine
		{
			get { return selectedAskingMPsMine; }
			set
			{
				selectedAskingMPsMine = value;
				OnPropertyChanged("SelectedAskingMPsMine");
			}
		}
		
		public ObservableCollection<Entity> SelectedAskingMPs
		{
			get { return selectedAskingMPs; }
			set
			{
				selectedAskingMPs = value;
				OnPropertyChanged("SelectedAskingMPs");
			}
		}

		

		public ObservableCollection<Entity> SelectedAnsweringMPsMine
		{
			get { return selectedAnsweringMPsMine; }
			set
			{
				selectedAnsweringMPsMine = value;
				OnPropertyChanged("SelectedAnsweringMPsMine");
			}
		}
		public ObservableCollection<Entity> SelectedAnsweringMPs
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
