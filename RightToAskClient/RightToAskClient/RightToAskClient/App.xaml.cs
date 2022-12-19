using Xamarin.Forms;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.Helpers;
using RightToAskClient.Resx;
using Xamarin.Essentials;
using System.Text.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Views;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Application = Xamarin.Forms.Application;

// [assembly: ExportFont("Roboto-Black.ttf", Alias = "AppFont")]
// [assembly: ExportFont("OpenSans-Regular.ttf", Alias = "AppFont")]
[assembly: ExportFont("Verdana.ttf", Alias = "AppFont")]
[assembly: ExportFont("Verdanab.ttf", Alias = "BoldAppFont")]
[assembly: ExportFont("DancingScript-VariableFont_wght.ttf", Alias = "DanceFont")]

namespace RightToAskClient
{
    public partial class App : Application
    {
        // The selections of MPs, authorities, and various other options that is gradually
        // built as we step through the question-writing process.
        public App()
        {
            LocalizationResourceManager.Current.PropertyChanged += (temp, temp2) => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;
            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);
            Task.Run(async () =>
            { 
                Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

                });
            });
            
            
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
            
            // We do not use these values, but for reasons I don't fully understand, the return value seemed to make the
            // async code actually execute.
            var MPInitSuccess = await ParliamentData.MPAndOtherData.TryInit();
            var CommitteeInitSuccess = await CommitteesAndHearingsData.CommitteesData.TryInitialisingFromServer();
            var signingKeyRetrieved = await ClientSignatureGenerationService.Init();
            
            IndividualParticipant.getInstance().Init();
            
            if(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown) 
            {
                FilterChoices.NeedToUpdateMyMpLists(this);
            }
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
            XamarinPreferences.shared.Clear();
            // TODO: wipe the crypto/signing key
        }
    }
}
