using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        public AdvancedSearchFiltersPage(FilterChoices FilterChoice)
        {
            InitializeComponent();
            BindingContext = new FilterViewModel(FilterChoice);
        }
    }
}