using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(QuestionAskerPage), typeof(QuestionAskerPage));
            Routing.RegisterRoute(nameof(QuestionDetailPage), typeof(QuestionDetailPage));
            Routing.RegisterRoute(nameof(RegisterAccountPage), typeof(RegisterAccountPage));
            Routing.RegisterRoute(nameof(FindMPsPage), typeof(FindMPsPage));
            // Routing.RegisterRoute(nameof(SecondPage), typeof(SecondPage));
            // Routing.RegisterRoute("//MainPage/SecondPage", typeof(SecondPage));
            // Routing.RegisterRoute(nameof(ReadingPage), typeof(ReadingPage));
            Routing.RegisterRoute(nameof(OtherUserProfilePage), typeof(OtherUserProfilePage));
            Routing.RegisterRoute(nameof(QuestionAnswererPage), typeof(QuestionAnswererPage));
            Routing.RegisterRoute(nameof(AdvancedSearchFiltersPage), typeof(AdvancedSearchFiltersPage));
            Routing.RegisterRoute(nameof(MPRegistrationVerificationPage), typeof(MPRegistrationVerificationPage));
            Routing.RegisterRoute(nameof(HowAnsweredOptionPage), typeof(HowAnsweredOptionPage));
            Routing.RegisterRoute(nameof(QuestionBackgroundPage), typeof(QuestionBackgroundPage));
            Routing.RegisterRoute(nameof(MetadataPage), typeof(MetadataPage));
            Routing.RegisterRoute(nameof(SelectableListPage), typeof(SelectableListPage));
            // Routing.RegisterRoute(nameof(FindMPsPage), typeof(FindMPsPage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (args.Target.Location.OriginalString.ToLower().Contains("account"))
            {
                AccountPageExchanger.Registration = IndividualParticipant.getInstance().ProfileData.RegistrationInfo;
            }
        }
    }
}