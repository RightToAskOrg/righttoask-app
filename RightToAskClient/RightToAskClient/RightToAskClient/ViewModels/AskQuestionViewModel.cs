using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.ViewModels
{
    public class AskQuestionViewModel : ObservableObject
    {
        public IAsyncCommand BackCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand RemoveQuestionCommand { get; }
        
        private ObservableCollection<QuestionDisplayCardViewModel> _questionsToDisplay = new ObservableCollection<QuestionDisplayCardViewModel>();
        public ObservableCollection<QuestionDisplayCardViewModel> QuestionsToDisplay
        {
            get => _questionsToDisplay;
            set => SetProperty(ref _questionsToDisplay, value);
        }
        
        private Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        public AskQuestionViewModel()
        {
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            RemoveQuestionCommand = new AsyncCommand(async () =>
            {
                
            });
            
            RefreshCommand = new AsyncCommand(async () =>
            {
                var questionsToDisplayList = await LoadQuestions();
                QuestionsToDisplay.Clear();
                foreach (var q in questionsToDisplayList)
                {  
                    QuestionsToDisplay.Add(q);
                }
            });
            RefreshCommand.ExecuteAsync();
        }
        
        private async Task<JOSResult<List<string>>> GetAppropriateQuestionList()
        {
            // TODO**: use the one stored in this class.
            var filters = new FilterChoices();
            
            // else use the filters to search for similar questions.
            var serverSearchQuestion = new QuestionSendToServer()
            {
                question_text = ""
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
        public async Task<List<QuestionDisplayCardViewModel>> LoadQuestions()
        {
            var serverQuestions = new List<QuestionReceiveFromServer>();
            var questionsToDisplay = new List<QuestionDisplayCardViewModel>();
            var httpResponse = await GetAppropriateQuestionList();
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            if (!httpValidation.isValid)
            {
                return questionsToDisplay;
            }
            
            // httpValidation isValid
            var questionIds = httpResponse.Data;
            
            // loop through the questions
            foreach (var questionId in questionIds)
            {
                // pull the individual question from the server by id
                QuestionReceiveFromServer tempQuestion;
                try
                {
                    // If retrieval is successful, add to the list of questions to be displayed.
                    var dataResult = await RTAClient.GetQuestionById(questionId);
                    if (dataResult.Success)
                    {
                        tempQuestion = dataResult.Data;
                        if (!string.IsNullOrEmpty(tempQuestion.question_text))
                        {
                            serverQuestions.Add(tempQuestion);
                        }
                    }
                    // Log retrieval failure.
                    else
                    {
                        var errorMessage = "Could not retrieve question with ID " + questionId + ". ";
                        if (dataResult is ErrorResult<QuestionReceiveFromServer> errorResult)
                        {
                            errorMessage += errorResult.Message;
                        }
                        Debug.WriteLine(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not find question: " + ex.Message);
                }
            }
            foreach (var serverQuestion in serverQuestions)
            {
                questionsToDisplay.Add(new QuestionDisplayCardViewModel(serverQuestion, new QuestionResponseRecords()));
            }

            return questionsToDisplay;
        }
    }
}