using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace RightToAskClient.Helpers
{
    public class XamarinPrefernces
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();

        public string Get(string key, string defaultValue, string sharedName)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                try
                {
                    return (string)storage[key];
                }
                catch (KeyNotFoundException)
                {
                    return defaultValue;
                }
            }
            else
            {
                Preferences.Get(key, defaultValue, sharedName);
            }
        }
        public bool Get(string key, bool defaultValue, string sharedName)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                try
                {
                    return (bool)storage[key];
                }
                catch (KeyNotFoundException)
                {
                    return defaultValue;
                }
            }
            else
            {
                Preferences.Get(key, defaultValue, sharedName);
            }
        }

        public void Set(string key, string value, string sharedName)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                storage.Add(key, value);
            }
            else
            {
                Preferences.Set(key, value, sharedName);
            }
        }

        public void Set(string key, bool value, string sharedName)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                storage.Add(key, value);
            }
            else
            {
                Preferences.Set(key, value, sharedName);
            }
        }

        // XamarinPrefernces.shared.
        // Preferences.
        public static XamarinPrefernces shared = new XamarinPrefernces();
    }
}