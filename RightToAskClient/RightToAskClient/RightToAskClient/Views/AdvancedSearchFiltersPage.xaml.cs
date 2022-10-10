using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        public bool CameFromMainPage;
        public AdvancedSearchFiltersPage()
        {
            //vm = new ReadingPageViewModel();
            //vm.Title = "Advanced Search Filters";
            //BindingContext = vm;
            InitializeComponent();

            MessagingCenter.Subscribe<MainPageViewModel>(this, "MainPage", (sender) =>
            {
                CameFromMainPage = true;
                MessagingCenter.Unsubscribe<MainPageViewModel>(this, "MainPage");
            });
        }

        // android back button override
        protected override bool OnBackButtonPressed()
        {
            if (CameFromMainPage)
            {
                Application.Current.MainPage.Navigation.PopToRootAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}