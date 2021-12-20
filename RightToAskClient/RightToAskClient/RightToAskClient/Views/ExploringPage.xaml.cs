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
		protected readonly ObservableCollection<Entity> allEntities;
		protected readonly ObservableCollection<Tag<Entity>> selectableEntities;
		
		// TODO: I would like to be able to use the type system to avoid this doubling-up, but 
		// I can't figure out how to do it. The roles of these three selected-lists are almost the
		// same regardless of their type (Authority, MP or Person), but they need to be
		// separate lists, and the code needs to know which one it's got, because we insert 
		// into them and I can't figure out how to do that elegantly in a consistently generic way.
		// It only matters for editing the selected elements at return time  - see OnDoneButton_Clicked (*)
		// below - if we could return a 
		// generic from this class, it would work easily, but I don't think we can.
		protected ObservableCollection<Authority> selectedAuthorities = new ObservableCollection<Authority>();
		protected ObservableCollection<MP> selectedMPs = new ObservableCollection<MP>();
		protected ObservableCollection<Person> selectedPeople = new ObservableCollection<Person>();
		protected Type typeOfEntities;

		public ExploringPage(ObservableCollection<MP> allEntities, 
			ObservableCollection<MP> selectedEntities, string? message=null)
		{
			InitializeComponent();
			
			typeOfEntities = typeof(MP);
			selectedMPs = selectedEntities;
			this.allEntities = new ObservableCollection<Entity>(allEntities);
			selectableEntities = wrapInTags<MP>(this.allEntities, selectedEntities);
			DoneButton.Clicked += DoneMPsButton_OnClicked;
			
			SetUpSelectableEntitiesAndIntroText(message);
		}
		 
		/* This constructor is only used for Authorities, and hence assumed that the list to be selected from
		 * consists of the complete list of authorities.
		 */
		public ExploringPage(ObservableCollection<Authority> selectedEntities, string message) 
		{
			InitializeComponent();

			typeOfEntities = typeof(Authority);
			selectedAuthorities = selectedEntities;
			allEntities = new ObservableCollection<Entity>(ParliamentData.AllAuthorities);
			selectableEntities = wrapInTags<Authority>(allEntities, selectedEntities);
			DoneButton.Clicked += DoneAuthoritiesButton_OnClicked;

			SetUpSelectableEntitiesAndIntroText(message);	
		}

		public ExploringPage(IEnumerable<IGrouping<ParliamentData.Chamber, MP>> groupedMPs, ObservableCollection<MP> selectedMPs, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			this.selectedMPs = selectedMPs;
			DoneButton.Clicked += DoneMPsButton_OnClicked;
			
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

		private void SetUpSelectableEntitiesAndIntroText(string message)
		{
			IntroText.Text = message ?? "";
			AuthorityListView.BindingContext = selectableEntities;
			AuthorityListView.ItemsSource = selectableEntities;
		}
		private class TaggedGroupedEntities : ObservableCollection<Tag<Entity>>
		{
			public TaggedGroupedEntities(ParliamentData.Chamber chamber, ObservableCollection<Tag<Entity>> entityGroup) : base(entityGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get; }
		}

		private void OnEntity_Selected(object sender, ItemTappedEventArgs e)
		{
			((Tag<Entity>) e.Item).Toggle();
		}

		// TODO Consider whether the semantics of 'back' should be different from
		// 'done', i.e. whether 'back' should undo.
		// Also consider whether this should raise a warning if neither of the types match.
		// 
		// TODO: (*) The code here is inelegant, because it really does need to know the type of the  '
		// list it's updating. Can't really see a way round that unfortunately.
		// Possibly some dynamic type lookup?
		async void DoneMPsButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList<MP>(selectableEntities, selectedMPs);
			await Navigation.PopAsync();
		}
		
		async void DoneAuthoritiesButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList<Authority>(selectableEntities, selectedAuthorities);
			await Navigation.PopAsync();
		}
			
		async void DonePeopleButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList<Person>(selectableEntities, selectedPeople);
			await Navigation.PopAsync();
		}

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