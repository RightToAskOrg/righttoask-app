using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPopup : Popup
    {
        public InfoPopup(string message)
        {
            InitializeComponent();
            popupText.Text = message;
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            Dismiss("Dismissed");
        }
    }
}