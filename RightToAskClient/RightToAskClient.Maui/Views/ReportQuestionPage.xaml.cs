using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Maui.Models;
using RightToAskClient.Maui.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
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