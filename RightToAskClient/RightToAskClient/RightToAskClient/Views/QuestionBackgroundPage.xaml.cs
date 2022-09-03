using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionBackgroundPage : ContentPage
    {
        public QuestionBackgroundPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
        }
    }
}