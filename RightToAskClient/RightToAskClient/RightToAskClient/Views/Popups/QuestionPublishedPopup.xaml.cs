using RightToAskClient.ViewModels;
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
    public partial class QuestionPublishedPopup : Popup
    {
        public QuestionPublishedPopup()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
        }

        private void WriteAnotherButtonClicked(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.GoHome = false;
            Dismiss("Dismissed");
        }

        private void GoHomeButtonClicked(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.GoHome = true;
            Dismiss("Dismissed");
        }
    }
}