using System.Text;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms.Internals;
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
            Assert.Equal("Name must not be empty.", validationErrMessage);
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

        [Fact]
        public void ShouldReturnFalseWhenUsernameIsOverCharLimit()
        {
            // arrange
            var registration = new Registration();
            registration.uid = "0123456789012345678901234567890";
            // act
            var (validName, validationErrMessage) = registration.ValidateUsername();

            // assert
            Assert.False(validName);
            Assert.Equal("The maximum character limit is 30.", validationErrMessage);
        }

        [Fact]
        public void ShouldReturnFalseWhenUsernameIsEmpty()
        {
            // arrange
            var registration = new Registration();
            registration.uid = "";
            // act
            var (validName, validationErrMessage) = registration.ValidateUsername();

            // assert
            Assert.False(validName);
            Assert.Equal("Username must not be empty.", validationErrMessage);
        }

        [Theory]
        [InlineData("$")]
        [InlineData(" ")]
        [InlineData("!")]
        [InlineData("@")]
        [InlineData("#")]
        [InlineData("%")]
        [InlineData("^")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("+")]
        [InlineData("=")]
        public void ShouldReturnFalseWhenUsernameHasSpecialCharacter(string username)
        {
            // arrange
            var registration = new Registration();
            registration.uid = username;
            // act
            var (validName, validationErrMessage) = registration.ValidateUsername();

            // assert
            Assert.False(validName);
            Assert.Equal(
                "Please use only letters (a-z), numbers (0-9), hyphen (-), underscore (_), and dot (.) without spaces.",
                validationErrMessage);
        }

        [Fact]
        public void ShouldReturnTrueWhenUsernameIsValid()
        {
            // arrange
            var registration = new Registration();
            registration.uid = "abc-0123_def.987-abc-0123_def.";

            // act
            var (validName, validationErrMessage) = registration.ValidateUsername();

            // assert
            Assert.True(validName);
            Assert.Empty(validationErrMessage);
        }
    }
}