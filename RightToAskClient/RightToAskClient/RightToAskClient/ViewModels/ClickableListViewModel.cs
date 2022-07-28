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

        public IEnumerable<Entity> EditableList;
        public string ListDisplayText => CreateTextGivenListEntities(EditableList?.ToList());

        private string _heading = "";

        public string Heading
        {
            get => _heading;
            set => SetProperty(ref _heading, value);
        }

        // commands
        public Command EditListCommand { get; set; }
        
        private string CreateTextGivenListEntities(IEnumerable<Entity> entityList)
        {
            return String.Join(", ", entityList.Select(e => e.ShortestName));
        }
    }
}
