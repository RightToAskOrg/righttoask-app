using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
    public class Tag<T> where T:Entity , INotifyPropertyChanged
    {
        // TODO probably Entity should be readonly.
        // TODO Also make generic.
        private T _tagEntity;
        private bool _selected;

        public Tag(T entity, bool selected)
        {
            _tagEntity = entity;
            _selected = selected;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        // This function allows for automatic UI updates when these properties change.
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public T TagEntity
        {
            get
            {
                return _tagEntity;
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        public void Toggle()
        {
            Selected = !Selected;
        }

        public bool NameContains(string query)
        {
            var normalizedQuery = query.ToLower();
            return _tagEntity.GetName().ToLowerInvariant().Contains(normalizedQuery);
        }
        
        public override string ToString ()
        {
            return TagEntity +"\n" + 
                (Selected ? "" : "Not ") +
                "Selected" + "\n";
        }
    }
}