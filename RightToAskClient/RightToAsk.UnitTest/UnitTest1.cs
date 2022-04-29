using RightToAskClient.ViewModels;
using System;
using Xunit;

namespace RightToAsk.UnitTest
{
    public class UnitTest1
    {
        public void ReadingPageQuestionListTest()
        {
            // Arrange
            var vm = new ReadingPageViewModel();

            // Act
            var questions = vm.QuestionsToDisplay;

            // Assert
            Assert.True(questions != null, "vm.QuestionsToDisplay is null!");
        }

        [Fact]
        public void Test1()
        {
            Assert.Equal(5, 2 + 3);
        }

        [Fact]
        public void Test2()
        {
            Assert.Equal(6, 2 + 3);
        }
    }
}
