using RightToAskClient.Controls;
using RightToAskClient.Models;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Xamarin.CommunityToolkit.ObjectModel;
using RightToAskClient.Resx;
using Xamarin.Forms;
using System.Threading.Tasks;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.ViewModels
{
    public class ClickableListViewModel : BaseViewModel 
    {
        // properties

        private static Func<string> createText;
            
        public string ListDisplayText
        {
            get => createText();
        }

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
            createText = (() => CreateTextGivenListSelectableEntities<MP>(mpList));
        }
        
        public ClickableListViewModel(SelectableList<Authority> authorityList)
        {
            SubscribeToTheRightMessages();
            createText = (() => CreateTextGivenListSelectableEntities<Authority>(authorityList));
        }

        // commands
        public Command EditListCommand { get; set; }
        
        private string CreateTextGivenListSelectableEntities<T>(SelectableList<T> entityList) where T:Entity
        {
            string namesList = String.Join(", ", entityList.SelectedEntities.Select(e => e.ShortestName));
            return String.IsNullOrEmpty(namesList) ? AppResources.NoneDefaultText : namesList ;
        }

        // For updating the display when the data from the filters has changed.
        private void SubscribeToTheRightMessages()
        {
            
            MessagingCenter.Subscribe<QuestionViewModel>(this, "UpdateFilters", (sender) =>
            {
                ReInitData();
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, "UpdateFilters");
            });
            MessagingCenter.Subscribe<SelectableListViewModel>(this, "UpdateFilters", (sender) =>
            {
                ReInitData();
                // Normally we'd want to unsubscribe to prevent multiple instances of the subscriber from happening,
                // but because these listeners happen when popping back to this page from a selectableList page we want to keep the listener/subscriber
                // active to update all of the lists/filters on this page with the newly selected data
                //MessagingCenter.Unsubscribe<SelectableListViewModel>(this, "UpdateFilters");
            });
        }
        
        private void ReInitData() {
            OnPropertyChanged("ListDisplayText");
        }
        
    }
}
