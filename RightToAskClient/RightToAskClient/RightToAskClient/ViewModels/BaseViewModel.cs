using Xamarin.Forms;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        public ContentPage Page { get; set; } = new ContentPage();

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string title = string.Empty;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string _reportLabelText = "";
        public string ReportLabelText
        {
            get => _reportLabelText;
            set => SetProperty(ref _reportLabelText, value);
        }
    }
}
