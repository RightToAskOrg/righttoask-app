using System.Collections.Generic;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class ReportReason
    {
        public int ID { get; set; }
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
        
        public ReportQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            ReportCommand = new AsyncCommand(async () =>
            {
                // TODO: Send request
                // Process respond
                var popup = new OneButtonPopup(
                    AppResources.ReportTitle,
                    AppResources.ReportMessage,
                    AppResources.DoneButtonText);
                _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                await App.Current.MainPage.Navigation.PopAsync();
            });

            ReasonList = new List<ReportReason>();
            ReasonList.Add(new ReportReason
            {
                ID = 0,
                Title = "Harassment",
                Subtitle = "Aggression against a specific individual or group.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 1,
                Title = "Threatening violence",
                Subtitle = "",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 2,
                Title = "Encouraging harm",
                Subtitle = "Encouragement of self-harm or harm to others.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 3,
                Title = "Hate",
                Subtitle = "Attacks or threats against an individual or group because of their protected attributes or anything else about them that does not relate to a political issue.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 4,
                Title = "Spam",
                Subtitle = "Malicious links, fake engagement, or advertisements.",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 5,
                Title = "Sharing personal information",
                Subtitle = "Sharing other people’s private information without their consent.",
                Selected = false
            });

            ReasonList[0].Selected = true;
        }
    }
}