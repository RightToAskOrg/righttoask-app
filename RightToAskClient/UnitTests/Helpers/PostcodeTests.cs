using RightToAskClient.Helpers;
using RightToAskClient.Models;
using Xunit;

namespace UnitTests.Helpers
{
    public class PostcodeTests
    {
        [Fact]
        public void CheckingIfPostcodeIsValidForNT()
        {
            var result = Postcode.Check(ParliamentData.StateEnum.NT, 2000);
            Assert.False(result);
        }
        
        [Fact]
        public void CheckingIfPostcodeIsValidForNSW()
        {
            var result = Postcode.Check(ParliamentData.StateEnum.NSW, 2900);
            Assert.False(result);
        }
    }
}