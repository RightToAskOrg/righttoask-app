

using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class Committee : Entity
    {
        private ParliamentData.Chamber _jurisdiction;
        public ParliamentData.Chamber Jurisdiction => _jurisdiction;

        private string _name = "";
        public string Name => _name;

        private string _url = "";
        public string Url => _url;

        private ParliamentData.CommitteeType _committeeType;
        public ParliamentData.CommitteeType CommitteeType => _committeeType;

        public override string ShortestName
        {
            get => _jurisdiction+": "+_name; 
        }
        
        // Contstructor
        public Committee(CommitteeInfo info)
        {
            _jurisdiction = info.jurisdiction; 
            _name = info.name;
            _url = info.url;
            _committeeType = info.committee_type;
        }
        
        // No need for the urls to be equal.
        public override bool DataEquals(object other)
        {
            var committee = other as Committee;
            return (committee != null)
                   && _name == committee.Name
                   && _jurisdiction == committee.Jurisdiction
                   && _committeeType == committee.CommitteeType;
        }
        public override string GetName()
        {
            return _jurisdiction+" "+_committeeType+" "+_name; 
        }
    }
}