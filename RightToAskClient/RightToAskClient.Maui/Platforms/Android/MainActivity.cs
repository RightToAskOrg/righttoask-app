using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace RightToAskClient.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //this.RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

            //Window?.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.LayoutInScreen);
            //Window?.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

            //var controller = Window?.InsetsController;
            //controller?.Hide(WindowInsets.Type.SystemBars());
            base.OnCreate(savedInstanceState);
        }
    }
}