using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MobRegPro;
using SQLite;
using System.IO;
using MobRegPro.iOS;
using MobRegPro.Helpers;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace MobRegPro.iOS
{
	public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS()
        { }
        public string GetDatabaseFileName()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            return Path.Combine(libraryPath, Constants.dbName);
        }

        public SQLite.SQLiteConnection GetSQLiteConnection()
        {
            // Create the connection
            var conn = new SQLite.SQLiteConnection(GetDatabaseFileName());
            // Return the database connection
            return conn;
        }

        public bool DeleteDatabase()
        {
            try
            {
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
