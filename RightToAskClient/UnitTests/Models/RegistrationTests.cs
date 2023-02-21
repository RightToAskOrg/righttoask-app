using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xunit;

namespace UnitTests.Models
{
    public class RegistrationTests
    {
        [Fact]
        public void ShouldReturnFalseWhenNameIsOverCharLimit()
        {
            // arrange
            var registration = new Registration();
            registration.display_name = "0123456789012345678901234567890123456789012345678901234567890";

            // act
            bool validName = registration.ValidateName();

            // assert
            Assert.False(validName);
        }
        
        [Fact]
        public void ShouldReturnFalseWhenNameIsEmpty()
        {
            // arrange
            var registration = new Registration();
            registration.display_name = "";

            // act
            bool validName = registration.ValidateName();

            // assert
            Assert.False(validName);
        }
        
        [Fact]
        public void ShouldReturnTrueWhenNameIsValid()
        {
            // arrange
            var registration = new Registration();
            registration.display_name = "012345678901234567890123456789012345678901234567890123456789";

            // act
            bool validName = registration.ValidateName();

            // assert
            Assert.True(validName);
        }
    }
}