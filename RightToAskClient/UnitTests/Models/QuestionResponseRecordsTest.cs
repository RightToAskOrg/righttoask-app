using RightToAskClient.Models;
using Xunit;

namespace UnitTests.Models
{
    public class QuestionResponseRecordsTest
    {
        // TODO: Figure out how to test proper storage to, and retrieval from, preferences.
        [Fact]
        public void CorrectInitOfQuestionResponseRecordsFromPreferences()
        {
            var testQuestionRecords = new QuestionResponseRecords();
        }
        
        [Fact]
        public void TestAddingDismissedQuestionSucceeds()
        {
            string testID = "testQuestionID";
            string unusedID = "saflkjsadlkjfsajlkfsalkjsdlfjsdkllksdf";
            var testQuestionRecords = new QuestionResponseRecords();

            testQuestionRecords.AddDismissedQuestion(testID);

            Assert.True(testQuestionRecords.IsAlreadyDismissed(testID));
            Assert.False(testQuestionRecords.IsAlreadyDismissed(unusedID));
        } 
        [Fact]
        public void TestAddingUpvotedQuestionSucceeds()
        {
            string testID = "testQuestionID";
            string unusedID = "saflkjsadlkjfsajlkfsalkjsdlfjsdkllksdf";
            var testQuestionRecords = new QuestionResponseRecords();

            testQuestionRecords.AddUpvotedQuestion(testID);

            Assert.True(testQuestionRecords.IsAlreadyUpvoted(testID));
            Assert.False(testQuestionRecords.IsAlreadyUpvoted(unusedID));
        } 
        [Fact]
        public void TestAddingDownvotedQuestionSucceeds()
        {
            string testID = "testQuestionID";
            string unusedID = "saflkjsadlkjfsajlkfsalkjsdlfjsdkllksdf";
            var testQuestionRecords = new QuestionResponseRecords();

            testQuestionRecords.AddDownvotedQuestion(testID);

            Assert.True(testQuestionRecords.IsAlreadyDownvoted(testID));
            Assert.False(testQuestionRecords.IsAlreadyDownvoted(unusedID));
        } 
        
        [Fact]
        public void TestAddingReportedQuestionSucceeds()
        {
            string testID = "testQuestionID";
            string unusedID = "saflkjsadlkjfsajlkfsalkjsdlfjsdkllksdf";
            var testQuestionRecords = new QuestionResponseRecords();

            testQuestionRecords.AddReportedQuestion(testID);

            Assert.True(testQuestionRecords.IsAlreadyReported(testID));
            Assert.False(testQuestionRecords.IsAlreadyReported(unusedID));
        } 
    }
}