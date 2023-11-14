using System;
using RightToAskClient.Maui.ViewModels;
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
    public partial class ReadingPagePopup : Popup
    {
        public ReadingPagePopup(ReadingPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //TODO: Dismiss("Dismissed");
        }
    }
}