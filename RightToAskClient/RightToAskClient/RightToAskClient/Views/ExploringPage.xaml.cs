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
		protected readonly ObservableCollection<Entity> allEntities;
		protected readonly ObservableCollection<Tag<Entity>> selectableEntities;
		protected ObservableCollection<Tag<Authority>>? selectableAuthorities;
		protected ObservableCollection<Tag<MP>>? selectableMPs;
		protected ObservableCollection<Authority>? selectedAuthorities;
		protected ObservableCollection<MP>? selectedMPs;
		protected Type typeOfEntities;

		/* This constructor is called only when the Entities are MPs.
		 * TODO: check. Also it's possible we won't need it at all if we organise all the MPs.
		 */
		
		public ExploringPage(ObservableCollection<MP> allEntities, 
			ObservableCollection<MP> selectedEntities, string? message=null)
		{
			InitializeComponent();

			this.allEntities = new ObservableCollection<Entity>(allEntities);
			
			selectedMPs = selectedEntities;
			// selectableMPs = wrapInTags<MP>(this.allEntities, selectedEntities);
			selectableEntities = wrapInTags<MP>(this.allEntities, selectedEntities);
			
			IntroText.Text = message ?? "";
			AuthorityListView.BindingContext = selectableEntities;
			AuthorityListView.ItemsSource = selectableEntities;
			typeOfEntities = typeof(MP);
		}
		 
		/* This constructor is only used for Authorities, and hence assumed that the list to be selected from
		 * consists of the complete list of authorities.
		 */
		public ExploringPage(ObservableCollection<Authority> selectedEntities, string message) 
		{
			InitializeComponent();

			this.allEntities = new ObservableCollection<Entity>(ParliamentData.AllAuthorities);
			// allEntities = ParliamentData.AllAuthorities;
			selectedAuthorities = selectedEntities;
			// selectableAuthorities = wrapInTags<Authority>(ParliamentData.AllAuthorities, selectedEntities);
			selectableEntities = wrapInTags<Authority>(allEntities, selectedEntities);
			
			IntroText.Text = message ?? "";
			AuthorityListView.BindingContext = selectableEntities;
			AuthorityListView.ItemsSource = selectableEntities;
			typeOfEntities = typeof(Authority);
		}

		public ExploringPage(IEnumerable<IGrouping<ParliamentData.Chamber, MP>> groupedMPs, ObservableCollection<MP> selectedMPs, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			this.selectedMPs = selectedMPs;
			
			AuthorityListView.IsGroupingEnabled = true;

			List<TaggedGroupedEntities> groupedMPsWithTags = new List<TaggedGroupedEntities>();
			foreach(IGrouping<ParliamentData.Chamber, MP> group in groupedMPs)
			{
				groupedMPsWithTags.Add(new TaggedGroupedEntities(
					group.Key,
					wrapInTags<MP>(new ObservableCollection<Entity>(group), selectedMPs)
				));
			}
			AuthorityListView.BindingContext = groupedMPsWithTags;
			AuthorityListView.GroupDisplayBinding = new Binding("Chamber");
			AuthorityListView.ItemsSource = groupedMPsWithTags;
			
			// Flat list for the purposes of updating/saving
			//selectableMPs 
			//	= new ObservableCollection<Tag<MP>>(groupedMPsWithTags.SelectMany(x => x).ToList());
			selectableEntities	= new ObservableCollection<Tag<Entity>>(groupedMPsWithTags.SelectMany(x => x).ToList());
		}

		private class TaggedGroupedEntities : ObservableCollection<Tag<Entity>>
		{
			public TaggedGroupedEntities(ParliamentData.Chamber chamber, ObservableCollection<Tag<Entity>> entityGroup) : base(entityGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get; }
		}

		private void Authority_Selected(object sender, ItemTappedEventArgs e)
		{
			((Tag<Entity>) e.Item).Toggle();
		}

		// TODO Consider whether the semantics of 'back' should be different from
		// 'done', i.e. whether 'back' should undo.
		// Also consider whether this should raise a warning if neither of the types match.
		// 
		// The code here is inelegant, because it really does need to know the type of the  '
		// list it's updating. Can't really see a way round that unfortunately.
		async void DoneButton_OnClicked(object sender, EventArgs e)
		{
			if (typeOfEntities == typeof(MP))
			{
				UpdateSelectedList<MP>(selectableEntities, selectedMPs);
			}

			if (typeOfEntities == typeof(Authority))
			{
				UpdateSelectedList<Authority>(selectableEntities, selectedAuthorities);
			}

			await Navigation.PopAsync();
		}

		// TODO*** This will need to be careful about which lists it is.
		private void UpdateSelectedList<T>(ObservableCollection<Tag<Entity>> selectableEntities, ObservableCollection<T> selectedEntities) where T:Entity
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
		
		protected ObservableCollection<Tag<Entity>> wrapInTags<T>(ObservableCollection<Entity>
			entities, ObservableCollection<T> selectedEntities) where T : Entity
		{
			return new ObservableCollection<Tag<Entity>>(entities.Select
				(a => a.WrapInTag(selectedEntities.Contains(a)))
			);
		}
	}
}