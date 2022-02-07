using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
/* Adds a searchbar to ExploringPage.
 */
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExploringPageWithSearch : ExploringPage 
    {
        private string _searchingFor;

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
            SearchBar entitySearch = new SearchBar() 
                { 
                    Placeholder = "Search",
                };
                entitySearch.TextChanged += OnKeywordChanged;
                
            MainLayout.Children.Insert(0, entitySearch);
        }

        private void OnKeywordChanged(object sender, TextChangedEventArgs e) 
        {
            _searchingFor = e.NewTextValue;
            if (!String.IsNullOrWhiteSpace(_searchingFor))
            {
                ObservableCollection<Tag<Entity>> listToDisplay = GetSearchResults(_searchingFor);
                AuthorityListView.ItemsSource = listToDisplay;
            }
        }

        private ObservableCollection<Tag<Entity>> GetSearchResults(string queryString) 
        {
            return new ObservableCollection<Tag<Entity>>(
                        SelectableEntities.Where(f => f.NameContains(queryString)));
        }
    }
}