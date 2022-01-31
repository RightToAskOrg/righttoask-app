using System;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
    public class Authority : Entity
    {
        private string _authorityName = "";
        private string _nickName = "";
        private UrlWebViewSource _url = new UrlWebViewSource();
        private string _rightToKnowURLSuffix = "";
        public string AuthorityName
        {
            get { return _authorityName; }
            set
            {
                _authorityName = value;
                // Disabled Resharper warning here, because it thinks the argument is
                // redundant but actually we're raising a property-changed for a different
                // property name.
                OnPropertyChanged("EntityName");
            }
        }

        // Return the nickname if there is one, otherwise the long name.
        public override string ShortestName
        {
            get { return _nickName != "" ? _nickName : _authorityName;}
        }
        public string NickName
        {
            get { return _nickName; }
            set
            {
                _nickName = value;
                OnPropertyChanged();
            }
        }
        public string RightToKnowURLSuffix
        {
            get { return _rightToKnowURLSuffix; }
            set
            {
                _rightToKnowURLSuffix = value;
                OnPropertyChanged();
            }
        }
        public UrlWebViewSource URL
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        private string NickNameIfPresent()
        {
            return String.IsNullOrEmpty(NickName) ? "" : " (" + NickName + ")";
        }


        public override string GetName()
        {
            return _authorityName + NickNameIfPresent();
        }

        public override string ToString()
        {
            return AuthorityName + NickNameIfPresent();
        }
    }
}