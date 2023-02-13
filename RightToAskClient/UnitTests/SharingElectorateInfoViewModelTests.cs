using System.Collections.Generic;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms.Platform.WPF.Extensions;
using Xunit;

namespace UnitTests
{
    public class SharingElectorateInfoViewModelTests
    {
        private Registration CreateRegistration(
            SharingElectorateInfoOptions options, 
            ParliamentData.StateEnum stateTerritory = ParliamentData.StateEnum.VIC)
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>();
            if ((options & SharingElectorateInfoOptions.StateOrTerritory) != 0)
            {
                electorates.Add(new ElectorateWithChamber(
                    ParliamentData.Chamber.Australian_Senate,
                    stateTerritory.ToString()));
            }

            if ((options & SharingElectorateInfoOptions.FederalElectorate) != 0)
            {
                electorates.Add(
                    new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
                        stateTerritory.ToString()));
            }

            if ((options & SharingElectorateInfoOptions.StateElectorate) != 0)
            {
                electorates.Add(
                    new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, 
                        stateTerritory.ToString()));
            }

            registration.UpdateStateStorePreferences((int)stateTerritory);

            registration.Electorates = electorates;

            return registration;
        }

        [Fact]
        public void ShouldEnableButtonAfterSelecting()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.False(viewModel.AbleToFinish);

            viewModel.SelectedIndexValue = 1;

            Assert.True(viewModel.AbleToFinish);
        }

        [Fact]
        public void ShouldShowPrivateForAllInfo_NOTHING()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.Nothing;

            Assert.True(viewModel.AbleToFinish);

            Assert.False(viewModel.IsStatePublic);
            Assert.False(viewModel.IsFederalElectoratePublic);
            Assert.False(viewModel.IsStateElectoratePublic);
        }

        [Fact]
        public void ShouldShowPublicForStateInfo_STATE_OR_TERRITORY()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.StateOrTerritory;

            Assert.True(viewModel.AbleToFinish);

            Assert.True(viewModel.IsStatePublic);
            Assert.False(viewModel.IsFederalElectoratePublic);
            Assert.False(viewModel.IsStateElectoratePublic);
        }

        [Fact]
        public void ShouldShowPublicForStateAndFederalElectorateInfo_FEDERAL_ELECTORATE_AND_STATE()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = 2; // FederalElectorateAndState;

            Assert.True(viewModel.AbleToFinish);

            Assert.True(viewModel.IsStatePublic);
            Assert.True(viewModel.IsFederalElectoratePublic);
            Assert.False(viewModel.IsStateElectoratePublic);
        }

        [Fact]
        public void ShouldShowPublicForStateAndStateElectorateInfo_STATE_ELECTORATE_AND_STATE()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = 3; // StateElectorateAndState;

            Assert.True(viewModel.AbleToFinish);

            Assert.True(viewModel.IsStatePublic);
            Assert.False(viewModel.IsFederalElectoratePublic);
            Assert.True(viewModel.IsStateElectoratePublic);
        }

        [Fact]
        public void ShouldShowPublicForAllInfo_ALL()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = 4; // All;

            Assert.True(viewModel.AbleToFinish);

            Assert.True(viewModel.IsStatePublic);
            Assert.True(viewModel.IsFederalElectoratePublic);
            Assert.True(viewModel.IsStateElectoratePublic);
        }

        [Fact]
        public void ShouldShowAllOptions_ElectoratesAllAnswered_State()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State",
                "Federal Electorate and state",
                "State Electorate and state",
                "Federal Electorate, State Electorate and state"
            }, viewModel.SharingElectorateInfoOptionValues);
            Assert.Equal("State", viewModel.StateOrTerritoryTitle);
            Assert.Equal("State Electorate", viewModel.StateOrTerritoryElectorateTitle);
        }

        [Fact]
        public void ShouldShowAllOptions_ElectoratesAllAnswered_Territory()
        {
            var viewModel = new SharingElectorateInfoViewModel(
                CreateRegistration(SharingElectorateInfoOptions.All, ParliamentData.StateEnum.NT));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "Territory",
                "Federal Electorate and territory",
                "Territory Electorate and territory",
                "Federal Electorate, Territory Electorate and territory"
            }, viewModel.SharingElectorateInfoOptionValues);
            Assert.Equal("Territory", viewModel.StateOrTerritoryTitle);
            Assert.Equal("Territory Electorate", viewModel.StateOrTerritoryElectorateTitle);
        }

        [Fact]
        public void ShouldShowAllOptions_OnlyStateAndFederalElectorate()
        {
            var viewModel =
                new SharingElectorateInfoViewModel(
                    CreateRegistration(SharingElectorateInfoOptions.FederalElectorateAndState));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State",
                "Federal Electorate and state"
            }, viewModel.SharingElectorateInfoOptionValues);
        }

        [Fact]
        public void ShouldShowAllOptions_OnlyStateAndStateElectorate()
        {
            var viewModel =
                new SharingElectorateInfoViewModel(
                    CreateRegistration(SharingElectorateInfoOptions.StateElectorateAndState));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State",
                "State Electorate and state",
            }, viewModel.SharingElectorateInfoOptionValues);
        }

        [Fact]
        public void ShouldShowAllOptions_OnlyStateAndFederalElectorate_EmptyStateElectorate()
        {
            var registration = CreateRegistration(SharingElectorateInfoOptions.FederalElectorateAndState);
            registration.Electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, ""));
            var viewModel = new SharingElectorateInfoViewModel(registration);

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State",
                "Federal Electorate and state"
            }, viewModel.SharingElectorateInfoOptionValues);
        }


        [Fact]
        public void ShouldShowAllOptions_OnlyState()
        {
            var viewModel =
                new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.StateOrTerritory));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State"
            }, viewModel.SharingElectorateInfoOptionValues);
        }
    }
}