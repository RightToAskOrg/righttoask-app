using System;
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
	 * to select some.  allEntites is the complete list of possible entities
	 * for selection. selectedEntities stores the list of
	 * selected Tags, to be updated when the page is popped.
	 * Optionally inputs a message to display at the top of the page.
	 */
	public partial class ExploringPage : ContentPage, INotifyPropertyChanged
	{
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
			recomputeSelectableLists();
				
			PropertyChanged += OnSourceEntitiesChanged;
			
			AuthorityListView.BindingContext = selectableEntities;
			AuthorityListView.ItemsSource = selectableEntities;
			
			AuthorityListView2.BindingContext = allEntities;
			AuthorityListView2.ItemsSource = allEntities;
		}

		private void recomputeSelectableLists()
		{
			selectableEntities = wrapInTags(allEntities, selectedEntities);
			OnPropertyChanged(nameof(selectableEntities));
		}

		private void Authority_Selected(object sender, ItemTappedEventArgs e)
		{
			((Tag) e.Item).Selected = !((Tag) e.Item).Selected;
		}

		//	private PropertyChangingEventHandler SourceEntitiesChanged;

		private void OnSourceEntitiesChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(allEntities))
			{
				//PropertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(nameof(allEntities)));
				//PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(selectableEntities)));
				recomputeSelectableLists();

			}
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
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		/*
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
		}
		*/
	}
}