using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models;
using RightToAskClient.Views;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class FilterDisplayTableView : TableView
    {
        private FilterChoices filterContext;
        public FilterDisplayTableView(FilterChoices filterContext)
        {
            BindingContext = filterContext;
            this.filterContext = filterContext;
            BackgroundColor = Color.NavajoWhite;
            Intent = TableIntent.Settings;
            VerticalOptions = LayoutOptions.Start;
            HeightRequest = Height; 
            var root = new TableRoot();
            var section1 = new TableSection() { Title = "Filters - click to edit"};
            var section2 = new TableSection() { };

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
            
            section1.Add(whoShouldAnswerItView);
            section2.Add(whoShouldAskItView);
            section2.Add(keywordentry);
            Root = root;
            root.Add(section1);
            root.Add(section2);

        }

        private void OnMoreAskersButtonClicked(object sender, EventArgs e)
        {
        }

        async void OnMoreButtonClicked(object sender, EventArgs e)
        {
			string message = "Choose others to add";
			
           	var departmentExploringPage = new ExploringPageWithSearchAndPreSelections(BackgroundElectorateAndMPData.AllAuthorities, 
                filterContext.SelectedAuthorities, message);
           	await Navigation.PushAsync (departmentExploringPage);
        }

        private void OnKewordEntryCompleted(object sender, EventArgs e)
        {
            filterContext.SearchKeyword = ((EntryCell)sender).Text;
        }
    }
}