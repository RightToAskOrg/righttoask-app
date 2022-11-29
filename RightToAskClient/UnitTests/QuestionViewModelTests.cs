using RightToAskClient;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
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
        static ElectorateWithChamber ElectorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.StateEnum.VIC.ToString());
        Registration ValidRegistrationWithValidElectorate = new Registration
        {
            uid = "TestUId02",
            public_key = "fakeButValidPublicKey2",
            Electorates = new List<ElectorateWithChamber>() { ElectorateWithChamber }
        };

        public QuestionViewModelTests()
        {

        }

        /* TODO: Update now that the Find Committee Command works.
        [Fact]
        public void FindCommitteeCommandTest()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.FindCommitteeCommand
            };

            // act
            button.Command.Execute(null);
            string result = String.Join(" ", App.GlobalFilterChoices.SelectedAuthorities) + " is appearing at Senate Estimates tomorrow";

            // assert
            Assert.Equal(result, vm.SenateEstimatesAppearanceText);
        }
        */

        // TODO : Check server connection for at least being able to retreive data for unit testing purposes
        [Fact]
        public void QuestionSuggesterCommandTest()
        {
            // arrange
            vm.Question.QuestionSuggester = "fakeTestUID"; // Even with a correct UID "TestUID01" trying to reach a server fails
            Button button = new Button()
            {
                Command = vm.QuestionSuggesterCommand
            };
            vm.ReportLabelText = "";

            // act
            button.Command.Execute(null);

            // assert
            Assert.True(!string.IsNullOrEmpty(vm.ReportLabelText));
        }

        [Fact]
        public void EditQuestionCommandTest()
        {
            // arrange
            IndividualParticipant.IsRegistered = false;
            Button button = new Button()
            {
                Command = vm.EditAnswerCommand
            };

            // act
            button.Command.Execute(null);

            // assert
            Assert.True(!string.IsNullOrEmpty(vm.ReportLabelText));
        }

        [Fact]
        public void MyMPRaiseCommandTest()
        {
            // arrange
            //ParliamentData.MPAndOtherData.IsInitialised = true;
            Button button = new Button()
            {
                Command = vm.myMPRaiseCommand,
                
            };

            // act
            button.Command.Execute(null);
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.GoToReadingPageNext, (sender) =>
            {
                messageReceived = true;
            });

            // assert
            Assert.False(messageReceived);
            Assert.True(vm.ShowReportLabel);
            Assert.False(vm.EnableMyMPShouldRaiseButton);
            Assert.False(vm.EnableAnotherMPShouldRaiseButton);
            Assert.Equal(ParliamentData.MPAndOtherData.ErrorMessage, vm.ReportLabelText);
            Assert.True(vm.AnswerInApp);
        }

        [Fact]
        public void OtherMPRaiseCommandTest()
        {
            // arrange
            //ParliamentData.MPAndOtherData.IsInitialised = true; // can't set, so check the other method's properties
            Button button = new Button()
            {
                Command = vm.OtherMPRaiseCommand
            };

            // act
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.GoToReadingPageNext, (sender) =>
            {
                messageReceived = true;
            });
            button.Command.Execute(null);

            // assert
            Assert.False(messageReceived);
            Assert.True(vm.ShowReportLabel);
            Assert.False(vm.EnableMyMPShouldRaiseButton);
            Assert.False(vm.EnableAnotherMPShouldRaiseButton);
            Assert.Equal(ParliamentData.MPAndOtherData.ErrorMessage, vm.ReportLabelText);
            Assert.True(vm.AnswerInApp);
        }

        [Fact]
        public void AnsweredByMyMPCommandTest()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.AnsweredByMyMPCommand
            };

            // act
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.GoToReadingPageNext, (sender) =>
            {
                messageReceived = true;
            });
            button.Command.Execute(null);

            // assert
            Assert.True(messageReceived);
        }

        [Fact]
        public void AnsweredByMyMPCommandFailTest()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.AnsweredByMyMPCommand
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
        public void AnsweredByOtherMPCommandOptionBTest()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.AnsweredByOtherMPCommandOptionB
            };

            // act
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, "OptionB", (sender) =>
            {
                messageReceived = true;
            });
            button.Command.Execute(null);

            // assert
            Assert.True(messageReceived);
        }

        [Fact]
        public void AnsweredByOtherMPCommandOptionBFailTest()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.AnsweredByOtherMPCommandOptionB
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
        public void UserShouldRaiseCommandTest()
        {
            // arrange
            Button button = new Button()
            {
                Command = vm.UserShouldRaiseCommand
            };

            // act
            button.Command.Execute(null);

            // assert -- fails due to crash on shell navigation, but otherwise true
            Assert.Equal("Not Implemented Yet", vm.AnotherUserButtonText);
            Assert.False(vm.AnswerInApp);
        }

        [Fact]
        public void UpvoteCommandTest()
        {
            // arrange
            IndividualParticipant.IsRegistered = true;
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
            IndividualParticipant.IsRegistered = true;
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
                Command = vm.AnsweredByOtherMPCommandOptionB
            };

            // act
            bool messageReceived = false;
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.GoToMetadataPageNext, (sender) =>
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
                Command = vm.AnsweredByOtherMPCommandOptionB
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
            IndividualParticipant.ProfileData.RegistrationInfo = ValidRegistrationWithValidElectorate;
            IndividualParticipant.IsRegistered = true;
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
            List<MP> mps = new List<MP>();
            mps.Add(answeringMP);
            FilterChoices filters = new FilterChoices() { SelectedAnsweringMPsMine = mps};
            vm.Question.Filters = filters;

            // act
            vm.ClearQuestionDataAddWriter();
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
