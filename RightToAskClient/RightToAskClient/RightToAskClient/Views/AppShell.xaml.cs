using System;
using System.Collections.Generic;
using Xamarin.Forms;
using RightToAskClient.Views;

namespace RightToAskClient
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ExploringPage), typeof(ExploringPage));
            Routing.RegisterRoute(nameof(ExploringPageWithSearch), typeof(ExploringPageWithSearch));
            Routing.RegisterRoute(nameof(ExploringPageWithSearchAndPreSelections), typeof(ExploringPageWithSearchAndPreSelections));
            Routing.RegisterRoute(nameof(QuestionAskerPage), typeof(QuestionAskerPage));
            Routing.RegisterRoute(nameof(QuestionDetailPage), typeof(QuestionDetailPage));
            Routing.RegisterRoute(nameof(RegisterPage1), typeof(RegisterPage1));
            Routing.RegisterRoute(nameof(RegisterPage2), typeof(RegisterPage2));
            Routing.RegisterRoute(nameof(SecondPage), typeof(SecondPage));
            Routing.RegisterRoute(nameof(ReadingPage), typeof(ReadingPage));
            Routing.RegisterRoute(nameof(OtherUserProfilePage), typeof(OtherUserProfilePage));
            Routing.RegisterRoute(nameof(FlowOptionPage), typeof(FlowOptionPage));
            Routing.RegisterRoute(nameof(AdvancedSearchFiltersPage), typeof(AdvancedSearchFiltersPage));
        }
    }
}