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
        BaseViewModel? baseViewModel;
        Question? model;
        // constructor for ViewModels
        public TwoButtonPopup(BaseViewModel vm, string popupTitle, string popupText, string cancelMessage, string approveMessage)
        {
            InitializeComponent();
            baseViewModel = vm;
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
            model = q;
            BindingContext = q;
            vmButtons.IsVisible = false;
            modelButtons.IsVisible = true;
            mainTitle.Text = popupTitle;
            mainMessage.Text = popupText;
            modelCancelButton.Text = cancelMessage;
            modelApproveButton.Text = approveMessage;
        }

        private void DismissPopup(object sender, EventArgs e)
        {
            Dismiss("Dismissed");
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            if (baseViewModel != null)
            {
                baseViewModel.CancelButtonClicked = true;
                baseViewModel.ApproveButtonClicked = false;
            }
            Dismiss("Dismissed");
        }
        private void ApproveButtonClicked(object sender, EventArgs e)
        {
            if (baseViewModel != null)
            {
                baseViewModel.ApproveButtonClicked = true;
                baseViewModel.CancelButtonClicked = false;
            }
            Dismiss("Dismissed");
        }

        private void ModelCancelButtonClicked(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.ApproveClicked = false;
                model.CancelClicked = true;
            }
            Dismiss("Dismissed");
        }
        private void ModelApproveButtonClicked(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.ApproveClicked = true;
                model.CancelClicked = false;
            }
            Dismiss("Dismissed");
        }
    }
}