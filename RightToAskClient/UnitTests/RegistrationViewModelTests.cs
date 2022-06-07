using RightToAskClient;
using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class RegistrationViewModelTests
    {
        // properties
        public ValidationTests vTests = new ValidationTests();

        [Fact]
        public void ConstructorIsRegisteredTest()
        {
            // arrange
            App.ReadingContext.ThisParticipant.RegistrationInfo = vTests.ValidRegistrationTest();
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            App.ReadingContext.IsReadingOnly = false;

            // act
            RegistrationViewModel vm = new RegistrationViewModel();
            bool validReg = App.ReadingContext.ThisParticipant.RegistrationInfo.Validate();

            // assert
            Assert.True(validReg);
            Assert.True(vm.ShowUpdateAccountButton);
            Assert.False(vm.CanEditUID);

            Assert.False(vm.ShowDMButton);
            Assert.False(vm.ShowSeeQuestionsButton);
            Assert.False(vm.ShowFollowButton);

            Assert.False(vm.ShowRegisterCitizenButton);
            Assert.False(vm.ShowRegisterOrgButton);
            Assert.False(vm.ShowRegisterMPButton);
            //Assert.True(vm.ShowDoneButton); unsure of where/when this variable needs to be true
        }

        [Fact]
        public void ConstructorNotRegisteredTest()
        {
            // arrange
            App.ReadingContext.ThisParticipant.IsRegistered = false;
            App.ReadingContext.IsReadingOnly = false;

            // act
            RegistrationViewModel vm = new RegistrationViewModel();

            // assert
            Assert.False(vm.ShowUpdateAccountButton);
            Assert.True(vm.CanEditUID);

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
            App.ReadingContext.ThisParticipant.IsRegistered = false;
            App.ReadingContext.IsReadingOnly = true;

            // act
            RegistrationViewModel vm = new RegistrationViewModel();

            // assert
            Assert.False(vm.ShowUpdateAccountButton);
            Assert.True(vm.CanEditUID);

            // usure what we want to do with these variables in the case of them not being registered, as it sets the button text but the buttons are hidden.
            Assert.False(vm.ShowDMButton);
            Assert.False(vm.ShowSeeQuestionsButton);
            Assert.False(vm.ShowFollowButton);

            Assert.False(vm.ShowRegisterCitizenButton);
            Assert.False(vm.ShowRegisterOrgButton);
            Assert.False(vm.ShowRegisterMPButton);
            Assert.False(vm.ShowDoneButton);
        }

        [Fact]
        public void ConstructorReadingOnlyIsRegisteredTest()
        {
            // arrange
            App.ReadingContext.ThisParticipant.RegistrationInfo = vTests.ValidRegistrationTest();
            App.ReadingContext.ThisParticipant.IsRegistered = true;
            App.ReadingContext.IsReadingOnly = true;

            // act
            RegistrationViewModel vm = new RegistrationViewModel();
            bool validReg = App.ReadingContext.ThisParticipant.RegistrationInfo.Validate();

            // assert
            Assert.True(validReg);
            Assert.True(vm.ShowUpdateAccountButton);
            Assert.False(vm.CanEditUID);

            // These might need to be re-enabled in the viewModel for this case, since it looks like they are setting the button text, but aren't being shown.
            //Assert.True(vm.ShowDMButton);
            //Assert.True(vm.ShowSeeQuestionsButton);
            //Assert.True(vm.ShowFollowButton);

            Assert.False(vm.ShowRegisterCitizenButton);
            Assert.False(vm.ShowRegisterOrgButton);
            Assert.False(vm.ShowRegisterMPButton);
            Assert.False(vm.ShowDoneButton);
        }
    }
}
