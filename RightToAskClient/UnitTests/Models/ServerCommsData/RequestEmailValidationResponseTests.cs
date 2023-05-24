using RightToAskClient.Models.ServerCommsData;
using Xunit;

namespace UnitTests.Models.ServerCommsData
{
    public class RequestEmailValidationResponseTests
    {
        [Fact]
        public void EmailValidationResponseWithEmailSentIsEmailSent()
        {
            var testResponse = new RequestEmailValidationResponse()
            {
                EmailSent = "testEmailSent",
                AlreadyValidated = ""
            };
            Assert.True(testResponse.IsValid);
            Assert.True(testResponse.IsEmailSent);
        }
        
        [Fact]
        public void EmailValidationResponseAlreadyValidatedIsAlreadyValidated()
        {
            var testResponse = new RequestEmailValidationResponse()
            {
                EmailSent = "",
                AlreadyValidated = "testAlreadyValidated"
            };
            Assert.True(testResponse.IsValid);
            Assert.False(testResponse.IsEmailSent);
        }
        
        [Fact]
        public void EmailValidationResponseWithTwoNonNullFieldsIsNotValid()
        {
            var testResponse = new RequestEmailValidationResponse()
            {
                EmailSent = "testEmailSent",
                AlreadyValidated = "TestAlreadyValidatedToo"
            };
            Assert.False(testResponse.IsValid);
        }
        
    }
}