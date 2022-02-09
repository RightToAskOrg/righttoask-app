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

        public static async void PushMyAnsweringMPsExploringPage()
        {
            string message = "These are your MPs.  Select the one(s) who should answer the question";
            // var mpsExploringPage = new ExploringPage(readingContext.ThisParticipant.MyMPs,
            var mpsExploringPage = new ExploringPage(App.ReadingContext.ThisParticipant.GroupedMPs,
                App.ReadingContext.Filters.SelectedAnsweringMPsMine, message);

            var nextPage = await ListMPsFindFirstIfNotAlreadyKnown(App.ReadingContext, mpsExploringPage, App.ReadingContext.Filters.SelectedAnsweringMPs);
            //App.Current.MainPage.Navigation.PushAsync(nextPage);
            await Shell.Current.GoToAsync($"{nameof(ExploringPage)}");
        }

        public static async void PushMyAskingMPsExploringPage()
        {
            string message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";

            var mpsExploringPage = new ExploringPage(App.ReadingContext.ThisParticipant.GroupedMPs,
                App.ReadingContext.Filters.SelectedAskingMPsMine, message);

            launchMPFindingAndSelectingPages(mpsExploringPage, App.ReadingContext);
        }

        private static async void launchMPFindingAndSelectingPages(ExploringPage mpsExploringPage, ReadingContext readingContext)
        {
            var nextPage = await ListMPsFindFirstIfNotAlreadyKnown(readingContext, mpsExploringPage, readingContext.Filters.SelectedAskingMPs);
            //await App.Current.MainPage.Navigation.PushAsync(nextPage);
            await Shell.Current.GoToAsync($"{nameof(ExploringPage)}");
        }
        /*
		 * Either push the list of selectable MPs directly, or push a registration page,
		 * instructed to push the MPs selection page after.
		 */
        public static async Task<Page> ListMPsFindFirstIfNotAlreadyKnown(ReadingContext readingContext, ExploringPage mpsExploringPage,
            ObservableCollection<MP> alreadySelectedMPs)
        {
            var thisParticipant = readingContext.ThisParticipant;

            if (!thisParticipant.MPsKnown)
            {
                var registrationPage = new RegisterPage2(thisParticipant, false, true, alreadySelectedMPs);
                return registrationPage;

            }
            else
            {
                return mpsExploringPage;
            }
        }

        public static async void PushAnsweringMPsExploringPage()
        {
            string message = "Here is the complete list of MPs - select which one(s) should answer";
            ExploringPageWithSearch mpsPage
                = new ExploringPageWithSearch(ParliamentData.AllMPs, App.ReadingContext.Filters.SelectedAnsweringMPs, message);
            //await App.Current.MainPage.Navigation.PushAsync(mpsPage);
            await Shell.Current.GoToAsync($"{nameof(ExploringPageWithSearch)}");
        }

        public static async void PushAskingMPsExploringPageAsync()
        {
            string message =
                "Here is the complete list of MPs - select which one(s) should raise your question in Parliament";
            ExploringPageWithSearch mpsPage
                = new ExploringPageWithSearch(ParliamentData.AllMPs, App.ReadingContext.Filters.SelectedAskingMPs, message);
            //App.Current.MainPage.Navigation.PushAsync(mpsPage);
            await Shell.Current.GoToAsync($"{nameof(ExploringPageWithSearch)}");
        }
    }
}