using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
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
            Title = "Other User's Profile";
            OtherUserDisplayName = "Did Not Get User Name";
            OtherUserUID = "Did Not Get UID";
            OtherUserState = "Did Not Get User's State";
            OtherUserEmail = "No Email Found";

            // populate fields based on passsed in user
            MessagingCenter.Subscribe<QuestionViewModel, ServerUser>(this, "OtherUser", (sender, arg) =>
            {
                ServerUser _sentUser = arg;
                if (_sentUser != null)
                {
                    Title = _sentUser.display_name + "'s Profile";
                    OtherUserUID = _sentUser.uid ?? "No UID Found";
                    OtherUserDisplayName = _sentUser.display_name ?? "No Display Name Found";
                    OtherUserState = _sentUser.state ?? "No State Found";
                    OtherUserEmail = "No Email Available";
                }
                MessagingCenter.Unsubscribe<QuestionViewModel, ServerUser>(this, "OtherUser");
            });

            //MessagingCenter.Subscribe<QuestionViewModel, Question>(this, "OtherUserQuestion", (sender, arg) =>
            //{
            //    Question _otherUserQuestion = arg;
            //    if (_otherUserQuestion != null)
            //    {
            //        OtherUserDisplayName = _otherUserQuestion.QuestionSuggester;
            //        Title = string.Format(AppResources.OtherUserProfileTitle, _otherUserQuestion.QuestionSuggester);
            //    }
            //});
        }
    }
}
