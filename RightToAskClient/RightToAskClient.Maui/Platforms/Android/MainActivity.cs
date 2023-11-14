using Android.App;
using Android.Content.PM;
using Android.OS;

namespace RightToAskClient.Maui
{
    [Activity(Label = "RightToAskClient", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
 
    }
}