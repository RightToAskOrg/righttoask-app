using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
    public class Tag<T> where T:Entity , INotifyPropertyChanged
    {
        // TODO probably Entity should be readonly.
        // TODO Also make generic.
        private T tagEntity;
        private bool selected;
        public event PropertyChangedEventHandler PropertyChanged;
        
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
                return tagEntity;
            }
            set
            {
                tagEntity = value;
                OnPropertyChanged("TagEntity");
            }
        }
        
        public bool Selected 
        {
                    get
                    {
                        return selected;
                    }
                    set
                    {
                        selected = value;
                        OnPropertyChanged("Selected");
                    }
                }
        public override string ToString ()
        {
            return TagEntity.ToString() +"\n" + 
                (Selected ? "" : "Not ") +
                "Selected" + "\n";
        }
    }
}