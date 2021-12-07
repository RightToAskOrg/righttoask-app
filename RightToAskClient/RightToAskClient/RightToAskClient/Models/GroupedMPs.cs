using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RightToAskClient.Models
{
    public class GroupedMPs
    {
        public ParliamentData.Chamber Key { get; set; }
        public ObservableCollection<Entity> MPsInGroup { get; set; } = new ObservableCollection<Entity>();
        // TODO This may not need to be exported if it is always blank.
        public ObservableCollection<Entity> BlankSelections { get; set; } = new ObservableCollection<Entity>();

        public string Heading
        {
            get { return Key.ToString(); }
        }
    }
}