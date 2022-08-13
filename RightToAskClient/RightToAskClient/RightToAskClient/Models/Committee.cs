

using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class Committee : Entity
    {
        // FIXME
        public ParliamentData.Chamber? jurisdiction { get; set; }

        public string name{ get; set; }

        public string url { get; set; }

        public ParliamentData.CommitteeType committee_type { get; set; }

        public override string ShortestName
        {
            get => jurisdiction+": "+name; 
        }
        
        // Contstructor
        public Committee(CommitteeInfo info)
        {
            
        }
        
        public override bool DataEquals(object obj)
        {
            throw new System.NotImplementedException();
        }
        public override string GetName()
        {
            throw new System.NotImplementedException();
        }
    }
}