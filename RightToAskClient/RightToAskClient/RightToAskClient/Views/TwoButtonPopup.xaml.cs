using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoButtonPopup : Popup
    {
        BaseViewModel baseViewModel;
        //ObservableObject model;
        // constructor for ViewModels
        public TwoButtonPopup(BaseViewModel vm, string popupTitle, string popupText, string cancelMessage, string approveMessage)
        {
            InitializeComponent();
            baseViewModel = vm;
            BindingContext = vm;
            mainTitle.Text = popupTitle;
            mainMessage.Text = popupText;
            cancelButton.Text = cancelMessage;
            approveButton.Text = approveMessage;
        }

        // constructor for Models -- The incoming Model must have boolean property to store the result of the button clicks
        //public TwoButtonPopup(ObservableObject m, string popupText, string cancelMessage, string approveMessage)
        //{
        //    InitializeComponent();
        //    model = m;
        //    BindingContext = m;
        //    mainMessage.Text = popupText;
        //    cancelButton.Text = cancelMessage;
        //    approveButton.Text = approveMessage;
        //}

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
    }
}