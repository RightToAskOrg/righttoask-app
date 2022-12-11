using System;
using System.Globalization;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using Xunit;

namespace UnitTests.Models
{
    // For testing Question.cs

    public class QuestionTests
    {

        /* FIXME - this test displays exactly the same unpredictable behaviour that was
         * identified for other tests using IndividualParticipant. Check how to make sure
         * they don't interfere.
         *
        [Fact]
        // Check that the Upvote command increases upvotes if you haven't already voted 
        public void UpvoteQuestionWhenUserIsRegisteredAndHasntUpvotedItAlready()
        {
            IndividualParticipant.getInstance().ProfileData.RegistrationInfo.registrationStatus = RegistrationStatus.Registered;
            var testQuestionResponseRecords = new QuestionResponseRecords(); 
            var testQuestion = new Question(testQuestionResponseRecords);

            testQuestion.UpvoteCommand.Execute(null);

            Assert.True(testQuestion.AlreadyUpvoted);
        }
        */
    }
}