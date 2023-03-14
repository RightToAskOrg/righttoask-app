using RightToAskClient.Helpers;
using UIKit;

namespace RightToAskClient.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            var id = UIDevice.CurrentDevice.IdentifierForVendor.ToString();
            if (XamarinPreferences.shared.Get(Constants.DeviceID, "").IsNullOrEmpty())
            {
                XamarinPreferences.shared.Set(Constants.DeviceID, id); // save device ID into preference
            }
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
