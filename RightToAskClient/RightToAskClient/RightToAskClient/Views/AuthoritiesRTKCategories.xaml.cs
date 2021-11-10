using System;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class AuthoritiesRTKCategories : ContentPage
	{
		public AuthoritiesRTKCategories()
		{
			InitializeComponent();
            
            // Intent = TableIntent.Menu;
            FedAuthoritiesTable.Intent = TableIntent.Menu;
            // HasUnevenRows = true;
            var section1 = new TableSection() { Title = "Federal Authorities"};
            // var section2 = new TableSection() { };

            /*
            var authorityDataTemplate = new DataTemplate(() =>
            {
                var grid = new Grid();
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                var selectedToggle = new Switch();

                nameLabel.SetBinding(Label.TextProperty, "TagEntity.NickName");
                selectedToggle.SetBinding(Switch.IsToggledProperty, "Selected");

                grid.Children.Add(nameLabel);
                grid.Children.Add(selectedToggle, 1, 0);
                
                return new ViewCell { View = grid };
            });

            */
            
            /*
            var entry2 = new EntryCell
            {
                Label = "Who should answer it?",
                Placeholder = "Not sure",
            };
            entry2.Completed += OnWhoShouldAnswerCompleted;
            */

            /*
            var authorityList = new ListView
            {
                VerticalOptions = LayoutOptions.Start, 
                SelectionMode = ListViewSelectionMode.None, 
                ItemsSource = readingContext.OtherAuthorities,
                HasUnevenRows = true,
                ItemTemplate = authorityDataTemplate
            };
            // TODO Figure out how to pick up clicks.
            authorityList.ItemTapped += Authority_Selected;
            // BindableLayout.SetItemsSource(authorityList, readingContext.OtherAuthorities);
            // BindableLayout.SetItemTemplate(authorityList, authorityDataTemplate);
                

                // var authorityListView = new ViewCell { View = authorityList };
            */
            
            var moreButton = new ViewCell
            {
                View = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children = {  new Label { Text = "More..." } }
                }
            };
            moreButton.Tapped += OnMoreButtonClicked;

            /*
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
                        // authorityList.ParentView,
                        // new View{ authorityList},
                        new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            VerticalOptions = LayoutOptions.Start,
                            Children =
                            {
                                authorityList
                            }
                        }
                    }
                }
            };
            */
            
            /*
            var entry3 = new EntryCell { Label = "Who should raise it in Parliament?", Placeholder = "Not sure" };
            var keywordentry = new EntryCell
            {
                Label = "Keyword", 
                Placeholder = "?", 
                Text = ((ReadingContext) BindingContext).SearchKeyword ?? null
            };
            keywordentry.Completed += OnKewordEntryCompleted;
            */
            
            // var switchc = new SwitchCell { Text = "SwitchCell Text" };
            // var image = new ImageCell { Text = "ImageCell Text", Detail = "ImageCell Detail", ImageSource = "XamarinLogo.png" };

            // section1.Add(entry2);
            // section1.Add(whoShouldAnswerItView);
            section1.Add(moreButton);
		}

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        async void OnSearchClicked(object sender, EventArgs e)
        {
			var webViewAuthoritySelectPage = new WebviewAuthoritySelect((ReadingContext) BindingContext);
			await Navigation.PushAsync(webViewAuthoritySelectPage);
        }
	}
}

