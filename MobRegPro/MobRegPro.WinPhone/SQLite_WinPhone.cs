using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Windows.Storage;
using MobRegPro.Helpers;
using Xamarin.Forms;
using MobRegPro.WinPhone;

[assembly: Dependency(typeof(SQLite_WinPhone))]
namespace MobRegPro.WinPhone
{
    public class SQLite_WinPhone : ISQLite
    {
        public SQLite_WinPhone() { }

        private string GetDatabaseFileName()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.dbName);
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
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
