using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
    public class FilterChoices : INotifyPropertyChanged
    {
        private string _searchKeyword = "";
        private ObservableCollection<MP> _selectedAnsweringMPs = new ObservableCollection<MP>();
        private ObservableCollection<MP> _selectedAnsweringMPsMine = new ObservableCollection<MP>();
        private ObservableCollection<MP> _selectedAskingMPs = new ObservableCollection<MP>();
        private ObservableCollection<MP> _selectedAskingMPsMine = new ObservableCollection<MP>();
        private ObservableCollection<Authority> _selectedAuthorities = new ObservableCollection<Authority>();
        private ObservableCollection<string> _selectedAskingCommittee = new ObservableCollection<string>();
        private ObservableCollection<Person> _selectedAskingUsers = new ObservableCollection<Person>();

        public ObservableCollection<Authority> SelectedAuthorities
        {
            get => _selectedAuthorities;
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
            get => _selectedAskingMPs;
            set
            {
                _selectedAskingMPs = value;
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
            get => _selectedAnsweringMPs;
            set
            {
                _selectedAnsweringMPs = value;
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

        public ObservableCollection<Person> SelectedAskingUsers
        {
            get => _selectedAskingUsers;
            set
            {
                _selectedAskingUsers = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Validate()
        {
            bool isValid = false;
            bool hasInvalidData = false;
            if (SelectedAnsweringMPs.Any())
            {
                foreach (MP mp in SelectedAnsweringMPs)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
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
            if (SelectedAskingMPs.Any())
            {
                foreach (MP mp in SelectedAskingMPs)
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
            if(SelectedAskingUsers != null)
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
