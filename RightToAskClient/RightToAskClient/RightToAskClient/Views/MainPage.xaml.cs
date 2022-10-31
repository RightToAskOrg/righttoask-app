using RightToAskClient.ViewModels;
using Xamarin.Essentials;

namespace RightToAskClient.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            var vm = BindingContext as MainPageViewModel;
            
            // check bools here because it gets called before the App.xaml.cs section where we get other data from preferences
            // because this is the first page loaded in the application
            vm.ShowMyQuestions = Preferences.Get(Constants.HasQuestions, false);
            vm.ShowTrendingMyElectorate = Preferences.Get(Constants.ElectoratesKnown, false);
            vm.ShowQuestionsForMe = Preferences.Get(Constants.IsVerifiedMPAccount, false);
        }
    }
}
