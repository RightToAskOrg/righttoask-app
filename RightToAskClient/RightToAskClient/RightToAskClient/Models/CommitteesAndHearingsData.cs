using System.Collections.ObjectModel;

namespace RightToAskClient.Models
{
    // This class reads in information about committees and upcoming hearings.
    public static class CommitteesAndHearingsData
    {
        public static readonly UpdatableCommitteesAndHearingsData CommitteesData =
            new UpdatableCommitteesAndHearingsData();
        
        
	    public static ObservableCollection<Committee> AllCommittees 
	    {
		    get
		    {
			    return new ObservableCollection<Committee>(CommitteesData.Committees);
		    }
	    }
    }
}