using RightToAskClient.Maui.Models;
using CommunityToolkit.Maui.Converters;
using RightToAskClient.Maui.Resx;
using System.Text.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.Maui.CryptoUtils;
using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.Models.ServerCommsData;
using RightToAskClient.Maui.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Application = Microsoft.Maui.Controls.Application;
[assembly: ExportFont("Verdana.ttf", Alias = "AppFont")]
[assembly: ExportFont("Verdanab.ttf", Alias = "BoldAppFont")]
[assembly: ExportFont("DancingScript-VariableFont_wght.ttf", Alias = "DanceFont")]
namespace RightToAskClient.Maui
{

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SetTheStyles();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            // ResetAppData(); // Toggle this line in and out as needed instead of resetting the emulator every time

            await Task.Run(async () =>
            {
                // We do not use these values, but for reasons I don't fully understand, the return value seemed to make the
                // async code actually execute.
                var MPInitSuccess = await ParliamentData.MPAndOtherData.TryInit();
                var CommitteeInitSuccess = await CommitteesAndHearingsData.CommitteesData.TryInitialisingFromServer();
                var signingKeyRetrieved = await ClientSignatureGenerationService.Init();

                IndividualParticipant.getInstance().Init();

                if (IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown)
                {
                    FilterChoices.NeedToUpdateMyMpLists(this);
                }
            });

            Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
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

                nameLabel.SetBinding(Label.TextProperty, "TagEntity");
                selectedToggle.SetBinding(CheckBox.IsCheckedProperty, "Selected");
                selectedToggle.HorizontalOptions = LayoutOptions.End;
                selectedToggle.VerticalOptions = LayoutOptions.Center;

                grid.Children.Add(nameLabel);
                //TODO: grid.Children.Add(selectedToggle, 4, 0);
                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumnSpan(nameLabel, 4);

                return new Microsoft.Maui.Controls.ViewCell { View = grid };
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