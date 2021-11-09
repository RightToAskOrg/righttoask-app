using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient
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
		public ExploringPageWithSearch(ObservableCollection<Entity> allEntities, 
			ObservableCollection<Entity> selectedEntities, string message=null) : base (allEntities, selectedEntities, message)
        {
            Label RTKThanks = new Label() { Text = "Using The Australian Authorities List from Right To Know." };
            SearchBar authoritySearch = new SearchBar() 
                { 
                    Placeholder = "Search",
                };
                authoritySearch.TextChanged += OnKeywordChanged;
                
            MainLayout.Children.Insert(0, RTKThanks);    
            MainLayout.Children.Insert(1, authoritySearch);
        }

        private void OnKeywordChanged(object sender, TextChangedEventArgs e)
        {
            searchingFor = e.NewTextValue;
            ObservableCollection <Tag> listToDisplay = GetSearchResults(searchingFor);
            AuthorityListView.ItemsSource = listToDisplay;
        }

        // Look up whether either the long-form name or nickname/acronym
        // includes the queryString.
        // TODO Consider whether this really needs the Entity name or could instead
        // use ToString and hence make the whole class completely generic.
        private ObservableCollection<Tag> GetSearchResults(string queryString)
        {
            var normalizedQuery = queryString?.ToLower() ?? "";
            return new ObservableCollection<Tag>(selectableEntities.
                Where(f => (f.TagEntity.EntityName+':'+f.TagEntity.NickName).
                    ToLowerInvariant().Contains(normalizedQuery)).ToList());
        } 
        
    }
}