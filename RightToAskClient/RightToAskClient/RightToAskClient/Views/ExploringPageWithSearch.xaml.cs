using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExploringPageWithSearch : ExploringPage 
    {
        private string searchingFor;

        // TODO Actually the best way to to this is to put the things that you selected at the beginning,
        // with the unselected things (or possibly everything) in the huge long list underneath.  
        // When the user rearranges or unselects things, don't rearrange them (possibly consider adding them into
        // the selected list, but maybe not.
        // Also use the BindingContext properly. I think the setting should be in the base, not here.
		public ExploringPageWithSearch(ObservableCollection<MP> allEntities, 
			ObservableCollection<MP> selectedEntities, string? message=null) : base (allEntities, selectedEntities, message)
        {
            SearchBar authoritySearch = new SearchBar() 
                { 
                    Placeholder = "Search",
                };
                authoritySearch.TextChanged += OnKeywordChanged<MP>;
                
            MainLayout.Children.Insert(0, authoritySearch);
        }

        public ExploringPageWithSearch(ObservableCollection<Authority> filtersSelectedAuthorities, string message) 
            : base(filtersSelectedAuthorities, message)
        {
        }


        private void OnKeywordChanged<T>(object sender, TextChangedEventArgs e) where T : Entity
        {
            searchingFor = e.NewTextValue;
            if (!String.IsNullOrWhiteSpace(searchingFor))
            {
                ObservableCollection<Tag<T>> listToDisplay = GetSearchResults<T>(searchingFor);
                AuthorityListView.ItemsSource = listToDisplay;
            }
        }

        // Look up whether anything in the Entity includes the queryString.
        // This is a little bit of a cludge because it means, for instance,
        // that if you look for "Nat" you'll get all the Senators.
        // TODO If this turns out to be problematic, implement something like
        // NamesToString, which returns only the separated, concatenated
        // strings that you'll want to search on.
        // TODO *** Add a Person version, so we can search for other users to tag.
        private ObservableCollection<Tag<T>> GetSearchResults<T>(string queryString) where T : Entity
        {
            var normalizedQuery = queryString?.ToLower() ?? "";

            if (typeof(T) == typeof(Authority))
            {
                return new ObservableCollection<Tag<T>>(
                    (IEnumerable<Tag<T>>)(selectableAuthorities.Where(f => matches<Authority>(normalizedQuery, f))).ToList());
            }
            
            if (typeof(T) == typeof(MP))
            {
                return new ObservableCollection<Tag<T>>(
                    (IEnumerable<Tag<T>>)selectableMPs.Where(f => matches<MP>(normalizedQuery, f)).ToList());
            }

            return new ObservableCollection<Tag<T>>();


            // throw new InvalidOperationException("ExploringPageWithSearch initiated with neither Authorities nor MPs");
        }

        private bool matches<T>(string normalizedQuery, Tag<T> entity) where T : Entity
        {
            return entity.TagEntity.GetName().ToLowerInvariant().Contains(normalizedQuery);
        } 

    }
}