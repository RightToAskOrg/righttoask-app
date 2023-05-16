using Android.App;
using Android.Content.PM;
using Android.OS;
using RightToAskClient.Helpers;
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
            
            /* var id = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId);
            if (XamarinPreferences.shared.Get(Constants.DeviceID, "").IsNullOrEmpty())
            {
                if (id != null)
                    XamarinPreferences.shared.Set(Constants.DeviceID, id); // save device ID into preference
            }
            */
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
