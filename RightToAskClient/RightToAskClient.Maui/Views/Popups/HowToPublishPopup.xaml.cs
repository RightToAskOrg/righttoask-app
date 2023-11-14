using System;
using Microsoft.Maui.Controls.Xaml;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.ImageSources;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Maui.Views;

namespace RightToAskClient.Maui.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowToPublishPopup : Popup
    {
        public HowToPublishPopup()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //TODO
            ////TODO: Dismiss("Dismissed");
        }
    }
}