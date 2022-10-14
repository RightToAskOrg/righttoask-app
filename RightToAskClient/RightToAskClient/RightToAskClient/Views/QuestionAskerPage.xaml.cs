using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        public QuestionAskerPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            QuestionViewModel.Instance.PopupLabelText = AppResources.QuestionAskerPopupText;
        }
    }
}