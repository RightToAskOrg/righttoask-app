using System;
using System.Collections.Generic;

namespace RightToAskClient.Helpers
{
    public class XamarinPreferences
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();

        public void Clear()
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                storage.Clear();
            }
            else
            {
                Preferences.Clear();
            }
        }

        public bool ContainsKey(string key)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                return storage.ContainsKey(key);
            }
            else
            {
                return Preferences.ContainsKey(key);
            }
        }

        public string Get(string key, string defaultValue, string? sharedName = null)
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
                if (sharedName == null)
                {
                    return Preferences.Get(key, defaultValue);
                }
                else
                {
                    return Preferences.Get(key, defaultValue, sharedName);
                }
            }
        }
        public bool Get(string key, bool defaultValue, string? sharedName = null)
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
                if (sharedName == null)
                {
                    return Preferences.Get(key, defaultValue);
                }
                else
                {
                    return Preferences.Get(key, defaultValue, sharedName);
                }
            }
        }

        public void Set(string key, string value, string? sharedName = null)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                if (storage.ContainsKey(key))
                {
                    storage[key] = value;
                }
                else
                {
                    storage.Add(key, value);
                }
            }
            else
            {
                if (sharedName == null)
                {
                    Preferences.Set(key, value);
                }
                else
                {
                    Preferences.Set(key, value, sharedName);
                }
            }
        }

        public void Set(string key, bool value, string? sharedName = null)
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                if (storage.ContainsKey(key))
                {
                    storage[key] = value;
                }
                else
                {
                    storage.Add(key, value);
                }
            }
            else
            {
                if (sharedName == null)
                {
                    Preferences.Set(key, value);
                }
                else
                {
                    Preferences.Set(key, value, sharedName);
                }
            }
        }
        
        // XamarinPrefernces.shared.
        // Preferences.
        public static XamarinPreferences shared = new XamarinPreferences();
    }
}