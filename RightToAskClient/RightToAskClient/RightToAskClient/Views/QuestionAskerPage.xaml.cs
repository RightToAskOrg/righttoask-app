using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Controls;
using RightToAskClient.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        private ReadingContext readingContext;
        public QuestionAskerPage(ReadingContext readingContext)
        {
            // TODO: Construct this properly.
            // FilterContext filters = new FilterContext {FilterKeyword = readingContext.SearchKeyword};
            BindingContext = readingContext;
            this.readingContext = readingContext;
            
            
            InitializeComponent();

            FilterDisplayTableView ttestableView = new FilterDisplayTableView(readingContext.Filters);
            WholePage.Children.Insert(0,ttestableView);
        }

        async void OnNavigateForwardButtonClicked(object sender, EventArgs e)
        {
			var readingPage = new ReadingPage(false, readingContext);
			await Navigation.PushAsync (readingPage);
        }
    }
}