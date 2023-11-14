using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyCollectionView : ContentView
    {
        public EmptyCollectionView()
        {
            InitializeComponent();
        }
    }
}