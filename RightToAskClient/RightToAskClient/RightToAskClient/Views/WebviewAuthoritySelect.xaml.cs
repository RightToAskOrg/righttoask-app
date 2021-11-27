using System;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class WebviewAuthoritySelect : ContentPage
    {
        private ReadingContext context;
        public WebviewAuthoritySelect (ReadingContext context)
        {
            InitializeComponent ();
            this.context = context;
            BindingContext = context;
            WebView1.Source = "https://www.righttoknow.org.au/body/list/all";
        }

        async void OnNavigation(object sender, WebNavigatingEventArgs e)
        {
            bool selectThis=false, doneSearching=false;
            string selectedAuthority = "";
            
            Uri wholeUri = new Uri(e.Url);
            int numSegments = wholeUri.Segments.Length;
            if (numSegments > 1)
            {
                // TODO This is very specific to the current RightToKnow website,
                // and probably very brittle as a result.
                string secondLastSegment = wholeUri.Segments[numSegments - 2];
                if (secondLastSegment == "body/")
                {
                    selectedAuthority = wholeUri.Segments[numSegments - 1];
                    selectThis = await DisplayAlert("Select this authority?", 
                       selectedAuthority ,"Select", "Cancel");
                
                    ((Xamarin.Forms.WebView) sender).GoBack();
                }
            }

            if (selectThis)
            {
                insertOrSelect(selectedAuthority);
                doneSearching = await DisplayAlert("Keep searching for more authorities?", 
                                       "Or go on to the next step","Next", "Keep searching");
            }

            // TODO Not clear that we want to pop - maybe we just want to go forwards.
            if (doneSearching)
            {
                await Navigation.PopAsync();
            }
        }

        // Checks to see if the selected authority is already in our list - 
        // it should be, but perhaps won't be.  If it is, it selects it;
        // if not, it adds it, selected.
        private void insertOrSelect(string authority)
        {
            var matchingAuthorities 
                = new ObservableCollection<Entity>(ParliamentData.AllAuthorities.
                    Where(w => w.RightToKnowURLSuffix != null && w.RightToKnowURLSuffix  == authority));
            if (matchingAuthorities.Count == 0)
            {
                Entity newAuthority = new Entity
                {
                    EntityName = authority,
                    NickName = authority
                };
                ParliamentData.AllAuthorities.Add(newAuthority);

            } else if (matchingAuthorities.Count == 1)
            {
                var authorityToBeAdded = matchingAuthorities[0]; 
                if(!context.Filters.SelectedAuthorities.Contains(authorityToBeAdded))
                {
                    context.Filters.SelectedAuthorities.Add(authorityToBeAdded);        
                }
            }
            else
            // TODO Do error handling better. It would be good to
            // have some good test cases.
            {
                DisplayAlert("Oops", "We seem to have the same authority twice", "OK");
            }
        }

        // TODO At the moment this pops back to the previous page, but
        // it would make a lot more sense to go forward both here and from
        // the MP selection.
        async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
