using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Models;
using RightToAskClient.Resx;
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

		private ObservableCollection<Tag<Entity>> _filteredSelectableEntities = new ObservableCollection<Tag<Entity>>();
		public ObservableCollection<Tag<Entity>> FilteredSelectableEntities
		{
			get => _filteredSelectableEntities;
			private set => SetProperty(ref _filteredSelectableEntities, value);
		}

		private bool _showFilteredResults = false;
		public bool ShowFilteredResults
		{
			get => _showFilteredResults;
			set => SetProperty(ref _showFilteredResults, value);
		}

    private ObservableCollection<TaggedGroupedEntities> _selectableGroupedEntities; 
		public ObservableCollection<TaggedGroupedEntities> SelectableGroupedEntities
		{
			get => _selectableGroupedEntities;
			private set
			{
				_selectableGroupedEntities = value;
				OnPropertyChanged();
			}
		}

		// The already-selected ones, for display at the top.
		public IEnumerable<Tag<Entity>> PreSelectedEntities
		{
			get => SelectableEntities.Where(te => te.Selected);
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

		private Binding _groupDisplay = new Binding("Chamber");
		public Binding GroupDisplay
		{
			get => _groupDisplay;
		}

		private string _doneButtonText = AppResources.NextButtonText;
		public string DoneButtonText
		{
			get => _doneButtonText;
			set => SetProperty(ref _doneButtonText, value);
		}

		private bool _showSearchFrame = false;
		public bool ShowSearchFrame
		{
			get => _showSearchFrame;
			set => SetProperty(ref _showSearchFrame, value);
		}

		private string _keyword = "";
		public string Keyword
		{
			get => _keyword;
			set
			{
				bool changed = SetProperty(ref _keyword, value);
				if (changed)
				{
					App.ReadingContext.Filters.SearchKeyword = _keyword;
                }
				FilteredSelectableEntities = GetSearchResults(_keyword);
				//ShowFilteredResults = !string.IsNullOrEmpty(_keyword);
				// after clearing the filtered list away and putting if off screen, update the original list with selected toggles
     //           if (string.IsNullOrEmpty(_keyword))
     //           {
					//foreach(Tag<Entity> selectedEntity in FilteredSelectableEntities)
     //               {
     //                   if (selectedEntity.Selected)
     //                   {
					//		Tag<Entity> temp = SelectableEntities.Where(e => e.TagEntity == selectedEntity.TagEntity).SingleOrDefault();
					//		if(!temp.Selected)
					//		temp.Toggle();
     //                   }
     //                   else
     //                   {
					//		Tag<Entity> temp = SelectableEntities.Where(e => e.TagEntity == selectedEntity.TagEntity).SingleOrDefault();
					//		if (temp.Selected)
					//			temp.Toggle();
					//	}
     //               }
     //           }
			}
		}

		public IAsyncCommand DoneButtonCommand { get; }
		public Command SearchToolbarCommand { get; }
		public Command ApplySearchCommand { get; }
		// public IAsyncCommand HomeButtonCommand { get;  }
		// public Command<(object,ItemTappedEventArgs)> EntitySelectedCommand { get;  }
		// public EventHandler<ItemTappedEventArgs> EntitySelectedEventHandler { get; }

		// protected ObservableCollection<Authority> SelectedAuthorities = new ObservableCollection<Authority>();
		// protected ObservableCollection<MP> SelectedMPs = new ObservableCollection<MP>();
		// protected ObservableCollection<Person> SelectedPeople = new ObservableCollection<Person>();

		// TODO These are just copy-pasted from the old code-behind. Might need a bit more thought.
		public bool CameFromReg2Page = false;
		public bool GoToReadingPageFinally = false;
		public bool GoToAskingPageNext = false;



        public SelectableListViewModel(SelectableList<Authority> authorityLists , string message) 
		{
			Keyword = App.ReadingContext.Filters.SearchKeyword;
			// SelectedAuthorities = new ObservableCollection<Authority>(authorityLists.SelectedEntities);
			// AllEntities = new ObservableCollection<Entity>(authorityLists.AllEntities); 
			SelectableEntities = WrapInTagsAndSortPreselections(new ObservableCollection<Entity>(authorityLists.AllEntities),  
								authorityLists.SelectedEntities);

			_titleText = "Authorities";
			IntroText = message;
			PopupLabelText = AppResources.SelectableListAuthoritiesPopupText;
			DoneButtonCommand = new AsyncCommand(async () =>
            {
                DoneButton_OnClicked(
	                () => UpdateSelectedList<Authority>(authorityLists)       
	                );
				MessagingCenter.Send(this, "UpdateFilters");
            });
			SearchToolbarCommand = new Command(() =>
			{
				ShowSearchFrame = !ShowSearchFrame; // just toggle it
			});
			ApplySearchCommand = new Command(() => 
			{
				// TODO: actually write a method to search through the list of entities
				FilteredSelectableEntities = GetSearchResults(_keyword);
			});
			SubscribeToTheRightMessages();
		}

		public SelectableListViewModel(SelectableList<MP> mpLists, string message)
		{
			Keyword = App.ReadingContext.Filters.SearchKeyword;
			SelectableEntities = WrapInTagsAndSortPreselections(new ObservableCollection<Entity>(mpLists.AllEntities),  mpLists.SelectedEntities);
			
			_titleText = "MPs";
			IntroText = message;
			PopupLabelText = AppResources.SelectableListMPsPopupText;
			DoneButtonCommand = new AsyncCommand(async () =>
            {
                DoneButton_OnClicked(
	                () => UpdateSelectedList<MP>(mpLists)       
	                );
				MessagingCenter.Send(this, "UpdateFilters");
            });
			SearchToolbarCommand = new Command(() =>
			{
				ShowSearchFrame = !ShowSearchFrame; // just toggle it
			});
			ApplySearchCommand = new Command(() =>
			{
				// TODO: actually write a method to search through the list of entities
				FilteredSelectableEntities = GetSearchResults(_keyword);
			});

			SubscribeToTheRightMessages();
		}
		
		// MPs are grouped only for display, but stored in simple (flat) lists.
		// If the grouping boolean is set, group the MPs by chamber before display. 
        public SelectableListViewModel(SelectableList<MP> mpLists, string message, bool grouping)
        {
			Keyword = App.ReadingContext.Filters.SearchKeyword;
			if (grouping)
	        {
		        var groupedMPs = mpLists.AllEntities.GroupBy(mp => mp.electorate.chamber);
		        List<TaggedGroupedEntities> groupedMPsWithTags = new List<TaggedGroupedEntities>();
		        foreach (IGrouping<ParliamentData.Chamber, MP> group in groupedMPs)
		        {
			        groupedMPsWithTags.Add(new TaggedGroupedEntities(
				        group.Key,
				        WrapInTagsAndSortPreselections(new ObservableCollection<Entity>(group), mpLists.SelectedEntities)
			        ));
		        }

		        // For display in groups
		        SelectableGroupedEntities = new ObservableCollection<TaggedGroupedEntities>(groupedMPsWithTags);
		        // For setting the selected/unselected ones 
		        SelectableEntities = new ObservableCollection<Tag<Entity>>(groupedMPsWithTags.SelectMany(x => x).ToList());
	        } 
	        else
			{
				SelectableEntities = WrapInTagsAndSortPreselections(new ObservableCollection<Entity>(mpLists.AllEntities),
					mpLists.SelectedEntities);
			}
	        
			_titleText = "Grouped MPs";
			IntroText = message;
			PopupLabelText = AppResources.SelectableListMPsPopupText;
			DoneButtonCommand = new AsyncCommand(async () =>
            {
                DoneButton_OnClicked(
	                () => UpdateSelectedList<MP>(mpLists)       
	                );
				MessagingCenter.Send(this, "UpdateFilters");
            });
			SearchToolbarCommand = new Command(() =>
			{
				ShowSearchFrame = !ShowSearchFrame; // just toggle it
			});
			ApplySearchCommand = new Command(() =>
			{
				// TODO: actually write a method to search through the list of entities
				FilteredSelectableEntities = GetSearchResults(_keyword);
			});

			SubscribeToTheRightMessages();
        }

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
			MessagingCenter.Subscribe<FindMPsViewModel>(this, "GoToReadingPage", (sender) =>
			{
				GoToReadingPageFinally = true;
				MessagingCenter.Unsubscribe<FindMPsViewModel>(this, "GoToReadingPage");
			});
			MessagingCenter.Subscribe<QuestionViewModel>(this, "GoToReadingPage", (sender) =>
			{
				GoToReadingPageFinally = true;
				MessagingCenter.Unsubscribe<QuestionViewModel>(this, "GoToReadingPage");
			});
			MessagingCenter.Subscribe<QuestionViewModel>(this, "OptionBGoToAskingPageNext", (sender) =>
			{
				GoToAskingPageNext = true;
				MessagingCenter.Unsubscribe<QuestionViewModel>(this, "OptionBGoToAskingPageNext");
			});
			MessagingCenter.Subscribe<FilterViewModel>(this, "FromFiltersPage", (sender) =>
			{
				DoneButtonText = AppResources.DoneButtonText;
				MessagingCenter.Unsubscribe<FilterViewModel>(this, "FromFiltersPage");
			});
		}

		public class TaggedGroupedEntities : ObservableCollection<Tag<Entity>>
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
		private async void DoneButton_OnClicked(Action updateAction)
		{
			// reset keyword search filter
			Keyword = "";
			updateAction();
			// Option B. We've finished choosing who should answer it, so now find out who should ask it.
			if (GoToAskingPageNext)
            {
				await Shell.Current.GoToAsync(nameof(QuestionAskerPage));
            }
			// Either Option A, or Option B and we've finished finding out who should ask it.
			else if (GoToReadingPageFinally)
			{
				await Shell.Current.GoToAsync(nameof(ReadingPage));
			}
			// For Advanced Search outside the main flow. Pop back to wherever we came from (i.e. the advance search page).
            else
            {
				await Shell.Current.Navigation.PopAsync();
			}
			MessagingCenter.Send(this, "UpdateFilters");
		}
			
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
			
			// There shouldn't be any duplicates, but this will remove them just in case.
			entities.SelectedEntities = newSelectedEntities.Distinct().ToList();
		}
		
		
	    // Wrap the entities in tags, with Selected toggled according to whether the entity
	    // is in the selectedEntities list or not.
	    private ObservableCollection<Tag<Entity>> WrapInTagsAndSortPreselections<T>(IEnumerable<Entity>
		 	entities, IEnumerable<T> selectedEntities) where T : Entity
		{
			return new ObservableCollection<Tag<Entity>>(entities.Select
				(a => a.WrapInTag(selectedEntities.Contains(a))).OrderByDescending(t => t.Selected)
			);
		}

		//private void OnKeywordChanged(object sender, TextChangedEventArgs e)
		//{
		//	_searchingFor = e.NewTextValue;
		//	if (!String.IsNullOrWhiteSpace(_searchingFor))
		//	{
		//		ObservableCollection<Tag<Entity>> listToDisplay = GetSearchResults(_searchingFor);
		//		AuthorityListView.ItemsSource = listToDisplay;
		//	}
		//}

		private ObservableCollection<Tag<Entity>> GetSearchResults(string queryString)
		{
			return new ObservableCollection<Tag<Entity>>(
						SelectableEntities.Where(f => f.NameContains(queryString)));
		}
	}
}