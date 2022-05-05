using RightToAskClient;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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

            var appShell = new AppShell();

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
        [InlineData("Test")]
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

        [Theory]
        [InlineData("FromReg1")]
        [InlineData("Test")]
        public void MessagingCenterTest2(string message)
        {
            bool messageReceived = false;

            MessagingCenter.Subscribe<MockMessageSendingService>(this, message, (sender) =>
            {
                messageReceived = true;
            });

            var mockMessageSendingService = new MockMessageSendingService("FromReg1");

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

        // ListToString Converter Tests
        [Fact]
        public void CreateTextGivenListMPsTest()
        {
            // arrange data
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, "VIC");
            MP validMP = new MP("firstname", "lastname", electorateWithChamber, "email", "role", "party");

            List<MP> mps = new List<MP>
            {

            };

            FilterViewModel vm = new FilterViewModel();
            //vm.CreateTextGivenListMPs();
        }

        // Validate Object Tests
        [Fact]
        public void ValidMPTest()
        {
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, "VIC");
            MP validMP = new MP("firstname", "lastname", electorateWithChamber, "email", "role", "party");

            Assert.NotNull(validMP);
            Assert.NotNull(electorateWithChamber);
            Assert.Equal(ParliamentData.Chamber.Vic_Legislative_Council, electorateWithChamber.chamber);
            Assert.Equal(ParliamentData.State.VIC, electorateWithChamber.region);
            Assert.True(validMP.first_name.Any());
            Assert.True(validMP.surname.Any());
            Assert.True(validMP.email.Any());
            Assert.True(validMP.role.Any());
            Assert.True(validMP.party.Any());
        }

        [Fact]
        public void ValidPersonTest()
        {
            // arrange
            Person person = new Person("testUserId");

            // create a registration ??? this causes the test to pass, but do we need to change the constructor for person?
            Registration registration = new Registration() { uid = "testUserId", public_key = "fakePublicKey" };
            person.RegistrationInfo = registration;

            // act
            bool isValid = person.Validate();

            // assert
            Assert.True(isValid);
            Assert.NotNull(person.RegistrationInfo);
            Assert.True(!string.IsNullOrEmpty(person.RegistrationInfo.uid));
            Assert.True(!string.IsNullOrEmpty(person.RegistrationInfo.public_key)); // needed for a valid registration and thus a valid person as well
        }

        [Fact]
        public void InvalidPersonTest()
        {
            // arrange
            Person person = new Person("invalidUser");

            // act
            bool isValid = person.Validate();

            // assert
            Assert.False(isValid); 
            Assert.NotNull(person.RegistrationInfo);
            Assert.True(!string.IsNullOrEmpty(person.RegistrationInfo.uid));
            Assert.True(string.IsNullOrEmpty(person.RegistrationInfo.public_key));
        }

        [Fact]
        public void ValidRegistrationTest()
        {
            // arrange
            Registration validRegistration = new Registration();
            validRegistration.uid = "testUid01";
            validRegistration.public_key = "fakeButValidPublicKey";

            // act
            bool isValidRegistration = validRegistration.Validate();

            // assert
            Assert.True(isValidRegistration);
            Assert.True(!string.IsNullOrEmpty(validRegistration.uid));
            Assert.True(!string.IsNullOrEmpty(validRegistration.public_key));
            Assert.False(validRegistration.electorates?.Any());
        }

        [Fact]
        public void ValidRegistrationWithValidElectorateTest()
        {
            // arrange
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, "VIC");
            Registration validRegistrationWithValidElectorate = new Registration();
            validRegistrationWithValidElectorate.uid = "TestUId02";
            validRegistrationWithValidElectorate.public_key = "fakeButValidPublicKey2";
            validRegistrationWithValidElectorate.electorates = new ObservableCollection<ElectorateWithChamber>() { electorateWithChamber };
            // act
            bool isValidRegistrationWithValidElectorate = validRegistrationWithValidElectorate.Validate();
            bool validElectorate = electorateWithChamber.Validate();

            // assert
            Assert.True(isValidRegistrationWithValidElectorate);
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithValidElectorate.uid));
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithValidElectorate.public_key));
            Assert.True(validRegistrationWithValidElectorate.electorates?.Any());
            Assert.True(validElectorate);
        }

        [Fact]
        public void ValidRegistrationWithInvalidElectorateTest()
        {
            // arrange
            // empty region should be invalid
            ElectorateWithChamber invalidElectorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, "");
            Registration validRegistrationWithInvalidElectorate = new Registration();
            validRegistrationWithInvalidElectorate.uid = "TestUId02";
            validRegistrationWithInvalidElectorate.public_key = "fakeButValidPublicKey2";
            validRegistrationWithInvalidElectorate.electorates = new ObservableCollection<ElectorateWithChamber>() { invalidElectorateWithChamber };

            // act
            bool isValidRegistrationWithInvalidElectorate = validRegistrationWithInvalidElectorate.Validate();
            bool invalidElectorate = invalidElectorateWithChamber.Validate();

            // assert
            Assert.False(isValidRegistrationWithInvalidElectorate);
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithInvalidElectorate.uid));
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithInvalidElectorate.public_key));
            Assert.True(validRegistrationWithInvalidElectorate.electorates?.Any());
            Assert.False(invalidElectorate);
        }

        [Fact]
        public void InvalidRegistrationTest()
        {
            // arrange
            Registration invalidRegistration = new Registration();

            // act
            bool isInvalidRegistration = invalidRegistration.Validate();

            // assert
            Assert.False(isInvalidRegistration);
            Assert.True(string.IsNullOrEmpty(invalidRegistration.uid));
            Assert.True(string.IsNullOrEmpty(invalidRegistration.public_key));
            Assert.False(invalidRegistration.electorates?.Any());
        }

        [Fact]
        public void FindParliamentDataTest()
        {
            // arrange
            string state = "VIC";
            string invalidState = "test";

            // act
            var data = ParliamentData.FindChambers(state);
            var data2 = ParliamentData.FindChambers(invalidState);

            // assert 
            Assert.NotNull(data);
            Assert.Equal(ParliamentData.Chamber.Australian_House_Of_Representatives, data[0]);
            Assert.Equal(ParliamentData.Chamber.Australian_Senate, data[1]);
            Assert.Equal(ParliamentData.Chamber.Vic_Legislative_Assembly, data[2]);
            Assert.Equal(ParliamentData.Chamber.Vic_Legislative_Council, data[3]);
            Assert.False(data2.Any()); // this line fails because we still set the first 2 chambers for invalid strings.
        }

        [Fact]
        public void ValidateQuestionTest()
        {
            // arrange
            Question validQuestion = new Question();
            validQuestion.QuestionId = "TestId";
            validQuestion.QuestionText = "Question Text for Testing the question's validator method.";

            Question invalidQuestion = new Question();

            // act
            bool isValid = validQuestion.Validate();
            bool invalid = invalidQuestion.Validate();

            // assert
            Assert.True(isValid);
            Assert.False(invalid);
            Assert.True(!string.IsNullOrEmpty(validQuestion.QuestionId));
            Assert.True(!string.IsNullOrEmpty(validQuestion.QuestionText));
        }

        // Boolean Converter Test
        [Fact]
        public void ConverterTest()
        {
            // arrange data
            bool falseConvert = false;
            bool trueConvert = true;

            Type t = typeof(bool);
            CultureInfo c = new CultureInfo("es-ES", false);

            InvertConvert converterClass = new InvertConvert();

            // act on the data
            falseConvert = (bool)converterClass.Convert(falseConvert, t, null, c);
            trueConvert = (bool)converterClass.Convert(trueConvert, t, null, c);

            // assert
            Assert.True(falseConvert);
            Assert.False(trueConvert);
        }
    }
}
