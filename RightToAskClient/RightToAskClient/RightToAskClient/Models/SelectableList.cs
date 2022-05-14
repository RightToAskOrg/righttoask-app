using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RightToAskClient.Views;

namespace RightToAskClient.Models
{

    public class SelectableList<T> where T: Entity
    {
        private IEnumerable<T> _allEntities;


        public IEnumerable<T> AllEntities
        {
            get => _allEntities ;
        }

        private IEnumerable<T> _selectedEntities;

        // TODO Consider whether this should raise PropertyChanged.
        public IEnumerable<T> SelectedEntities
        {
            get => _selectedEntities;
            set {
                _selectedEntities = value;
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
    }
}
