using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using RightToAskClient.Views.Popups;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

/* This class contains some utilities for popping up and using pages, which may be accessed
 * at any time by the app, for example the pages for finding your MP, which needs to be
 * popped whenever you do something requiring knowing your MPs but haven't found them yet.
 * TODO The Authority-selecting list should probably be here too, rather than being repeated in the Advanced Search and
 * Question-Answerer steps.
 */
namespace RightToAskClient.Helpers
{
    public static class NavigationUtils
    {
        public static async Task PushMyAnsweringMPsExploringPage(bool electoratesKnown, SelectableList<MP> answeringMPsListsMine)
        {
            var message = "These are your MPs.  Select the one(s) who should answer the question";

            //TODO** Seems unnecessary if our MPs are not initialized. 
            // Below, don't make the pages that are never used. The code is (somewhat redundant but)
            // correct but names are confusing.
            var mpsSelectableListPage = new SelectableListPage(answeringMPsListsMine, message);
            var nextPage = ListMPsFindFirstIfNotAlreadyKnown(mpsSelectableListPage, electoratesKnown);
            await Application.Current.MainPage.Navigation.PushAsync(nextPage);
        }

        public static async Task PushMyAskingMPsExploringPage(bool electoratesKnown, SelectableList<MP> askingMPsListsMine)
        {
            var message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";

            var mpsSelectableListPage = new SelectableListPage(askingMPsListsMine, message);
            await LaunchMPFindingAndSelectingPages(mpsSelectableListPage, electoratesKnown);
        }

        //
        private static async Task LaunchMPFindingAndSelectingPages(SelectableListPage mpsListPage, bool electoratesKnown)
        {
            var nextPage = ListMPsFindFirstIfNotAlreadyKnown(mpsListPage, electoratesKnown);
            await Application.Current.MainPage.Navigation.PushAsync(nextPage);
        }
        
        /*
		 * Either push the list of selectable MPs directly, or push a registration page,
		 * instructed to push the MPs selection page after.
		 */
        public static Page ListMPsFindFirstIfNotAlreadyKnown(SelectableListPage mpsListPage, bool electoratesKnown)
        {
            if (electoratesKnown)
            {
                return mpsListPage;
            }
            else
            {
                var registrationPage = new FindMPsPage();
                return registrationPage;
            }
        }

        public static async Task PushAnsweringMPsNotMineSelectableListPage(SelectableList<MP> answeringMPsListsNotMine)
        {
            var message = AppResources.SelectMPToAnswerText;
            var mpsPage =
                new SelectableListPage(answeringMPsListsNotMine, message);
            await Application.Current.MainPage.Navigation.PushAsync(mpsPage);
        }

        public static async Task PushAskingMPsNotMineSelectableListPageAsync(SelectableList<MP> askingMPsListsNotMine)
        {
            var message = AppResources.SelectMPToRaiseText; 
            var mpsPage =
                new SelectableListPage(askingMPsListsNotMine, message);
            await Application.Current.MainPage.Navigation.PushAsync(mpsPage);
        }
        
        public static async Task EditCommitteesClicked(SelectableList<Committee> committeeLists)
        {
            var committeeSelectableListPage
                = new SelectableListPage(committeeLists, AppResources.CommitteeText);
            await Application.Current.MainPage.Navigation.PushAsync(committeeSelectableListPage);
        }
        
        public static async Task DoRegistrationCheck(Registration registration, string cancelMessage)
        {
            var popup = new TwoButtonPopup(
                AppResources.MakeAccountQuestionText, 
                AppResources.CreateAccountPopUpText, 
                cancelMessage, 
                AppResources.OKText, 
                false);
            //TODO:
            //var popupResult = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            //if (popup.HasApproved(popupResult))
            //{
            //    var registerAccountFlow = new CodeOfConductPage(registration);
            //    await Application.Current.MainPage.Navigation.PushAsync(registerAccountFlow);
            //    // var registerAccountPage = new RegisterAccountPage(registration);
            //    // await Application.Current.MainPage.Navigation.PushAsync(registerAccountPage);
            //}
        }
    }
}