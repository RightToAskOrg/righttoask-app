using Xamarin.Forms;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.Helpers;
using RightToAskClient.Resx;
using Xamarin.Essentials;
using System.Text.Json;
using System.Diagnostics;
using RightToAskClient.Models.ServerCommsData;

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
            var me = ReadingContext.ThisParticipant;
            
            // Order is important here: the Filters need to be (re-)initialised after we've read MP and Committee data.
		    ReadingContext.Filters.InitSelectableLists();
            
            // get account info from preferences
            var registrationPref = Preferences.Get(Constants.RegistrationInfo, "");
            if (!string.IsNullOrEmpty(registrationPref))
            {
                var registrationObj = JsonSerializer.Deserialize<ServerUser>(registrationPref);
                me.RegistrationInfo 
                    = registrationObj is null ? new Registration() : new Registration(registrationObj);
                
                // We actually need to check for the stored "IsRegistered" boolean, in case they tried to
                // register but failed, for example because the server was offline.
                // So we may have stored Registration data, but not have actually succeeded in uploading it.
                var registrationSuccess = Preferences.Get(Constants.IsRegistered, false);
                me.IsRegistered = registrationSuccess;
                
                // We have a problem if our stored registration is null but we think we registered successfully.
                Debug.Assert(registrationObj != null || registrationSuccess is false);
                
                // If we got electorates, let the app know to skip the Find My MPs step
                // TODO We may want to distinguish between ElectoratesKnown and Electorates.Any, because
                // the latter is true if only the state is known (Senate State being an electorate).
                // At the moment, this will set ElectoratesKnown, in both the app and the preferences, when the person
                // has selected their state, regardless of whether we know their electorate.
                var electoratesKnown = Preferences.Get(Constants.ElectoratesKnown, false);
                me.ElectoratesKnown = electoratesKnown;
                if(electoratesKnown) 
                {
			        ReadingContext.Filters.UpdateMyMPLists();
                }
                
                // Retrieve MP/staffer registration. Note that staffers have both the IsVerifiedMPAccount flag and the
                // IsVerifiedMPStafferAccount flag set to true.
                var isVerifiedMPAccount = Preferences.Get(Constants.IsVerifiedMPAccount, false);
                me.IsVerifiedMPAccount = isVerifiedMPAccount;
                if (isVerifiedMPAccount)
                {
                    me.IsVerifiedMPStafferAccount =
                        Preferences.Get(Constants.IsVerifiedMPStafferAccount, false);
                    
                    // Used when uploading an answer. 
                    var MPRepresentingjson = Preferences.Get(Constants.MPRegisteredAs, "");
                    if (!string.IsNullOrEmpty(MPRepresentingjson))
                    {
                        var MPRepresenting = JsonSerializer.Deserialize<MP>(MPRepresentingjson);
                        if (MPRepresenting != null)
                        {
                            // See if we can find the registered MP in our existing list.
                            // Using field-equality operator.
                            // If so, just keep a pointer to it; if not, use a new MP object.
                            me.MPRegisteredAs =
                                ParliamentData.FindMPOrMakeNewOne(MPRepresenting);
                        }
                    }
                }
            }
            
            // If we already have stored a valid state, use it and set StateKnown to true.
            me.RegistrationInfo.StateKnown = false; // Should already be the default.
            var stateString =  Preferences.Get(Constants.State, "");
            var state = ParliamentData.StateStringToEnum(stateString);
            if (string.IsNullOrEmpty(state.Err) && !string.IsNullOrEmpty(stateString))
            {
                me.RegistrationInfo.StateKnown = true;
                me.RegistrationInfo.SelectedStateAsEnum = state.Ok;
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
                var grid = new Grid
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
