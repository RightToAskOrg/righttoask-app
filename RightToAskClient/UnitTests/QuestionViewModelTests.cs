using RightToAskClient;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xunit;

namespace UnitTests
{
    public class QuestionViewModelTests
    {
        // properties for testing
        public QuestionViewModel vm = new QuestionViewModel();
        Question TestQuestion = new Question()
        {
            QuestionText = "test",
            QuestionSuggester = "me",
            QuestionId = "fakeId",
            Version = "fakeVersion",
            HasAnswer = true,
            AnswerAccepted = true
        };
        static ElectorateWithChamber ElectorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.State.VIC);
        Registration ValidRegistrationWithValidElectorate = new Registration
        {
            uid = "TestUId02",
            public_key = "fakeButValidPublicKey2",
            electorates = new ObservableCollection<ElectorateWithChamber>() { ElectorateWithChamber }
        };

        public QuestionViewModelTests()
        {

        }

        [Fact]
        public void RaisedOptionSelected0Command()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.RaisedOptionSelectedCommand,
                CommandParameter = 0
            };

            // act
            button.Command.Execute(null);
            string result = String.Join(" ", App.ReadingContext.Filters.SelectedAuthorities) + " is appearing at Senate Estimates tomorrow";

            // assert
            Assert.Equal(result, vm.SenateEstimatesAppearanceText);
        }

        [Fact]
        public void RaisedOptionSelected1Command()
        {
            // arrange
            //ParliamentData.MPAndOtherData.IsInitialised = true;
            Button button = new Button()
            {
                Command = vm.RaisedOptionSelectedCommand,
                CommandParameter = "1"
            };

            // act
            button.Command.Execute(null);
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, "GoToReadingPage", (sender) =>
            {
                messageReceived = true;
            });

            // assert
            Assert.True(messageReceived);
        }

        [Fact]
        public void RaisedOptionSelected1FailCommand()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.RaisedOptionSelectedCommand,
                CommandParameter = 1
            };

            // act
            button.Command.Execute(null);
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, "GoToReadingPage", (sender) =>
            {
                messageReceived = true;
            });

            // assert
            Assert.True(messageReceived);
        }

        [Fact]
        public void UpvoteCommandTest()
        {
            // arrange
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            vm.Question.UpVotes = 0;
            vm.Question.AlreadyUpvoted = false;
            Button button = new Button
            {
                Command = vm.UpvoteCommand
            };

            // act
            button.Command.Execute(null);

            Assert.Equal(1, vm.Question.UpVotes);
        }

        [Fact]
        public void UndoUpvoteCommandTest()
        {
            // arrange
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            vm.Question.UpVotes = 1;
            vm.Question.AlreadyUpvoted = true;
            Button button = new Button
            {
                Command = vm.UpvoteCommand
            };

            // act
            button.Command.Execute(null);

            Assert.Equal(0, vm.Question.UpVotes);
        }

        [Fact]
        public void AnsweredByOtherMPCommandTest()
        {
            // arrange
            Button button = new Button
            {
                Command = vm.AnsweredByOtherMPCommand
            };

            // act
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, "GoToReadingPage", (sender) =>
            {
                messageReceived = true;
            });
            button.Command.Execute(null);

            // assert
            Assert.True(messageReceived);
        }

        [Fact]
        public void AnsweredByOtherMPCommandFailTest()
        {
            // arrange
            Button button = new Button
            {
                Command = vm.AnsweredByOtherMPCommand
            };

            // act
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, "WrongMessage", (sender) =>
            {
                messageReceived = true;
            });
            button.Command.Execute(null);

            // assert
            Assert.False(messageReceived);
        }

        [Fact]
        public void SaveQuestionCommandTest()
        {
            // arrange
            IndividualParticipant testUser = new IndividualParticipant();
            testUser.RegistrationInfo = ValidRegistrationWithValidElectorate;
            App.ReadingContext.ThisParticipant = testUser;
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            vm.Question = TestQuestion;
            //var questionDetailPage = new QuestionDetailPage();
            Button button = new Button
            {
                Command = vm.SaveQuestionCommand
            };

            // TODO server configuration mock
            ServerConfig serverConfig = new ServerConfig();
            serverConfig.url = "fakeTestingURL";

            // act
            bool validQuestion = vm.Question.ValidateNewQuestion();
            button.Command.Execute(null);

            // assert
            Assert.True(validQuestion);
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
            vm.Question.Updates.background = vm.Question.Background;

            // assert
            Assert.True(isValid);
            Assert.True(!string.IsNullOrEmpty(vm.Question.Background));
            Assert.True(!string.IsNullOrEmpty(vm.Question.Updates.background));
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
