using System;
using System.Diagnostics;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
    public class Authority : Entity, IEquatable<Authority>
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

        // Constructors
        // TODO Look up the authority's other details. 
        // Consider how to deal with the problem that org name is null.
        public Authority(string authority)
        {
            Debug.Assert(authority != null, nameof(authority) + " != null");
            AuthorityName = authority ?? "";
        }

        public Authority() { }
        private string NickNameIfPresent()
        {
            return String.IsNullOrEmpty(NickName) ? "" : " (" + NickName + ")";
        }

        public override string GetName()
        {
            return _authorityName + NickNameIfPresent();
        }

        // Equality is satisfied if the names are equal
        // TODO we may want to edit with server to consider whether a level/jurisdiction should be considered too, 
        // e.g. "department of health" "vic" or something.
        public bool Equals(Authority other)
        {
            return _authorityName == other.AuthorityName;
        }

        public override bool DataEquals(object other)
        {
            var auth = other as Authority;
            return (auth != null) && _authorityName == auth.AuthorityName;
        }

        public override string ToString()
        {
            return AuthorityName + NickNameIfPresent();
        }

        public bool Validate()
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(AuthorityName))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}