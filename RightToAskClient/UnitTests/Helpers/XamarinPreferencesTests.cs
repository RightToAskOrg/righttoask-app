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
            var preferences = new XamarinPrefernces();
            
            preferences.Set("key0", "stringValue0", "");
            var key0value = preferences.Get("key0", "", "");

            Assert.Equal("stringValue0",key0value);
        }
        
        [Fact]
        public void GetSetOneBooleanTest()
        {
            var preferences = new XamarinPrefernces();
            
            preferences.Set("key0", true, "");
            var key0value = preferences.Get("key0", false, "");

            Assert.Equal(true, key0value);
        }
        
        [Fact]
        public void GetStringDefaultValueNoKeyTest()
        {
            var preferences = new XamarinPrefernces();
            var key0value = preferences.Get("key0", "default", "");

            Assert.Equal("default", key0value);
        }
        
        [Fact]
        public void GetBoolDefaultValueNoKeyTest()
        {
            var preferences = new XamarinPrefernces();
            var key0value = preferences.Get("key0", true, "");

            Assert.Equal(true, key0value);
        }
    }
}