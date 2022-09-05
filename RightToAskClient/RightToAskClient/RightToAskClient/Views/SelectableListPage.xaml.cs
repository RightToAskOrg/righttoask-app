using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Org.BouncyCastle.Asn1.Tsp;
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
			//SelectableListView.ItemsSource = vm.SelectableEntities;
			//SelectableListView.IsGroupingEnabled = false;
			// Note this overrides the base setting of AuthorityListView.ItemsSource, which 
			// otherwise includes both selected and non-selected items.
			//AuthorityListView.ItemsSource = authorityLists.AllEntities;
		}

        public SelectableListPage(SelectableList<Committee> committeeLists, string message, bool grouping)
        {
	        InitializeComponent();
	        var vm = new SelectableListViewModel(committeeLists, message);
	        BindingContext = vm;
        }

        public SelectableListPage(SelectableList<MP> MPLists, string message, bool grouping=false, bool singleSelection=false, bool registerMP=false)
        {
			InitializeComponent();
			var vm = new SelectableListViewModel(MPLists, message, grouping, singleSelection, registerMP);
			BindingContext = vm;	
        }

        public SelectableListPage(SelectableList<Person> matchingParticipants, string message)
        {
	        InitializeComponent();
	        var vm = new SelectableListViewModel(matchingParticipants, message);
	        BindingContext = vm;
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
	}
}
