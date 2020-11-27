using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System.IO;
using MobRegPro.Droid;
using Xamarin.Forms;
using MobRegPro.Helpers;

[assembly: Dependency(typeof(SQLite_Android))]

namespace MobRegPro.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }

        string GetDatabaseFileName()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            string sqliteFilename = Path.Combine(documentsPath, Constants.dbName);
            return sqliteFilename;
        }

        public SQLiteConnection GetSQLiteConnection()
        {
            // Create the connection
            var conn = new SQLite.SQLiteConnection(GetDatabaseFileName());
            // Return the database connection
            return conn;
        }

        public bool DeleteDatabase()
        {
            try {
                File.Delete(GetDatabaseFileName());
            }
            catch (Exception) { return false; }

            return true;
        }

        public bool DatabaseExists()
        {
            bool exists = false;
            try
            {
                exists = File.Exists(GetDatabaseFileName());
            }
            catch (Exception) { return false; }

            return exists;
        }

    }
}