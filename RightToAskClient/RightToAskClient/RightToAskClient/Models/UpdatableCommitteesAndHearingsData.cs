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
    /* This class initialises all the data about Parliamentary committees that we
     * get from the server.
     */
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

            // Success. Set list of selectable committees and update filters to reflect new list.
            if (String.IsNullOrEmpty(serverCommitteeList.Err))
            {
                _isInitialised = true;
                _committees = serverCommitteeList.Ok.Select(com => new Committee(com)).ToList();
				App.ReadingContext.Filters.InitSelectableLists();
                return new Result<bool>() { Ok = true };
            }

            return new Result<bool>()
            {
                Err = serverCommitteeList.Err
            };
        }
    }
}
