﻿using Xamarin.Forms;
using RightToAskClient.Views;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.Helpers;
using RightToAskClient.Resx;
using Xamarin.Essentials;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            //Preferences.Clear(); // Toggle this line in and out as needed instead of resetting the emulator every time
            ParliamentData.MPAndOtherData.TryInit();
            // get account info from preferences
            var registrationPref = Preferences.Get("RegistrationInfo", "");
            if (!string.IsNullOrEmpty(registrationPref))
            {
                var registrationObj = JsonSerializer.Deserialize<Registration>(registrationPref);
                ReadingContext.ThisParticipant.RegistrationInfo = registrationObj ?? new Registration();
                // if we got back a uid then the user should be considered as registered
                if (!string.IsNullOrEmpty(ReadingContext.ThisParticipant.RegistrationInfo.uid))
                {
                    ReadingContext.ThisParticipant.IsRegistered = true;
                }
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
                ReadingContext.ThisParticipant.RegistrationInfo.State = ParliamentData.StatesAndTerritories[stateID];
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        private void SetTheStyles()
        {
            var selectableDataTemplate = new DataTemplate(() =>
            {
                var grid = new Grid();
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                var selectedToggle = new Switch();

                // nameLabel.SetBinding(Label.TextProperty, "TagEntity.NickName");
                nameLabel.SetBinding(Label.TextProperty, "TagEntity");
                selectedToggle.SetBinding(Switch.IsToggledProperty, "Selected");

                grid.Children.Add(nameLabel);
                grid.Children.Add(selectedToggle, 1, 0);

                return new Xamarin.Forms.ViewCell { View = grid };
            });
            Resources.Add("SelectableDataTemplate", selectableDataTemplate);
        }
    }
}
