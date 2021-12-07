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
		private ObservableCollection<Entity> allEntities;
		protected ObservableCollection<Tag> selectableEntities;
		protected ObservableCollection<Entity> selectedEntities;

		public ExploringPage(ObservableCollection<Entity> allEntities, 
			ObservableCollection<Entity> selectedEntities, string message=null)
		{
			InitializeComponent();

			IntroText.Text = message;

			this.allEntities = allEntities;
			this.selectedEntities = selectedEntities;
				
			selectableEntities = wrapInTags(allEntities, selectedEntities);
			
			//AuthorityListView.ItemTemplate = (DataTemplate)Application.Current.Resources["SelectableDataTemplate"];
			AuthorityListView.BindingContext = selectableEntities;
			AuthorityListView.ItemsSource = selectableEntities;
		}

		/*
		public ExploringPage(
			List<(ObservableCollection<Entity> someEntities, ObservableCollection<Entity> selectedOnes, string heading)>
				entities, string message = null)
		{
			IntroText.Text = message;
			var tagWrappedGroups = entities.Select(e => wrapInTags(e.someEntities, e.selectedOnes));
			AuthorityListView.IsGroupingEnabled = true;
			AuthorityListView.BindingContext = tagWrappedGroups;
		} */

		/*
		public ExploringPage(IEnumerable<GroupedMPs> groupedMPs, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			var tagWrappedGroups = groupedMPs.Select(g => wrapInTags(g.MPsInGroup, g.BlankSelections));
			AuthorityListView.IsGroupingEnabled = true;
			AuthorityListView.BindingContext = tagWrappedGroups;
		}
		*/

		/*
		public ExploringPage(IEnumerable<IGrouping<ParliamentData.Chamber, Entity>> groupedMPs, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			AuthorityListView.IsGroupingEnabled = true;
			AuthorityListView.BindingContext = groupedMPs;
			AuthorityListView.ItemsSource = groupedMPs;
		}
		*/

		// TODO wrap MPs in selectable tags.
		/* THis one works.
		public ExploringPage(IEnumerable<GroupedMPs> groupedMPs, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			AuthorityListView.IsGroupingEnabled = true;
			AuthorityListView.BindingContext = groupedMPs;
			AuthorityListView.GroupDisplayBinding = new Binding("Chamber");
			AuthorityListView.ItemsSource = groupedMPs;
		}*/
		
		public ExploringPage(IEnumerable<GroupedMPs> groupedMPs, ObservableCollection<Entity> selectedEntities, string message)
		{
			InitializeComponent();
			
			IntroText.Text = message;
			this.selectedEntities = selectedEntities;
			
			AuthorityListView.IsGroupingEnabled = true;

			List<TaggedGroupedMPs> groupedMPsWithTags = new List<TaggedGroupedMPs>();
			foreach(GroupedMPs group in groupedMPs)
			{
				groupedMPsWithTags.Add(new TaggedGroupedMPs(
					group.Chamber,
					wrapInTags(group, selectedEntities)
				));
			}
			AuthorityListView.BindingContext = groupedMPsWithTags;
			AuthorityListView.GroupDisplayBinding = new Binding("Chamber");
			AuthorityListView.ItemsSource = groupedMPsWithTags;
			
			// Flat list for the purposes of updating/saving
			this.selectableEntities 
				= new ObservableCollection<Tag>(groupedMPsWithTags.SelectMany(x => x).ToList());
		}

		private class TaggedGroupedMPs : ObservableCollection<Tag>
		{
			public TaggedGroupedMPs(ParliamentData.Chamber chamber, ObservableCollection<Tag> mpGroup) : base(mpGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get; }
		}
		
		/*
		public ExploringPage(List<(ObservableCollection<Entity> someEntities, ObservableCollection<Entity> selectedOnes , string heading)> entities, string message = null)
		{
			
			InitializeComponent();

			IntroText.Text = message;
			this.entitiesList = entities;

			if (entities.Count > 0)
			{
				this.allEntities = entities[0].someEntities;
				this.selectedEntities =	entities[0].selectedOnes; 

				selectableEntities = wrapInTags(allEntities, selectedEntities);

				AuthorityListView.Header = entities[0].heading; 
				AuthorityListView.BindingContext = selectableEntities;
				AuthorityListView.ItemsSource = selectableEntities;
			}

			for (int i = 0; i < entities.Count; i++)
			{
				var newSelectableEntities = wrapInTags(entities[i].someEntities, entities[i].someEntities);
				var listView = new ListView()
				{
					Header = entities[i].heading,
					BindingContext = newSelectableEntities,
					ItemsSource = newSelectableEntities
				};
			} 
		} */

		private void Authority_Selected(object sender, ItemTappedEventArgs e)
		{
			((Tag) e.Item).Selected = !((Tag) e.Item).Selected;
		}

		// Note: At the moment, this simply pops the page, i.e. the same
		// as Back.
		// Consider whether the semantics of 'back' should be different from
		// 'done', i.e. whether 'back' should undo.
		async void DoneButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList();
			
			await Navigation.PopAsync();
		}

		// TODO*** This will need to be careful about which lists it is.
		private void UpdateSelectedList()
		{
			var toBeIncluded = selectableEntities.Where(w => w.Selected).Select(t => t.TagEntity);	
			foreach (Entity selectedEntity in toBeIncluded)
			{
				if (!selectedEntities.Contains(selectedEntity))
				{
					selectedEntities.Add(selectedEntity);	
				}
			}
			
			var toBeRemoved = selectableEntities.Where(w => !w.Selected).Select(t => t.TagEntity);
			foreach (Entity notSelectedEntity in toBeRemoved)
			{
				selectedEntities.Remove(notSelectedEntity);	
			}
		}
		
		protected ObservableCollection<Tag> wrapInTags(ObservableCollection<Entity> entities, ObservableCollection<Entity> selectedEntities)
		{
			return new ObservableCollection<Tag>(entities.Select
				(authority => new Tag
					{
						TagEntity = authority,
						Selected =  selectedEntities.Contains(authority)
					}
				)
			);
		}
	}
}