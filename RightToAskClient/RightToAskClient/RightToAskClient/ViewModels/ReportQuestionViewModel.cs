using System.Collections.Generic;
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
        
        public ReportQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });

            ReasonList = new List<ReportReason>();
            ReasonList.Add(new ReportReason
            {
                ID = 0,
                Title = "A",
                Subtitle = "aaaaaaaa",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 1,
                Title = "B",
                Subtitle = "bbbbbbbb",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 2,
                Title = "C",
                Subtitle = "ccccccc",
                Selected = false
            });
            ReasonList.Add(new ReportReason
            {
                ID = 3,
                Title = "D",
                Subtitle = "ddddddd",
                Selected = false
            });

            ReasonList[0].Selected = true;
        }
    }
}