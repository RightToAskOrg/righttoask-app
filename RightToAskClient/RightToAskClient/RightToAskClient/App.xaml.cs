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
using Switch = Xamarin.Forms.Switch;

[assembly: ExportFont("Roboto-Black.ttf", Alias = "AppFont")]
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

        protected override void OnStart()
        {
            // ResetAppData(); // Toggle this line in and out as needed instead of resetting the emulator every time
            ParliamentData.MPAndOtherData.TryInit();
            
            // get account info from preferences
            var registrationPref = Preferences.Get("RegistrationInfo", "");
            if (!string.IsNullOrEmpty(registrationPref))
            {
                var registrationObj = JsonSerializer.Deserialize<ServerUser>(registrationPref);
                ReadingContext.ThisParticipant.RegistrationInfo 
                    = registrationObj is null ? new Registration() : new Registration(registrationObj);
                
                // We actually need to check for the stored "IsRegistered" boolean, in case they tried to
                // register but failed, for example because the server was offline.
                // So we may have stored Registration data, but not have actually succeeded in uploading it.
                var registrationSuccess = Preferences.Get("IsRegistered", false);
                ReadingContext.ThisParticipant.IsRegistered = registrationSuccess;
                
                // We have a problem if our stored registration is null but we think we registered successfully.
                Debug.Assert(registrationObj != null || registrationSuccess is false);
                
                // if we got electorates, let the app know to skip the Find My MPs step
                // TODO We definitely shouldn't have to do this here - the Registration
                // data structure should take care of whether the MPs are known and updated.
                if (ReadingContext.ThisParticipant.RegistrationInfo.electorates.Any())
                {
                    ReadingContext.ThisParticipant.MPsKnown = true;
                    ReadingContext.ThisParticipant.UpdateMPs(); // to refresh the list
                }
            }
            // sets state pickers
            int stateID = Preferences.Get("StateID", -1);
            if (stateID >= 0)
            {
                ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsIndex = stateID;
            }

            // set popup bool
            ReadingContext.DontShowFirstTimeReadingPopup = Preferences.Get("DontShowFirstTimeReadingPopup", false);
            ReadingContext.ShowHowToPublishPopup = Preferences.Get("ShowHowToPublishPopup", true);
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
