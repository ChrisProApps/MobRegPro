using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using MobRegPro.Helpers;
using MobRegPro.ORM;
using RegServices.Data;
using SQLite;
using System.Threading.Tasks;
using RegServices;
using MobRegPro.Resources;
using Xamarin.Essentials;

namespace MobRegPro
{
	public enum HasNavigatedFrom {PreviousPage, SummaryPage, InfoPage, SignaturePage};

	public partial class StatusChangePage : ContentPage
	{
		private List<StatusSequence> statusSequences;
		private Planning planning;
		private HasNavigatedFrom hasNavigatedFrom;
		private SQLiteConnection db;
        
		public static bool IsStillRunning;

		//int originalStatusID;
		//long orginalPauzeTime;
		int startCount;
		DateTime time;
		DateTime previousTime;
		int newStatusID;

		public StatusChangePage (Planning planning)
		{
			InitializeComponent ();
			hasNavigatedFrom = HasNavigatedFrom.PreviousPage;
			this.planning = planning;
			this.Title = AppResources.btnStatusChange;
			lvStatus.ItemTapped +=  async (object sender, ItemTappedEventArgs e) => {
				Status status = e.Item as Status;
				lvStatus.SelectedItem = null;
				newStatusID = status.ID;
				await PrepareStatusChange(status.ID);
			}; // end event ItemTapped
		}

		protected override void OnDisappearing()
		{
			StatusChangePage.IsStillRunning = false;
			base.OnDisappearing ();
		}

		async protected override void OnAppearing()
		{
			base.OnAppearing ();
			StatusChangePage.IsStillRunning = true;
			if (hasNavigatedFrom == HasNavigatedFrom.PreviousPage) {
				Status status = Common.GetStatus (planning.StatusID);
				imgStatus.Source = Common.FormatImageName (status.iconBlack);
				lbStatus.Text = $"{AppResources.Status} : {status.Text}";
				lbStatusTime.Text = planning.StatusDateTime.ToString ("dd-MMM-yyyy HH:mm:ss");
				panelStatus.BackgroundColor = Color.FromRgb (status.R, status.G, status.B);

				statusSequences = StatusSequenceBuilder.Run ();
				int statusID = planning.StatusID; 
				StatusSequence s = (from sq in statusSequences
					where sq.current.ID == statusID
					select sq).FirstOrDefault ();
				if (s != null)
					lvStatus.ItemsSource = s.next;
			}
			if (hasNavigatedFrom == HasNavigatedFrom.InfoPage) {
				// Continue processing Started status after returning from SummaryPage
				//
				hasNavigatedFrom = HasNavigatedFrom.PreviousPage;
				// Check if no status DriveTo and first Started => set planning.ExecStartDateTime = time
				int driveToCount = (from h in db.Table<PlanningHistory>() where h.PlanningID == planning.ID && h.StatusID == StatusTypes.DriveTo select h).Count();
				if (startCount == 0 && driveToCount == 0) planning.ExecStartDateTime = time;
				if (planning.StatusID == StatusTypes.Pauzed) planning.PauzeTime += (time.Ticks - previousTime.Ticks);
				await PerformStatusChange(newStatusID);
			}
			if (hasNavigatedFrom == HasNavigatedFrom.SignaturePage) {
				planning.Signature = null;

				//if (Device.OS == TargetPlatform.Android) {
                if(Device.RuntimePlatform == Device.Android) {
					await Task.Run (() => {
						do {
						} while(SignaturePage.IsStillrunning);
					});
				}
				// Check signature
				//
				if (planning.SignaturePoints == null || planning.SignaturePoints.Length <= 200 || string.IsNullOrEmpty (planning.SignatureName))
					await DisplayAlert (AppResources.Error, AppResources.MissingSignature, AppResources.OK);
				else {
					// Save Registrations, ArticleReg, Planning (selected fields) and pictures to server
					//
					if (await SaveRegistrationsAndSignatureDataToServer (planning, newStatusID)) {
						// Update status on server
						//
						await PerformStatusChange (newStatusID);
					}
				}
			}

		}

		void UpdateUI(bool setBusy)
		{
			activityIndicator.IsRunning = activityIndicator.IsVisible = setBusy;
			lvStatus.IsEnabled = !setBusy;
		}

		protected async Task PrepareStatusChange(int selectedStatusID)
		{
			// Check if network available
			//
			if (Connectivity.NetworkAccess != NetworkAccess.Internet)
			{
                await DisplayAlert(AppResources.Warning, AppResources.NoNetwork, AppResources.OK);
			}

			// Set variables
			//
			db =  App.dbHandler.db;
			//originalStatusID = planning.StatusID;   //save original value on entry
			//orginalPauzeTime = planning.PauzeTime; 	//save original value on entry
			previousTime = planning.StatusDateTime;
			time = DateTime.Now;

			Summary summary;

			UpdateUI (false);

			// Check before status change is allowed
			//
			switch (selectedStatusID)
			{
			case StatusTypes.DriveTo:
				planning.ExecStartDateTime = time;
				await PerformStatusChange(selectedStatusID);
				break;
			case StatusTypes.Started:
				// Check for end of pauze
				if (planning.StatusID == StatusTypes.Pauzed)
					planning.PauzeTime += (time.Ticks - previousTime.Ticks);
				
				startCount = (from h in db.Table<PlanningHistory> ()
					where h.PlanningID == planning.ID && h.StatusID == StatusTypes.Started
					select h).Count ();

				if (startCount == 0) { //if first time ever the planning is started, show the to display registrations if any
					int regsCount = (from r in db.Table<Registration> ()
						where r.PlanningID == planning.ID && !r.IsDeleted && r.IsDisplayed
						select r).Count ();
					if (regsCount > 0) { //only show when there are registrations to display
						hasNavigatedFrom = HasNavigatedFrom.InfoPage;
					    summary = new Summary () {
							Planning = planning,
							StopTime = time,
							PauzeTime = 0,
							SummaryType=SummaryTypes.Display  //Display summary
						};
						await App.Current.MainPage.Navigation.PushAsync (new InfoPage (summary));
					}
					else await PerformStatusChange (selectedStatusID);
				}
				else 
					await PerformStatusChange (selectedStatusID);
				break;

			case StatusTypes.Stopped:
				// Check for end of pauze
				if (planning.StatusID == StatusTypes.Pauzed)
					planning.PauzeTime += (time.Ticks - previousTime.Ticks);
				// Get count of missing required registrations
				//
				int count = (from r in db.Table<Registration> ()
				             where r.PlanningID == planning.ID && r.IsRequired && r.Result == "" && !r.IsDeleted
				             select r).Count ();
				if (count > 0) {
					await DisplayAlert (AppResources.Warning, AppResources.NotAllRequiredRegistrationsProcessed, AppResources.OK);
					return;
				}

				// Check if all pictures are taken
				//
				//var regPics = from r in db.Table<Registration> ()
				//              where r.PlanningID == planning.ID && r.RegTypeID == RegistrationTypes.Picture && !r.IsDeleted
				//              select r;
				//foreach (Registration r in regPics) {
					//string fileName = string.Format(@"{0}\{1}.jpg", ProgramVars.pictureDirectory, r.ID.ToString());
					//if (!System.IO.File.Exists(fileName)) //possibility that picture does not exist, show message box but keep going
					//	Common.ShowMessage(string.Format(text.MissingPictures, r.Caption));
				//}

				// Get signature
				//
				summary = new Summary () {
					Planning = planning,
					StopTime = time,
					PauzeTime = planning.PauzeTime,
					SummaryType = SummaryTypes.Completed
				};
				hasNavigatedFrom = HasNavigatedFrom.SignaturePage;
				await App.Current.MainPage.Navigation.PushAsync (new SignaturePage (summary));
				break;

			default:
				await PerformStatusChange (selectedStatusID);
				break;

			} //end switch



		}//end method


		protected async Task PerformStatusChange(int selectedStatusID)
		{
			StatusResult result;
			try {
				//Update server
				//
				UpdateUI(true);
				MobRegService service = new MobRegService (ProgramVars.URL);
				UpdateStatusInput input = new UpdateStatusInput () {
					newStatus = selectedStatusID,
					userID = App.appSettings.loginVars.userID.ToString (),
					installationID = App.appSettings.installationID,
					planningID = planning.ID,
					resources = Common.GetResourcesForPlanningID (planning.ID)
				};
				result = await service.UpdateStatusForGroupAsync (input);

				if (result.statusCode != 0)
					throw new Exception (result.status);
				//Update client
				//
				Common.ChangeStatus(planning, selectedStatusID);


				// Do post status change processing
				//
				if (selectedStatusID == StatusTypes.FinishedOK || selectedStatusID == StatusTypes.FinishedNOK) {
					planning.ExecEndDateTime = time;
					// Send ExecXXXDateTimes to server
					//
					SavePlanningInput saveInput = new SavePlanningInput () {
						userIDin = App.appSettings.loginVars.userID.ToString (),
						installationID = App.appSettings.installationID,
						dataIn = planning.TOrsPlanning (),
						overWriteExecEndDateTime = true,
						includeSignatureData = false,
						sendReport = true,
						resources = Common.GetResourcesForPlanningID (planning.ID)
					};
					result = await service.SavePlanningAsync (saveInput);
					if (result.statusCode != 0)
						throw new Exception (result.status);
					
					// Delete all planning related data from client
					//
					Common.DeletePlanningByPlanningID (planning.ID);
				} else {
					// Update PlanningHistory table
					//
					planning.StatusID = selectedStatusID;
					App.dbHandler.db.Update (planning);
					PlanningHistory his = new PlanningHistory () {
						PlanningID = planning.ID,
						StatusID = planning.StatusID,
						StartTime = time,
						EndTime = time
					};
					App.dbHandler.db.Insert (his);
				}
				//activityIndicator.IsRunning = activityIndicator.IsVisible = false;
				UpdateUI(false);
				await App.Current.MainPage.Navigation.PopAsync (true);

			} catch (Exception ex) {
				UpdateUI (false);
				//activityIndicator.IsRunning = activityIndicator.IsVisible = false;
				await DisplayAlert (AppResources.Error, ex.Message, AppResources.Cancel);
			}

		}


		async private Task<bool> SaveRegistrationsAndSignatureDataToServer(Planning planning, int statusID)
		{
			try {
				SQLiteConnection db = App.dbHandler.db;
				StatusResult statusResult;

				//lbStatus.Text = "Sending registrations"
				//activityIndicator.IsRunning = activityIndicator.IsVisible = true;
				UpdateUI(true);

				var regs = from r in db.Table<Registration> ()
				           where r.PlanningID == planning.ID
				           select r;
				List<rsRegistration> registrationList = new List<rsRegistration> (regs.Count () + 1);
				foreach (Registration r in regs) {
					rsRegistration reg = new rsRegistration () {
						ID = r.IDguid,
						PlanningID = r.PlanningIDguid,
						OrderID = r.OrderIDguid,
						UserID = r.UserIDguid,
						Date = r.Date,
						RegTypeID = r.RegTypeID,
						Priority = r.Priority,
						IsDisplayed = r.IsDisplayed,
						IsOnReport = r.IsOnReport,
						IsRequired = r.IsRequired,
						IsReadingOnly = r.IsReadingOnly,
						Caption = r.Caption,
						Result = r.Result,
						PathName = r.PathName,
						Input = r.Input,
						IsClientReg = r.IsClientReg,
						IsChanged = r.IsChanged,
						IsDeleted = r.IsDeleted
					};
					registrationList.Add (reg);
				}

				//lbSendStatus.Text = text.SendingArticles;

				var artRegs = (from a in db.Table<ArticleReg> ()
				               where a.PlanningID == planning.ID
				               select new rsArticleReg () {ArticleID = a.ArticleID, Article = "", OrderID = a.OrderIDguid, PlanningID = a.PlanningIDguid, Qty = a.Qty, IsChanged = a.IsChanged, 
					IsDeleted = a.IsDeleted, PriceIn = a.PriceIn, PriceOut = a.PriceOut
				}).ToList<rsArticleReg> ();
				
				// Save registrations and articleregistrations to server
				//
				SaveRegistrationInput input = new SaveRegistrationInput () {
					userIDin = App.appSettings.loginVars.userID.ToString (),
					installationID = App.appSettings.installationID,
					registrations = registrationList,
					articles = artRegs
				};
				MobRegService service = new MobRegService (ProgramVars.URL);
				statusResult = await service.SaveRegistrationsAsync (input);
				if (statusResult.statusCode != 0)
					throw new Exception (statusResult.status);

						
				//
				// SaveRegistrations completed OK, next SavePlanning
				//
				// input.dataIn, input.overWriteExecEndDateTime, input.includeSignatureData, input.sendReport, input.resources
				rsPlanning rsplanning = planning.TOrsPlanning ();
				var planningResources = db.Table<Resource> ().Where (r => r.PlanningID == planning.ID);
				if (planningResources != null && planningResources.Count () > 0) {
					rsplanning.Resources = new List<rsResource> (planningResources.Count ());
					foreach (Resource res in planningResources) {
						rsplanning.Resources.Add (res.TOrsResource ());
					}
				}

				SavePlanningInput saveInput = new SavePlanningInput () {
					userIDin = App.appSettings.loginVars.userID.ToString (),
					installationID = App.appSettings.installationID,
					dataIn = rsplanning,
					overWriteExecEndDateTime = false,
					includeSignatureData = true,
					sendReport = false,
					resources = Common.GetResourcesForPlanningID (planning.ID)
				};
				statusResult = await service.SavePlanningAsync (saveInput);
				if (statusResult.statusCode != 0)
					throw new Exception (statusResult.status);

				// Save all pictures with changes and not deleted to server
				//
				var regPics = from r in db.Table<Registration> ()
				                    where r.PlanningID == planning.ID && r.RegTypeID == RegistrationTypes.Picture && r.IsChanged && !r.IsDeleted
				                    select r;
				IFileSystem fileSystem = DependencyService.Get<IFileSystem>();
				string handlerUrl, path;
				foreach(Registration r in regPics)
				{
					path = fileSystem.GetDataPath($"{r.ID}.jpg");
					if(fileSystem.ExistsFile(path).statusCode == 0){
						handlerUrl = string.Format ("{0}?regid={1}&userid={2}&instid={3:d}&mode=up", ProgramVars.ImageHandlerUrl, 
							r.ID, r.UserID, App.appSettings.installationID);
						statusResult = await service.SaveBinaryAsync(handlerUrl, fileSystem.OpenFileStream(path));
						if(statusResult.statusCode != 0) throw new Exception(statusResult.status);
					}
				}

				//statusPanel.Visible = false;

				// Save signature
				//
				handlerUrl = string.Format ("{0}?regid={1}&userid={2}&instid={3:d}&mode=SIGNATURE", ProgramVars.ImageHandlerUrl, 
					planning.ID, App.appSettings.loginVars.userID.ToString(), App.appSettings.installationID);
				path = fileSystem.GetDataPath($"sig{planning.ID}.png");
				statusResult = await service.SaveBinaryAsync(handlerUrl, fileSystem.OpenFileStream(path));
				if(statusResult.statusCode != 0) throw new Exception(statusResult.status);

				//activityIndicator.IsRunning = activityIndicator.IsVisible = false;
				UpdateUI(true);

				return true;
			} catch (Exception ex) {
				//statusPanel.Visible = false;
				//activityIndicator.IsRunning = activityIndicator.IsVisible = false;
				UpdateUI(false);
				await DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);
				return false;
			}


		}

	}
}
		
	


