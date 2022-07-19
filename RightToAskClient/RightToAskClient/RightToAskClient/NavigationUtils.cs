using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using Xamarin.Forms;

/* This class contains some utilities for popping up and using pages, which may be accessed
 * at any time by the app, for example the pages for finding your MP, which needs to be
 * popped whenever you do something requiring knowing your MPs but haven't found them yet.
 */
namespace RightToAskClient
{
    public static class NavigationUtils
    {
        public static async Task PushMyAnsweringMPsExploringPage()
        {
            string message = "These are your MPs.  Select the one(s) who should answer the question";

            //TODO** Seems unnecessary if our MPs are not initialized. 
            // Below, don't make the pages that are never used. The code is (somewhat redundant but)
            // correct but names are confusing.
            var mpsSelectableListPage = new SelectableListPage(App.ReadingContext.Filters.AnsweringMPsListsMine, message, true);
            var nextPage = ListMPsFindFirstIfNotAlreadyKnown(mpsSelectableListPage);
            await Application.Current.MainPage.Navigation.PushAsync(nextPage);
        }

        public static async Task PushMyAskingMPsExploringPage()
        {
            string message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";

            var mpsSelectableListPage = new SelectableListPage(App.ReadingContext.Filters.AskingMPsListsMine, message, true);
            await LaunchMPFindingAndSelectingPages(mpsSelectableListPage);
        }

        //
        private static async Task LaunchMPFindingAndSelectingPages(SelectableListPage mpsListPage)
        {
            var nextPage = ListMPsFindFirstIfNotAlreadyKnown(mpsListPage);
            await Application.Current.MainPage.Navigation.PushAsync(nextPage);
        }
        
        /*
		 * Either push the list of selectable MPs directly, or push a registration page,
		 * instructed to push the MPs selection page after.
		 */
        public static Page ListMPsFindFirstIfNotAlreadyKnown(SelectableListPage mpsListPage)
        {
            var thisParticipant = App.ReadingContext.ThisParticipant;

            if (!thisParticipant.MPsKnown)
            {
                var registrationPage = new RegisterPage2();
                return registrationPage;
            }
            else
            {
                return mpsListPage;
            }
        }

        public static async Task PushAnsweringMPsNotMineSelectableListPage()
        {
            string message = AppResources.SelectMPToAnswerText;
            SelectableListPage mpsPage =
                new SelectableListPage(App.ReadingContext.Filters.AnsweringMPsListsNotMine, message, false);
            await Application.Current.MainPage.Navigation.PushAsync(mpsPage);
        }

        public static async Task PushAskingMPsNotMineSelectableListPageAsync()
        {
            string message = AppResources.SelectMPToRaiseText; 
            SelectableListPage mpsPage =
                new SelectableListPage(App.ReadingContext.Filters.AskingMPsListsNotMine, message, false);
            await Application.Current.MainPage.Navigation.PushAsync(mpsPage);
        }
    }
}