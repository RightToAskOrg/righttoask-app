using System.Collections.Generic;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xunit;

namespace UnitTests
{
    public class SharingElectorateInfoViewModelTests
    {
        private Registration CreateRegistration(SharingElectorateInfoOptions options)
        {
            var registration = new Registration();
            var electorates = new List<ElectorateWithChamber>();
            if ((options & SharingElectorateInfoOptions.StateOrTerritory) != 0)
            {
                electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, "VIC"));
            }
            if ((options & SharingElectorateInfoOptions.FederalElectorate) != 0)
            {
                electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives, "VIC"));
            }
            if ((options & SharingElectorateInfoOptions.StateElectorate) != 0)
            {
                electorates.Add(new ElectorateWithChamber(ParliamentData.Chamber.Qld_Legislative_Assembly, "VIC"));
            }
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
        public void ShouldShowAllOptions_ElectoratesAllAnswered()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.All));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State or Territory",
                "Federal Electorate and state/ territory",
                "State Electorate and state/ territory",
                "Federal Electorate, State Electorate and state/ territory"
            }, viewModel.SharingElectorateInfoOptionValues);
        }

        [Fact]
        public void ShouldShowAllOptions_OnlyStateAndFederalElectorate()
        {
            
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.FederalElectorateAndState));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State or Territory",
                "Federal Electorate and state/ territory"
            }, viewModel.SharingElectorateInfoOptionValues);
        }

        [Fact]
        public void ShouldShowAllOptions_OnlyStateAndStateElectorate()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.StateElectorateAndState));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State or Territory",
                "State Electorate and state/ territory",
            }, viewModel.SharingElectorateInfoOptionValues);
        }

        [Fact]
        public void ShouldShowAllOptions_OnlyState()
        {
            var viewModel = new SharingElectorateInfoViewModel(CreateRegistration(SharingElectorateInfoOptions.StateOrTerritory));

            Assert.Equal(new List<string>
            {
                "Nothing",
                "State or Territory"
            }, viewModel.SharingElectorateInfoOptionValues);
        }
    }
}