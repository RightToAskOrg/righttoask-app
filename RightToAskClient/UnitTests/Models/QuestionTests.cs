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

        [Fact]
        // Check that the Upvote command increases upvotes if you haven't already voted 
        public void UpvoteQuestionWhenUserIsRegisteredAndHasntUpvotedItAlready()
        {
            var testQuestion = new Question();
            testQuestion.AlreadyUpvoted = false;

            testQuestion.UpvoteCommand.Execute(null);

            Assert.True(testQuestion.AlreadyUpvoted);
        }
    }
}