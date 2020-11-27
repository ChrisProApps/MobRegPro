using System;
using System.Linq;
using MobRegPro.ORM;
using Xamarin.Forms;
using SQLite;
using System.Collections.Generic;
using RegServices.Data;
using Newtonsoft.Json;

namespace MobRegPro.Helpers
{
	public class Common
	{
		public static Status GetStatus(int statusID)
		{
			var status = App.dbHandler.db.Query<Status> ("select * from Status where ID=?", statusID).FirstOrDefault ();;
			if (status == null)
				status = new Status () { ID = statusID, Text = "???", iconBlack = "questionmark.png" };
			return status;
		}

		public static void ChangeStatus(Planning planning, int statusID)
		{
			planning.StatusID = statusID;
			planning.StatusDateTime = DateTime.Now;
			planning.StatusImage = FormatImageName (GetStatus (statusID).iconBlack);

			/*
			var lastHis = App.dbHandler.db.Query<PlanningHistory>("select * from PlanningHistory where PlanningID=? order by ", planning.ID).



			if (statusID == StatusTypes.DriveTo)
				planning.ExecStartDateTime = planning.StatusDateTime;
			if (statusID == StatusTypes.FinishedNOK || statusID == StatusTypes.FinishedOK)
			*/	
		}

		public static string FormatImageName(string imageName)
		{
			//if (Device.OS == TargetPlatform.iOS)
            if(Device.RuntimePlatform == Device.iOS)
				return string.Format ("Images/{0}", imageName);
			else
				return imageName;
		}

		public static void DeletePictureFromRegistration(Registration registration)
		{
			if (registration.RegTypeID == RegistrationTypes.Picture) {

				IFileSystem fileSystem = DependencyService.Get<IFileSystem> ();
				fileSystem.DeleteFile (fileSystem.GetDataPath ($"{registration.ID}.jpg" ) );
			}
		}

		public static void DeleteAllRegistationAndPossibleAssignedPicturesFromPlanning(Planning planning)
		{
			SQLiteConnection db = App.dbHandler.db;

			var regs = db.Table<Registration> ().Where (r => r.PlanningID == planning.ID).ToList<Registration> ();
			foreach (Registration r in regs) {
				DeletePictureFromRegistration (r);
				db.Delete (r);
			}
		}

		public static void DeleteSignature(Planning planning)
		{
			IFileSystem fileSystem = DependencyService.Get<IFileSystem> ();
			fileSystem.DeleteFile (fileSystem.GetDataPath($"sig{planning.ID}.png") );
		}

		/// <summary>
		/// Delete planning with planningID, this is a single planning item 
		///	and delete references : resources, registrations, articleregs and pictures
		/// </summary>
		/// <param name="planningID">ID of planning items to delete</param>
		public static void DeletePlanningByPlanningID(string planningID)
		{
			SQLiteConnection db = App.dbHandler.db;
			var planningToDelete = db.Table<Planning> ().Where (p => p.ID == planningID).ToList<Planning> ();
			DeleteAllPlanningItems (db, planningToDelete); 
		}


		/// <summary>
		/// Deletes the planning with status in StatusID.
		///	and delete references : resources, registrations, articleregs and pictures
		/// </summary>
		/// <param name="statusID">Status ID to match</param>
		public static void DeletePlanningByStatusID(int statusID)
		{
			SQLiteConnection db = App.dbHandler.db;
			var planningToDelete = db.Table<Planning> ().Where (p => p.StatusID == statusID).ToList<Planning> ();
			DeleteAllPlanningItems (db, planningToDelete); 
		}


		/// <summary>
		/// Deletes all item from planning with status==Planned and userID match
		///	and delete references : resources, registrations, articleregs and pictures
		/// </summary>
		/// <param name="userID">User ID to search for</param>
		public static void DeletePlannedPlanningByUserID(string userID)
		{
			SQLiteConnection db = App.dbHandler.db;
			var planningToDelete = db.Table<Planning> ().Where (p => p.StatusID == StatusTypes.Planned && p.UserID==userID).ToList<Planning> ();
			DeleteAllPlanningItems (db, planningToDelete); 
		}
			
		/// <summary>
		/// Deletes all planning items.
		///	and delete references : resources, registrations, articleregs and pictures
		/// </summary>
		/// <param name="db">Db.</param>
		/// <param name="planningToDelete">Planning to delete.</param>
		public static void DeleteAllPlanningItems (SQLiteConnection db, List<Planning> planningToDelete)
		{
			foreach (Planning p in planningToDelete) {
				var hisToDelete = db.Table<PlanningHistory> ().Where (r => r.PlanningID == p.ID).ToList<PlanningHistory> ();
				foreach (PlanningHistory r in hisToDelete)
					db.Delete (r);
				var resToDelete = db.Table<Resource> ().Where (r => r.PlanningID == p.ID).ToList<Resource> ();
				foreach (Resource r in resToDelete)
					db.Delete (r);
				var regsToDelete = db.Table<Registration> ().Where (r => r.PlanningID == p.ID).ToList<Registration> ();
				foreach (Registration r in regsToDelete) {
					DeletePictureFromRegistration (r);
					db.Delete (r);
				}
				DeleteSignature (p);
				var artRegsToDelete = db.Table<ArticleReg> ().Where (r => r.PlanningID == p.ID).ToList<ArticleReg> ();
				foreach (ArticleReg r in artRegsToDelete)
					db.Delete (r);
				db.Delete<Planning> (p.ID);
			}
			//foreach planning
		}



		public static List<rsResource> GetResourcesForPlanningID(string planningID)
		{
			var dbResources = (from r in App.dbHandler.db.Table<Resource> ()
			                   where r.PlanningID == planningID
				select new rsResource () { UserID=r.UserIDGuid, FriendlyName=r.FriendlyName, OwnPlanningID=r.OwnPlanningIDGuid, IsDriver=r.IsDriver, IsPresent=r.IsPresent,
					IsSeparate=r.IsSeparate, StartDate=r.StartDate, EndDate = r.EndDate
			}).ToList<rsResource> ();
			return dbResources;
		}

		#region Database Setting handling
		public static Setting ReadApplicationSettingsFromDatabase()
		{
			var dbSetting = Common.ReadSettingFromDatabase (Constants.applicationSettingsID);
			return dbSetting;
		}

		public static bool SaveApplicationSettingsToDatabase(ApplicationSettings applicationSettings)
		{
			try {
				Setting dbSetting = Common.ReadApplicationSettingsFromDatabase ();
				if (dbSetting == null) {
					dbSetting = new Setting();
					dbSetting.ID = Constants.applicationSettingsID;
					App.dbHandler.db.Insert(dbSetting);
				}
				dbSetting.IsActiv = true;
				dbSetting.Content = JsonConvert.SerializeObject (applicationSettings);
				App.dbHandler.db.Update (dbSetting);

			} catch (Exception) {
				return false;
			}
			return true;
			
		}

		public static Setting ReadSettingFromDatabase(string settingsID)
		{
			return (from s in App.dbHandler.db.Table<Setting>() where s.ID==settingsID select s).FirstOrDefault();

		}


		#endregion

	}
}

