using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LabeledPicker : StackLayout
    {
        public LabeledPicker()
        {
            InitializeComponent();
        }
    }
}