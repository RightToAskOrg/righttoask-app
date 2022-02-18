using Xamarin.Forms;
using RightToAskClient.Views;
using RightToAskClient.Models;

namespace RightToAskClient
{
    public partial class App : Application
    {
        public static ReadingContext ReadingContext = new ReadingContext();
        public App()
        {
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
            ParliamentData.MPAndOtherData.TryInit();
            ReadingContext = new ReadingContext();
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
