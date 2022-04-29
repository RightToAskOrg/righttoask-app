using Microsoft.VisualStudio.TestTools.UnitTesting;
using RightToAskClient.ViewModels;
using System;
using Xunit;

namespace RightToAsk.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadingPageQuestionListTest()
        {
            // Arrange
            var vm = new ReadingPageViewModel();

            // Act
            var questions = vm.QuestionsToDisplay;

            // Assert
            Assert.IsTrue(questions != null, "vm.QuestionsToDisplay is null!");
        }

        [Fact]
        public void Test1()
        {
            Assert.AreEqual(5, 2 + 3);
        }
    }
}
