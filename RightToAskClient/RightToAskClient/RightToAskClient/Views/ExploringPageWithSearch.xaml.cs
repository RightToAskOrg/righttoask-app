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

		public ExploringPageWithSearch(ObservableCollection<MP> allEntities, 
			ObservableCollection<MP> selectedEntities, string? message=null) : base (allEntities, selectedEntities, message)
        {
            addSearchBar();
        }

        public ExploringPageWithSearch(ObservableCollection<Authority> filtersSelectedAuthorities, string message) 
            : base(filtersSelectedAuthorities, message)
        {
            addSearchBar();
        }

        private void addSearchBar()
        {
            SearchBar authoritySearch = new SearchBar() 
                { 
                    Placeholder = "Search",
                };
                authoritySearch.TextChanged += OnKeywordChanged<MP>;
                
            MainLayout.Children.Insert(0, authoritySearch);
            
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

        // TODO: There should be a much more elegant way of choosing the right
        // selectable List according to its type. At the moment, this uses whichever one
        // out of selectableAuthorities or SelectableMPs is not null, which is fine but
        // has the potential to break if they're not set up right.
        private ObservableCollection<Tag<T>> GetSearchResults<T>(string queryString) where T : Entity
        {
            if (typeof(T) == typeof(Authority))
            {
                if (selectableAuthorities != null)
                    return new ObservableCollection<Tag<T>>(
                        (IEnumerable<Tag<T>>)selectableAuthorities.Where<Tag<Authority>>(f => f.NameContains(queryString)));
            }

            if (typeof(T) == typeof(MP))
            {
                
                if (selectableMPs != null)
                    return new ObservableCollection<Tag<T>>(
                        (IEnumerable<Tag<T>>)selectableMPs.Where<Tag<MP>>(f => f.NameContains(queryString)));
            }

            return new ObservableCollection<Tag<T>>();

            /*
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
            */
        }

        private bool matches<T>(string normalizedQuery, Tag<T> entity) where T : Entity
        {
            return entity.TagEntity.GetName().ToLowerInvariant().Contains(normalizedQuery);
        } 

    }
}