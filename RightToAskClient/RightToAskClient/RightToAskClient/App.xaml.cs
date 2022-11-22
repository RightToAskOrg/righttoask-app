using Xamarin.Forms;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.Helpers;
using RightToAskClient.Resx;
using Xamarin.Essentials;
using System.Text.Json;
using System.Diagnostics;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Views;

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
            
            // We do not use these values, but for reasons I don't fully understand, the return value seemed to make the
            // async code actually execute.
            var MPInitSuccess = await ParliamentData.MPAndOtherData.TryInit();
            var CommitteeInitSuccess = await CommitteesAndHearingsData.CommitteesData.TryInitialisingFromServer();
            var signingKeyRetrieved = await ClientSignatureGenerationService.Init();
            
            // Order is important here: the Filters need to be (re-)initialised after we've read MP and Committee data.
		    ReadingContext.Filters.InitSelectableLists();
            
	        // Consider awaiting. I don't think so, though, because there's no reason everything else should wait for it.
            IndividualParticipant.Init();
            
            if(IndividualParticipant.ElectoratesKnown) 
            {
			    ReadingContext.Filters.UpdateMyMPLists();
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
