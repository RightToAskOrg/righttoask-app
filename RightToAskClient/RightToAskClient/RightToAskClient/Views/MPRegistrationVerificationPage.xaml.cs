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

        // Shell navigation requires a default constructor
        public MPRegistrationVerificationPage()
        {
            InitializeComponent();

            DomainPicker.ItemsSource = ParliamentData.Domains;
        }
    }
}