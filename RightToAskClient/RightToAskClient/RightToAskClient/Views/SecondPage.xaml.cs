using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class SecondPage
    {
        public SecondPage()
        {
            InitializeComponent();

            // reset the question if navigating back before this page in the stack
            QuestionViewModel.Instance.ResetInstance();

            BindingContext = QuestionViewModel.Instance;
            if (App.ReadingContext.IsReadingOnly)
            {
                Title = "Find questions";
                QuestionViewModel.Instance.IsReadingOnly = true;
            }
            else
            {
                QuestionViewModel.Instance.IsReadingOnly = false;
                Title = "Direct my question";
            }
        }
    }
}

