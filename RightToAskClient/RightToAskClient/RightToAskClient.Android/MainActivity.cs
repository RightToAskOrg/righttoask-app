using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace RightToAskClient.Droid
{
    [Activity(Label = "RightToAskClient", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().
                UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
        }

        /*
        private int counter = 0;
        public override void OnBackPressed()
        {
            counter++;
            if (counter >= 2)
            {
                base.OnBackPressed(); // removing this call will prevent the user from being able to leave the app by pressing the back button
                counter = 0;
            }
        }*/
    }
}
