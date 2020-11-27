using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using SQLite;
using MobRegPro.ORM;
using RegServices.Data;
using MobRegPro.Helpers;
using RegServices;
using System.IO;
using System.Diagnostics;
using MobRegPro.Resources;

namespace MobRegPro
{
    public partial class PlanningListPage : ContentPage
    {
		private SQLiteConnection db;

        public PlanningListPage()
		{
			InitializeComponent ();
			//this.Icon = "Images/ok.png";
			this.Title = AppResources.cpPlanningList;

			db = App.dbHandler.db;

			// Add the event handler only once, else issues on iOS
			//
			lvPlanning.ItemTapped += LvPlanning_ItemTapped;
			ToolbarItem item = new ToolbarItem () { Text = AppResources.itemRefresh };
			item.Clicked += RefreshItem_Clicked;
			ToolbarItems.Add (item);
		}

        async void RefreshItem_Clicked (object sender, EventArgs e)
        {
			await PreparePlanning ();
			//await RefreshPlanning();
			ShowPlanning ();
        }

		protected override void OnDisappearing ()
		{
			Debug.WriteLine ("PlanningListPage : OnDisappearing");
			lvPlanning.IsVisible = false;
			base.OnDisappearing ();
		}

		async protected override void OnAppearing ()
		{
			base.OnAppearing ();
			Debug.WriteLine ("PlanningListPage : OnAppearing");
			await PreparePlanning ();
			//await RefreshPlanning ();
			ShowPlanning ();
		}

		void SetWaitStatus(bool showWaitStatus)
		{
			if (showWaitStatus) {
				statusPanel.IsVisible = true;
				activityIndicator.IsRunning = true;
				lvPlanning.IsEnabled = false;
			} 
			else {
				statusPanel.IsVisible = false;
				activityIndicator.IsRunning = false;
				lvPlanning.IsEnabled = true;
			}

		}

        async void LvPlanning_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			db = App.dbHandler.db; //set db could be cleaned up
			Planning p = e.Item as Planning;
			// if planningItem tapped then load registrations from server
			//  only if status == Planned
			if (p.StatusID == StatusTypes.Planned) {
				try {
					SetWaitStatus(true);
					lbProgress.Text = AppResources.lblProgressRegistrations;

					MobRegService service = new MobRegService (ProgramVars.URL);
			
					RegResult result = await service.GetRegistrationsAsync (new GetRegistrationInput () {
						userID = App.appSettings.loginVars.userID.ToString (),
						installationID = App.appSettings.installationID,
						planningID = p.ID
					});
					if (result.statusCode == 0) {
						// Has Registrations
						//
						if (result.Registrations != null) {
							foreach (rsRegistration rg in result.Registrations) {
								Registration registration = new Registration () {
									ID = rg.ID.ToString (), PlanningID = rg.PlanningID.ToString (), OrderID = rg.OrderID.ToString (), UserID = rg.UserID.ToString (),
									RegTypeID = rg.RegTypeID, Priority = rg.Priority, IsDisplayed = rg.IsDisplayed, IsOnReport = rg.IsOnReport,
									IsRequired = rg.IsRequired, IsReadingOnly = rg.IsReadingOnly, Caption = rg.Caption, Result = rg.Result,
									PathName = rg.PathName, Input = rg.Input, Date = rg.Date, IsChanged = rg.IsChanged, IsClientReg = rg.IsClientReg, IsDeleted = rg.IsDeleted
								};
								db.Insert (registration);
								if (registration.RegTypeID == RegistrationTypes.Picture) {
									IFileSystem fileSystem = DependencyService.Get<IFileSystem> ();

									using (Stream saveToStream = fileSystem.CreateFileStream (fileSystem.GetDataPath ($"{registration.ID}.jpg"))) {
										MobRegService httpService = new MobRegService (ProgramVars.ImageHandlerUrl);
										string handlerUrl = string.Format ("{0}?regid={1}&userid={2}&instid={3:d}&mode=down", ProgramVars.ImageHandlerUrl, 
											registration.ID, registration.UserID, App.appSettings.installationID);
										await httpService.LoadBinaryAsync (handlerUrl, saveToStream);
										await saveToStream.FlushAsync ();
									} // end using
							
								}//end load picture
							}// foreach registration
						} // if has registrations

                        // Has Articles
                        //
                        if (result.Articles != null) {
							foreach (rsArticleReg ar in result.Articles) {
								ArticleReg articleReg = new ArticleReg () { 
                                    ID = ArticleReg.CreateID(ar.ArticleID, ar.PlanningID.ToString()),
									ArticleID = ar.ArticleID, OrderID = ar.OrderID.ToString (), PlanningID = ar.PlanningID.ToString (),
									Qty = ar.Qty, PriceIn = ar.PriceIn, PriceOut = ar.PriceOut, IsChanged = ar.IsChanged, IsDeleted = ar.IsDeleted
								};
								db.Insert (articleReg);
							}
						}
						SetWaitStatus(false);
						await App.Current.MainPage.Navigation.PushAsync (new MainMenuPage (p));

					}// result.StatusCode == 0
				else {
						SetWaitStatus(false);
						lvPlanning.SelectedItem = null;
                        await DisplayAlert(AppResources.Error, result.status, AppResources.OK);
					}
				} //try
				catch (Exception ex) {
					SetWaitStatus(false);
                    await DisplayAlert(AppResources.Error, ex.Message, AppResources.Cancel);
                    //Roll back all changes
                    try
                    {
                        Common.DeleteAllRegistationAndPossibleAssignedPicturesFromPlanning(p);
                        //db.Execute("delete from Registration where PlanningID=?", p.ID); //not needed since previous call also deletes all registrations, not only the one with pictures
                        db.Execute("delete from ArticleReg where PlanningID=?", p.ID);
                    }
                    catch(Exception ex2)
                    {
                        string msg = ex.Message;
                    }
					lvPlanning.SelectedItem = null;
				}
			} // end if StatusID ==
			else
				await App.Current.MainPage.Navigation.PushAsync (new MainMenuPage (p));
		}//end event ItemTapped

		void ShowPlanning()
		{
			if (db == null)
				db = App.dbHandler.db;
			var planning = db.Table<Planning> ().ToList<Planning> ().Where (p => p.UserIDguid == App.appSettings.loginVars.userID);
			lvPlanning.ItemsSource = planning;
			lvPlanning.IsVisible = true;
		}
	
		async Task PreparePlanning()
		{
			// Display waiter
			SetWaitStatus(true);
			lbProgress.Text = AppResources.lblProgressPlanning;

			// Set db again else db is cleared after power down/up, resulting in program crash
			//
			try {
				db = App.dbHandler.db;
                MobRegService service = new MobRegService(ProgramVars.URL);

                // 1. Delete all 'planned' plannings and associated data from local database
                //
                Common.DeletePlannedPlanningByUserID (App.appSettings.loginVars.userID.ToString ());
				// 2. Validate rest of local stored planning data against server stored data
				//
				var plannings = db.Table<Planning> ().ToList<Planning> ().Where (p => p.UserIDguid == App.appSettings.loginVars.userID);
				ValidatePlanningInput input = new ValidatePlanningInput () { 
					installationID = App.appSettings.installationID,
					userIDin = App.appSettings.loginVars.userID.ToString ()
				};
				input.plannings = new List<rsPlanningValidation> (plannings.Count() + 1);
				foreach (Planning p in plannings)
					input.plannings.Add (new rsPlanningValidation () { PlanningID = p.IDguid, StatusID = p.StatusID });
			 
				ValidatePlanningResult valResult = await service.ValidatePlanningAsync (input);
				if (valResult.statusCode != 0)
					throw new Exception (valResult.status);

				if (valResult.PlanningResults != null) {
					foreach (rsPlanningValidation plan in valResult.PlanningResults) {
						if (plan.StatusCode != 0)
							Common.DeletePlanningByPlanningID (plan.PlanningID.ToString ());
					}

				}

				// 3. Get new planning from server
				//
				PlanningResult result = await service.GetPlanningAsync (new RegServices.Data.GetPlanningInput () {
					userID = App.appSettings.loginVars.userID.ToString (),
					installationID = App.appSettings.installationID,
					carID = App.appSettings.loginVars.carID
				});
				if(result.statusCode != 0) throw new Exception(result.status);
				foreach (rsPlanning p in result.Plannings) {
					Planning planning = new Planning () {
						ID = p.ID.ToString (),
						OrderID = p.OrderID.ToString (),
						StatusID = p.StatusID,
						CarID = p.CarID,
						StartTime = p.StartDateTime,
						EndTime = p.EndDateTime,
						Project = p.Project,
						Alias = p.Alias,
						OrderType = p.OrderType,
						Description = p.Description,
						Comment = p.Comment,
						Reference = p.Reference,
						Customer = p.Customer,
						ContactName = p.ContactName,
						Phone = p.Phone,
						Street = p.Street,
						HouseNr = p.HouseNr,
						Zip = p.Zip,
						City = p.City,
						Country = p.Country,
						UserID = p.UserID.ToString (),
						PauzeTime = p.PauzeTime,
						ExecStartDateTime = p.ExecStartDateTime,
						ExecEndDateTime = p.ExecEndDateTime,
						Email = p.Email,
						SignatureName = p.SignatureName
					};
					Common.ChangeStatus (planning, planning.StatusID);
					PlanningHistory his = new PlanningHistory () {
						PlanningID = planning.ID,
						StatusID = planning.StatusID,
						StartTime = DateTime.Now,
						EndTime = DateTime.Now
					};
					db.Insert (his);
					db.Insert (planning);
					foreach (rsResource r in p.Resources) {
						Resource res = new Resource () {
							PlanningID = planning.ID.ToString (),
							UserID = r.UserID.ToString (),
							FriendlyName = r.FriendlyName,
							OwnPlanningID = r.OwnPlanningID.ToString (),
							IsDriver = r.IsDriver,
							IsSeparate = r.IsSeparate,
							IsPresent = r.IsPresent,
							StartDate = r.StartDate,
							EndDate = r.EndDate
						};
						res.ID = res.PlanningID + res.UserID;
						//fake clustered PK, need this for deleting records
						db.Insert (res);
					}
				}//end foreach
				SetWaitStatus(false);

			} catch (Exception ex) {
				SetWaitStatus(false);
                await DisplayAlert(AppResources.Error, ex.Message, AppResources.Cancel);
			}
		}

		/*
		async Task RefreshPlanning ()
		{
			// Set db again else db is cleared after power down/up, resulting in program crash
			//
			db = App.dbHandler.db;
			// Delete all plannings with statusID = planned
			//	and delete references : resources, registrations, articleregs and pictures
			//
			Common.DeletePlanningByStatusID (StatusTypes.Planned);
			// Get planning from server
			//
			MobRegService service = new MobRegService (ProgramVars.URL);
			PlanningResult result = await service.GetPlanningAsync (new RegServices.Data.GetPlanningInput () {
				userID = App.loginVars.userID.ToString (),
				installationID = App.installationID,
				carID = App.loginVars.carID
			});
			if (result.statusCode == 0) {
				foreach (rsPlanning p in result.Plannings) {
					Planning planning = new Planning () {
						ID = p.ID.ToString (),
						OrderID = p.OrderID.ToString (),
						StatusID = p.StatusID,
						CarID = p.CarID,
						StartTime = p.StartDateTime,
						EndTime = p.EndDateTime,
						Project = p.Project,
						Alias = p.Alias,
						OrderType = p.OrderType,
						Description = p.Description,
						Comment = p.Comment,
						Reference = p.Reference,
						Customer = p.Customer,
						ContactName = p.ContactName,
						Phone = p.Phone,
						Street = p.Street,
						HouseNr = p.HouseNr,
						Zip = p.Zip,
						City = p.City,
						Country = p.Country,
						UserID = p.UserID.ToString (),
						PauzeTime = p.PauzeTime,
						ExecStartDateTime = p.ExecStartDateTime,
						ExecEndDateTime = p.ExecEndDateTime,
						Email = p.Email,
						SignatureName = p.SignatureName
					};
					Common.ChangeStatus (planning, planning.StatusID);
					PlanningHistory his = new PlanningHistory () {
						PlanningID = planning.ID,
						StatusID = planning.StatusID,
						StartTime = DateTime.Now,
						EndTime = DateTime.Now
					};
					db.Insert (his);
					db.Insert (planning);
					foreach (rsResource r in p.Resources) {
						Resource res = new Resource () {
							PlanningID = planning.ID.ToString (),
							UserID = r.UserID.ToString (),
							FriendlyName = r.FriendlyName,
							OwnPlanningID = r.OwnPlanningID.ToString (),
							IsDriver = r.IsDriver,
							IsSeparate = r.IsSeparate,
							IsPresent = r.IsPresent,
							StartDate = r.StartDate,
							EndDate = r.EndDate
						};
						res.ID = res.PlanningID + res.UserID;
						//fake clustered PK, need this for deleting records
						db.Insert (res);
					}
				}
				//end foreach
			}
			//end statusCode == 0	
			else
				await DisplayAlert ("Error", result.status, "Cancel");
		}
*/

    }
}
