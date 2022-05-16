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
			BindingContext = new SelectableListViewModel(authorityLists, message);
		}

        public SelectableListPage(SelectableList<MP> MPLists, string message, bool grouping)
        {
			InitializeComponent();
			BindingContext = new SelectableListViewModel(MPLists, message, grouping);
        }

        /*
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
			MessagingCenter.Subscribe<QuestionViewModel>(this, "GoToReadingPage", (sender) =>
			{
				GoToReadingPageNext = true;
				MessagingCenter.Unsubscribe<QuestionViewModel>(this, "GoToReadingPage");
			});
			MessagingCenter.Subscribe<QuestionViewModel>(this, "OptionB", (sender) =>
			{
				OptionB = true;
				MessagingCenter.Unsubscribe<QuestionViewModel>(this, "OptionB");
			});
		}
		*/

        // TODO Just use the one in BaseViewModel. 
		private async void HomeButton_Clicked(object sender, EventArgs e)
		{
			string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go home? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
			if (result == "Yes, I'm sure.")
			{
				await App.Current.MainPage.Navigation.PopToRootAsync();
			}
		}
		
		private class TaggedGroupedEntities : ObservableCollection<Tag<Entity>>
		{
			public TaggedGroupedEntities(ParliamentData.Chamber chamber, ObservableCollection<Tag<Entity>> entityGroup) : base(entityGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get;  }
		}

		/* It wold probably be more elegantly MVVM if this was in the ViewModel rather than the code-behind, but I
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
