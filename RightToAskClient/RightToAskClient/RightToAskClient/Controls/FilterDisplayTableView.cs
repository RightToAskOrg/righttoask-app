using System;
using RightToAskClient.Models;
using RightToAskClient.Views;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class FilterDisplayTableView : Grid
    {
        private FilterChoices _filterContext;
        public FilterDisplayTableView()
        {
            _filterContext = App.ReadingContext.Filters;
            BindingContext = _filterContext;
            
            BackgroundColor = Color.NavajoWhite;
            VerticalOptions = LayoutOptions.Start;
            HeightRequest = Height; 
            
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            for (var i=0 ; i<6 ; i++)
            {
                RowDefinitions.Add(new RowDefinition{ Height = new GridLength(35) });
            }
            
            var whichAuthorityShouldAnswerItView = new ClickableEntityListView<Authority>
            {
	            ClickableListLabel = "Authorities who should answer the question",
	            ClickableListContents = _filterContext.SelectedAuthorities,
	            UpdateAction = OnEditAuthoritiesButtonClicked
            };
            
            var whichMyMPShouldAnswerItView = new ClickableEntityListView<MP>
            {
	            ClickableListLabel = "My MPs who should answer the question",
	            ClickableListContents = _filterContext.SelectedAnsweringMPsMine,
	            UpdateAction = OnEditSelectedAnsweringMPsMineButtonClicked
            };
            
            var whichMyMPShouldAskItView = new ClickableEntityListView<MP>
            {
	            ClickableListLabel = "My MPs who should raise the question in Parliament",
	            ClickableListContents = _filterContext.SelectedAskingMPsMine,
	            UpdateAction = OnEditSelectedAskingMPsMineButtonClicked
            };
            
            var whichMPShouldAnswerItView = new ClickableEntityListView<MP>
            {
	            ClickableListLabel = "Other MPs who should answer the question",
	            ClickableListContents = _filterContext.SelectedAnsweringMPs,
	            UpdateAction = OnEditSelectedAnsweringMPsButtonClicked
            };
            
            var whichMPShouldAskItView = new ClickableEntityListView<MP>
            {
	            ClickableListLabel = "Other MPs who should raise the question in Parliament",
	            ClickableListContents = _filterContext.SelectedAskingMPs,
	            UpdateAction = OnEditSelectedAskingMPsButtonClicked
            };
            
            // TODO Add one for committees when they're included.
            
            var keywordentry = new Entry
            {
                WidthRequest = 200,
                Placeholder = "?", 
                Text = _filterContext.SearchKeyword 
            };
            keywordentry.Completed += OnKewordEntryCompleted;

            View keywordentryView = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label(){Text = "Keyword"},
                    keywordentry
                }
            };
            
            Children.Add(new Label(){
                HorizontalOptions = LayoutOptions.Center,
                Text = "Filters - Tap to edit"
                },0,0);
            Children.Add(whichAuthorityShouldAnswerItView,0,1);
            Children.Add(whichMyMPShouldAnswerItView,0,2);
            Children.Add(whichMyMPShouldAskItView,0,3);
            Children.Add(whichMPShouldAnswerItView,0,4);
            Children.Add(whichMPShouldAskItView,0,5);
            Children.Add(keywordentryView,0,6);

        }


        async void OnEditAuthoritiesButtonClicked(object sender, EventArgs e)
        {
			string message = "Choose others to add";
			
           	var departmentExploringPage 
                = new ExploringPageWithSearchAndPreSelections(_filterContext.SelectedAuthorities, message);
            await Navigation.PushAsync (departmentExploringPage);
            //await Shell.Current.GoToAsync($"{nameof(ExploringPageWithSearchAndPreSelections)}");
        }

        void OnEditSelectedAnsweringMPsMineButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushMyAnsweringMPsExploringPage();
            }
        }

        private void OnEditSelectedAskingMPsMineButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushMyAskingMPsExploringPage();
            }
            
        }
        private void OnEditSelectedAnsweringMPsButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushAnsweringMPsExploringPage();
            }
        }
        private void OnEditSelectedAskingMPsButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushAskingMPsExploringPageAsync();
            }
        }


        private void OnKewordEntryCompleted(object sender, EventArgs e)
        {
            if (sender is Entry entry)
            {
                _filterContext.SearchKeyword = entry.Text;
            }
        }
    }
}