using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    
    public static class ReadingPageExchanger
    {
        public static bool ByQuestionWriter;
    }
    public partial class ReadingPage : ContentPage
    {
        public ReadingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (ReadingPageExchanger.ByQuestionWriter)
            {
                var vm = new ReadingPageViewModel(ReadingPageExchanger.ByQuestionWriter);
                BindingContext = vm;
                vm.Title = AppResources.MyQuestionsTitle;
                ReadingPageExchanger.ByQuestionWriter = false;
            }
        }
        
        protected override void OnDisappearing()
        {
            // clear the selected item
            QuestionList.SelectedItem = null;
            base.OnDisappearing();
        }
    }
}