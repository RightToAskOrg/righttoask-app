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

        // ListToString Converter Tests
        [Fact]
        public void CreateTextGivenListEntitiesTest()
        {
            // arrange data
            ElectorateWithChamber electorateWithChamber = new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, "VIC");
            #region create MP data
            MP validMP = new MP()
            {
                first_name = "firstname",
                surname = "lastname",
                electorate = electorateWithChamber,
                email = "email",
                role = "role",
                party = "party"
            };
            MP validMP2 = new MP()
            {
                first_name = "firstname2",
                surname = "lastname2",
                electorate = electorateWithChamber,
                email = "email2",
                role = "role2",
                party = "party2"
            };
            MP validMP3 = new MP()
            {
                first_name = "firstname3",
                surname = "lastname3",
                electorate = electorateWithChamber,
                email = "email3",
                role = "role3",
                party = "party3"
            };
            #endregion

            List<MP> mps = new List<MP>
            {
                validMP,
                validMP2,
                validMP3
            };

            // act
            FilterViewModel vm = new FilterViewModel();
            string result = vm.CreateTextGivenListEntities(mps);

            // assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public void SendNewQuestionToServerTest()
        {
            // Not sure how I want to go about testing for this and other server related communications yet
        }

        [Fact]
        public void ReinitDataTest() // Function Test
        {
            // arrange
            FilterViewModel vm = new FilterViewModel();
            FilterChoices filters = new FilterChoices(); // need to test for filterchoices first?
            filters.SearchKeyword = "test";
            App.ReadingContext.Filters.SearchKeyword = filters.SearchKeyword;

            // act
            vm.ReinitData(); // this should set vm.Keyword

            // assert
            Assert.True(!string.IsNullOrEmpty(filters.SearchKeyword));
            Assert.True(!string.IsNullOrEmpty(vm.Keyword));
        }

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
