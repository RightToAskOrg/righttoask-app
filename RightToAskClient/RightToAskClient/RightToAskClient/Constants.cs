using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using System.IO;
// using Xamarin.Forms.Internals;

namespace RightToAskClient
{
    public static class Constants
    { 
	// URL of REST service
        //public static string RestUrl = "https://YOURPROJECT.azurewebsites.net:8081/api/todoitems/{0}";

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        // public static string RestUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000/api/todoitems/{0}" : "http://localhost:5000/api/todoitems/{0}";
        public static string RegUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/new_registration" : "http://localhost:8099/new_registration/{0}";
        public static string UserListUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8099/get_user_list" : "http://localhost:8099/get_user_list";

        public const string DatabaseFilename = "TodoSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }
}
