using System;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
    public class Authority : Entity
    {
        protected string authorityName;
        protected string nickName;
        protected UrlWebViewSource url;
        protected string rightToKnowURLSuffix;
        public string AuthorityName
        {
            get { return authorityName; }
            set
            {
                authorityName = value;
                OnPropertyChanged("EntityName");
            }
        }

        // Return the nickname if there is one, otherwise the long name.
        public string ShortestName
        {
            get { return nickName != "" ? nickName : authorityName;}
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

        private string NickNameIfPresent()
        {
            return String.IsNullOrEmpty(NickName) ? "" : " (" + NickName + ")";
        }


        public override string GetName()
        {
            return authorityName + NickNameIfPresent();
        }

        public override string ToString()
        {
            return AuthorityName + NickNameIfPresent();
        }
    }
}