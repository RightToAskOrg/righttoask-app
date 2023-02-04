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
        [Fact]
        public void ReinitDataTest() // Function Test - Filter ViewModel
        {
            // Filter must be empty 
            FilterChoices filters = new FilterChoices();
            
            // arrange
            FilterViewModel vm = new FilterViewModel(filters);
            filters.SearchKeyword = "changed Keyword";
            vm.FilterChoices = filters;

            // act
            vm.ReinitData(); // this should set vm.Keyword
            // TODO: (unit-tests) we need another test that checks that `ReinitData` throws exception when we have some filters
            // This test works well
            
            // assert
            Assert.True(!string.IsNullOrEmpty(filters.SearchKeyword));
            Assert.Equal("changed Keyword", filters.SearchKeyword);
            Assert.True(!string.IsNullOrEmpty(vm.Keyword));
            Assert.NotNull(filters.SelectedAnsweringMPsMine);
            Assert.False(filters.SelectedAnsweringMPsMine.Any());
        }
    }
}
