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

            ShowSearchFrame = true;
            
            // Get the question list for display
            RefreshCommand.ExecuteAsync();
        }

        // Gets the list of question IDs, using 'similarity' material
        private async Task<JOSResult<List<string>>> GetQuestionListBySearch()
        {
            var filters = FilterChoices;
            
            // use the filters to search for similar questions.
            var serverSearchQuery = new WeightedSearchRequest()
            {
                question_text = Keyword,
                page = new QuestionListPage()
                {
                    from = 0,
                    to = Constants.DefaultPageSize 
                },
                weights = new Weights()
                {
                    metadata = Constants.ReadingPageMetadataWeight,
                    net_votes = Constants.ReadingNetVotesWeight,
                    total_votes = Constants.ReadingPageTotalVotesWeight,
                    recentness = Constants.ReadingPageRecentnessWeight,
                    recentness_timescale = Constants.ReadingPageRecentnessTimescale,
                    text = Constants.ReadingPageTextSimilarityWeight
                },
                entity_who_should_answer_the_question = filters.TranscribeQuestionAnswerersForUpload(),
                mp_who_should_ask_the_question = filters.TranscribeQuestionAskersForUpload()
            };
            
            // Search based on filters and/or search/draft words.
            var scoredList = await RTAClient.GetSortedSimilarQuestionIDs(serverSearchQuery);

            // Error
            if (scoredList.Failure)
            {
                if (scoredList is ErrorResult<SortedQuestionList> errorResult)
                {
                    return new ErrorResult<List<string>>(errorResult.Message);
                }
                // Fallback error case - currently not reachable.
                return new ErrorResult<List<string>>("Error getting questions from server.");
            }

            // scoredList.success
            // For the moment, ignore both the token and the individual-question scores.
            return new SuccessResult<List<string>>(scoredList.Data.questions.Select(sq => sq.id).ToList());
        }
    }
}