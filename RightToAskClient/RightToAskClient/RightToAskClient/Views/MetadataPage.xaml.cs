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
    public partial class MetadataPage : ContentPage
    {
        public MetadataPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
        }
    }
}