using RightToAskClient;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using RightToAskClient.Models;
using RightToAskClient.Views.Controls;
using Xunit;

namespace UnitTests
{
    public class RegistrationViewModelTests
    {
        // properties
        private Registration createRegistration()
        {
            Registration registration = new Registration();
            registration.uid = "testUid01";
            registration.public_key = "fakeButValidPublicKey";
            registration.SelectedStateAsEnum = ParliamentData.StateEnum.VIC;
            registration.StateKnown = true;
            return registration;
        }

        [Fact]
        public void ConstructorIsRegisteredTest()
        {
            // arrange
            var registration = createRegistration();
            registration.registrationStatus = RegistrationStatus.Registered;

            // act
            RegistrationViewModel vm = new RegistrationViewModel(registration);
            bool validReg = registration.Validate();

            // assert
            Assert.True(validReg);
            Assert.True(vm.IsMyAccount);
            Assert.False(vm.CanEditUid);

            Assert.False(vm.ShowDMButton);
            Assert.False(vm.ShowSeeQuestionsButton);
            Assert.False(vm.ShowFollowButton);

            Assert.False(vm.ShowRegisterCitizenButton);
            Assert.False(vm.ShowRegisterOrgButton);
            Assert.True(vm.ShowRegisterMPButton);
            //Assert.True(vm.ShowDoneButton); unsure of where/when this variable needs to be true
        }

        [Fact]
        public void ConstructorNotRegisteredTest()
        {
            // arrange
            var registration = createRegistration();
            registration.registrationStatus = RegistrationStatus.NotRegistered;

            // act
            RegistrationViewModel vm = new RegistrationViewModel(registration);

            // assert
            Assert.False(vm.IsMyAccount);
            Assert.True(vm.CanEditUid);

            Assert.False(vm.ShowDMButton);
            Assert.False(vm.ShowSeeQuestionsButton);
            Assert.False(vm.ShowFollowButton);

            Assert.True(vm.ShowRegisterCitizenButton);
            Assert.True(vm.ShowRegisterOrgButton);
            Assert.True(vm.ShowRegisterMPButton);
            //Assert.True(vm.ShowDoneButton); unsure of where/when this variable needs to be true
        }

        [Fact]
        public void ConstructorReadingOnlyNotRegisteredTest()
        {
            // arrange
            var registration = createRegistration();
            registration.registrationStatus = RegistrationStatus.NotRegistered;

            // act
            RegistrationViewModel vm = new RegistrationViewModel(registration);

            // assert
            Assert.False(vm.IsMyAccount);
            Assert.True(vm.CanEditUid);

            // usure what we want to do with these variables in the case of them not being registered, as it sets the button text but the buttons are hidden.
            Assert.False(vm.ShowDMButton);
            Assert.False(vm.ShowSeeQuestionsButton);
            Assert.False(vm.ShowFollowButton);

            Assert.True(vm.ShowRegisterCitizenButton);
            //Assert.False(vm.ShowRegisterOrgButton);
            Assert.True(vm.ShowRegisterMPButton);
            Assert.False(vm.ShowDoneButton);
        }

        [Fact]
        public void ConstructorReadingOnlyIsRegisteredTest()
        {
            // arrange
            var registration = createRegistration();
            registration.registrationStatus = RegistrationStatus.Registered;

            // act
            RegistrationViewModel vm = new RegistrationViewModel(registration);
            bool validReg = registration.Validate();

            // assert
            Assert.True(validReg);
            Assert.True(vm.IsMyAccount);
            Assert.False(vm.CanEditUid);

            // These might need to be re-enabled in the viewModel for this case, since it looks like they are setting the button text, but aren't being shown.
            //Assert.True(vm.ShowDMButton);
            //Assert.True(vm.ShowSeeQuestionsButton);
            //Assert.True(vm.ShowFollowButton);

            Assert.False(vm.ShowRegisterCitizenButton);
            Assert.False(vm.ShowRegisterOrgButton);
            Assert.True(vm.ShowRegisterMPButton);
            Assert.False(vm.ShowDoneButton);
        }

        [Theory]
        [InlineData("valid_uid", "Valid Name", "", "", true)]
        [InlineData("valid_uid", "", "", "Name must not be empty.", false)]
        [InlineData("", "Valid Name", "", "", false)]
        [InlineData("", "", "", "Name must not be empty.", false)]
        [InlineData("valid_uid", "Invalid Name because it is tooooooooooooooooooooooooooooo long", "",
            "The maximum character limit is 60.", false)]
        [InlineData("valid=uid", "Valid Name", "", "", false)]
        public void ContinueButtonAndReport_ValidateName(
            string uid,
            string name,
            string usernameReport,
            string nameReport,
            bool enabled)
        {
            // arrange
            RegistrationViewModel vm = new RegistrationViewModel(createRegistration());
            vm.UserId = uid;
            vm.DisplayName = name;

            // act
            vm.ValidateName();

            // assert
            Assert.Equal(usernameReport, vm.ReportLabelText);
            Assert.Equal(nameReport, vm.NameLabelText);
            Assert.Equal(enabled, vm.AbleToContinue);
            Assert.Equal(enabled ? Accessibility.AccessibilityTrait.None : Accessibility.AccessibilityTrait.Disabled,
                vm.ContinueButtonAccessibilityTrait);
        }

        [Theory]
        [InlineData("valid_uid", "Valid Name", "", "", true)]
        [InlineData("valid_uid", "", "", "", false)]
        [InlineData("", "Valid Name", "Username must not be empty.", "", false)]
        [InlineData("", "", "Username must not be empty.", "", false)]
        [InlineData("valid_uid", "Invalid Name because it is tooooooooooooooooooooooooooooo long", "", "", false)]
        [InlineData("valid=uid", "Valid Name",
            "Please use only letters (a-z), numbers (0-9), hyphen (-), underscore (_), and dot (.) without spaces.", "",
            false)]
        [InlineData("valid_uid-valid_uid-valid_uid-valid_uid-", "Valid Name", "The maximum character limit is 30.", "",
            false)]
        public void ContinueButtonAndReport_ValidateUsername(
            string uid,
            string name,
            string usernameReport,
            string nameReport,
            bool enabled)
        {
            // arrange
            RegistrationViewModel vm = new RegistrationViewModel(createRegistration());
            vm.UserId = uid;
            vm.DisplayName = name;

            // act
            vm.ValidateUsername();

            // assert
            Assert.Equal(usernameReport, vm.ReportLabelText);
            Assert.Equal(nameReport, vm.NameLabelText);
            Assert.Equal(enabled, vm.AbleToContinue);
            Assert.Equal(enabled ? Accessibility.AccessibilityTrait.None : Accessibility.AccessibilityTrait.Disabled,
                vm.ContinueButtonAccessibilityTrait);
        }
    }
}