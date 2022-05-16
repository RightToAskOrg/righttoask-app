using System.Collections.ObjectModel;
using System.Threading.Tasks;
using RightToAskClient.Models;
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
            // var mpsExploringPage = new ExploringPage(readingContext.ThisParticipant.MyMPs,
            var mpsExploringPage = new ExploringPage(App.ReadingContext.ThisParticipant.GroupedMPs,
                App.ReadingContext.Filters.SelectedAnsweringMPsMine, message);

            var nextPage = ListMPsFindFirstIfNotAlreadyKnown(mpsExploringPage);
            await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            //await Shell.Current.GoToAsync($"{nameof(ExploringPage)}");
        }

        public static async Task PushMyAskingMPsExploringPage()
        {
            string message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";

            var mpsExploringPage = new ExploringPage(App.ReadingContext.ThisParticipant.GroupedMPs,
                App.ReadingContext.Filters.SelectedAskingMPsMine, message);

            await LaunchMPFindingAndSelectingPages(mpsExploringPage);
        }

        private static async Task LaunchMPFindingAndSelectingPages(ExploringPage mpsExploringPage)
        {
            var nextPage = ListMPsFindFirstIfNotAlreadyKnown(mpsExploringPage);
            await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            //await Shell.Current.GoToAsync($"{nameof(ExploringPage)}");
        }
        /*
		 * Either push the list of selectable MPs directly, or push a registration page,
		 * instructed to push the MPs selection page after.
		 */
        public static Page ListMPsFindFirstIfNotAlreadyKnown(ExploringPage mpsExploringPage)
        {
            var thisParticipant = App.ReadingContext.ThisParticipant;

            if (!thisParticipant.MPsKnown)
            {
                var registrationPage = new RegisterPage2();
                return registrationPage;
            }
            else
            {
                return mpsExploringPage;
            }
        }

        public static async Task PushAnsweringMPsNotMineSelectableListPage()
        {
            string message = "Here is the complete list of MPs - select which one(s) should answer";
            SelectableListPage mpsPage =
                new SelectableListPage(App.ReadingContext.Filters.OtherMPAnsweringLists, message, false);
            // ExploringPageWithSearch mpsPage
            //     = new ExploringPageWithSearch(ParliamentData.AllMPs, App.ReadingContext.Filters.SelectedAnsweringMPs, message);
            await Application.Current.MainPage.Navigation.PushAsync(mpsPage);
            //await Shell.Current.GoToAsync($"{nameof(ExploringPageWithSearch)}");
        }

        public static async Task PushAskingMPsNotMineSelectableListPageAsync()
        {
            string message =
                "Here is the complete list of MPs - select which one(s) should raise your question in Parliament";
            SelectableListPage mpsPage =
                new SelectableListPage(App.ReadingContext.Filters.OtherMPAskingLists, message, false);
            // ExploringPageWithSearch mpsPage
            //    = new ExploringPageWithSearch(ParliamentData.AllMPs, App.ReadingContext.Filters.SelectedAskingMPs, message);
            await Application.Current.MainPage.Navigation.PushAsync(mpsPage);
            //await Shell.Current.GoToAsync($"{nameof(ExploringPageWithSearch)}");
        }
    }
}