using MobRegPro.Helpers;
using MobRegPro.ORM;
using RegServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MobRegPro.Resources;
using System.Globalization;

using System.Reflection;
using System.Resources;

namespace MobRegPro
{
    public class App : Application
    {
 		public static bool isPropertiesSavingRequired = false;
		public static DBHandler _dbHandler = null;
		public static DBHandler dbHandler {
				get {
					if (_dbHandler == null)
						_dbHandler = new DBHandler ();
					return _dbHandler;
				} 
				set {
					_dbHandler = value;
				}
			}
	
		#region Properties settings

		public static ApplicationSettings appSettings = new ApplicationSettings();

		// Use this method to avoid crashes on Android when exiting the Application using back button on device/emulator
		//
		public static async Task SaveAllPropertiesAsync()
		{
			//Save in Current object
			//
			string jsonStr = JsonConvert.SerializeObject (appSettings);
			if (!App.Current.Properties.ContainsKey ("MySettings"))
				App.Current.Properties.Add ("MySettings", jsonStr);
			else
				App.Current.Properties ["MySettings"] = jsonStr;
			await Application.Current.SavePropertiesAsync ();
		
			// Save in DB
			//
			await Task.Run (() => {	  
				Common.SaveApplicationSettingsToDatabase (appSettings);
			});

		}

		public static void SaveAllProperties()
		{
			Task.Run ( async() => {	  
				await SaveAllPropertiesAsync ();
			});
		}

		private bool appSettingsUseDatabaseStore = true;

		public void LoadAllProperties()
		{
			// Read Application settings from database and store in dbProperties OR set defaults
			//

			if (appSettingsUseDatabaseStore) {
				Setting dbSetting = Common.ReadApplicationSettingsFromDatabase ();
				if (dbSetting != null && dbSetting.IsActiv) {
					App.appSettings = JsonConvert.DeserializeObject <ApplicationSettings> (dbSetting.Content);
				}
			}

			// Read Application settings from Current object
			//
			if (!appSettingsUseDatabaseStore) {
				if (App.Current.Properties.ContainsKey ("MySettings")) {
					string jsonStr = App.Current.Properties ["MySettings"] as string;
					App.appSettings = JsonConvert.DeserializeObject<ApplicationSettings> (jsonStr);
				}
			}
		}

		#endregion


        public  App()
        {
			// Rev 18.2.0.48
			//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzAwNTQyQDMxMzgyZTMyMmUzMFFJaEtjRXNsTi9la0lmbUl6OHl4aGdOL3A4enZnME5ITGtQekpURWZVYlE9");

			// The root page of your application
			dbHandler = null;

			// Create / Load database
			//
			dbHandler = new DBHandler();
			// WARNING : if database is deleted save a copy of settings table first !!!
			//dbHandler.DeleteDatabase ();
           
			//if (!dbHandler.DatabaseExists())
                dbHandler.CreateTables();

			// For testing delete ApplicationSettings from Setting table
			//dbHandler.db.DeleteAll<Setting>();

			LoadAllProperties();
			SaveAllProperties();

			// Set default culture for AppResources
			AppResources.Culture = new CultureInfo(appSettings.languageID);
	
			StartPage startPage = new StartPage();
			MainPage = new NavigationPage(startPage);
            //TestService();
            //Test();

        }
		//b45c796a-be43-4725-ad6b-a0bfc0b85f94

        protected override void OnStart()
        {
			base.OnStart ();
		
			#region test resources
			/*
			// If it doesnt work as expected clean the project/solution
			//
			ResourceManager resManager = new ResourceManager("MobRegPro.Resources.AppResources",
														 typeof(TranslateExtention).GetTypeInfo().Assembly);
			CultureInfo ciENUS = new CultureInfo("en-US");
			CultureInfo ciNLBE = new CultureInfo("nl-BE");
			CultureInfo ciFRBE = new CultureInfo("fr-BE");
			string result;

			result = resManager.GetString("BtnLogin", ciENUS);
			result = resManager.GetString("BtnLogin", ciNLBE);
			result = resManager.GetString("BtnLogin", ciFRBE);

			AppResources.Culture = new CultureInfo("en-US");
			result = AppResources.BtnLogin;
			*/
			#endregion


		}

        protected override void OnSleep()
		{
			base.OnSleep ();
			// Handle when your app sleeps
			//if(isPropertiesSavingRequired)
			//		SaveAllProperties ();
			dbHandler.Dispose ();
		}

        protected override void OnResume()
        {
			base.OnResume ();
            // Handle when your app resumes
			dbHandler = new DBHandler ();
        }

		#region TEST CODE

        void Test()
        {
            SQLite.SQLiteConnection db = DependencyService.Get<ISQLite>().GetSQLiteConnection();
//           var table = (from tm in db.TableMappings where tm.TableName == "Status" select tm).FirstOrDefault();
//            if (table == null)
//            {
//                int i = db.CreateTable<Status>();
//                Status status = new Status() { ID = 1, B = 0, R = 255, G = 0, Text = "Started", doStopTime = false, doEndTime = false };
//                db.Insert(status);
//            }
                var statusList = db.Table<Status>();

            foreach(Status s in statusList)
            {
                string t = s.Text;
                bool flag = s.doStopTime;
				bool flag2 = s.doStartTime;
            }

            db.Commit();
            db.Close();

        }
			

		async Task<RestLoadResult> TestHttpService ()
		{
			RegServices.MobRegService service = new RegServices.MobRegService (ProgramVars.URL);
			LoadSyncInput inputObject = new LoadSyncInput() { installationID = 1, userID = "ff38aa48-20cd-40fd-b90b-a106ec9e3b13", loadSyncList = "Status", parameters = "" };

			RestLoadResult result = await service.LoadAsync(inputObject);
			return result;
		}


        void TestService()
        {
			RegServices.Service service = new RegServices.Service(ProgramVars.URL);

            LoadSyncInput inputObject = new LoadSyncInput() { installationID = 1, userID = "ff38aa48-20cd-40fd-b90b-a106ec9e3b13", loadSyncList = "Status|MobUser|RegistrationType", parameters = "" };
            service.Load(inputObject, (response) =>
            {
                string message;
                if (service.HasError) message = "ERROR : " + service.Error;
                else message = "Upload done :" + response.status;
                Action action = delegate
                {
                    //App.mainLabel.Text = message;
					LoginPage startPage = new LoginPage();
						MainPage.Navigation.PushAsync(startPage, true);
                };
                Device.BeginInvokeOnMainThread(action);

            });
           

            //service.Execute();
        }
		#endregion
    }
}
