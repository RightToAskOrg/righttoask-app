using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    /* This class initialises all the data about Parliamentary committees that we
     * get from the server.
     */
    public class UpdatableCommitteesAndHearingsData
    {
        public List<Committee> Committees { get; private set; } = new List<Committee>();


        public bool IsInitialised { get; private set; }


        public async Task<JOSResult> TryInitialisingFromServer()
        {
            var serverCommitteeList = await RTAClient.GetCommitteeData();
            if (serverCommitteeList is null)
            {
                // TODO Consider changing to a specific 'server unreachable' error.
                return new ErrorResult<bool>("Could not reach server.");
            }

            // Success. Set list of selectable committees and update filters to reflect new list.
            if (serverCommitteeList.Success)
            {
                IsInitialised = true;
                Committees = serverCommitteeList.Data.Select(com => new Committee(com)).ToList();
				// App.ReadingContext.Filters.InitSelectableLists();
				FilterChoices.NeedToInitCommitteeLists(this);
                return new SuccessResult();
            }

            // serverCommitteeList.Failure
            if (serverCommitteeList is ErrorResult<List<CommitteeInfo>> errorResult)
            {
                return new ErrorResult(errorResult.Message);
            }

            return new ErrorResult("Couldn't get committee and hearing data from server.");
        }
    }
}
