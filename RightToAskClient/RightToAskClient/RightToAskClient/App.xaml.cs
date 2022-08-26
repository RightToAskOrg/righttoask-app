using System;
using Xamarin.Forms;
using RightToAskClient.Views;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.Helpers;
using RightToAskClient.Resx;
using Xamarin.Essentials;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using RightToAskClient.Models.ServerCommsData;
using SQLite;
using static Xamarin.Forms.Editor;
using Switch = Xamarin.Forms.Switch;

// [assembly: ExportFont("Roboto-Black.ttf", Alias = "AppFont")]
// [assembly: ExportFont("OpenSans-Regular.ttf", Alias = "AppFont")]
[assembly: ExportFont("Verdana.ttf", Alias = "AppFont")]
[assembly: ExportFont("Verdanab.ttf", Alias = "BoldAppFont")]
[assembly: ExportFont("DancingScript-VariableFont_wght.ttf", Alias = "DanceFont")]

namespace RightToAskClient
{
    public partial class App : Application
    {
        public static ReadingContext ReadingContext = new ReadingContext();
        public App()
        {
            LocalizationResourceManager.Current.PropertyChanged += (temp, temp2) => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;
            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);

            InitializeComponent();
            SetTheStyles();

            /* MS Docs say static classes are
             * " is guaranteed to be loaded and to have its fields initialized
             * and its static constructor called before the class is referenced
             * for the first time in your program."
             * */
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            // ResetAppData(); // Toggle this line in and out as needed instead of resetting the emulator every time
            var MPInitSuccess = await ParliamentData.MPAndOtherData.TryInit();
            var CommitteeInitSuccess = await CommitteesAndHearingsData.CommitteesData.TryInitialisingFromServer();
            
            // get account info from preferences
            var registrationPref = Preferences.Get(Constants.RegistrationInfo, "");
            if (!string.IsNullOrEmpty(registrationPref))
            {
                var registrationObj = JsonSerializer.Deserialize<ServerUser>(registrationPref);
                ReadingContext.ThisParticipant.RegistrationInfo 
                    = registrationObj is null ? new Registration() : new Registration(registrationObj);
                
                // We actually need to check for the stored "IsRegistered" boolean, in case they tried to
                // register but failed, for example because the server was offline.
                // So we may have stored Registration data, but not have actually succeeded in uploading it.
                var registrationSuccess = Preferences.Get(Constants.IsRegistered, false);
                ReadingContext.ThisParticipant.IsRegistered = registrationSuccess;
                
                // We have a problem if our stored registration is null but we think we registered successfully.
                Debug.Assert(registrationObj != null || registrationSuccess is false);
                
                // If we got electorates, let the app know to skip the Find My MPs step
                // TODO We may want to distinguish between ElectoratesKnown and Electorates.Any, because
                // the latter is true if only the state is known (Senate State being an electorate).
                // At the moment, this will set ElectoratesKnown, in both the app and the preferences, when the person
                // has selected their state, regardless of whether we know their electorate.
                bool electoratesKnown = Preferences.Get(Constants.ElectoratesKnown, false);
                ReadingContext.ThisParticipant.ElectoratesKnown = electoratesKnown;
                if(electoratesKnown) 
                {
			        ReadingContext.Filters.UpdateMyMPLists();
                }
                
                // Retrieve MP/staffer registration. Note that staffers have both the IsVerifiedMPAccount flag and the
                // IsVerifiedMPStafferAccount flag set to true.
                bool isVerifiedMPAccount = Preferences.Get(Constants.IsVerifiedMPAccount, false);
                ReadingContext.ThisParticipant.IsVerifiedMPAccount = isVerifiedMPAccount;
                if (isVerifiedMPAccount)
                {
                    ReadingContext.ThisParticipant.IsVerifiedMPStafferAccount =
                        Preferences.Get(Constants.IsVerifiedMPStafferAccount, false);
                    
                    // Used when uploading an answer. 
                    var MPRepresentingjson = Preferences.Get(Constants.MPRegisteredAs, "");
                    if (!String.IsNullOrEmpty(MPRepresentingjson))
                    {
                        MP? MPRepresenting = JsonSerializer.Deserialize<MP>(MPRepresentingjson);
                        if (MPRepresenting != null)
                        {
                            // See if we can find the registered MP in our existing list.
                            // Using field-equality operator.
                            // If so, just keep a pointer to it; if not, use a new MP object.
                            List<MP> matchingMPs = ParliamentData.AllMPs.Where(mp => mp.Equals(MPRepresenting)).ToList();
                            ReadingContext.ThisParticipant.MPRegisteredAs 
                                = matchingMPs.Any() ? matchingMPs.First() : MPRepresenting;
                        }
                    }
                }
            }
            
            // If we already have stored a valid state, use it and set StateKnown to true.
            ReadingContext.ThisParticipant.RegistrationInfo.StateKnown = false; // Should already be the default.
            string stateString =  Preferences.Get(Constants.State, "");
            Result <ParliamentData.StateEnum> state = ParliamentData.StateStringToEnum(stateString);
            if (String.IsNullOrEmpty(state.Err) && !String.IsNullOrEmpty(stateString))
            {
                ReadingContext.ThisParticipant.RegistrationInfo.StateKnown = true;
                ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsEnum = state.Ok;
            }
            
            // set popup bool
            ReadingContext.DontShowFirstTimeReadingPopup = Preferences.Get(Constants.DontShowFirstTimeReadingPopup, false);
            ReadingContext.ShowHowToPublishPopup = Preferences.Get(Constants.ShowHowToPublishPopup, true);
            //ReadingContext.ThisParticipant.HasQuestions = Preferences.Get(Constants.HasQuestions, false);
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        // maybe port this into a content View instead?
        private void SetTheStyles()
        {
            var selectableDataTemplate = new DataTemplate(() =>
            {
                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition(),
                        new ColumnDefinition()
                    }
                };

                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                var selectedToggle = new CheckBox();

                // nameLabel.SetBinding(Label.TextProperty, "TagEntity.NickName");
                nameLabel.SetBinding(Label.TextProperty, "TagEntity");
                selectedToggle.SetBinding(CheckBox.IsCheckedProperty, "Selected");
                selectedToggle.HorizontalOptions = LayoutOptions.End;
                selectedToggle.VerticalOptions = LayoutOptions.Center;

                grid.Children.Add(nameLabel);
                grid.Children.Add(selectedToggle, 4, 0);
                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumnSpan(nameLabel, 4);

                return new Xamarin.Forms.ViewCell { View = grid };
            });
            Resources.Add("SelectableDataTemplate", selectableDataTemplate);
        }

        // method for resetting the data in the application. Needs to be run when we re-initiliaze the databases on the server
        private void ResetAppData()
        {
            // clear the preferences, which holds the user's account registration info
            Preferences.Clear();
            // TODO: wipe the crypto/signing key
        }
    }
}
