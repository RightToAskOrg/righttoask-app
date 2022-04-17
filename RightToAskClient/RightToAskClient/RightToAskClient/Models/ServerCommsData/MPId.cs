using System;

namespace RightToAskClient.Models.ServerCommsData
{
    public class MPId 
    {
        public MPId(MP mp)
        {
            first_name = mp.first_name;
            electorate = mp.electorate;
            surname = mp.surname;
        }

        public MPId()
        {
        }

        public string first_name { get; }

        public string surname { get; }

        public ElectorateWithChamber electorate { get; set; }
    }
}