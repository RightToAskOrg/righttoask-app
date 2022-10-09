using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;

namespace RightToAskClient.Models
{
    /* This class initialises all the data about Parliamentary committees that we
     * get from the server.
     */
    public class UpdatableCommitteesAndHearingsData
    {
        public List<Committee> Committees { get; private set; } = new List<Committee>();


        public bool IsInitialised { get; private set; }


        public async Task<Result<bool>> TryInitialisingFromServer()
        {
            var serverCommitteeList = await RTAClient.GetCommitteeData();
            if (serverCommitteeList is null)
            {
                return new Result<bool>() { Err = "Could not reach server." };
            }

            // Success. Set list of selectable committees and update filters to reflect new list.
            if (string.IsNullOrEmpty(serverCommitteeList.Err))
            {
                IsInitialised = true;
                Committees = serverCommitteeList.Ok.Select(com => new Committee(com)).ToList();
				// App.ReadingContext.Filters.InitSelectableLists();
                return new Result<bool>() { Ok = true };
            }

            return new Result<bool>()
            {
                Err = serverCommitteeList.Err
            };
        }
    }
}
