using System.Collections.Generic;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.ViewModels
{
    public class ReadingPageByQuestionWriterViewModel : ReadingPageBaseViewModel
    {
        // Constructor
        public ReadingPageByQuestionWriterViewModel()
        {
            Title = AppResources.MyQuestionsTitle;
            
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions(GetQuestionListByWriter());
                doQuestionDisplayRefresh(questionsToDisplayList);
                IsRefreshing = false;
            });
            
            // Get the question list for display
            RefreshCommand.ExecuteAsync();
        }
        
        private async Task<JOSResult<List<string>>> GetQuestionListByWriter()
        {
            var myUID = IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid;
            if (string.IsNullOrEmpty(myUID))
            {
                // TODO: Sensible error result when not registered.
                return new SuccessResult<List<string>>(new List<string>());
            }

            var questionIDs = await RTAClient.GetQuestionsByWriterId(myUID);

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