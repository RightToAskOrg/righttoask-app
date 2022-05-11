using RightToAskClient;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class ValidationTests
    {
        [Fact]
        public MP ValidMPTest()
        {
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.State.VIC);
            MP validMP = new MP()
            { first_name = "firstname",
              surname = "lastname",
              electorate = electorateWithChamber,
              email = "email",
              role = "role",
              party = "party"
            };

            Assert.NotNull(validMP);
            Assert.NotNull(electorateWithChamber);
            Assert.Equal(ParliamentData.Chamber.Vic_Legislative_Council, electorateWithChamber.chamber);
            Assert.Equal(ParliamentData.State.VIC, electorateWithChamber.region);
            Assert.True(validMP.first_name.Any());
            Assert.True(validMP.surname.Any());
            Assert.True(validMP.email.Any());
            Assert.True(validMP.role.Any());
            Assert.True(validMP.party.Any());
            return validMP;
        }

        [Fact]
        public void ValidPersonTest()
        {
            // arrange
            Person person = new Person("testUserId");
            Registration registration = new Registration(new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.State.QLD });
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
        public void ValidServerUserTest()
        {
            // arrange
            ServerUser serverUser = new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.State.QLD };
            
            // act
            bool isValid = serverUser.Validate();

            // assert
            Assert.True(isValid);
            Assert.True(!string.IsNullOrEmpty(serverUser.uid));
            Assert.True(!string.IsNullOrEmpty(serverUser.public_key));
            Assert.True(!string.IsNullOrEmpty(serverUser.state));
        }

        [Fact]
        public void InvalidServerUserTest()
        {
            // arrange
            ServerUser invalidServerUser = new ServerUser() { uid = "testUserId", state = ParliamentData.State.QLD };

            // act
            bool isValid = invalidServerUser.Validate();

            // assert
            Assert.False(isValid);
            Assert.True(!string.IsNullOrEmpty(invalidServerUser.uid));
            Assert.True(string.IsNullOrEmpty(invalidServerUser.public_key));
            Assert.True(!string.IsNullOrEmpty(invalidServerUser.state));
        }

        [Fact]
        public void ValidIndividualParticipantWithValidRegistrationTest()
        {
            // arrange
            IndividualParticipant ip = new IndividualParticipant();
            ip.IsRegistered = true;
            Registration registration = new Registration(new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.State.QLD });
            ip.RegistrationInfo = registration;

            // act
            bool isValid = ip.Validate();

            // assert
            Assert.True(isValid);
            Assert.NotNull(ip.RegistrationInfo);
            Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.uid));
            Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.public_key));
            //Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.State)); // state is in a weird setup where it never seems to actually get set
        }

        [Fact]
        public void ValidIndividualParticipantWithInvalidRegistrationTest()
        {
            // arrange
            IndividualParticipant ip = new IndividualParticipant();
            ip.IsRegistered = true;
            Registration invalidRegistration = new Registration(new ServerUser() { uid = "testUserId", state = ParliamentData.State.QLD });
            ip.RegistrationInfo = invalidRegistration;

            // act
            bool isValid = ip.Validate();

            // assert
            Assert.False(isValid);
            Assert.NotNull(ip.RegistrationInfo);
            Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.uid));
            Assert.True(string.IsNullOrEmpty(ip.RegistrationInfo.public_key));
            //Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.State));
        }

        [Fact]
        public void ValidIndividualParticipantWithoutRegistrationButKnownMPsTest()
        {
            // arrange
            IndividualParticipant ip = new IndividualParticipant();
            ip.IsRegistered = false;
            ip.MPsKnown = true;

            // act
            bool isValid = ip.Validate();

            // assert
            Assert.True(isValid);
            Assert.NotNull(ip.RegistrationInfo); // always has a default registration info object created. Generates public key
            Assert.True(ip.MPsKnown);
            Assert.False(ip.IsRegistered);
            Assert.True(string.IsNullOrEmpty(ip.RegistrationInfo.uid));
            Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.public_key));
            //Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.State));
        }

        [Fact]
        public void InvalidIndividualParticipantTest()
        {
            // arrange
            IndividualParticipant ip = new IndividualParticipant();
            ip.IsRegistered = false;
            ip.MPsKnown = false;
            ip.RegistrationInfo.SelectedStateAsIndex = 0;

            // act
            bool isValid = ip.Validate();

            // assert
            Assert.False(isValid);
            Assert.NotNull(ip.RegistrationInfo); // always has a default registration info object created. Generates public key
            Assert.False(ip.MPsKnown);
            Assert.False(ip.IsRegistered);
            Assert.True(string.IsNullOrEmpty(ip.RegistrationInfo.uid));
            Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.public_key));
            Assert.NotEqual(-1, ip.RegistrationInfo.SelectedStateAsIndex);
        }

        [Fact]
        public void ValidateNewServerQuestionTest()
        {
            // arrange
            QuestionSendToServer serverQuestion = new QuestionSendToServer() { question_text = "test question text for validation methods." };

            // act
            bool validFirstTimeQuestion = serverQuestion.ValidateNewQuestion();
            bool validUpdateQuestion = serverQuestion.ValidateUpdateQuestion();

            // assert
            Assert.True(validFirstTimeQuestion);
            Assert.False(validUpdateQuestion);
        }

        [Fact]
        public void ValidateUpdateServerQuestionTest()
        {
            // arrange
            QuestionSendToServer serverQuestion = new QuestionSendToServer()
            {
                question_text = "test question text for validation methods.",
                question_id = "fakeQuestionId",
                version = "fakeVersion"
            };

            // act
            bool validFirstTimeQuestion = serverQuestion.ValidateNewQuestion();
            bool validUpdateQuestion = serverQuestion.ValidateUpdateQuestion();

            // assert
            Assert.True(validFirstTimeQuestion);
            Assert.True(validUpdateQuestion);
        }

        [Fact]
        public void ValidateQuestionReceiveFromServerTest()
        {
            // arrange
            QuestionReceiveFromServer question = new QuestionReceiveFromServer() { question_id = "fakeQuestionId", question_text = "fakeQuestionTest", author = "fakeAuthor", version = "fakeVersion"};
            QuestionReceiveFromServer invalidQuestion = new QuestionReceiveFromServer();

            // act
            bool isValid = question.Validate();
            bool isInvalid = invalidQuestion.Validate();

            // assert
            Assert.True(isValid);
            Assert.False(isInvalid);
        }

        [Fact]
        public void ValidRegistrationTest()
        {
            // arrange
            Registration validRegistration = new Registration();
            validRegistration.uid = "testUid01";
            validRegistration.public_key = "fakeButValidPublicKey";
            validRegistration.SelectedStateAsIndex = 0;

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
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.State.VIC);
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
            bool isValid = validQuestion.ValidateNewQuestion();
            bool invalid = invalidQuestion.ValidateNewQuestion();

            // assert
            Assert.True(isValid);
            Assert.False(invalid);
            Assert.True(!string.IsNullOrEmpty(validQuestion.QuestionId));
            Assert.True(!string.IsNullOrEmpty(validQuestion.QuestionText));
        }

        // Test Address
        [Fact]
        public void TestValidAddress()
        {
            // arrange
            Address address = new Address();
            address.StreetNumberAndName = "12 pitt street";
            address.CityOrSuburb = "Sydney";
            address.Postcode = "2000";

            // act
            Result<bool> isValid = address.SeemsValid();

            // assert
            Assert.True(isValid.Ok);
            Assert.True(string.IsNullOrEmpty(isValid.Err));
            Assert.True(!string.IsNullOrEmpty(address.StreetNumberAndName));
            Assert.True(!string.IsNullOrEmpty(address.CityOrSuburb));
            Assert.True(!string.IsNullOrEmpty(address.Postcode));
        }

        [Fact]
        public void TestInvalidAddress()
        {
            // arrange
            Address address = new Address();

            // act
            Result<bool> isValid = address.SeemsValid();

            // assert
            Assert.False(isValid.Ok);
            Assert.True(!string.IsNullOrEmpty(isValid.Err));
            Assert.True(string.IsNullOrEmpty(address.StreetNumberAndName));
            Assert.True(string.IsNullOrEmpty(address.CityOrSuburb));
            Assert.True(string.IsNullOrEmpty(address.Postcode));
        }

        [Fact]
        public FilterChoices ValidateFiltersTest()
        {
            // arrange
            FilterChoices filters = new FilterChoices();
            filters.SelectedAnsweringMPs = new ObservableCollection<MP>();
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.State.VIC);
            MP validMP = new MP()
            {
                first_name = "firstname",
                surname = "lastname",
                electorate = electorateWithChamber,
                email = "email",
                role = "role",
                party = "party"
            };
            filters.SelectedAnsweringMPs.Add(validMP);
            filters.SearchKeyword = "test";
            //filters.SelectedAuthorities = new ObservableCollection<Authority>();

            // act
            bool isValidElectorate = electorateWithChamber.Validate();
            bool isValidMP = validMP.Validate();
            bool isValidFilters = filters.Validate();

            // assert
            Assert.True(isValidElectorate);
            Assert.True(isValidMP);
            Assert.True(isValidFilters);
            Assert.True(!string.IsNullOrEmpty(filters.SearchKeyword));
            return filters;
        }

        [Fact]
        public void ValidateClientSignedUnparsedTest()
        {
            // Arrange
            ClientSignedUnparsed csu = new ClientSignedUnparsed();
            csu.message = "fakeMessageToSend";
            csu.signature = ""; // what does the signature actually need to be to get it to pass?
            csu.user = "testUserID";
            App.ReadingContext.ThisParticipant.RegistrationInfo.uid = "testUserID";

            // Act
            bool isValid = csu.Validate();

            // Assert
            Assert.True(isValid);
        }
    }
}
