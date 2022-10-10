using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{

    public class SelectableList<T> : INotifyPropertyChanged // where T: Entity
    {
        private IEnumerable<T> _allEntities;


        public IEnumerable<T> AllEntities
        {
            get => _allEntities;
            set
            {
                _allEntities = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<T> _selectedEntities;

        // TODO Consider whether this should raise OnPropertyChanged for
        // specific other data, e.g. PublicAuthoritiesText. 
        public IEnumerable<T> SelectedEntities
        {
            get => _selectedEntities;
            set {
                _selectedEntities = value;
                OnPropertyChanged();
            }
        }
        
        // Constructor.
        // Can either be initialized with a pre-existing list of selected Entities or with none.
        // TODO consider checking whether selectedEntities is a subset of AllEntities.
        public SelectableList(IEnumerable<T> allEntities, IEnumerable<T>? selectedEntities = null)
        {
            _allEntities = allEntities;
            _selectedEntities = selectedEntities ?? new ObservableCollection<T>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
