using RightToAskClient.Models;
using Xunit;

namespace UnitTests.Models
{
    public class QuestionResponseRecordsTest
    {
        [Fact]
        
        public void TestAddingUpvotedQuestionSucceeds()
        {
            string testID = "testQuestionID";
            var testQuestionRecords = new QuestionResponseRecords();

            testQuestionRecords.AddUpvotedQuestion(testID);

            Assert.True(testQuestionRecords.IsAlreadyUpvoted(testID));
        } 
    }
}