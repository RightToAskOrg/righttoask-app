using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
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

        public ReportQuestionPage(string questionId, QuestionResponseRecords responseRecords, Command command)
        {
            InitializeComponent();
            BindingContext = new ReportQuestionViewModel(questionId, responseRecords, command);
        }

        private void RadioButton_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var vm = BindingContext as ReportQuestionViewModel;
            if (vm != null)
            {
                vm.IsSelected = true;
            }
        }
    }
}