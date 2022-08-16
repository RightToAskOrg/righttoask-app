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
    public partial class MetadataPage : ContentPage
    {
        public MetadataPage()
        {
            InitializeComponent();
            BindingContext = FilterViewModel.Instance;

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