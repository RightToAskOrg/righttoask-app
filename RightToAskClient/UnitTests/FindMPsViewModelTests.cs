using RightToAskClient;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xunit;

namespace UnitTests
{
    public class FindMPsViewModelTests
    {
        // properties
        private FindMPsViewModel vm = new FindMPsViewModel();
        private Button button = new Button();
        public ValidationTests vTests = new ValidationTests();
        private Registration registrationInfo = new Registration();
        private Address address = new Address();


        // This test started crashing at the prompt displayed to choose to save electorates
        // but then after commenting that part out it also started failing/crashing at the GeoscapeClient call to find the electorates.
        [Fact]
        public void SubmitAddressButtonCommand()
        {
            // arrange data
            button.Command = vm.SubmitAddressButtonCommand;

            // act on the data
            registrationInfo = vTests.ValidRegistrationTest();
            IndividualParticipant.ProfileData.RegistrationInfo = registrationInfo;
            address = vTests.TestValidAddress();
            IndividualParticipant.ProfileData.Address = address;
            vm.Address = address;
            bool validReg = registrationInfo.Validate();
            button.Command.Execute(null);

            // assert
            Assert.True(validReg);
            Assert.True(!string.IsNullOrEmpty(IndividualParticipant.ProfileData.RegistrationInfo.State));
            Assert.True(string.IsNullOrEmpty(vm.ReportLabelText));
            Assert.True(vm.ShowFindMPsButton);
            Assert.False(vm.ShowSkipButton);
        }

        // the actual method is private and called from the command that was tried to be tested above
        // but instead I am just checking the serialization/deserialization process of the fields to the
        // Xamarin.Essentials Preferences storage, but since we can't access the Preferences from the test project
        // I had to remove those lines too.
        [Fact]
        public void SaveAddressTest()
        {
            vm.Address = vTests.TestValidAddress();
            var fullAddress = JsonSerializer.Serialize(vm.Address);
            //Preferences.Set("TestAddress", fullAddress); // save the full address
            //Preferences.Set("TestStateID", vm.SelectedState);
            //string prefAddress = Preferences.Get("TestAddress", "");
            //string state = Preferences.Get("TestStateID", "");
            var addressObj = JsonSerializer.Deserialize<Address>(fullAddress);

            // assert
            //Assert.True(!string.IsNullOrEmpty(state));
            //Assert.True(!string.IsNullOrEmpty(prefAddress));
            Assert.True(!string.IsNullOrEmpty(addressObj.Postcode));
            Assert.True(!string.IsNullOrEmpty(addressObj.CityOrSuburb));
            Assert.True(!string.IsNullOrEmpty(addressObj.StreetNumberAndName));
        }

        [Fact]
        public void KnowElectoratesCommandTest()
        {
            // arrange data
            button.Command = vm.KnowElectoratesCommand;

            // act
            button.Command.Execute(null);

            // assert
            Assert.True(vm.ShowKnowElectoratesFrame);
            Assert.False(vm.ShowAddressStack);
        }

        // Fails due to the preferences storage call in the method
        [Fact]
        public void LookupElectoratesCommandTest()
        {
            // arrange data
            button.Command = vm.LookupElectoratesCommand;

            // act
            button.Command.Execute(null);

            // assert
            Assert.False(vm.ShowKnowElectoratesFrame);
            Assert.True(vm.ShowAddressStack);
        }
    }
}
