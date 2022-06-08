using RightToAskClient;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xunit;

namespace UnitTests
{
    public class UnitTest1
    {
        // properties 
        public ValidationTests vTests = new ValidationTests();

        // Sample Tests
        #region Sample Tests
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.NotEqual(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public void MyFirstTheory(int value)
        {
            Assert.True(IsOdd(value));
        }

        bool IsOdd(int value)
        {
            return value % 2 == 1;
        }
        #endregion

        // Boolean Converter Test
        [Fact]
        public void ConverterTest()
        {
            // arrange data
            bool falseConvert = false;
            bool trueConvert = true;

            Type t = typeof(bool);
            CultureInfo c = new CultureInfo("es-ES", false);

            InvertConvert converterClass = new InvertConvert();

            // act on the data
            falseConvert = (bool)converterClass.Convert(falseConvert, t, null, c);
            trueConvert = (bool)converterClass.Convert(trueConvert, t, null, c);

            // assert
            Assert.True(falseConvert);
            Assert.False(trueConvert);
        }
    }
}
