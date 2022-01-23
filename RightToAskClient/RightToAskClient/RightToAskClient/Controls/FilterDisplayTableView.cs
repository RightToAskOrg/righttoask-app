using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models;
using RightToAskClient.Views;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class FilterDisplayTableView : Grid
    {
        private Label _authorityList = new Label();
        private View _whoShouldAnswerItView;
        private FilterChoices _filterContext;
        private readonly TableSection _contents;
        public FilterDisplayTableView(FilterChoices filterContext)
        {
            BindingContext = filterContext;
            _filterContext = filterContext;
            BackgroundColor = Color.NavajoWhite;
            // Intent = TableIntent.Settings;
            VerticalOptions = LayoutOptions.Start;
            HeightRequest = Height; 
            // var root = new TableRoot() { Title = "Filters - tap to edit"};
            
            //RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) }) ;
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition());
            //RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });
            
            // Title = "Filters - tap to edit";
            // _authorityList.Text = String.Join(",", filterContext.SelectedAuthorities.Select(a => a.ShortestName));

            _whoShouldAnswerItView = BuildWhoShouldAnswerItView();
            
            /*
              new ViewCell
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
                                _authorityList,
                            }
                        }
                    }
                }
            };
            _whoShouldAnswerItView.Tapped += OnMoreButtonClicked;
            */
            
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
            
            var keywordentry = new Entry
            {
                // Label = "Keyword", 
                Placeholder = "?", 
                Text = filterContext.SearchKeyword 
            };
            keywordentry.Completed += OnKewordEntryCompleted;

            View keywordentryView = new StackLayout()
            {
                Children =
                {
                    new Label(){Text = "Keyword"},
                    keywordentry
                }
            };
            /*
            Root = root;
            _contents = new TableSection()
            {
                _whoShouldAnswerItView,
                whoShouldAskItView,
                keywordentry
            };
            _contents.Title = "Filters - tap to edit";
            root.Add(_contents);
            
            Children =
            {
                _whoShouldAnswerItView,
                whoShouldAskItView,
                keywordentry
            }
            */
            Children.Add(new Label(){Text = "Filters - Tap to edit"},0,0);
            Children.Add(_whoShouldAnswerItView,0,1);
            // Children.Add(whoShouldAskItView);
            Children.Add(keywordentryView,0,2);

        }

        private void OnMoreAskersButtonClicked(object sender, EventArgs e)
        {
        }

        async void OnMoreButtonClicked(object sender, EventArgs e)
        {
			string message = "Choose others to add";
			
           	var departmentExploringPage 
                = new ExploringPageWithSearchAndPreSelections(_filterContext.SelectedAuthorities, message);
           	await Navigation.PushAsync (departmentExploringPage);
            DealWithUpdate(); 
        }

        // TODO*** - this is not working.
        private void DealWithUpdate()
        {
            _whoShouldAnswerItView = BuildWhoShouldAnswerItView();
            // Root.Add(new TableSection(){_whoShouldAnswerItView});
        }

        private View BuildWhoShouldAnswerItView()
        {
            Label authorityList = new Label()
            {
                Text = String.Join(",", _filterContext.SelectedAuthorities.Select(a => a.ShortestName))
            };
            
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnMoreButtonClicked;
            
            // var view = new BoxView()
            //var viewCell = new ViewCell
            //    {
            //        View = new StackLayout
            var view = new StackLayout()
                    {
                    // Orientation = StackOrientation.Horizontal,
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
                // }
            };
            view.GestureRecognizers.Add(tapGestureRecognizer);

            // viewCell.GestureRecognizers.Add(tapGestureRecognizer);
            return view;
        }

        private void OnKewordEntryCompleted(object sender, EventArgs e)
        {
            _filterContext.SearchKeyword = ((Entry)sender).Text;
        }
    }
}