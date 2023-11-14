using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SharingElectorateInfoPage : ContentPage
    {
        public SharingElectorateInfoPage()
        {
            InitializeComponent();
        }
        public SharingElectorateInfoPage(Registration registration)
        {
            InitializeComponent();
            BindingContext = new SharingElectorateInfoViewModel(registration);
        }
    }
}