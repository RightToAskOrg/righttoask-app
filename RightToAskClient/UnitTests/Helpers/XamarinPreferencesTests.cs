using System.Globalization;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using Xunit;

namespace UnitTests.Helpers
{
    public class XamarinPreferencesTests
    {
        [Fact]
        public void GetSetOneStringTest()
        {
            var preferences = new XamarinPreferences();
            
            preferences.Set("key0", "stringValue0");
            var key0value = preferences.Get("key0", "");

            Assert.Equal("stringValue0",key0value);
        }
        
        [Fact]
        public void GetSetOneBooleanTest()
        {
            var preferences = new XamarinPreferences();
            
            preferences.Set("key0", true);
            var key0value = preferences.Get("key0", false);

            Assert.Equal(true, key0value);
        }
        
        [Fact]
        public void GetStringDefaultValueNoKeyTest()
        {
            var preferences = new XamarinPreferences();
            var key0value = preferences.Get("key0", "default");

            Assert.Equal("default", key0value);
        }
        
        [Fact]
        public void GetBoolDefaultValueNoKeyTest()
        {
            var preferences = new XamarinPreferences();
            var key0value = preferences.Get("key0", true);

            Assert.Equal(true, key0value);
        }
        [Fact]
        public void GetSetOneStringWithSharedNameTest()
        {
            var preferences = new XamarinPreferences();
            
            preferences.Set("key0", "stringValue0", "");
            var key0value = preferences.Get("key0", "", "");

            Assert.Equal("stringValue0",key0value);
        }
        
        [Fact]
        public void GetSetOneBooleanWithSharedNameTest()
        {
            var preferences = new XamarinPreferences();
            
            preferences.Set("key0", true, "");
            var key0value = preferences.Get("key0", false, "");

            Assert.Equal(true, key0value);
        }
        
        [Fact]
        public void GetStringDefaultValueNoKeyWithSharedNameTest()
        {
            var preferences = new XamarinPreferences();
            var key0value = preferences.Get("key0", "default", "");

            Assert.Equal("default", key0value);
        }
        
        [Fact]
        public void GetBoolDefaultValueNoKeyWithSharedNameTest()
        {
            var preferences = new XamarinPreferences();
            var key0value = preferences.Get("key0", true, "");

            Assert.Equal(true, key0value);
        }

        [Fact]
        public void ContainsKeyTest()
        {
            var preferences = new XamarinPreferences();
            preferences.Set("key0", true, "");
            
            Assert.True(preferences.ContainsKey("key0"));
            Assert.False(preferences.ContainsKey("key1"));
        }

        [Fact]
        public void ClearTest()
        {
            var preferences = new XamarinPreferences();
            preferences.Set("key0", true, "");
            preferences.Set("key1", "true", "");

            preferences.Clear();
            
            Assert.False(preferences.ContainsKey("key0"));
            Assert.False(preferences.ContainsKey("key1"));
        }
    }
}