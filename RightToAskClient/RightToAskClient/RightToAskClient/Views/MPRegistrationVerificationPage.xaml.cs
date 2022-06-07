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
        public MPRegistrationVerificationPage(SelectableList<MP> selectableMpList)
        {
            InitializeComponent();

            // TODO Not sure this is the best way to do this.
            var mpRegViewModel = BindingContext as MPRegistrationVerificationViewModel;
            mpRegViewModel.SelectableMPList = selectableMpList;
            
            DomainPicker.ItemsSource = ParliamentData.Domains;
        }
    }
}