using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;

/*
 * Inputs a list of tags to be displayed in a way that allows the user
 * to select some.
 * allEntities is the complete list of possible entities for selection.
 * selectedEntities stores the list of selected Tags, to be updated when the page is popped.
 * 
 * Optionally inputs a message to display at the top of the page.
 *
 * Constructors are either a single unstructured list of entities,
 * or a list of lists, each with their own header and selectable set.
 *
 * Inherited classes ExploringPageWithSearch and ExploringPageWithPreselections
 * add a searchbar and a separate list of prior-selected items.
 */
namespace RightToAskClient.Views
{
	public partial class SelectableListPage 
	{
		private readonly object _entityLists;

		// TODO These are probably no longer necessary here.
		// Possibly they should be bindable properties of the view model;
		// Possibly they should just be set by default in the view model, as they are now.
		// public bool CameFromReg2Page = false;
		// public bool GoToReadingPageNext = false;
		// public bool OptionB = false;

        public SelectableListPage(SelectableList<Authority> authorityLists , string message) 
		{
			InitializeComponent();
			var vm = new SelectableListViewModel(authorityLists, message);
			BindingContext = vm;
			
			SelectableListView.ItemsSource = vm.SelectableEntities;
			SelectableListView.IsGroupingEnabled = false;

            if (authorityLists.SelectedEntities.Any())
            {
				var selectionsListView = setUpPage();
				selectionsListView.ItemsSource = authorityLists.SelectedEntities;
			}
			// Note this overrides the base setting of AuthorityListView.ItemsSource, which 
			// otherwise includes both selected and non-selected items.
			//AuthorityListView.ItemsSource = authorityLists.AllEntities;
		}

        public SelectableListPage(SelectableList<MP> MPLists, string message, bool grouping)
        {
			InitializeComponent();
			var vm = new SelectableListViewModel(MPLists, message, grouping);
			BindingContext = vm;	
			
			SelectableListView.ItemsSource = grouping ? (IEnumerable)vm.SelectableGroupedEntities : vm.SelectableEntities;
			SelectableListView.IsGroupingEnabled = grouping;
			SelectableListView.GroupDisplayBinding = vm.GroupDisplay;
        }
		
		/* TODO It wold probably be more elegantly MVVM if this was in the ViewModel rather than the code-behind, but I
		 * can't figure out how to bind an ItemTappedEvent.
		 */
		protected void OnEntity_Selected(object sender, ItemTappedEventArgs e)
		{
			if (e.Item is Tag<Entity> tag)
			{
				tag.Toggle();
			}
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
				ItemTemplate = (DataTemplate)Application.Current.Resources["SelectableDataTemplate"]
			};
			selections.ItemTapped += OnEntity_Selected;

			MainLayout.Children.Insert(2, selections);
			return selections;
		}
	}
}
