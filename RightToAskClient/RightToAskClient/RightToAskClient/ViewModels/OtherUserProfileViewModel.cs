using RightToAskClient.Models;
using RightToAskClient.Resx;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class OtherUserProfileViewModel : BaseViewModel
    {
        private string _otherUserUID = "";
        public string OtherUserUID
        {
            get => _otherUserUID;
            set => SetProperty(ref _otherUserUID, value);
        }

        private string _otherUserDisplayName = "";
        public string OtherUserDisplayName
        {
            get => _otherUserDisplayName;
            set => SetProperty(ref _otherUserDisplayName, value);
        }

        private string _otherUserState = "";
        public string OtherUserState
        {
            get => _otherUserState;
            set => SetProperty(ref _otherUserState, value);
        }

        private string _otherUserEmail = "";
        public string OtherUserEmail
        {
            get => _otherUserEmail;
            set => SetProperty(ref _otherUserEmail, value);
        }

        public OtherUserProfileViewModel()
        {
            // initialize defaults in case we don't hit the message subscriber
            OtherUserDisplayName = "Did Not Get User Name";
            OtherUserUID = "Did Not Get UID";
            OtherUserState = "Did Not Get User's State";
            OtherUserEmail = "No Email Found";

            // populate fields based on passsed in user
            MessagingCenter.Subscribe<QuestionViewModel, Person>(this, "OtherUser", (sender, arg) =>
            {
                Person _sentUser = arg;
                if(_sentUser != null)
                {
                    OtherUserUID = _sentUser.RegistrationInfo.uid;
                    OtherUserDisplayName = _sentUser.RegistrationInfo.display_name;
                    OtherUserState = _sentUser.RegistrationInfo.State;
                    OtherUserEmail = _sentUser.UserEmail;
                }
            });

            MessagingCenter.Subscribe<QuestionViewModel, Question>(this, "OtherUserQuestion", (sender, arg) =>
            {
                Question _otherUserQuestion = arg;
                if (_otherUserQuestion != null)
                {
                    OtherUserDisplayName = _otherUserQuestion.QuestionSuggester;
                    Title = string.Format(AppResources.OtherUserProfileTitle, _otherUserQuestion.QuestionSuggester);
                }
            });
        }
    }
}
