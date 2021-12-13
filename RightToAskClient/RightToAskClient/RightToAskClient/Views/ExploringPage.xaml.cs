using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Annotations;
using RightToAskClient.Models;
using Xamarin.Forms;
using PropertyChangingEventHandler = Xamarin.Forms.PropertyChangingEventHandler;

namespace RightToAskClient.Views
{
	/*
	 * Inputs a list of tags to be displayed in a way that allows the user
	 * to select some.  allEntities is the complete list of possible entities
	 * for selection. selectedEntities stores the list of
	 * selected Tags, to be updated when the page is popped.
	 * Optionally inputs a message to display at the top of the page.
	 *
	 * Constructors are either a single unstructured list of entities,
	 * or a list of lists, each with their own header and selectable set.
	 */
	public partial class ExploringPage : ContentPage
	{
		private List<(ObservableCollection<Entity> someEntities, ObservableCollection<Entity> selectedOnes , string heading)> entitiesList;
		// I would like to be able to use the type system to avoid this doubling-up, but 
		// I can't figure out how to do it. The roles of these three pairs are almost the
		// same regardless of their type: all entities is the complete available list;
		// selectableEntities are the same, wrapped in a Tag so they can be selected;
		// selected Entities are the ones selected initially. 
		private ObservableCollection<Authority> allTheAuthorities;
		// private ObservableCollection<MP> allTheMPs;
		protected ObservableCollection<Tag<Authority>> selectableAuthorities;
		protected ObservableCollection<Tag<MP>> selectableMPs;
		protected ObservableCollection<Authority> selectedAuthorities;
		protected ObservableCollection<MP> selectedMPs;
		protected Type typeOfEntities;

		/* This constructor is called only when the Entities are MPs.
		 * TODO: check. Also it's possible we won't need it at all if we organise all the MPs.
		 */
		
		public ExploringPage(ObservableCollection<MP> allEntities, 
			ObservableCollection<MP> selectedEntities, string? message=null)
		{
			InitializeComponent();

			selectedMPs = selectedEntities;
				
			selectableMPs = wrapInTags<MP>(allEntities, selectedEntities);
			
			IntroText.Text = message ?? "";
			AuthorityListView.BindingContext = selectableMPs;
			AuthorityListView.ItemsSource = selectableMPs;
			typeOfEntities = typeof(MP);
		}
		 
		/* This constructor is only used for Authorities, and hence assumed that the list to be selected from
		 * consists of the complete list of authorities.
		 */
		public ExploringPage(ObservableCollection<Authority> selectedEntities, string message) 
		{
			InitializeComponent();


			// allEntities = ParliamentData.AllAuthorities;
			selectedAuthorities = selectedEntities;
			selectableAuthorities = wrapInTags<Authority>(ParliamentData.AllAuthorities, selectedEntities);
			
			IntroText.Text = message ?? "";
			AuthorityListView.BindingContext = selectableAuthorities;
			AuthorityListView.ItemsSource = selectableAuthorities;
			typeOfEntities = typeof(Authority);
		}

		/* This constructor is called only when the inputs are MPs.
		 */
		public ExploringPage(IEnumerable<MPGroupedByChamber> groupedMPs, ObservableCollection<MP> selectedEntities, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			selectedMPs = selectedEntities;
			
			AuthorityListView.IsGroupingEnabled = true;

			List<TaggedGroupedMPs> groupedMPsWithTags = new List<TaggedGroupedMPs>();
			foreach(MPGroupedByChamber group in groupedMPs)
			{
				groupedMPsWithTags.Add(new TaggedGroupedMPs(
					group.Chamber,
					wrapInTags<MP>(group, selectedEntities)
				));
			}
			AuthorityListView.BindingContext = groupedMPsWithTags;
			AuthorityListView.GroupDisplayBinding = new Binding("Chamber");
			AuthorityListView.ItemsSource = groupedMPsWithTags;
			
			// Flat list for the purposes of updating/saving
			selectableMPs 
				= new ObservableCollection<Tag<MP>>(groupedMPsWithTags.SelectMany(x => x).ToList());
		}

		private class TaggedGroupedMPs : ObservableCollection<Tag<MP>>
		{
			public TaggedGroupedMPs(ParliamentData.Chamber chamber, ObservableCollection<Tag<MP>> mpGroup) : base(mpGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get; }
		}

		// TODO: Check whether this deals properly with derived classes of Entity 
		private void Authority_Selected(object sender, ItemTappedEventArgs e)
		{
				// ((Tag<Entity>) e.Item).Selected = !((Tag<Entity>) e.Item).Selected;
			if (e.Item is Tag<Entity> t) 
			{
				t.Selected = !t.Selected;
			}
			if (e.Item is Tag<Authority> t1) 
			{
				t1.Selected = !t1.Selected;
			}
			if (e.Item is Tag<MP> t2) 
			{
				t2.Selected = !t2.Selected;
			}
			if(e.Item is Tag<Person> t3)
			{
				t3.Selected = !t3.Selected;
			}
		}

		// TODO Consider whether the semantics of 'back' should be different from
		// 'done', i.e. whether 'back' should undo.
		// Also consider whether this should raise a warning if neither of the types match.
		async void DoneButton_OnClicked(object sender, EventArgs e)
		{
			if (typeOfEntities == typeof(MP))
			{
				UpdateSelectedList<MP>(selectableMPs, selectedMPs);
			}

			if (typeOfEntities == typeof(Authority))
			{
				UpdateSelectedList<Authority>(selectableAuthorities, selectedAuthorities);
			}

			await Navigation.PopAsync();
		}

		// TODO*** This will need to be careful about which lists it is.
		private void UpdateSelectedList<T>(ObservableCollection<Tag<T>> selectableEntities, ObservableCollection<T> selectedEntities) where T:Entity
		{
			var toBeIncluded = selectableEntities.Where(w => w.Selected).Select(t => t.TagEntity);	
			foreach (T selectedEntity in toBeIncluded)
			{
				if (!selectedEntities.Contains(selectedEntity))
				{
					selectedEntities.Add(selectedEntity);	
				}
			}
			
			var toBeRemoved = selectableEntities.Where(w => !w.Selected).Select(t => t.TagEntity);
			foreach (T notSelectedEntity in toBeRemoved)
			{
				selectedEntities.Remove(notSelectedEntity);	
			}
		}
		
		protected ObservableCollection<Tag<T>> wrapInTags<T>(ObservableCollection<T>
			entities, ObservableCollection<T> selectedEntities) where T : Entity
		{
			return new ObservableCollection<Tag<T>>(entities.Select
				(authority => new Tag<T>
					{
						TagEntity = authority,
						Selected =  selectedEntities.Contains(authority)
					}
				)
			);
		}
	}
}