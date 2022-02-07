using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* This page provides a view of all the authorities, with the already-selected ones at the top and
 * the (presumably much longer) list of unselected ones in a separate section below. Any of them can be searched or toggled.
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExploringPageWithSearchAndPreSelections  : RightToAskClient.Views.ExploringPageWithSearch 
    {
        public ExploringPageWithSearchAndPreSelections(ObservableCollection<Authority> filtersSelectedAuthorities,
            string message)
            : base(filtersSelectedAuthorities, message)
        {
            var selectionsListView = setUpPage();
            selectionsListView.ItemsSource = SelectableEntities.Where(te => te.Selected);
            // Note this overrides the base setting of AuthorityListView.ItemsSource, which 
            // otherwise includes both selected and non-selected items.
            AuthorityListView.ItemsSource = SelectableEntities.Where(te => !te.Selected); 
        }

        // Sets up the page, mostly by inserting a list for selected items at the beginning.
        // Returns a pointer to the new ListView, so that its ItemSource can be set according to
        // type of data.
        private ListView setUpPage()
        {
            Label alreadySelected = new Label() 
                { 
                    Text = "Already selected",
                };
                
            MainLayout.Children.Insert(1, alreadySelected);

            ListView selections = new ListView()
            {
                ItemTemplate=(DataTemplate)Application.Current.Resources["SelectableDataTemplate"]
            };
            selections.ItemTapped += OnEntity_Selected;
            
            MainLayout.Children.Insert(2,selections);
            return selections;
        }
    }
}