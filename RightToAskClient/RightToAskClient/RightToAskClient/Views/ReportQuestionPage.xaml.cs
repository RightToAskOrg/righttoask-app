using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportQuestionPage : ContentPage
    {
        public ReportQuestionPage()
        {
            InitializeComponent();
        }

        public ReportQuestionPage(string question_id)
        {
            BindingContext = new ReportQuestionViewModel(question_id);
            InitializeComponent();
            
        }
    }
}