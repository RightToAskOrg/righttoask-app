using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class QuestionViewModelTests
    {
        public QuestionViewModel vm = new QuestionViewModel();
        Question TestQuestion = new Question() { QuestionText = "test", QuestionSuggester = "me", QuestionId = "fakeId", 
            Version = "fakeVersion", HasAnswer = true, AnswerAccepted = true };

        public QuestionViewModelTests()
        {

        }

        [Fact]
        public void ReinitQuestionUpdatesTest()
        {
            // arrange
            vm.Question = TestQuestion;
            vm.Question.Background = "updatingBackground";
            bool isValid = vm.Question.ValidateUpdateQuestion();

            // act
            vm.ReinitQuestionUpdates();

            // assert
            Assert.True(isValid);
            Assert.True(!string.IsNullOrEmpty(vm.Question.Background));
            Assert.True(!string.IsNullOrEmpty(vm._serverQuestionUpdates.background));
        }

        [Fact]
        public void ResetInstanceTest()
        {
            // arrange            
            vm.Question = TestQuestion;
            // how do we want to include filters for a question into this test?
            ElectorateWithChamber electorate = new ElectorateWithChamber(ParliamentData.Chamber.ACT_Legislative_Assembly, ParliamentData.StatesAndTerritories[0]);
            MP answeringMP = new MP() { first_name = "firstname", surname = "testSurname", electorate = electorate };
            ObservableCollection<MP> mps = new ObservableCollection<MP>();
            mps.Add(answeringMP);
            FilterChoices filters = new FilterChoices() { SelectedAnsweringMPs = mps};
            vm.Question.Filters = filters;

            // act
            vm.ResetInstance();
            bool validElectorate = electorate.Validate();
            bool validMP = answeringMP.Validate();
            bool validQuestion = vm.Question.ValidateNewQuestion();

            // assert
            Assert.True(validElectorate);
            Assert.True(validMP);
            //Assert.True(validQuestion);
            Assert.NotNull(vm.Question);
            Assert.False(vm.Question.HasAnswer);
            Assert.False(vm.Question.AnswerAccepted);
            Assert.True(string.IsNullOrEmpty(vm.Question.QuestionText));
            Assert.True(string.IsNullOrEmpty(vm.Question.QuestionSuggester));
        }
    }
}
