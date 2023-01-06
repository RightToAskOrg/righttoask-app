using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class ReadingPageViewModel : ReadingPageBaseViewModel
    {
        // constructor
        // Most of the real work is in the ReadingPageBaseViewModel - this just adds the loading
        // function that is specific to the main reading page.
        public ReadingPageViewModel()
        {
            // Note: There is a race condition here, in that it is possible
            // for this command to be executed multiple times simultaneously,
            // producing multiple calls to Clear and the simultaneous insertion
            // of questions from various versions of questionsToDisplay List.
            // I don't *think* this will cause a lock because QuestionsToDisplay
            // ought to be able to be cleared and added to in any order.
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions(GetQuestionListBySearch());
                doQuestionDisplayRefresh(questionsToDisplayList);
                IsRefreshing = false;
            });
            
            // Get the question list for display
            RefreshCommand.ExecuteAsync();
        }

        // Gets the list of question IDs, using 'similarity' material
        // depending on whether this page was reached
        // by searching, drafting a question, 'what's trending' or by looking for all the questions written by a
        // given user.
        private async Task<JOSResult<List<string>>> GetQuestionListBySearch()
        {
            var filters = FilterChoices;
            
            // use the filters to search for similar questions.
            var serverSearchQuestion = new QuestionSendToServer()
            {
                question_text = DraftQuestion + " " + Keyword
            };
            
            // If there are no filters, keyword or draft question set, just ask for all questions.
            if( string.IsNullOrWhiteSpace(serverSearchQuestion.question_text) 
                && !serverSearchQuestion.TranscribeQuestionFiltersForUpload(filters))
            {
                return await RTAClient.GetQuestionList();
            }

            // Search based on filters and/or search/draft words.
            var scoredList = await RTAClient.GetSimilarQuestionIDs(serverSearchQuestion);

            // Error
            if (scoredList.Failure)
            {
                if (scoredList is ErrorResult<List<ScoredIDs>> errorResult)
                {
                    return new ErrorResult<List<string>>(errorResult.Message);
                }
                // Fallback error case - currently not reachable.
                return new ErrorResult<List<string>>("Error getting questions from server.");
            }

            // scoredList.Success
            // If we've successfully retrieved a list of scored question IDs, filter them
            // to select the ones we want
            var questionIDsOverThreshold = scoredList.Data
                .Where(q => q.score > Constants.similarityThreshold).Select(q => q.id).ToList();
            if (questionIDsOverThreshold.Any())
            {
                return new SuccessResult<List<string>>(questionIDsOverThreshold);
            }
            
            return new ErrorResult<List<string>>(AppResources.EmptyMatchingQuestionCollectionViewString);
        }
    }
}