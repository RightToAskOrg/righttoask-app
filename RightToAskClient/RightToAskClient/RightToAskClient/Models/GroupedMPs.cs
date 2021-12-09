using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RightToAskClient.Models
{
    public class GroupedMPs : ObservableCollection<MP>
    {
        public GroupedMPs(ParliamentData.Chamber chamber, IEnumerable<MP> mpGroup) : base(mpGroup)
        {
            Chamber = chamber;
        }

        public ParliamentData.Chamber Chamber
        {
            get;
        }
    }
}