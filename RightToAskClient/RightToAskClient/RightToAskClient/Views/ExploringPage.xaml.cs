using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient
{
	/*
	 * Inputs a list of tags to be displayed in a way that allows the user
	 * to select some.  allEntites is the complete list of possible entities
	 * for selection. selectedEntities stores the list of
	 * selected Tags, to be updated when the page is popped.
	 * Optionally inputs a message to display at the top of the page.
	 */
	public partial class ExploringPage : ContentPage
	{
		protected ObservableCollection<Tag> selectableEntities;
		protected ObservableCollection<Entity> selectedEntities;

		public ExploringPage(ObservableCollection<Entity> allEntities, 
			ObservableCollection<Entity> selectedEntities, string message=null)
		{
			InitializeComponent();

			introText.Text = message;

			this.selectedEntities = selectedEntities;
			selectableEntities = wrapInTags(allEntities, selectedEntities);
			
			BindingContext = selectableEntities;
			AuthorityListView.ItemsSource = selectableEntities;
		} 

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