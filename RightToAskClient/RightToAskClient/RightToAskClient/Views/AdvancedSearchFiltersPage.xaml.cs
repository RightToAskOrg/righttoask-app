using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        public bool CameFromMainPage = false;
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
                App.Current.MainPage.Navigation.PopToRootAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}