using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xunit;

namespace UnitTests.Models
{
    public class FilterChoicesTests
    {
        [Fact]
        public void ListsNotNullOnInit()
        {
            var testFilters = new FilterChoices();
            
            Assert.NotNull(testFilters.AuthorityLists);
            Assert.NotNull(testFilters.QuestionWriterLists);
            Assert.NotNull(testFilters.CommitteeLists);
            Assert.NotNull(testFilters.AnsweringMPsListsMine);
            Assert.NotNull(testFilters.AnsweringMPsListsNotMine);
            Assert.NotNull(testFilters.AskingMPsListsMine);
            Assert.NotNull(testFilters.AskingMPsListsNotMine);
        }
        
        [Fact]
        public void SelectionsEmptyOnInit()
        {
            var testFilters = new FilterChoices();
            
            Assert.Empty(testFilters.AuthorityLists.SelectedEntities);
            Assert.Empty(testFilters.QuestionWriterLists.SelectedEntities);
            Assert.Empty(testFilters.CommitteeLists.SelectedEntities);
            Assert.Empty(testFilters.AnsweringMPsListsMine.SelectedEntities);
            Assert.Empty(testFilters.AnsweringMPsListsNotMine.SelectedEntities);
            Assert.Empty(testFilters.AskingMPsListsMine.SelectedEntities);
            Assert.Empty(testFilters.AskingMPsListsNotMine.SelectedEntities);
        }

        [Fact]
        public void SearchwordEmptyOnInit()
        {
            var testFilters = new FilterChoices();

            Assert.True(String.IsNullOrEmpty(testFilters.SearchKeyword));
        }

        [Fact]
        public void SearchwordErasedByRemoveAllSelections()
        {
            var testFilters = new FilterChoices();

            testFilters.SearchKeyword = "TestKeyword";
            Assert.Equal(testFilters.SearchKeyword, "TestKeyword");
            
            testFilters.RemoveAllSelections();
            Assert.True(String.IsNullOrEmpty(testFilters.SearchKeyword));
        }

        [Fact]
        public void SelectionsErasedByRemoveAllSelections()
        {
            var testFilters = new FilterChoices();
            var testMP1 = new MP()
            {
                first_name = "TestMPFirstName",
                surname = "TestMPsurname"
            };

            var mps = new List<MP>();
            mps.Add(testMP1);

            var testAuthority = new Authority()
            {
                AuthorityName = "TestAuthorityName"
            };
            var authorities = new List<Authority>();
            authorities.Add(testAuthority);
            
            var testCommittee = new Committee(new CommitteeInfo()
            {
                jurisdiction    = ParliamentData.Jurisdiction.Federal,
                name = "TestCommitteeName",
            });
            var committees = new List<Committee>();
            committees.Add(testCommittee);
            
            var testPerson = new Person(new ServerUser()
            {
                display_name = "testPerson",
                uid = "TestUID"
            });
            var people = new List<Person>();
            people.Add(testPerson);
            
            testFilters.AnsweringMPsListsNotMine.SelectedEntities = mps;
	        testFilters.AnsweringMPsListsMine.SelectedEntities = mps;
	        testFilters.AskingMPsListsNotMine.SelectedEntities = mps;
	        testFilters.AskingMPsListsMine.SelectedEntities = mps;
	        testFilters.AuthorityLists.SelectedEntities = authorities;
            testFilters.CommitteeLists.SelectedEntities = committees;
            testFilters.QuestionWriterLists.SelectedEntities = people;
            
            testFilters.RemoveAllSelections();
            
            Assert.Empty(testFilters.AuthorityLists.SelectedEntities);
            Assert.Empty(testFilters.QuestionWriterLists.SelectedEntities);
            Assert.Empty(testFilters.CommitteeLists.SelectedEntities);
            Assert.Empty(testFilters.AnsweringMPsListsMine.SelectedEntities);
            Assert.Empty(testFilters.AnsweringMPsListsNotMine.SelectedEntities);
            Assert.Empty(testFilters.AskingMPsListsMine.SelectedEntities);
            Assert.Empty(testFilters.AskingMPsListsNotMine.SelectedEntities);
        }

        [Fact]
        public void TranscribeAuthorityAnswerersTest()
        {
            var testFilters = new FilterChoices();

            var testAuthority = new Authority()
            {
                AuthorityName = "TestAuthorityName"
            };
            testFilters.SelectedAuthorities.Add(testAuthority);

            // Check that the answerers have exactly the single correct entry.
            var dataForTheServer = testFilters.TranscribeQuestionAnswerersForUpload();
            var listOfUploadedNames = dataForTheServer.Select(a => a.AsAuthority.AuthorityName).ToList();
            Assert.Single(listOfUploadedNames);
            Assert.Contains("TestAuthorityName", listOfUploadedNames);
            
            // Check that the askers are blank.
            var askerDataForTheServer = testFilters.TranscribeQuestionAskersForUpload();
            Assert.Empty(askerDataForTheServer);
        }

        [Fact]
        public void TranscribeMPsMineAnswerersTest()
        {
            var testFilters = new FilterChoices();

            var testMP1 = new MP()
            {
                first_name = "TestMP1FirstName",
                surname = "TestMP1Surname"
            };
            var testMP2 = new MP()
            {
                first_name = "TestMP2FirstName",
                surname = "TestMP2Surname"
            };

            testFilters.SelectedAnsweringMPsMine.Add(testMP1);
            testFilters.SelectedAnsweringMPsMine.Add(testMP2);

            // Check that the answerers have the two records we expect.
            var dataForTheServer = testFilters.TranscribeQuestionAnswerersForUpload();
            var listOfUploadedSurnames = dataForTheServer.Select(a => a.AsMP?.surname).ToList();
            Assert.Equal(2, listOfUploadedSurnames.Count);
            Assert.Contains("TestMP1Surname", listOfUploadedSurnames);
            Assert.Contains("TestMP2Surname", listOfUploadedSurnames);
            var listOfUploadedFirstnames = dataForTheServer.Select(a => a.AsMP?.first_name).ToList();
            Assert.Equal(2, listOfUploadedFirstnames.Count);
            Assert.Contains("TestMP1FirstName", listOfUploadedFirstnames);
            Assert.Contains("TestMP2FirstName", listOfUploadedFirstnames);
            
            // Check that the askers are blank.
            var askerDataForTheServer = testFilters.TranscribeQuestionAskersForUpload();
            Assert.Empty(askerDataForTheServer);
        }
        [Fact]
        public void TranscribeMPsNotMineAnswerersTest()
{
            var testFilters = new FilterChoices();

            var testMP1 = new MP()
            {
                first_name = "TestMP1FirstName",
                surname = "TestMP1Surname"
            };
            var testMP2 = new MP()
            {
                first_name = "TestMP2FirstName",
                surname = "TestMP2Surname"
            };

            testFilters.SelectedAnsweringMPsNotMine.Add(testMP1);
            testFilters.SelectedAnsweringMPsNotMine.Add(testMP2);

            // Check that the two records we expect are present.
            var dataForTheServer = testFilters.TranscribeQuestionAnswerersForUpload();
            var listOfUploadedSurnames = dataForTheServer.Select(a => a.AsMP?.surname).ToList();
            Assert.Equal(2, listOfUploadedSurnames.Count);
            Assert.Contains("TestMP1Surname", listOfUploadedSurnames);
            Assert.Contains("TestMP2Surname", listOfUploadedSurnames);
            var listOfUploadedFirstnames = dataForTheServer.Select(a => a.AsMP?.first_name).ToList();
            Assert.Equal(2, listOfUploadedFirstnames.Count);
            Assert.Contains("TestMP1FirstName", listOfUploadedFirstnames);
            Assert.Contains("TestMP2FirstName", listOfUploadedFirstnames);
            
            // Check that the askers are blank.
            var askerDataForTheServer = testFilters.TranscribeQuestionAskersForUpload();
            Assert.Empty(askerDataForTheServer);
        }
        
        [Fact]
        public void TranscribeCommitteeAskersTest()
        {
            var testFilters = new FilterChoices();

            var testCommittee = new Committee(new CommitteeInfo()
            {
                jurisdiction    = ParliamentData.Jurisdiction.Federal,
                name = "TestCommitteeName",
            });
            testFilters.SelectedCommittees.Add(testCommittee);

            // Check that the single committee we expect is present.
            var dataForTheServer = testFilters.TranscribeQuestionAskersForUpload();
            var listOfUploadedCommitteeNames = dataForTheServer.Select(a => a.AsCommittee?.Name).ToList();
            Assert.Single(listOfUploadedCommitteeNames);
            Assert.Contains("TestCommitteeName", listOfUploadedCommitteeNames);
            
            // Check that the answerers are blank.
            var answererDataForTheServer = testFilters.TranscribeQuestionAnswerersForUpload();
            Assert.Empty(answererDataForTheServer);
        }        
        
        [Fact]
        public void TranscribeMPsMineAskersTest()
        {
            var testFilters = new FilterChoices();

            var testMP1 = new MP()
            {
                first_name = "TestMP1FirstName",
                surname = "TestMP1Surname"
            };
            var testMP2 = new MP()
            {
                first_name = "TestMP2FirstName",
                surname = "TestMP2Surname"
            };

            testFilters.SelectedAskingMPsMine.Add(testMP1);
            testFilters.SelectedAskingMPsMine.Add(testMP2);

            // Check that the 2 asking MPs we expect are present.
            var dataForTheServer = testFilters.TranscribeQuestionAskersForUpload();
            var listOfUploadedSurnames = dataForTheServer.Select(a => a.AsMP?.surname).ToList();
            Assert.Equal(2, listOfUploadedSurnames.Count);
            Assert.Contains("TestMP1Surname", listOfUploadedSurnames);
            Assert.Contains("TestMP2Surname", listOfUploadedSurnames);
            var listOfUploadedFirstnames = dataForTheServer.Select(a => a.AsMP?.first_name).ToList();
            Assert.Equal(2, listOfUploadedFirstnames.Count);
            Assert.Contains("TestMP1FirstName", listOfUploadedFirstnames);
            Assert.Contains("TestMP2FirstName", listOfUploadedFirstnames);
            
            // Check that the answerers are blank.
            var answererDataForTheServer = testFilters.TranscribeQuestionAnswerersForUpload();
            Assert.Empty(answererDataForTheServer);
        }
        [Fact]
        public void TranscribeMPsNotMineAskersTest()
{
            var testFilters = new FilterChoices();

            var testMP1 = new MP()
            {
                first_name = "TestMP1FirstName",
                surname = "TestMP1Surname"
            };
            var testMP2 = new MP()
            {
                first_name = "TestMP2FirstName",
                surname = "TestMP2Surname"
            };

            testFilters.SelectedAskingMPsNotMine.Add(testMP1);
            testFilters.SelectedAskingMPsNotMine.Add(testMP2);

            // Check that the two asking MPs we expect are present.
            var dataForTheServer = testFilters.TranscribeQuestionAskersForUpload();
            var listOfUploadedSurnames = dataForTheServer.Select(a => a.AsMP?.surname).ToList();
            Assert.Equal(2, listOfUploadedSurnames.Count);
            Assert.Contains("TestMP1Surname", listOfUploadedSurnames);
            Assert.Contains("TestMP2Surname", listOfUploadedSurnames);
            var listOfUploadedFirstnames = dataForTheServer.Select(a => a.AsMP?.first_name).ToList();
            Assert.Equal(2, listOfUploadedFirstnames.Count);
            Assert.Contains("TestMP1FirstName", listOfUploadedFirstnames);
            Assert.Contains("TestMP2FirstName", listOfUploadedFirstnames);
            
            // Check that the answerers are blank.
            var answererDataForTheServer = testFilters.TranscribeQuestionAnswerersForUpload();
            Assert.Empty(answererDataForTheServer);
        }
    }
}

