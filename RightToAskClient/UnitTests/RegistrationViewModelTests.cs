using RightToAskClient;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using RightToAskClient.Models;
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
            Assert.True(vm.ShowUpdateAccountButton);
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
            Assert.False(vm.ShowUpdateAccountButton);
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
            Assert.False(vm.ShowUpdateAccountButton);
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
            Assert.True(vm.ShowUpdateAccountButton);
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
    }
}
