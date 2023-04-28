using System;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xunit;

namespace UnitTests.Models.ServerCommsData
{
    public class ServerResultTests
    {
        [Fact]
        public void ServerOKWithNoDataProducesJOSOKWithNoData()
        {
            var testServerResult = new ServerResult<string>()
            {
                Ok = null
            };

            var testJOSResult = testServerResult.ToDatalessJOSResult();
            
            Assert.NotNull(testJOSResult);
            Assert.True(testJOSResult.Success);
        } 
        
        [Fact]
        public void ServerOKWithNoDataCausesExceptionWhenTryingToMakeJOSOKWithData()
        {
            var testServerResult = new ServerResult<string>()
            {
                Ok = null
            };

            Assert.Throws<Exception>( () =>
                testServerResult.ToJOSResult()
                );
        } 
        
        [Fact]
        public void ServerOKWithDataCausesExceptionWhenTryingToMakeJOSOKWithNoData()
        {
            var testServerResult = new ServerResult<string>()
            {
                Ok = "Test string" 
            };

            Assert.Throws<Exception>( () =>
                testServerResult.ToDatalessJOSResult()
                );
        } 
        // "Int" here is meant as a stand-in for any data type.
        [Fact]
        public void ServerOKWithIntProducesJOSOKWithSameInt()
        {
            var testInt = 42;
            var testServerResult = new ServerResult<int>()
            {
                Ok = testInt 
            };

            var testJOSResult = testServerResult.ToJOSResult();
            
            Assert.NotNull(testJOSResult);
            Assert.True(testJOSResult.Success);
            Assert.Equal(testInt, testJOSResult.Data);
        } 
        
        // Again, int could be any type.
        [Fact]
        public void ServerErrorWithDataTypeProducesJOSErrorWithSameMessage()
        {
            var errorMessage = "Test error message";
            var testServerResult = new ServerResult<int>()
            {
                Err = errorMessage
            };

            var testJOSResult = testServerResult.ToJOSResult();
            var testErrorResult = testJOSResult as ErrorResult<int>;

            Assert.NotNull(testErrorResult);
            Assert.False(testErrorResult.Success);
            Assert.True(testErrorResult.Failure);
            Assert.Equal(errorMessage, testErrorResult.Message);
        }
    }
}