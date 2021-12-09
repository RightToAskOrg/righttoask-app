using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Models;
using RightToAskClient.Views;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class FilterDisplayTableView : TableView
    {
        private FilterChoices filterContext;
        private readonly TableSection contents;
        public FilterDisplayTableView(FilterChoices filterContext)
        {
            BindingContext = filterContext;
            this.filterContext = filterContext;
            BackgroundColor = Color.NavajoWhite;
            Intent = TableIntent.Settings;
            VerticalOptions = LayoutOptions.Start;
            HeightRequest = Height; 
            var root = new TableRoot() { Title = "Filters - tap to edit"};

            var authorityList = new Label()
            {
                Text = String.Join(",", filterContext.SelectedAuthorities.Select((a => a.ShortestName)))
            };

            var whoShouldAnswerItView = new ViewCell
            {
                View = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                        new Label { Text = "Who should answer it?" },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Start,
                            Children =
                            {
                                authorityList,
                            }
                        }
                    }
                }
            };
            whoShouldAnswerItView.Tapped += OnMoreButtonClicked;
            
            var whoShouldAskItMPs = String.Join(",", filterContext.SelectedAskingMPs);
            var whoShouldAskItMyMPs = String.Join(",", filterContext.SelectedAskingMPsMine);
            var whoShouldAskItCommittees = String.Join(",", filterContext.SelectedAskingCommittee);
            var whoShouldAskItUsers = String.Join(",", filterContext.SelectedAskingUsers);

            var whoShouldAskItList = new Label()
            {
                Text = String.Join("\n", new List<string>()
                    { whoShouldAskItCommittees, whoShouldAskItMPs, whoShouldAskItMyMPs, whoShouldAskItUsers }
                )
            };

            var whoShouldAskItView = new ViewCell
            {
                View = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                        new Label { Text = "Who should raise it in Parliament?"},
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Start,
                            Children =
                            {
                                whoShouldAskItList
                            }
                        }
                    }
                }
            };
            whoShouldAskItView.Tapped += OnMoreAskersButtonClicked;
            
            var keywordentry = new EntryCell
            {
                Label = "Keyword", 
                Placeholder = "?", 
                Text = filterContext.SearchKeyword ?? null,
            };
            keywordentry.Completed += OnKewordEntryCompleted;
            
            // var section1 = new TableSection();
            // var section2 = new TableSection() { };
            // section1.Add(whoShouldAnswerItView);
            //section2.Add(whoShouldAskItView);
            // section2.Add(keywordentry);
            Root = root;
            contents = new TableSection()
            {
                whoShouldAnswerItView,
                whoShouldAskItView,
                keywordentry
            };
            contents.Title = "Filters - tap to edit";
            root.Add(contents);

        }

        // TODO Unfortunately this seems to remove the data but not shrink the footprint.
        public void Shrink()
        {
            Root.Title = "Filters - tap to show";
            // Root.Remove(0);
            Root.Remove(contents);
        }

        public void UnShrink()
        {
            Root.Title = "Filters - tap to edit";
            Root.Add(contents);
        }
        private void OnMoreAskersButtonClicked(object sender, EventArgs e)
        {
        }

        async void OnMoreButtonClicked(object sender, EventArgs e)
        {
			string message = "Choose others to add";
			
           	var departmentExploringPage 
                = new ExploringPageWithSearchAndPreSelections(filterContext.SelectedAuthorities, message);
           	await Navigation.PushAsync (departmentExploringPage);
        }

        private void OnKewordEntryCompleted(object sender, EventArgs e)
        {
            filterContext.SearchKeyword = ((EntryCell)sender).Text;
        }
    }
}