using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoButtonPopup : Popup
    {
        // constructor for ViewModels
        public TwoButtonPopup(BaseViewModel vm, string popupTitle, string popupText, string cancelMessage, string approveMessage)
        {
            InitializeComponent();
            BindingContext = vm;
            vmButtons.IsVisible = true;
            modelButtons.IsVisible = false;
            mainTitle.Text = popupTitle;
            mainMessage.Text = popupText;
            cancelButton.Text = cancelMessage;
            approveButton.Text = approveMessage;
            //cancelText.Text = cancelMessage;
            //approveText.Text = approveMessage;
        }

        //constructor for Models -- The incoming Question Model type must have boolean property to store the result of the button clicks
        public TwoButtonPopup(Question q, string popupTitle, string popupText, string cancelMessage, string approveMessage)
        {
            InitializeComponent();
            BindingContext = q;
            vmButtons.IsVisible = false;
            modelButtons.IsVisible = true;
            mainTitle.Text = popupTitle;
            mainMessage.Text = popupText;
            modelCancelButton.Text = cancelMessage;
            modelApproveButton.Text = approveMessage;
        }

        public bool hasApproved(object? value)
        {
            return value?.ToString() == "OK";
        }

        private void DismissPopup(object sender, EventArgs e)
        {
            Dismiss("cancel");
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Dismiss("cancel");
        }
        private void ApproveButtonClicked(object sender, EventArgs e)
        {
            Dismiss("OK");
        }

        private void ModelCancelButtonClicked(object sender, EventArgs e)
        {
            Dismiss("cancel");
        }
        private void ModelApproveButtonClicked(object sender, EventArgs e)
        { 
            Dismiss("OK");
        }
    }
}