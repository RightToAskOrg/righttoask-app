using System.Collections.Generic;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xunit;

namespace UnitTests.Models
{
    public class ServerUserTests
    {
        [Fact]
        public void ShouldNotFilterElectorateByDefault_SavingLocalUserFlow()
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>()
            {
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives, "VIC")
            };
            registration.Electorates = electorates;
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.StateOrTerritory;

            var serverUser = new ServerUser(registration);

            Assert.Equal(electorates, serverUser.electorates);
            Assert.Equal(SharingElectorateInfoOptions.StateOrTerritory, serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_Nothing_SavingUserToServerFlow()
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>()
            {
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, "VIC")
            };
            registration.Electorates = electorates;
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.Nothing;

            var serverUser = new ServerUser(registration, true);

            Assert.Empty(serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_State_SavingUserToServerFlow()
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>()
            {
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, "VIC")
            };
            registration.Electorates = electorates;
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.StateOrTerritory;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(
                new List<ElectorateWithChamber>
                    { new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC") },
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_FederalElectorateAndState_SavingUserToServerFlow()
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>()
            {
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
                    "house of representative"),
                new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, "VIC")
            };
            registration.Electorates = electorates;
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.FederalElectorateAndState;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(
                new List<ElectorateWithChamber>
                {
                    new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                    new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
                        "house of representative"),
                },
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }

        [Fact]
        public void ShouldFilterElectorate_StateElectorateAndState_SavingUserToServerFlow()
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>()
            {
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
                    "house of representative"),
                new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, "legislative")
            };
            registration.Electorates = electorates;
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.StateElectorateAndState;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(
                new List<ElectorateWithChamber>
                {
                    new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                    new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, "legislative")
                },
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }


        [Fact]
        public void ShouldFilterElectorate_All_SavingUserToServerFlow()
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>()
            {
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"),
                new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
                    "house of representative"),
                new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Assembly, "legislative"),
                new ElectorateWithChamber(ParliamentData.Chamber.Vic_Legislative_Council, "legislative")
            };
            registration.Electorates = electorates;
            registration.SharingElectorateInfoOption = SharingElectorateInfoOptions.All;

            var serverUser = new ServerUser(registration, true);

            Assert.Equal(
                electorates,
                serverUser.electorates);
            Assert.Null(serverUser.sharing_electorate_info);
        }
    }
}