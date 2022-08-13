using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.ViewModels;

namespace RightToAskClient.Models
{
    /* This class initialises all the data about MPs and Parliamentary structures that we
     * get from the server, on the basis that it might change. It also tries initialising from
     * a file if the server is unreachable.
     * This means the MPs and their information, plus info about electorate names and which
     * states or regions they're in.
     * Currently, we only bother with Vic sub-regions, because although WA (at the time of
     * writing) still has them, they are planning to abolish them.
     * */
    public class UpdatableCommitteesAndHearingsData
    {
        private List<Committee> _committees = new List<Committee>();

        public List<Committee> Committees
        {
            get => _committees;
        }
        
        
		private bool _isInitialised;  // Defaults to false.
		public bool IsInitialised
		{
			get => _isInitialised;
		}


        public async Task<Result<bool>> TryInitialisingFromServer()
        {
            Result<List<CommitteeInfo>>? serverCommitteeList = await RTAClient.GetCommitteeData();
            if (serverCommitteeList is null)
            {
                return new Result<bool>() { Err = "Could not reach server." };
            }

            if (String.IsNullOrEmpty(serverCommitteeList.Err))
            {
                _isInitialised = true;
                _committees = serverCommitteeList.Ok.Select(com => new Committee(com)).ToList();
                return new Result<bool>() { Ok = true };
            }

            return new Result<bool>()
            {
                Err = serverCommitteeList.Err
            };
        }
    }
}
