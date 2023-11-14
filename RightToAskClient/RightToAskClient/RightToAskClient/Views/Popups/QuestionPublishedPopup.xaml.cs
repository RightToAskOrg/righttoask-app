using System;
using RightToAskClient.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.ImageSources;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Maui.Views;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionPublishedPopup : ContentPage
    {
        public QuestionPublishedPopup()
        {
            BindingContext = QuestionViewModel.Instance;
        }

        private void WriteAnotherButtonClicked(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.GoHome = false;
            //TODO: Dismiss("Dismissed");
        }

        private void GoHomeButtonClicked(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.GoHome = true;
            //TODO: Dismiss("Dismissed");
        }
    }
}