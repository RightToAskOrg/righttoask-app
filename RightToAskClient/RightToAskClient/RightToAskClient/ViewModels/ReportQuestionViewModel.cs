using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.Input;

namespace RightToAskClient.ViewModels
{
    public class ReportReason
    {
        public CensorshipReason ID { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public bool Selected { get; set; }
        
        public override string ToString()
        {
            return Title + " (" + Subtitle + ")";
        }   
    }
    public class ReportQuestionViewModel: BaseViewModel
    {
        public IList<ReportReason> ReasonList { get; private set; }
        public IAsyncRelayCommand  BackCommand { get; }
        public IAsyncRelayCommand  ReportCommand { get; }
        
        private QuestionResponseRecords _responseRecords = new QuestionResponseRecords();
        
        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ReportQuestionViewModel()
        {
            BackCommand = new AsyncRelayCommand (async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            ReportCommand = new AsyncRelayCommand (async () =>
            {
                ReportReason? reportReason = null;
                foreach (var v in ReasonList)
                {
                    if (v.Selected)
                    {
                        reportReason = v;
                        break;
                    }
                }
                if (reportReason == null) return;
                bool success = await SendReport(reportReason);
                // Process respond
                if (success)
                {
                    _responseRecords.AddReportedQuestion(_questionID);
                   // var popup = new OneButtonPopup(
                   //     AppResources.ReportTitle,
                   //     AppResources.ReportMessage,
                   //     AppResources.DoneButtonText);
                    await Application.Current.MainPage.DisplayAlert(AppResources.ReportTitle,
                        AppResources.ReportMessage,
                        AppResources.DoneButtonText);
                    _command.Execute(true);
                    await App.Current.MainPage.Navigation.PopAsync(); 
                }
                else
                {
                    //var popup = new OneButtonPopup(
                    //    AppResources.ReportTitleError,
                    //    AppResources.ReportMessageError,
                    //    AppResources.OKText);
                    await Application.Current.MainPage.DisplayAlert(AppResources.ReportTitleError,
                        AppResources.ReportMessageError,
                        AppResources.OKText);
                }

            });

            ReasonList = new List<ReportReason>();
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.TargetedHarassment,
                Title = "Harassment",
                Subtitle = "Aggression against a specific individual or group.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.ThreateningViolence,
                Title = "Threatening violence",
                Subtitle = "",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.EncouragesHarm,
                Title = "Encouraging harm",
                Subtitle = "Encouragement of self-harm or harm to others.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.IncitesHatredOrDiscrimination,
                Title = "Hate",
                Subtitle = "Attacks or threats against an individual or group because of their protected attributes or anything else about them that does not relate to a political issue.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.Spam,
                Title = "Spam",
                Subtitle = "Malicious links, fake engagement, or advertisements.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.IncludesPrivateInformation,
                Title = "Sharing personal information",
                Subtitle = "Sharing other people’s private information without their consent.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.DefamatoryInsinuation,
                Title = "Insulting someone",
                Subtitle = "Accusations or other defamatory statements.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.Impersonation,
                Title = "Impersonation",
                Subtitle = "Claiming to be someone they’re not",
                Selected = false
            });
            
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.Illegal,
                Title = "Illegal",
                Subtitle = "Copyright violation, promotion of criminal activities, money scams etc.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.NotAQuestion,
                Title = "Not a question",
                Subtitle = "",
                Selected = false
            });
        }

        private string _questionID = "";
        private Command _command = null;

        public ReportQuestionViewModel(string questionId, QuestionResponseRecords responseRecords, Command command): this()
        {
            _questionID = questionId;
            _responseRecords = responseRecords;
            _command = command;
        }

        private async Task<bool> SendReport(ReportReason reason)
        {
            var reportQuestion = new ReportQuestionCommand()
            {
                question_id = _questionID,
                reason = reason.ID
            };
            var httpResponse = await RTAClient.SendReportQuestion(reportQuestion,
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid);
            (bool isValid, string errorMessage) = RTAClient.ValidateHttpResponse(httpResponse, "Report question");  
            if(!isValid) 
            {
                var error =  "Error Reporting question: " + errorMessage;
                ReportLabelText = error;
                Debug.WriteLine(error);
                return false;
            }

            return true;
        }
    } 
}