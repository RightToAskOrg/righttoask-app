using System;
using RightToAskClient.Maui.Models;
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
    public partial class TwoButtonPopup : Popup
    {
        public TwoButtonPopup(string popupTitle, string popupText, string cancelMessage, string approveMessage, bool isQuestion)
        {
            InitializeComponent();
            mainTitle.Text = popupTitle;
            mainMessage.Text = popupText;
            vmButtons.IsVisible = !isQuestion;
            modelButtons.IsVisible = isQuestion;
            if (isQuestion)
            {
                modelCancelButton.Text = cancelMessage;
                modelApproveButton.Text = approveMessage;
            }
            else
            {
                cancelButton.Text = cancelMessage;
                approveButton.Text = approveMessage;
            }

            mainTitle.IsVisible = popupTitle.Length > 0;
        }

        public bool HasApproved(object? value)
        {
            return value?.ToString() == "OK";
        }

        private void DismissPopup(object sender, EventArgs e)
        {
            //TODO: Dismiss("cancel");
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            //TODO: Dismiss("cancel");
        }
        private void ApproveButtonClicked(object sender, EventArgs e)
        {
            //TODO: Dismiss("OK");
        }
    }
}