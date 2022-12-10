using System;
using System.Globalization;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using Xunit;

namespace UnitTests.Helpers
{
    // For testing Converters.cs
    
    public class ConvertersTests
    {

        [Fact]
        public void IsInParliamentConverterReturnsTrueWhenInParliamentPassed()
        {
            var isInParliamentConverter = new IsInParliament();

            var output = (bool)isInParliamentConverter.Convert(HowAnsweredOptions.InParliament,
                typeof(HowAnsweredOptions), "", CultureInfo.CurrentCulture);
            Assert.True(output);
        }

        [Fact]
        public void IsInParliamentConverterReturnsFalseWhenInAppPassed()
        {
            var isInParliamentConverter = new IsInParliament();
            var output = (bool)isInParliamentConverter.Convert(HowAnsweredOptions.InApp, typeof(HowAnsweredOptions), "",
                CultureInfo.CurrentCulture);
            Assert.False(output);
        }

        [Fact]
        public void IsInParliamentConverterReturnsFalseWhenDontKnowPassed()
        {
            var isInParliamentConverter = new IsInParliament();
            var output = (bool) isInParliamentConverter.Convert(HowAnsweredOptions.DontKnow, typeof(HowAnsweredOptions), "", CultureInfo.CurrentCulture);
            Assert.False(output);
        }

        [Fact]
        public void IsInParliamentConverterReturnsFalseWhenOtherTypePassed()
        {
            var isInParliamentConverter = new IsInParliament();
            var output = (bool) isInParliamentConverter.Convert("", typeof(HowAnsweredOptions), "", CultureInfo.CurrentCulture);
            Assert.False(output);
        }
    }
}