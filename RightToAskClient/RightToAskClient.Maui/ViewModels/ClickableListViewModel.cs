using RightToAskClient.Maui.Models;
using System;
using System.Linq;
using RightToAskClient.Maui.Resx;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.ViewModels
{
    public class ClickableListViewModel : BaseViewModel 
    {
        // properties

        private readonly Func<string> createText;

        public string ListDisplayText => createText();

        private readonly Func<bool> _anySelections;
        public bool AnySelections => _anySelections();

        private string _heading = "";

        public string Heading
        {
            get => _heading;
            set => SetProperty(ref _heading, value);
        }

        // constructors
        // We shouldn't really have to do this separately for each type - this is a workaround
        // because xaml doesn't (yet) handle generics.
        public ClickableListViewModel(SelectableList<MP> mpList)
        {
            SubscribeToTheRightMessages();
            createText = (() => CreateTextGivenListSelectableEntities(mpList));
            _anySelections = () => mpList.SelectedEntities.Any();
        }
        
        public ClickableListViewModel(SelectableList<Authority> authorityList)
        {
            SubscribeToTheRightMessages();
            createText = (() => CreateTextGivenListSelectableEntities(authorityList));
            _anySelections = () => authorityList.SelectedEntities.Any();
        }

        public ClickableListViewModel(SelectableList<Committee> committeeList)
        {
            SubscribeToTheRightMessages();
            createText = (() => CreateTextGivenListSelectableEntities(committeeList));
            _anySelections = () => committeeList.SelectedEntities.Any();
        }

        // commands
        public Command EditListCommand { get; set; }
        
        private string CreateTextGivenListSelectableEntities<T>(SelectableList<T> entityList) where T:Entity
        {
            var namesList = string.Join(", ", entityList.SelectedEntities.Select(e => e.ShortestName));
            return string.IsNullOrEmpty(namesList) ? AppResources.NoneDefaultText : namesList ;
        }

        // For updating the display when the data from the filters has changed.
        private void SubscribeToTheRightMessages()
        {
            MessagingCenter.Subscribe<FilterViewModel>(this, Constants.UpdateFilters, (sender) =>
            {
                ReInitData();
            });
            
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.UpdateFilters, (sender) =>
            {
                ReInitData();
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, Constants.UpdateFilters);
            });
            MessagingCenter.Subscribe<SelectableListViewModel>(this, Constants.UpdateFilters, (sender) =>
            {
                ReInitData();
                // Normally we'd want to unsubscribe to prevent multiple instances of the subscriber from happening,
                // but because these listeners happen when popping back to this page from a selectableList page we want to keep the listener/subscriber
                // active to update all of the lists/filters on this page with the newly selected data
                //MessagingCenter.Unsubscribe<SelectableListViewModel>(this, Constants.UpdateFilters);
            });
        }
        
        private void ReInitData() {
            OnPropertyChanged("ListDisplayText");
        }
        
    }
}
