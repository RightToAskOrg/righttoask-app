using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.ViewModels
{
    public class AskQuestionViewModel : ReadingPageViewModel
    {
        public IAsyncCommand BackCommand { get; }
        public AskQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
        }
       
    }
}