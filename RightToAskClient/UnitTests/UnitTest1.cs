using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using System;
using Xamarin.Forms;
using Xunit;

namespace UnitTests
{
    public class UnitTest1
    {
        // Sample Tests
        #region Sample Tests
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        public void MyFirstTheory(int value)
        {
            Assert.True(IsOdd(value));
        }

        bool IsOdd(int value)
        {
            return value % 2 == 1;
        }
        #endregion

        // Messaging Center Tests
        #region Messaging Center Tests
        [Fact]
        public void MessagingCenterUpdateFiltersTest()
        {
            bool messageReceived = false;
            // can't get button command because exploringPages don't have ViewModels.
            MessagingCenter.Subscribe<UpdateFiltersService>(this, "UpdateFilters", (sender) =>
            {
                messageReceived = true;
            });

            var filterService = new UpdateFiltersService();

            Assert.True(messageReceived);
        }

        [Fact]
        public void MessagingCenterFiltersFromMainTest()
        {
            bool messageReceived = false;
            MainPageViewModel vm = new MainPageViewModel();

            MessagingCenter.Subscribe<MainPageViewModel>(this, "MainPage", (sender) =>
            {
                messageReceived = true;
                MessagingCenter.Unsubscribe<MainPageViewModel>(this, "MainPage");
            });
            vm.AdvancedSearchButtonCommand.Execute(null); // breaks due to the continue async stuff in the button press method as well as navigating away from the page.

            Assert.True(messageReceived);
        }

        [Theory]
        [InlineData("UpdateFilters")]
        [InlineData("MainPage")]
        [InlineData("FromReg1")]
        [InlineData("Test")]
        public void MessagingCenterTestsGeneric(string message)
        {
            bool messageReceived = false;
            QuestionViewModel questionViewModel = new QuestionViewModel();

            MessagingCenter.Subscribe<MockMessageSendingService>(this, message, (sender) =>
            {
                messageReceived = true;
            });
            questionViewModel.SaveQuestionCommand.Execute(null);
            var mockMessageSendingService = new MockMessageSendingService(message);

            Assert.True(messageReceived);
        }

        [Theory]
        [InlineData("FromReg1")]
        [InlineData("UpdateFilters")]
        [InlineData("MainPage")]
        public void MessagingCenterTest(string message)
        {
            bool messageReceived = false;

            MessagingCenter.Subscribe<MockMessageSendingService>(this, "FromReg1", (sender) =>
            {
                messageReceived = true;
            });

            var mockMessageSendingService = new MockMessageSendingService(message);

            Assert.True(messageReceived);
        }

        public class MockMessageSendingService 
        {
            public MockMessageSendingService(string message)
            {
                MessagingCenter.Send(this, message);
            }
        }

        public class MainPageService
        {
            public MainPageService()
            {
                MessagingCenter.Send(this, "MainPage");
            }
        }

        public class UpdateFiltersService
        {
            public UpdateFiltersService()
            {
                MessagingCenter.Send(this, "UpdateFilters");
            }
        }
        #endregion

        // Validate Object Tests

    }
}
