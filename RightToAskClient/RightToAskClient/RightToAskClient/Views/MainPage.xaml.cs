using System;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
          
            // If we don't have a server public key, then there was something wrong with our initialization file.
            // Although this is not a useful message for users, it's a sufficiently useful message for developers
            // that it's worth producing here.
            if (String.IsNullOrEmpty(RTAClient.ServerPublicKey))
            {
                DisplayAlert(AppResources.ServerConfigErrorText, AppResources.ServerConfigErrorRecommendation, "", "OK");
            }
        }
    }
}
