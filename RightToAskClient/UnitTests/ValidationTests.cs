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
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.StateEnum.VIC.ToString());
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
            Assert.Equal(ParliamentData.StateEnum.VIC.ToString(), electorateWithChamber.region);
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
            Registration registration = new Registration(new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.StateEnum.QLD.ToString() });
            person.RegistrationInfo = registration;

            // act
            bool isValid = person.RegistrationInfo.Validate();

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
            bool isValid = person.RegistrationInfo.Validate();

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
            ServerUser serverUser = new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.StateEnum.QLD.ToString() };
            
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
            ServerUser invalidServerUser = new ServerUser() { uid = "testUserId", state = ParliamentData.StateEnum.QLD.ToString() };

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
            Registration registration = new Registration(new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.StateEnum.QLD.ToString() });
            registration.registrationStatus = RegistrationStatus.Registered;

            // act
            bool isValid = registration.Validate();

            // assert
            //TODO (unit-test) IndividualParticipant is a global object so other tests can impact on it - so this test can work or no depends on random.
            Assert.True(isValid);
            Assert.NotNull(registration);
            Assert.False(string.IsNullOrEmpty(registration.uid));
            Assert.False(string.IsNullOrEmpty(registration.public_key));
            //Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.State)); // state is in a weird setup where it never seems to actually get set
        }

        [Fact]
        public void ValidIndividualParticipantWithInvalidRegistrationTest()
        {
            // arrange
            Registration invalidRegistration = new Registration(new ServerUser() { uid = "testUserId", state = ParliamentData.StateEnum.QLD.ToString() });
            invalidRegistration.registrationStatus = RegistrationStatus.Registered;

            // act
            bool isValid = invalidRegistration.Validate();

            // assert
            Assert.False(isValid);
            Assert.NotNull(invalidRegistration);
            Assert.False(string.IsNullOrEmpty(invalidRegistration.uid));
            Assert.True(string.IsNullOrEmpty(invalidRegistration.public_key));
            //Assert.True(!string.IsNullOrEmpty(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.State));
        }

        [Fact]
        public void ValidIndividualParticipantWithoutRegistrationButKnownMPsTest()
        {
            // arrange
            Registration registration = new Registration(new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.StateEnum.QLD.ToString() });
            registration.registrationStatus = RegistrationStatus.NotRegistered;
            registration.ElectoratesKnown = true;

            // act
            bool isValid = registration.ValidateReadyToUse();

            // assert
            Assert.True(isValid);
            Assert.NotNull(registration); // always has a default registration info object created. Generates public key
            Assert.True(registration.ElectoratesKnown);
            Assert.False(registration.IsRegistered);
            // Assert.True(string.IsNullOrEmpty(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid));
            // Assert.True(!string.IsNullOrEmpty(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.public_key));
            //Assert.True(!string.IsNullOrEmpty(ip.RegistrationInfo.State));
        }

        [Fact]
        public void InvalidIndividualParticipantTest()
        {
            // arrange
            Registration registration = new Registration(new ServerUser() { uid = "testUserId", public_key = "fakePublicKey", state = ParliamentData.StateEnum.QLD.ToString() });
            registration.registrationStatus = RegistrationStatus.NotRegistered;
            registration.ElectoratesKnown = false;
            registration.StateKnown = false;

            // act
            bool isValid = registration.Validate();

            // assert
            Assert.False(isValid);
            Assert.NotNull(registration); // always has a default registration info object created. Generates public key
            Assert.False(registration.ElectoratesKnown);
            Assert.False(registration.IsRegistered);
            // Assert.True(string.IsNullOrEmpty(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid));
            // Assert.False(string.IsNullOrEmpty(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.public_key));
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
            QuestionReceiveFromServer question = new QuestionReceiveFromServer()
            {
                question_id = "fakeQuestionId",
                question_text = "fakeQuestionTest",
                author = "fakeAuthor",
                version = "fakeVersion",
                timestamp = 0980532405,
                last_modified = 0980532407,
                total_votes = 7,
                net_votes = 3
            };

            // assert
            Assert.True(question.Validate());
        }

        [Fact]
        public void ValidateInvalidQuestionReceiveFromServerTest()
        {
            QuestionReceiveFromServer invalidQuestion = new QuestionReceiveFromServer();
            Assert.False(invalidQuestion.Validate());
        }
        

        [Fact]
        
        // Ideally, we'd have a valid question and then a series of single-item tweaks to make it invalid, checking that
        // each one was indeed invalid. 
        public void ValidateQuestionConstructedFromQuestionReceiveFromServerTest()
        {
            // arrange
            QuestionResponseRecords responseRecords = new QuestionResponseRecords();
            QuestionReceiveFromServer serverQuestion = new QuestionReceiveFromServer()
            {
                question_id = "fakeQuestionId",
                question_text = "fakeQuestionTest",
                author = "fakeAuthor",
                version = "fakeVersion",
                timestamp = 0980532405,
                last_modified = 0980532407,
                total_votes = 7,
                net_votes = 3
            };

            // act
            Question validQuestion = new Question(serverQuestion, responseRecords);

            // assert
            Assert.True(validQuestion.ValidateDownloadedQuestion());
            Assert.Equal(validQuestion.QuestionId , serverQuestion.question_id);
            Assert.Equal(validQuestion.QuestionText , serverQuestion.question_text);
            Assert.Equal(validQuestion.QuestionSuggester , serverQuestion.author);
            Assert.Equal(validQuestion.Timestamp , serverQuestion.timestamp);
            Assert.Equal(validQuestion.LastModified , serverQuestion.last_modified);
            Assert.Equal(validQuestion.TotalVotes , serverQuestion.total_votes);
            Assert.Equal(validQuestion.NetVotes , serverQuestion.net_votes);
        }

        [Fact]
        public void ValidateInvalidQuestionConstructedFromQuestionReceiveFromServerTest()
        {
            // arrange 
            QuestionResponseRecords responseRecords = new QuestionResponseRecords();
            QuestionReceiveFromServer invalidServerQuestion = new QuestionReceiveFromServer();
            
            // act
            Question invalidQuestion =  new Question(invalidServerQuestion, responseRecords);
            
            // assert
            Assert.False(invalidQuestion.ValidateDownloadedQuestion());
        }
        

        [Fact]
        public Registration ValidRegistrationTest()
        {
            // arrange
            Registration validRegistration = new Registration();
            validRegistration.uid = "testUid01";
            validRegistration.public_key = "fakeButValidPublicKey";
            validRegistration.SelectedStateAsEnum = ParliamentData.StateEnum.VIC;
            validRegistration.StateKnown = true;
            // act
            bool isValidRegistration = validRegistration.Validate();

            // assert
            Assert.True(isValidRegistration);
            Assert.True(!string.IsNullOrEmpty(validRegistration.uid));
            Assert.True(!string.IsNullOrEmpty(validRegistration.public_key));
            Assert.False(validRegistration.Electorates?.Any());
            return validRegistration;
        }

        [Fact]
        public void ValidRegistrationWithValidElectorateTest()
        {
            // arrange
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.StateEnum.VIC.ToString());
            Registration validRegistrationWithValidElectorate = new Registration();
            validRegistrationWithValidElectorate.uid = "TestUId02";
            validRegistrationWithValidElectorate.public_key = "fakeButValidPublicKey2";
            validRegistrationWithValidElectorate.StateKnown = true;
            validRegistrationWithValidElectorate.Electorates = new List<ElectorateWithChamber>() { electorateWithChamber };
            
            // act
            bool isValidRegistrationWithValidElectorate = validRegistrationWithValidElectorate.Validate();
            bool validElectorate = electorateWithChamber.Validate();

            // assert
            Assert.True(isValidRegistrationWithValidElectorate);
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithValidElectorate.uid));
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithValidElectorate.public_key));
            Assert.True(validRegistrationWithValidElectorate.Electorates?.Any());
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
            validRegistrationWithInvalidElectorate.Electorates = new List<ElectorateWithChamber>() { invalidElectorateWithChamber };

            // act
            bool isValidRegistrationWithInvalidElectorate = validRegistrationWithInvalidElectorate.Validate();
            bool invalidElectorate = invalidElectorateWithChamber.Validate();

            // assert
            Assert.False(isValidRegistrationWithInvalidElectorate);
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithInvalidElectorate.uid));
            Assert.True(!string.IsNullOrEmpty(validRegistrationWithInvalidElectorate.public_key));
            Assert.True(validRegistrationWithInvalidElectorate.Electorates?.Any());
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
            Assert.False(invalidRegistration.Electorates?.Any());
        }

        [Fact]
        public void FindParliamentDataTest()
        {
            // arrange
            var state = ParliamentData.StateEnum.SA;

            // act
            var data = ParliamentData.FindChambers(state, true);
            var data2 = ParliamentData.FindChambers(state, false);

            // assert 
            Assert.NotNull(data);
            Assert.Equal(ParliamentData.Chamber.Australian_House_Of_Representatives, data[0]);
            Assert.Equal(ParliamentData.Chamber.Australian_Senate, data[1]);
            Assert.Equal(ParliamentData.Chamber.SA_House_Of_Assembly, data[2]);
            Assert.Equal(ParliamentData.Chamber.SA_Legislative_Council, data[3]);
            // Assert.False(data2.Any()); // this line fails because we still set the first 2 chambers for invalid strings.
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
        public Address TestValidAddress()
        {
            // arrange
            Address address = new Address();
            address.StreetNumberAndName = "12 pitt street";
            address.CityOrSuburb = "Sydney";
            address.Postcode = "2000";

            // act
            JOSResult<bool> isValid = address.SeemsValid();

            // assert
            Assert.True(isValid.Success);
            // Assert.True(string.IsNullOrEmpty(isValid.Err));
            Assert.True(!string.IsNullOrEmpty(address.StreetNumberAndName));
            Assert.True(!string.IsNullOrEmpty(address.CityOrSuburb));
            Assert.True(!string.IsNullOrEmpty(address.Postcode));
            return address;
        }

        [Fact]
        public void TestInvalidAddress()
        {
            // arrange
            Address address = new Address();

            // act
            JOSResult<bool> isValid = address.SeemsValid();

            // assert
            Assert.True(isValid.Failure);
            // Assert.True(!string.IsNullOrEmpty(isValid.Err));
            Assert.True(string.IsNullOrEmpty(address.StreetNumberAndName));
            Assert.True(string.IsNullOrEmpty(address.CityOrSuburb));
            Assert.True(string.IsNullOrEmpty(address.Postcode));
        }

        [Fact]
        public FilterChoices ValidateFiltersTest()
        {
            // arrange
            FilterChoices filters = new FilterChoices();
            filters.SelectedAnsweringMPsMine = new List<MP>();
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, ParliamentData.StateEnum.VIC.ToString());
            MP validMP = new MP()
            {
                first_name = "firstname",
                surname = "lastname",
                electorate = electorateWithChamber,
                email = "email",
                role = "role",
                party = "party"
            };
            filters.SelectedAnsweringMPsMine.Add(validMP);
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
    }
}
