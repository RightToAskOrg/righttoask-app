using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

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
        public IAsyncCommand BackCommand { get; }
        public IAsyncCommand ReportCommand { get; }
        
        private QuestionResponseRecords _responseRecords = new QuestionResponseRecords();
        
        public ReportQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            ReportCommand = new AsyncCommand(async () =>
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
                    var popup = new OneButtonPopup(
                        AppResources.ReportTitle,
                        AppResources.ReportMessage,
                        AppResources.DoneButtonText);
                    _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    _command.Execute(true);
                    await App.Current.MainPage.Navigation.PopAsync(); 
                }
                else
                {
                    var popup = new OneButtonPopup(
                        AppResources.ReportTitleError,
                        AppResources.ReportMessageError,
                        AppResources.OKText);
                    _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
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
                ID = CensorshipReason.IncitesHatredorDiscrimination,
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
                Subtitle = "",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = CensorshipReason.NotAQuestion,
                Title = "Not a question",
                Subtitle = "",
                Selected = false
            });
            ReasonList[0].Selected = true;
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
            (bool isValid, string errorMessage, string _) = RTAClient.ValidateHttpResponse(httpResponse, "Report question");  
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