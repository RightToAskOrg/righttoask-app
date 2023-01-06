using System.Collections.Generic;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;

namespace RightToAskClient.ViewModels
{
    public class ReadingPageByQuestionWriterViewModel : ReadingPageViewModel
    {
        private async Task<JOSResult<List<string>>> GetAppropriateQuestionList()
        {
            var questionIDs = await RTAClient.GetQuestionsByWriterId("AppTestUser64");

            // If there's an error result, pass it back.
            if (questionIDs.Failure)
            {
                return questionIDs;
            }

            // Success. Return question list.
            return new SuccessResult<List<string>>(questionIDs.Data);
        }
    }
}