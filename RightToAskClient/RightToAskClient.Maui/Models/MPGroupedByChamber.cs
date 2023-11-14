using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RightToAskClient.Maui.Models
{
    public class MPGroupedByChamber : ObservableCollection<MP>
    {
        public MPGroupedByChamber(ParliamentData.Chamber chamber, IEnumerable<MP> mpGroup) : base(mpGroup)
        {
            Chamber = chamber;
        }

        public ParliamentData.Chamber Chamber
        {
            get;
        }
    }
}