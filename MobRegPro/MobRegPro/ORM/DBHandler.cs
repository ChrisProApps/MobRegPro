using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace MobRegPro.ORM
{
	public class DBHandler : IDisposable
    {
		public SQLiteConnection db { get { return _db; } }

		private SQLiteConnection _db = null;

		
		public DBHandler()
		{
            //DependencyService.Register<ISQLite>();
           //ISQLite is = DependencyService.Get<ISQLite>();
			 _db = DependencyService.Get<ISQLite>().GetSQLiteConnection();
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			_db.Close ();
			_db = null;
		}

		#endregion

		public void DeleteDatabase()
		{
			// Close existing database and then delete file
			//
			_db.Close ();
			DependencyService.Get<ISQLite> ().DeleteDatabase ();

			// Create new database
			//
			_db = DependencyService.Get<ISQLite>().GetSQLiteConnection();
		}

        public bool DatabaseExists()
        {
            return DependencyService.Get<ISQLite>().DatabaseExists();
        }

        public bool CreateTables()
		{
			CreateTableResult result;
            bool isOK = true;

            result = db.CreateTable<Status>();
            if (result != CreateTableResult.Created) isOK = false;
			result = db.CreateTable<Planning>();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<Resource>();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<Status> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<RegistrationType> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<Registration> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<ArticleReg> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<PlanningHistory> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<Article> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<ArticleGroup> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<ArticleReg> ();
            if (result != CreateTableResult.Created) isOK = false;
            result = db.CreateTable<Setting> ();
            if (result != CreateTableResult.Created) isOK = false;
            //result += db.CreateTable<MobUser> ();
            return (isOK);
		}

    }
}
