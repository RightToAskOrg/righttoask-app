using Xamarin.Forms;
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
            // Preferences.Clear(); // Toggle this line in and out as needed instead of resetting the emulator every time
            ParliamentData.MPAndOtherData.TryInit();
            ReadingContext = new ReadingContext();
            // get the registration info from preferences or default to not registered
            ReadingContext.ThisParticipant.IsRegistered = Preferences.Get("IsRegistered", false);
            if (ReadingContext.ThisParticipant.IsRegistered)
            {
                // get account info from preferences
                ReadingContext.ThisParticipant.RegistrationInfo.display_name = Preferences.Get("DisplayName", "Display name not found");
                ReadingContext.ThisParticipant.RegistrationInfo.uid = Preferences.Get("UID", "User ID not found");
                int stateID = Preferences.Get("StateID", -1);
                if(stateID >= 0)
                {
                    ReadingContext.ThisParticipant.RegistrationInfo.State = ParliamentData.StatesAndTerritories[stateID];
                }
                // grab the electorates from storage
                var electoratePref = Preferences.Get("Electorates", "");
                if (!string.IsNullOrEmpty(electoratePref))
                {
                    var electorates = JsonSerializer.Deserialize<ObservableCollection<ElectorateWithChamber>>(electoratePref);
                    ReadingContext.ThisParticipant.RegistrationInfo.electorates = electorates ?? new ObservableCollection<ElectorateWithChamber>();
                    // if we got electorates, let the app know to skip the Find My MPs step
                    if (ReadingContext.ThisParticipant.RegistrationInfo.electorates.Any())
                    {
                        ReadingContext.ThisParticipant.MPsKnown = true;
                    }
                }
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
