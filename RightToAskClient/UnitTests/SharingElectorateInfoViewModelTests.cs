using RightToAskClient.ViewModels;
using Xunit;

namespace UnitTests
{
    public class SharingElectorateInfoViewModelTests_
    {
        [Fact]
        public void ShouldEnableButtonAfterSelecting()
        {
            var viewModel = new SharingElectorateInfoViewModel();
            
            Assert.False(viewModel.AbleToFinish);

            viewModel.SelectedIndexValue = 1;
            
            Assert.True(viewModel.AbleToFinish);
        }
        
        [Fact]
        public void ShouldShowPrivateForAllInfo_NOTHING()
        {
            var viewModel = new SharingElectorateInfoViewModel();
            
            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.NOTHING;
            
            Assert.True(viewModel.AbleToFinish);
            
            Assert.False(viewModel.IsStatePublic);
            Assert.False(viewModel.IsFederalElectoratePublic);
            Assert.False(viewModel.IsStateElectoratePublic);
        }
        
        [Fact]
        public void ShouldShowPublicForStateInfo_STATE_OR_TERRITORY()
        {
            var viewModel = new SharingElectorateInfoViewModel();
            
            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.STATE_OR_TERRITORY;
            
            Assert.True(viewModel.AbleToFinish);
            
            Assert.True(viewModel.IsStatePublic);
            Assert.False(viewModel.IsFederalElectoratePublic);
            Assert.False(viewModel.IsStateElectoratePublic);
        }
        
        [Fact]
        public void ShouldShowPublicForStateAndFederalElectorateInfo_FEDERAL_ELECTORATE_AND_STATE()
        {
            var viewModel = new SharingElectorateInfoViewModel();
            
            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.FEDERAL_ELECTORATE_AND_STATE;
            
            Assert.True(viewModel.AbleToFinish);
            
            Assert.True(viewModel.IsStatePublic);
            Assert.True(viewModel.IsFederalElectoratePublic);
            Assert.False(viewModel.IsStateElectoratePublic);
        }
        
        [Fact]
        public void ShouldShowPublicForStateAndStateElectorateInfo_STATE_ELECTORATE_AND_STATE()
        {
            var viewModel = new SharingElectorateInfoViewModel();
            
            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.STATE_ELECTORATE_AND_STATE;
            
            Assert.True(viewModel.AbleToFinish);
            
            Assert.True(viewModel.IsStatePublic);
            Assert.False(viewModel.IsFederalElectoratePublic);
            Assert.True(viewModel.IsStateElectoratePublic);
        }
        [Fact]
        public void ShouldShowPublicForAllInfo_ALL()
        {
            var viewModel = new SharingElectorateInfoViewModel();
            
            Assert.False(viewModel.AbleToFinish);
            viewModel.SelectedIndexValue = (int)SharingElectorateInfoOptions.ALL;
            
            Assert.True(viewModel.AbleToFinish);
            
            Assert.True(viewModel.IsStatePublic);
            Assert.True(viewModel.IsFederalElectoratePublic);
            Assert.True(viewModel.IsStateElectoratePublic);
        }
    }
}