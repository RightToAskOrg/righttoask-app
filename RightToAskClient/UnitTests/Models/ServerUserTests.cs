using System.Collections.Generic;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xunit;

namespace UnitTests.Models
{
    public class ServerUserTests
    {
        private List<ElectorateWithChamber> CreateElectorates(SharingElectorateInfoOptions options)
        {
            var electorates = new List<ElectorateWithChamber>();
            if ((options & SharingElectorateInfoOptions.StateOrTerritory) != 0)
            {
                electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"));
            }

            if ((options & SharingElectorateInfoOptions.FederalElectorate) != 0)
            {
                electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
                    "house of representative"));
            }

            if ((options & SharingElectorateInfoOptions.StateElectorate) != 0)
            {
                electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly,
                    "legislative"));
            }

            return electorates;
        }

        private Registration CreateRegistration(SharingElectorateInfoOptions options)
        {
            var registration = new Registration();
            var electorates = CreateElectorates(options);
            if ((options & SharingElectorateInfoOptions.StateOrTerritory) != 0)
            {
                registration.UpdateStateStorePreferences((int)ParliamentData.StateEnum.VIC);
            }

            registration.Electorates = electorates;

            return registration;
        }

        [Fact]
        public void ShouldNotFilterElectorateByDefault_SavingLocalUserFlow()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.All);
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.All;
            var serverUser = new ServerUser(registration);

            Assert.Equal(registration.State, serverUser.state);
            Assert.Equal(CreateElectorates(SharingElectorateInfoOptions.All), serverUser.electorates);
            Assert.Equal(SharingElectorateInfoOptions.All, serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_Nothing_SavingUserToServerFlow()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.All);

            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.Nothing;

            var serverUser = new ServerUser(registration, true);

            Assert.Null(serverUser.state);
            Assert.Empty(serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_State_SavingUserToServerFlow()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.All);
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.StateOrTerritory;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(registration.State, serverUser.state);
            Assert.Equal(
                CreateElectorates(SharingElectorateInfoOptions.StateOrTerritory),
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_FederalElectorateAndState_SavingUserToServerFlow()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.All);
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.FederalElectorateAndState;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(registration.State, serverUser.state);
            Assert.Equal(
                CreateElectorates(SharingElectorateInfoOptions.FederalElectorateAndState),
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_StateElectorateAndState_SavingUserToServerFlow()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.All);
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.StateElectorateAndState;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(registration.State, serverUser.state);
            Assert.Equal(
                CreateElectorates(SharingElectorateInfoOptions.StateElectorateAndState),
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }


        [Fact]
        public void ShouldFilterElectorate_All_SavingUserToServerFlow()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.All);
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.All;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(registration.State, serverUser.state);
            Assert.Equal(
                CreateElectorates(SharingElectorateInfoOptions.All),
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }
    }
}