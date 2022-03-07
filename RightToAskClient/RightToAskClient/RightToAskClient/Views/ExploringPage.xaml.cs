using System;
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
	public partial class ExploringPage 
	{
		protected readonly ObservableCollection<Entity> AllEntities = new ObservableCollection<Entity>();
		protected readonly ObservableCollection<Tag<Entity>> SelectableEntities;
		
		// TODO: I would like to be able to use the type system to avoid this doubling-up, but 
		// I can't figure out how to do it. The roles of these three selected-lists are almost the
		// same regardless of their type (Authority, MP or Person), but they need to be
		// separate lists, and the code needs to know which one it's got, because we insert 
		// into them and I can't figure out how to do that elegantly in a consistently generic way.
		// It only matters for editing the selected elements at return time  - see OnDoneButton_Clicked (*)
		// below - if we could return a 
		// generic from this class, it would work easily, but I don't think we can.
		protected ObservableCollection<Authority> SelectedAuthorities = new ObservableCollection<Authority>();
		protected ObservableCollection<MP> SelectedMPs = new ObservableCollection<MP>();
		protected ObservableCollection<Person> SelectedPeople = new ObservableCollection<Person>();

		public bool CameFromReg2Page = false;

		public ExploringPage(ObservableCollection<MP> allEntities, 
			ObservableCollection<MP> selectedEntities, string message="")
		{
			InitializeComponent();
			
			SelectedMPs = selectedEntities;
			AllEntities = new ObservableCollection<Entity>(allEntities);
			SelectableEntities = wrapInTags(AllEntities, selectedEntities);
			DoneButton.Clicked += DoneMPsButton_OnClicked;
            HomeButton.Clicked += HomeButton_Clicked;
			
			SetUpSelectableEntitiesAndIntroText(message);

			MessagingCenter.Subscribe<FindMPsViewModel, bool>(this, "PreviousPage", (sender, arg) =>
			{
				if (arg)
                {
					CameFromReg2Page = true;
				}
				MessagingCenter.Unsubscribe<FindMPsViewModel, bool>(this, "PreviousPage");
			});
		}

        /* This constructor is only used for Authorities, and hence assumed that the list to be selected from
		 * consists of the complete list of authorities.
		 */
        public ExploringPage(ObservableCollection<Authority> selectedEntities, string message) 
		{
			InitializeComponent();

			SelectedAuthorities = selectedEntities;
			AllEntities = new ObservableCollection<Entity>(ParliamentData.AllAuthorities);
			SelectableEntities = wrapInTags(AllEntities, selectedEntities);
			DoneButton.Clicked += DoneAuthoritiesButton_OnClicked;
			HomeButton.Clicked += HomeButton_Clicked;

			SetUpSelectableEntitiesAndIntroText(message);

			MessagingCenter.Subscribe<FindMPsViewModel, bool>(this, "PreviousPage", (sender, arg) =>
			{
				if (arg)
				{
					CameFromReg2Page = true;
				}
				MessagingCenter.Unsubscribe<FindMPsViewModel, bool>(this, "PreviousPage");
			});
		}

		public ExploringPage(IEnumerable<IGrouping<ParliamentData.Chamber, MP>> groupedMPs, ObservableCollection<MP> selectedMPs, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			this.SelectedMPs = selectedMPs;
			DoneButton.Clicked += DoneMPsButton_OnClicked;
			HomeButton.Clicked += HomeButton_Clicked;

			AuthorityListView.IsGroupingEnabled = true;

			List<TaggedGroupedEntities> groupedMPsWithTags = new List<TaggedGroupedEntities>();
			foreach(IGrouping<ParliamentData.Chamber, MP> group in groupedMPs)
			{
				groupedMPsWithTags.Add(new TaggedGroupedEntities(
					group.Key,
					wrapInTags(new ObservableCollection<Entity>(group), selectedMPs)
				));
			}
			AuthorityListView.BindingContext = groupedMPsWithTags;
			AuthorityListView.GroupDisplayBinding = new Binding("Chamber");
			AuthorityListView.ItemsSource = groupedMPsWithTags;
			
			// Flat list for the purposes of updating/saving selectableMPs 
			//	= new ObservableCollection<Tag<MP>>(groupedMPsWithTags.SelectMany(x => x).ToList());
			SelectableEntities	= new ObservableCollection<Tag<Entity>>(groupedMPsWithTags.SelectMany(x => x).ToList());

			MessagingCenter.Subscribe<FindMPsViewModel, bool>(this, "PreviousPage", (sender, arg) =>
			{
				if (arg)
				{
					CameFromReg2Page = true;
				}
				MessagingCenter.Unsubscribe<FindMPsViewModel, bool>(this, "PreviousPage");
			});
		}

		private async void HomeButton_Clicked(object sender, EventArgs e)
		{
			string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go home? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
			if (result == "Yes, I'm sure.")
			{
				await App.Current.MainPage.Navigation.PopToRootAsync();
			}
		}
		private void SetUpSelectableEntitiesAndIntroText(string message)
		{
			IntroText.Text = message;
			AuthorityListView.BindingContext = SelectableEntities;
			AuthorityListView.ItemsSource = SelectableEntities;
		}
		private class TaggedGroupedEntities : ObservableCollection<Tag<Entity>>
		{
			public TaggedGroupedEntities(ParliamentData.Chamber chamber, ObservableCollection<Tag<Entity>> entityGroup) : base(entityGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get;  }
		}

		protected void OnEntity_Selected(object sender, ItemTappedEventArgs e)
		{
			if (e.Item is Tag<Entity> tag)
			{
				tag.Toggle();
			}
		}

		// TODO Consider whether the semantics of 'back' should be different from
		// 'done', i.e. whether 'back' should undo.
		// Also consider whether this should raise a warning if neither of the types match.
		// 
		// TODO: The code here is inelegant, because it really does need to know the type of the  '
		// list it's updating. Can't really see a way round that unfortunately.
		async void DoneMPsButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList(SelectedMPs);
            if (CameFromReg2Page)
            {
				CameFromReg2Page = false;
				await Shell.Current.GoToAsync("../.."); // double pop
			}
            else
            {
				await Navigation.PopAsync(); // single pop
			}			
		}
		
		async void DoneAuthoritiesButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList(SelectedAuthorities);
			await Navigation.PopAsync();
			//await Shell.Current.GoToAsync("../..");
		}
			
		/*
		async void DonePeopleButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList(selectableEntities, selectedPeople);
			await Navigation.PopAsync();
		}
		*/

		private void UpdateSelectedList<T>(ObservableCollection<T> selectedEntities) where T:Entity
		{
			var toBeIncluded = SelectableEntities.Where(w => w.Selected).Select(t => t.TagEntity);	
			foreach (Entity selectedEntity in toBeIncluded)
			{
				if (!selectedEntities.Contains(selectedEntity))
				{
					if (selectedEntity is T s)
					{
						selectedEntities.Add(s);
                        OnPropertyChanged("SelectedAuthorities");
                        OnPropertyChanged("SelectedMPs");
						OnPropertyChanged("SelectedPeople");
					}
				}
			}
			
			var toBeRemoved = SelectableEntities.Where(w => !w.Selected).Select(t => t.TagEntity);
			foreach (Entity notSelectedEntity in toBeRemoved)
			{
				if (notSelectedEntity is T s)
				{
					selectedEntities.Remove(s);
					OnPropertyChanged("SelectedAuthorities");
					OnPropertyChanged("SelectedMPs");
					OnPropertyChanged("SelectedPeople");
				}
			}
		}
		
		
	    // Wrap the entities in tags, with Selected toggled according to whether the entity
	    // is in the selectedEntities list or not.
	    private ObservableCollection<Tag<Entity>> wrapInTags<T>(ObservableCollection<Entity>
		 	entities, ObservableCollection<T> selectedEntities) where T : Entity
		{
			return new ObservableCollection<Tag<Entity>>(entities.Select
				(a => a.WrapInTag(selectedEntities.Contains(a)))
			);
		}
	}
}