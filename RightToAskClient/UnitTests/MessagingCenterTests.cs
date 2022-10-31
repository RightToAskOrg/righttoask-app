using RightToAskClient.Models;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using RightToAskClient;
using Xamarin.Forms;
using Xunit;

namespace UnitTests
{
    public class MessagingCenterTests
    {
        // Properties
        public ValidationTests vTests = new ValidationTests();
        public Command TestCommand { get; set; }
        // Mock Sender and Receiver
        public class MockMessageSendingService
        {
            public MockMessageSendingService(string message)
            {
                MessagingCenter.Send(this, message);
            }
        }

        public class MockMessageReceiver
        {
            public bool messageReceived = false;
            public MockMessageReceiver(string message)
            {
                MessagingCenter.Subscribe<MockMessageSendingService>(this, message, (sender) =>
                {
                    messageReceived = true;
                });
            }
        }


        //[SetUp]
        //public void SetUp()
        //{
        //    Xamarin.Forms.Mocks.MockForms.Init();
        //}

        // Created regions for each unique message to test 4 different combinations
        // 1 - Mock receiver with valid message
        // 2 - Mock receiver with Invalid message
        // 3 - Mock sender with valid message
        // 4 - Mock sender with Invalid message
        #region "FromReg1" Message
        [Theory]
        [InlineData("FromReg1")]
        public void MessagingCenterFromReg1TestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService("FromReg1");

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterFromReg1TestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService("FromReg1");

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FromReg1")]
        public void MessagingCenterFromReg1TestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("FromReg1");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterFromReg1TestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("FromReg1");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
        #region "UpdateFilters" Message
        // This page is no longer used so this test is irrelevant
        //[Fact]
        //public void ExploringPageDoneButtonTest()
        //{
        //    // arrange
        //    ObservableCollection<MP> selectedEntities = new ObservableCollection<MP>();
        //    MP validMP = vTests.ValidMPTest();
        //    selectedEntities.Add(validMP);
        //    // mock the exploringPage - must call Xamarin.Forms.Init()
        //    ExploringPage exploringPage = new ExploringPage(selectedEntities, selectedEntities, "fakeTitle");
        //    Button button = (Button)exploringPage.FindByName("DoneButton");
        //    bool messageSendingReached = false;
        //    // act
        //    // implement TestCommand
        //    TestCommand = new Command(async () => 
        //    {
        //        //exploringPage.UpdateSelectedList(exploringPage.SelectedMPs);
        //        if (exploringPage.CameFromReg2Page)
        //        {
        //            exploringPage.CameFromReg2Page = false;
        //            await Shell.Current.GoToAsync("../.."); // double pop
        //        }
        //        else if (exploringPage.GoToReadingPageNext && !exploringPage.OptionB)
        //        {
        //            //SelectedOptionA = false;
        //            await Shell.Current.GoToAsync(nameof(ReadingPage));
        //        }
        //        else if (exploringPage.OptionB)
        //        {
        //            await Shell.Current.GoToAsync(nameof(QuestionAskerPage));
        //        }
        //        else
        //        {
        //            //await Navigation.PopAsync(); // single pop
        //        }
        //        MessagingCenter.Send(this, "UpdateFilters");
        //        messageSendingReached = true;
        //    });
        //    // set and execute the command
        //    button.Command = TestCommand;
        //    button.Command.Execute(null);

        //    // assert?
        //    Assert.True(messageSendingReached);
        //}

        [Theory]
        [InlineData("UpdateFilters")]
        public void MessagingCenterUpdateFiltersTestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService("UpdateFilters");

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterUpdateFiltersTestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService("UpdateFilters");

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData("UpdateFilters")]
        public void MessagingCenterUpdateFiltersTestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("UpdateFilters");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterUpdateFiltersTestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("UpdateFilters");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
        #region "MainPage" Message
        [Theory]
        [InlineData("MainPage")]
        public void MessagingCenterMainPageTestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService("MainPage");

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterMainPageTestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService("MainPage");

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData("MainPage")]
        public void MessagingCenterMainPageTestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("MainPage");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterMainPageTestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("MainPage");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
        #region "PreviousPage" Message
        [Theory]
        [InlineData("PreviousPage")]
        public void MessagingCenterPreviousPageTestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService("PreviousPage");

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterPreviousPageTestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService("PreviousPage");

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData("PreviousPage")]
        public void MessagingCenterPreviousPageTestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("PreviousPage");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterPreviousPageTestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("PreviousPage");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
        #region "OtherUser" Message
        [Theory]
        [InlineData("OtherUser")]
        public void MessagingCenterOtherUserTestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService("OtherUser");

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterOtherUserTestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService("OtherUser");

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData("OtherUser")]
        public void MessagingCenterOtherUserTestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("OtherUser");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterOtherUserTestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("OtherUser");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
        #region "OptionB" Message
        [Theory]
        [InlineData("OptionB")]
        public void MessagingCenterOptionBTestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService("OptionB");

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterOptionBTestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService("OptionB");

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData("OptionB")]
        public void MessagingCenterOptionBTestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("OptionB");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterOptionBTestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver("OptionB");

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
        #region "GoToReadingPageNext" Message
        [Theory]
        [InlineData(Constants.GoToReadingPageNext)]
        public void MessagingCenterGoToReadingPageTestValidMockReceiver(string message)
        {
            // arrange -> create receiver
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act -> send message
            _ = new MockMessageSendingService(Constants.GoToReadingPageNext);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterGoToReadingPageTestInvalidMockReceiver(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(message);

            // act
            _ = new MockMessageSendingService(Constants.GoToReadingPageNext);

            // assert
            Assert.False(receiver.messageReceived);
        }

        [Theory]
        [InlineData(Constants.GoToReadingPageNext)]
        public void MessagingCenterGoToReadingPageTestValidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(Constants.GoToReadingPageNext);

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.True(receiver.messageReceived);
        }

        [Theory]
        [InlineData("FakeMessage")]
        public void MessagingCenterGoToReadingPageTestInvalidMockSender(string message)
        {
            // arrange
            MockMessageReceiver receiver = new MockMessageReceiver(Constants.GoToReadingPageNext);

            // act
            _ = new MockMessageSendingService(message);

            // assert
            Assert.False(receiver.messageReceived);
        }
        #endregion
    }
}
