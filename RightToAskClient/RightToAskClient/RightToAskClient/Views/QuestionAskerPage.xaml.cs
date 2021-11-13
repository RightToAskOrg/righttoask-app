using System;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Controls;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        private ReadingContext readingContext;
        public QuestionAskerPage(ReadingContext readingContext)
        {
            // TODO: Construct this properly.
            // FilterContext filters = new FilterContext {FilterKeyword = readingContext.SearchKeyword};
            BindingContext = readingContext;
            this.readingContext = readingContext;
            
            
            InitializeComponent();

            FilterDisplayTableView ttestableView = new FilterDisplayTableView(readingContext.Filters);
            WholePage.Children.Insert(0,ttestableView);
        }

        private void OnFindCommitteeButtonClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = $"Finding Committees not implemented yet";	
        }

        // TODO: at the moment this doesn't properly select the MPs-  it just lists them and lets
        // it looks like you've selected them.
        private void OnMyMPRaiseButtonClicked(object sender, EventArgs e)
        {
            string message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";
            
            // TODO (Issue #9) update to use the properly-computed MPs in ThisParticipant.MyMPs
            var mpsExploringPage = new ExploringPage(readingContext.TestCurrentMPs, readingContext.Filters.SelectedAskingMPsMine, message);
			
            ListMPsFindFirstIfNotAlreadyKnown(mpsExploringPage);
        }
        
        // TODO This is a repeat of the code in SecondPage.xaml.cs. Factor out better.
        void ListMPsFindFirstIfNotAlreadyKnown(ExploringPage mpsExploringPage)
        {
            var thisParticipant = readingContext.ThisParticipant;
			
            if (! thisParticipant.MPsKnown)
            {
                var registrationPage = new RegisterPage2(thisParticipant, false, mpsExploringPage);
				
                Navigation.PushAsync(registrationPage);
            }
            else
            {
                Navigation.PushAsync(mpsExploringPage);
            }
        }
        private async void OnOtherMPRaiseButtonClicked(object sender, EventArgs e)
        {
            var selectableMPs =
                new ObservableCollection<Tag>(BackgroundElectorateAndMPData.AllMPs.Select
                    (mp => new Tag
                        {
                            TagEntity = mp, 
                            Selected = false
                        }
                    )
                );

            var allMPsAsEntities = new ObservableCollection<Entity>(BackgroundElectorateAndMPData.AllMPs); 
            ExploringPageWithSearch mpsPage 
                = new ExploringPageWithSearch(allMPsAsEntities, readingContext.Filters.SelectedAskingMPs, "Here is the complete list of MPs");
            await Navigation.PushAsync(mpsPage);
        }
        async void OnNavigateForwardButtonClicked(object sender, EventArgs e)
        {
			var readingPage = new ReadingPage(false, readingContext);
			await Navigation.PushAsync (readingPage);
        }
    }
}