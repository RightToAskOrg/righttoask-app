using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MetadataPage : ContentPage
    {

        public MetadataPage()
        {
            InitializeComponent();
            // if(filterChoices)
            BindingContext = new FilterViewModel(new FilterChoices());

            // Hide the clickable lists that are invisible.
            hideEmpties(myMPsToAnswer);
            hideEmpties(otherMPsToAnswer);
            hideEmpties(myMPsToRaise);
            hideEmpties(otherMPsToRaise);
            hideEmpties(authoritiesToAnswer);
            hideEmpties(committees);
        }

        private void hideEmpties(ClickableListView clickableListView)
        {
            clickableListView.IsVisible = (clickableListView.BindingContext as ClickableListViewModel)?.AnySelections ?? true;
        }

    }
}