using System;
using System.Globalization;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using Xamarin.Forms;
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

        [Fact]
        public void colorConverterReturnsGrayWhenFalsePassed()
        {
            var colorConverter = new ColorConverter();
            var colorOutput = (string) colorConverter.Convert(false, typeof(string),"", CultureInfo.CurrentCulture);
            
            var defaultColor = Application.Current.Resources["UnselectedOptionButtonColor"];
            Assert.Equal(defaultColor, colorOutput);
        }
        
        [Fact]
        public void colorConverterReturnsGrayWhenNonBoolPassed()
        {
            var colorConverter = new ColorConverter();
            var colorOutput = (string) colorConverter.Convert("Invalid input", typeof(string),"", CultureInfo.CurrentCulture);
            
            var defaultColor = Application.Current.Resources["UnselectedOptionButtonColor"];
            Assert.Equal(defaultColor, colorOutput);
        }
        
        [Fact]
        public void colorConverterReturnsParameterWhenTruePassed()
        {
            var colorConverter = new ColorConverter();
            var expectedColor = "Red";
            var colorOutput = (string) colorConverter.Convert(true, typeof(string),expectedColor, CultureInfo.CurrentCulture);
            Assert.Equal(expectedColor, colorOutput);
        }
        
        [Fact]
        public void colorConverterReturnsEmptyStringParameterWhenTruePassed()
        {
            var colorConverter = new ColorConverter();
            var expectedColor = "";
            var colorOutput = (string) colorConverter.Convert(true, typeof(string),expectedColor, CultureInfo.CurrentCulture);
            Assert.Equal(expectedColor, colorOutput);
        }
        
        [Fact]
        public void colorConverterReturnsDefaultWhenPassedNonStringParameter()
        {
            var colorConverter = new ColorConverter();
            var colorOutput = (string) colorConverter.Convert(true, typeof(string), null, CultureInfo.CurrentCulture);
            var defaultColor = "Gray";
            Assert.Equal(defaultColor, colorOutput);
        }
        
    }
}