

using RightToAskClient.Maui.Models.ServerCommsData;
using RightToAskClient.Maui.Resx;

namespace RightToAskClient.Maui.Models
{
    public class Committee : Entity
    {
        public ParliamentData.Jurisdiction Jurisdiction { get; }

        public string Name { get; } = "";

        public string Url { get; } = "";

        public string CommitteeType { get; }

        public override string ShortestName => Jurisdiction+": "+Name;

        // Constructor
        public Committee(CommitteeInfo info)
        {
            Jurisdiction = info.jurisdiction; 
            Name = info.name;
            Url = info.url;
            CommitteeType = info.committee_type;
        }
        
        // No need for the urls to be equal.
        public override bool DataEquals(object other)
        {
            var committee = other as Committee;
            return (committee != null)
                   && Name == committee.Name
                   && Jurisdiction == committee.Jurisdiction
                   && CommitteeType == committee.CommitteeType;
        }
        public override string GetName()
        {
            return Jurisdiction+" "+CommitteeType+" "+Name; 
        }

        public override string ToString()
        {
            var descr = (Jurisdiction == ParliamentData.Jurisdiction.Federal)
                ? AppResources.FederalParliamentText
                : Jurisdiction.ToString();
            return Name+"\n"+descr; 
        }
    }
}