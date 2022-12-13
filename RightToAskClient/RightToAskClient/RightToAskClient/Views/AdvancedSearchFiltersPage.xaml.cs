using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        public AdvancedSearchFiltersPage()
        {
            InitializeComponent();
            // TODO: because it should be shared?
            BindingContext = FilterViewModel.Instance;
            // TODO: pass it through constructor
            FilterViewModel.Instance.FilterChoices = App.GlobalFilterChoices;
        }
    }
}