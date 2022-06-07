using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MPRegistrationVerificationPage : ContentPage
    {
        MPRegistrationVerificationViewModel mpRegViewModel;

        // Shell navigation requires a default constructor
        // set this up to at least allow me to reach the page without having the selectableMPList figured out yet.
        public MPRegistrationVerificationPage()
        {
            InitializeComponent();
            mpRegViewModel = BindingContext as MPRegistrationVerificationViewModel ?? new MPRegistrationVerificationViewModel();
            //mpRegViewModel.SelectableMPList = new SelectableList<MP>();

            DomainPicker.ItemsSource = ParliamentData.Domains;
        }

        public MPRegistrationVerificationPage(SelectableList<MP> selectableMpList)
        {
            InitializeComponent();
            // TODO Not sure this is the best way to do this.
            mpRegViewModel = BindingContext as MPRegistrationVerificationViewModel ?? new MPRegistrationVerificationViewModel();
            mpRegViewModel.SelectableMPList = selectableMpList;

            DomainPicker.ItemsSource = ParliamentData.Domains;
        }
    }
}