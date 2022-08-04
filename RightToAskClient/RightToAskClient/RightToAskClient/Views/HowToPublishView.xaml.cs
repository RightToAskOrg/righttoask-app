using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Resx;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowToPublishView : ContentView
    {
        public HowToPublishView()
        {
            InitializeComponent();
        //    RaisedLabel.Text =
        //        AppResources.HowToStep2b + AppResources.HowToStep2bbold + AppResources.HowToStep2bend; 
        }
    }
}