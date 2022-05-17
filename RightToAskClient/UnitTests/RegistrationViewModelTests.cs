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
        [Fact]
        public void ConstructorTest()
        {
            // arrange
            App.ReadingContext.ThisParticipant.IsRegistered = true;

            // act
            RegistrationViewModel vm = new RegistrationViewModel();

            // assert
            Assert.True(vm.ShowUpdateAccountButton);
            //Assert.True(vm.ShowUpdateAccountButton);
            //Assert.True(vm.ShowUpdateAccountButton);
            //Assert.True(vm.ShowUpdateAccountButton);
            //Assert.True(vm.ShowUpdateAccountButton);
            //Assert.True(vm.ShowUpdateAccountButton);
        }
    }
}
