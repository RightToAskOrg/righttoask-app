using System;
using System.Collections.Generic;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xunit;

namespace UnitTests.Models
{
    public class QuestionUpdatesTests
    {
        private string testQuestionID = "testQuestionID";
        private string testVersionNumber = "testVersionNumber";
        private string testBackground = "testBackground";
        private string testAnswer = "testAnswer";
        private string testHansardLink = "https://test.parliament.vic.gov.au";
        
        
        [Fact]
        public void EmptyQuestionUpdates_HasNoContents()
        {
            var updates = new QuestionUpdates();
            
            Assert.False(updates.AnyUpdates);
            Assert.True(String.IsNullOrEmpty(updates.QuestionID));
            Assert.True(String.IsNullOrEmpty(updates.Version));
            Assert.True(String.IsNullOrEmpty(updates.NewBackground));
            Assert.Empty(updates.NewAnswers);
            Assert.Empty(updates.NewHansardLinks);
            Assert.Equal(RTAPermissions.NoChange, updates.WhoShouldAnswerPermissions);
            Assert.Equal(RTAPermissions.NoChange, updates.WhoShouldAskPermissions);
        }
        
        [Fact]
        public void InitialQuestionUpdates_HasNoUpdates()
        {
            var updates = new QuestionUpdates(testQuestionID, testVersionNumber);
            
            Assert.False(updates.AnyUpdates);
            Assert.Equal(testQuestionID, updates.QuestionID);
            Assert.Equal(testVersionNumber, updates.Version); 
        }
        
        [Fact]
        public void UpdatedBackground_HasUpdates()
        {
            var updates = new QuestionUpdates(testQuestionID, testVersionNumber);

            updates.NewBackground = testBackground;
            
            Assert.True(updates.AnyUpdates);
            Assert.False(String.IsNullOrEmpty(updates.NewBackground));
        }
        
        [Fact]
        public void UpdatedAnswer_HasUpdates()
        {
            var updates = new QuestionUpdates(testQuestionID, testVersionNumber);

            updates.NewAnswers = new List<QuestionAnswer>()
            {
                new QuestionAnswer()
                {
                    answer = testAnswer,
                    answered_by = "",
                    mp = new MPId()
                    {
                        first_name = "testFirstName",
                        surname = "testSurname",
                        electorate = new ElectorateWithChamber( ParliamentData.Chamber.Australian_Senate, "Vic")
                    }
                }
            };
            
            Assert.True(updates.AnyUpdates);
            Assert.NotEmpty(updates.NewAnswers);
        }
        
        [Fact]
        public void UpdatedHansardLinks_HasUpdates()
        {
            var updates = new QuestionUpdates(testQuestionID, testVersionNumber);

            updates.NewHansardLinks = new List<HansardLink>()
            {
                new HansardLink()
                {
                    url = testHansardLink
                }
            };
            
            Assert.True(updates.AnyUpdates);
            Assert.NotEmpty(updates.NewHansardLinks);
        }
        
        [Fact]
        public void UpdatedWhoCanAnswerPermissions_HasUpdates()
        {
            var updates = new QuestionUpdates(testQuestionID, testVersionNumber);

            updates.WhoShouldAnswerPermissions = RTAPermissions.Others;
            
            Assert.True(updates.AnyUpdates);
            Assert.Equal(RTAPermissions.Others, updates.WhoShouldAnswerPermissions);
        }
        
        [Fact]
        public void UpdatedWhoCanAskPermissions_HasUpdates()
        {
            var updates = new QuestionUpdates(testQuestionID, testVersionNumber);

            updates.WhoShouldAskPermissions = RTAPermissions.WriterOnly;
            
            Assert.True(updates.AnyUpdates);
            Assert.Equal(RTAPermissions.WriterOnly, updates.WhoShouldAskPermissions);
        }
    }
}