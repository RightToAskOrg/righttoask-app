using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        public AdvancedSearchFiltersPage() : this (new FilterChoices())
        {
        }

        public AdvancedSearchFiltersPage(FilterChoices FilterChoice)
        {
            InitializeComponent();
            BindingContext = FilterViewModel.Instance;
            FilterViewModel.Instance.FilterChoices = FilterChoice;
        }
    }
}