using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.ViewModels
{
    public class ReportQuestionViewModel: BaseViewModel
    {
        public IAsyncCommand BackCommand { get; }
        
        public ReportQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
        }
    }
}