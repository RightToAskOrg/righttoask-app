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
            var (validName, validationErrMessage) = registration.ValidateName();

            // assert
            Assert.False(validName);
            Assert.Equal("The maximum character limit is 60.", validationErrMessage);
        }

        [Fact]
        public void ShouldReturnFalseWhenNameIsEmpty()
        {
            // arrange
            var registration = new Registration();
            registration.display_name = "";

            // act
            var (validName, validationErrMessage) = registration.ValidateName();

            // assert
            Assert.False(validName);
            Assert.Equal("Display name must not be empty.", validationErrMessage);
        }

        [Fact]
        public void ShouldReturnTrueWhenNameIsValid()
        {
            // arrange
            var registration = new Registration();
            registration.display_name = "012345678901234567890123456789012345678901234567890123456789";
            // act
            var (validName, validationErrMessage) = registration.ValidateName();

            // assert
            Assert.True(validName);
            Assert.Empty(validationErrMessage);
        }
    }
}