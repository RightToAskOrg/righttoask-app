using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RightToAskClient.Models
{
    public class GroupedMPs : ObservableCollection<Entity>
    //public class GroupedMPs 
    {
        public GroupedMPs(ParliamentData.Chamber chamber, IEnumerable<Entity> mpGroup) : base(mpGroup)
        {
            Chamber = chamber.ToString();
            //MPGroup = mpGroup;
        }

        // public ObservableCollection<Entity> MPsInGroup { get; set; } = new ObservableCollection<Entity>();
        // TODO This may not need to be exported if it is always blank.
        // public ObservableCollection<Entity> BlankSelections { get; set; } = new ObservableCollection<Entity>();

        public string Chamber
        {
            get;
            private set;
        }
        /*
        public ObservableCollection<Entity> MPGroup
        {
            get
            { return this. }
        }
        */
    }
}