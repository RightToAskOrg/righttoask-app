using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Models;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

// xaml code-behind can't be generic, but View Models can be.
namespace RightToAskClient.ViewModels
{
    public class SelectableListViewModel : BaseViewModel
    {
		// private readonly object _entityLists;
		// protected readonly ObservableCollection<Entity> AllEntities = new ObservableCollection<Entity>();

		private ObservableCollection<Tag<Entity>> _selectableEntities = new ObservableCollection<Tag<Entity>>();
		public ObservableCollection<Tag<Entity>> SelectableEntities
		{
			get => _selectableEntities;
			private set => SetProperty(ref _selectableEntities, value);
		}

		private string _introText = "";
		public string IntroText
		{
			get => _introText;
			set => SetProperty(ref _introText, value);
		}

		private string _titleText = "";
		public string TitleText
		{
			get => _titleText;
			set => SetProperty(ref _titleText, value);
		}

		// Only relevant for groups of MPs
		private bool _enableGrouping = false;

		public bool EnableGrouping
		{
			get => _enableGrouping;
		}
		private Binding _groupDisplay = new Binding("Chamber");
		public Binding GroupDisplay
		{
			get => _groupDisplay;
		} 

		public IAsyncCommand DoneButtonCommand { get;  }
		// public IAsyncCommand HomeButtonCommand { get;  }
		// public Command<(object,ItemTappedEventArgs)> EntitySelectedCommand { get;  }
		// public EventHandler<ItemTappedEventArgs> EntitySelectedEventHandler { get; }

		// protected ObservableCollection<Authority> SelectedAuthorities = new ObservableCollection<Authority>();
		// protected ObservableCollection<MP> SelectedMPs = new ObservableCollection<MP>();
		// protected ObservableCollection<Person> SelectedPeople = new ObservableCollection<Person>();

		// TODO These are just copy-pasted from the old code-behind. Might need a bit more thought.
		public bool CameFromReg2Page = false;
		public bool GoToReadingPageNext = false;
		public bool OptionB = false;



        public SelectableListViewModel(SelectableList<Authority> authorityLists , string message) 
		{
			// SelectedAuthorities = new ObservableCollection<Authority>(authorityLists.SelectedEntities);
			// AllEntities = new ObservableCollection<Entity>(authorityLists.AllEntities); 
			SelectableEntities = wrapInTags(new ObservableCollection<Entity>(authorityLists.AllEntities),  
								authorityLists.SelectedEntities);

			_titleText = message;
            DoneButtonCommand = new AsyncCommand(async () =>
            {
                DoneButton_OnClicked(
	                () => UpdateSelectedList<Authority>(authorityLists)       
	                );
				MessagingCenter.Send(this, "UpdateFilters");
            });
            
			SubscribeToTheRightMessages();
		}

		public SelectableListViewModel(SelectableList<MP> mpLists, string message)
		{
			SelectableEntities = wrapInTags(new ObservableCollection<Entity>(mpLists.AllEntities),  mpLists.SelectedEntities);
			
			_titleText = message;
            DoneButtonCommand = new AsyncCommand(async () =>
            {
                DoneButton_OnClicked(
	                () => UpdateSelectedList<MP>(mpLists)       
	                );
				MessagingCenter.Send(this, "UpdateFilters");
            });

            SubscribeToTheRightMessages();
		}
		
		// MPs are grouped only for display, but stored in simple (flat) lists.
		// If the grouping boolean is set, group the MPs by chamber before display, and set the
		// bindable GroupingEnabled on.
        public SelectableListViewModel(SelectableList<MP> mpLists, string message, bool grouping)
        {
	        _enableGrouping = grouping;
	        if (grouping)
	        {
		        var groupedMPs = mpLists.AllEntities.GroupBy(mp => mp.electorate.chamber);
		        List<TaggedGroupedEntities> groupedMPsWithTags = new List<TaggedGroupedEntities>();
		        foreach (IGrouping<ParliamentData.Chamber, MP> group in groupedMPs)
		        {
			        groupedMPsWithTags.Add(new TaggedGroupedEntities(
				        group.Key,
				        wrapInTags(new ObservableCollection<Entity>(group), mpLists.SelectedEntities)
			        ));
		        }

		        SelectableEntities =
			        new ObservableCollection<Tag<Entity>>(groupedMPsWithTags.SelectMany(x => x).ToList());
	        } 
	        else
	        {
				SelectableEntities = wrapInTags(new ObservableCollection<Entity>(mpLists.AllEntities),  mpLists.SelectedEntities);
	        }
	        
			_titleText = message;
            DoneButtonCommand = new AsyncCommand(async () =>
            {
                DoneButton_OnClicked(
	                () => UpdateSelectedList<MP>(mpLists)       
	                );
				MessagingCenter.Send(this, "UpdateFilters");
            });
            
			SubscribeToTheRightMessages();
        }

        /*
		public SelectableListPage(IEnumerable<IGrouping<ParliamentData.Chamber, MP>> groupedMPs, ObservableCollection<MP> selectedMPs, string message)
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

        /*
		private async void HomeButton_Clicked()
		{
			string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go home? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
			if (result == "Yes, I'm sure.")
			{
				await App.Current.MainPage.Navigation.PopToRootAsync();
			}
		}
		*/
		/*
		private void SetUpSelectableEntitiesAndIntroText(string message)
		{
			// IntroText.Text = message;
			// AuthorityListView.BindingContext = SelectableEntities;
			// AuthorityListView.ItemsSource = SelectableEntities;
		}
		*/
		private void SubscribeToTheRightMessages()
		{
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
		private class TaggedGroupedEntities : ObservableCollection<Tag<Entity>>
		{
			public TaggedGroupedEntities(ParliamentData.Chamber chamber, ObservableCollection<Tag<Entity>> entityGroup) : base(entityGroup)
			{
				Chamber = chamber.ToString();
			}

			public string Chamber { get;  }
		}

		// TODO consider whether this can be here in the ViewModel rather than in the code-behind (where
		// it is now). 
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
		// TODO The DoneXButton_OnClicked functions could be unified because the if(camefromreg2page) 
		// only applies to MPs and could be harmlessly included in the general function.
		/*
		async void DoneMPsButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList(SelectedMPs);
            if (CameFromReg2Page)
            {
				CameFromReg2Page = false;
				await Shell.Current.GoToAsync("../.."); // double pop
			}
			else if (GoToReadingPageNext && !OptionB)
            {
				//SelectedOptionA = false;
				await Shell.Current.GoToAsync(nameof(ReadingPage));
            }
			else if (OptionB)
            {
				await Shell.Current.GoToAsync(nameof(QuestionAskerPage));
			}
            else
            {
				await Shell.Current.Navigation.PopAsync(); // single pop
			}
			MessagingCenter.Send(this, "UpdateFilters");
		}
		*/

		private async void DoneButton_OnClicked(Action updateAction)
		{
			updateAction();
			if (OptionB)
            {
				await Shell.Current.GoToAsync(nameof(QuestionAskerPage));
            }
			else if (GoToReadingPageNext && !OptionB)
			{
				//SelectedOptionA = false;
				await Shell.Current.GoToAsync(nameof(ReadingPage));
			}
            else
            {
				await Shell.Current.Navigation.PopAsync();
			}
			// MessagingCenter.Send(this, "UpdateFilters");
		}
			
		/*
		async void DonePeopleButton_OnClicked(object sender, EventArgs e)
		{
			UpdateSelectedList(selectableEntities, selectedPeople);
			await Navigation.PopAsync();
		}
		*/

		private void UpdateSelectedList<T>(SelectableList<T> entities) where T:Entity
		{
			var newSelectedEntities = new List<T>();
			
			var toBeIncluded = SelectableEntities.Where(w => w.Selected).Select(t => t.TagEntity);	
			foreach (Entity selectedEntity in toBeIncluded)
			{
				// There shouldn't be duplicates, but check just in case.
				if (!newSelectedEntities.Contains(selectedEntity))
				{
					if (selectedEntity is T s)
					{
						newSelectedEntities.Add(s);
						// OnPropertyChanged("SelectedAuthorities");
						// OnPropertyChanged("SelectedMPs");
						// OnPropertyChanged("SelectedPeople");
					}
				}
			}
			
			/*
			var toBeRemoved = SelectableEntities.Where(w => !w.Selected).Select(t => t.TagEntity);
			foreach (Entity notSelectedEntity in toBeRemoved)
			{
				if (notSelectedEntity is T s)
				{
					// FIXME This won't work because it makes a new list
					selectedEntities.ToList().Remove(s);
					// OnPropertyChanged("SelectedAuthorities");
					// OnPropertyChanged("SelectedMPs");
					// OnPropertyChanged("SelectedPeople");
				}
			}
			*/

			// There shouldn't be any duplicates, but this will remove them just in case.
			entities.SelectedEntities = newSelectedEntities.Distinct().ToList();
                    // Update the selected authorities as identified in the SelectableListPage.
                    // FIXME Unfortunately this doesn't ever seem to get called.
                    // App.ReadingContext.Filters.SelectedAuthorities =
                    // new ObservableCollection<Authority>(selectableList.SelectedEntities);
			// MessagingCenter.Send(this, "UpdateFilters");
		}
		
		
	    // Wrap the entities in tags, with Selected toggled according to whether the entity
	    // is in the selectedEntities list or not.
	    private ObservableCollection<Tag<Entity>> wrapInTags<T>(IEnumerable<Entity>
		 	entities, IEnumerable<T> selectedEntities) where T : Entity
		{
			return new ObservableCollection<Tag<Entity>>(entities.Select
				(a => a.WrapInTag(selectedEntities.Contains(a)))
			);
		}
	}
}