using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlowOptionPage : ContentPage
    {
        public FlowOptionPage()
        {
            InitializeComponent();

            BindingContext = QuestionViewModel.Instance;
            QuestionViewModel.Instance.PopupLabelText = AppResources.AnswerQuestionOptionsPopupText;
        }
    }
}