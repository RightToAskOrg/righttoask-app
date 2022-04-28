using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*
 * This page allows a person to find which electorates they live in,
 * and hence which MPs represent them.
 *
 * This is used in two possible places:
 * (1) if the person clicks on 'My MP' when setting question metadata,
 * we need to know who their MPs are. After this page,
 * there will be a list of MPs loaded for them to choose from.
 * This is indicated by setting LaunchMPsSelectionPageNext to true.
 * 
 * (2) if the person tries to vote or post a question.
 * In this case, they have generated a name via RegisterPage1
 * and can skip this step (so showSkip should be set to true).
 * And we don't follow with a list of MPs for them to chose from.
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage2 : ContentPage
    {
        // private ParliamentData.Chamber stateLCChamber=ParliamentData.Chamber.Vic_Legislative_Council;
        // private ParliamentData.Chamber stateLAChamber=ParliamentData.Chamber.Vic_Legislative_Assembly;
        // alreadySelectedMPs are passed in if a Selection page is to be launched next.
        // If they're null/absent, no selection page is launched.
        public RegisterPage2()
        {
            InitializeComponent();
        }
    }
}