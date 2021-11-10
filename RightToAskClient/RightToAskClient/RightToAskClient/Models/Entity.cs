using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

// This class represents an entity such as a person or authority.
// Can be subclassed for a person (add a picture)
// or an authority or committee.
// Also, in future, this can include public keys for signing & decryption.
namespace RightToAskClient.Models
{
    public class Entity : INotifyPropertyChanged
    {
        protected string entityName;
        protected string nickName;
        protected UrlWebViewSource url;
        // TODO: Refactor into a 'public authority' 
        protected string rightToKnowURLSuffix;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
        public string EntityName
        {
            get { return entityName; }
            set
            {
                entityName = value;
                OnPropertyChanged("EntityName");
            }
        }

        // Return the nickname if there is one, otherwise the long name.
        public string ShortestName
        {
            get { return nickName != "" ? nickName : entityName;}
        }
        public string NickName
        {
            get { return nickName; }
            set
            {
                nickName = value;
                OnPropertyChanged("NickName");
            }
        }
        public string RightToKnowURLSuffix
        {
            get { return rightToKnowURLSuffix; }
            set
            {
                rightToKnowURLSuffix = value;
                OnPropertyChanged("RightToKnowURLSuffix");
            }
        }
        public UrlWebViewSource URL
        {
            get { return url; }
            set
            {
                url = value;
                OnPropertyChanged("URL");
            }
        }

        public override string ToString()
        {
            var nickNameIfPresent = String.IsNullOrEmpty(NickName) ? "" : "(" + NickName + ")";
            return EntityName + nickNameIfPresent;
        }
    }
}