using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowAnsweredOptionPage : ContentPage
    {
        public HowAnsweredOptionPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
        }
    }
}