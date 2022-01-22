using System.ComponentModel;
using System.Runtime.CompilerServices;

// This class represents an entity such as a person or authority.
// Can be subclassed for a person (add a picture)
// or an authority or committee.
// Also, in future, this can include public keys for signing & decryption.
namespace RightToAskClient.Models
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public abstract string GetName();
        public Tag<Entity> WrapInTag(bool selected = false)
        {
            return new Tag<Entity>(this, selected);
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}