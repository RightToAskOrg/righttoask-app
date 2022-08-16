using RightToAskClient;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xunit;

namespace UnitTests
{
    public class FilterViewModelTests
    {
        // properties 
        public ValidationTests vTests = new ValidationTests();

        [Fact]
        public void ReinitDataTest() // Function Test - Filter ViewModel
        {
            // arrange
            FilterViewModel vm = new FilterViewModel();
            FilterChoices filters = vTests.ValidateFiltersTest();
            filters.SearchKeyword = "changed Keyword";
            App.ReadingContext.Filters.SearchKeyword = filters.SearchKeyword;

            // act
            vm.ReinitData(); // this should set vm.Keyword

            // assert
            Assert.True(!string.IsNullOrEmpty(filters.SearchKeyword));
            Assert.Equal("changed Keyword", filters.SearchKeyword);
            Assert.True(!string.IsNullOrEmpty(vm.Keyword));
            Assert.NotNull(filters.SelectedAnsweringMPsMine);
            Assert.False(filters.SelectedAnsweringMPsMine.Any());
            Assert.True(!string.IsNullOrEmpty(vm.SelectedAnsweringMPsText));
        }

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

        //[Fact]
        //public void BackCommandTest()
        //{
        //    // arrange
        //    FilterViewModel vm = new FilterViewModel();
        //    vm.CameFromMainPage = false;
        //    Button button = new Button()
        //    {
        //        Command = vm.BackCommand
        //    };
        //    App.Current = new App(); // need the XamarinForms.Mocks plugin to initialize the shell and xamarin forms.init() before we can test navigtaion.
        //    App.Current.MainPage = new Shell();

        //    // act
        //    button.Command.Execute(null);

        //    // assert
        //    Assert.True(App.Current.MainPage.Navigation.NavigationStack.Count > 0);
        //}

        //[Fact]
        //public void BackCommandFromMainPageTest()
        //{
        //    // arrange
        //    FilterViewModel vm = new FilterViewModel();
        //    vm.CameFromMainPage = true;
        //    Button button = new Button()
        //    {
        //        Command = vm.BackCommand
        //    };

        //    // act
        //    button.Command.Execute(null);

        //    // assert
        //    Assert.True(App.Current.MainPage.Navigation.NavigationStack.Count == 0);
        //}
    }
}
