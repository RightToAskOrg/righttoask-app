using System;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

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
        }
    }
}
