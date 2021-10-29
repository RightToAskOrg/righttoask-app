using Xamarin.Forms;
using RightToAskClient.Views;
using RightToAskClient.Data;

namespace RightToAskClient
{
    public partial class App : Application
    {
        public static ItemManager RegItemManager { get; private set; }

        public App()
        {
            InitializeComponent();
            RegItemManager = new ItemManager (new RestService ());

            MainPage = new NavigationPage(new ListPage())
            {
                BarTextColor = Color.White,
                BarBackgroundColor = (Color)App.Current.Resources["primaryGreen"]
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
