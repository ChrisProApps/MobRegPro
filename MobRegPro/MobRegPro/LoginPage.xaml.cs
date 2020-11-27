using RegServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using MobRegPro.ORM;
using MobRegPro.Helpers;
using SQLite;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class LoginPage : ContentPage
	{
		private bool IsReturning;
		
		public LoginPage ()
		{
			InitializeComponent ();
		
			this.Disappearing += StartPage_Disappearing;
            this.Title = AppResources.cpLogin;
            ShowActivityUI(false);
			//this.imgOK.Aspect = Aspect.AspectFit;
			//this.imgOK.Source = ImageSource.FromResource("MobRegPro.Images.ok.png");
			//this.BackgroundColor = Color.White;
			IsReturning = false;
		}

		private void ShowActivityUI (bool isActiv)
		{
			activityIndicator.IsVisible = activityIndicator.IsRunning = isActiv;
			BtnLogin.IsEnabled = !isActiv;
		}

		private async void StartPage_Disappearing (object sender, EventArgs e)
		{
			await App.SaveAllPropertiesAsync ();
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			if (IsReturning) await LogoutAsync ();
			App.appSettings.maxHeigth = Bounds.Width;
		}

		async private void BtnLoginClicked (object sender, EventArgs e)
		{
			 await LogInAsync ();
		}

		protected async Task LogInAsync ()
		{
			try {
				ShowActivityUI (true);
				LoginUserInput userInput = new LoginUserInput () {
//					userName = string.IsNullOrEmpty (tbUserName.Text) ? "Chris" : tbUserName.Text,
//					password = string.IsNullOrEmpty (tbPassword.Text) ? "test" : tbPassword.Text,
//					carID = string.IsNullOrEmpty (tbCar.Text) ? "1ABC001" : tbCar.Text,
					userName = App.appSettings.defaultInUse ? App.appSettings.defaultUserName : tbUserName.Text,
					carID = App.appSettings.defaultInUse ? App.appSettings.defaultCarID : tbCar.Text,
					password = App.appSettings.defaultInUse ? App.appSettings.defaultPassword : tbPassword.Text,
					installationID = App.appSettings.installationID,
					deviceLangID = App.appSettings.languageID,
					loginHistoryID = App.appSettings.loginVars.loginHistory,
					login = true
				};
				RegServices.MobRegService service = new RegServices.MobRegService (ProgramVars.URL);
				LoginResult result = await service.LoginUserAsync (userInput);
				if (result.statusCode == 0) {
					App.appSettings.loginVars.loginHistory = result.historyID;
					App.appSettings.loginVars.userID = result.userID;
					App.appSettings.loginVars.userName = userInput.userName;
					App.appSettings.loginVars.passWord = userInput.password;
					App.appSettings.loginVars.carID = userInput.carID;
					App.isPropertiesSavingRequired = true;
					await LoadLookupDataAsync ();
					await SynchronizeDataAsync ();
					PlanningListPage planningListPage = new PlanningListPage ();
					IsReturning = true;
					await App.Current.MainPage.Navigation.PushAsync (planningListPage);
				} else {
					ShowActivityUI (false);
					await DisplayAlert (AppResources.Error, result.status, AppResources.OK);
				}
			} catch (Exception ex) {
				ShowActivityUI (false);
				await DisplayAlert (AppResources.Error, ex.Message, AppResources.OK);
			}
		}
	
		protected async Task LogoutAsync()
		{
			if (string.IsNullOrEmpty (App.appSettings.loginVars.carID))
				return;

			ShowActivityUI (true);

			LoginUserInput userInput = new LoginUserInput () {
				loginHistoryID = App.appSettings.loginVars.loginHistory,
				userName = App.appSettings.loginVars.userName,
				password = App.appSettings.loginVars.passWord,
				carID = App.appSettings.loginVars.carID,
				installationID = App.appSettings.installationID,
				deviceLangID = App.appSettings.languageID,
				login = false
			};
			RegServices.MobRegService service = new RegServices.MobRegService (ProgramVars.URL);
			LoginResult result = await service.LoginUserAsync (userInput);
			ShowActivityUI (false);
			if (result.statusCode == 0 || result.statusCode == 7) { //7 occurs when user wasn't logged in, is not a failure
				App.appSettings.loginVars.loginHistory = -1;
				App.appSettings.loginVars.userID = result.userID;
				App.appSettings.languageID = result.language;
			} 
			else {
				await DisplayAlert (AppResources.Error, result.status, AppResources.Cancel);
			}
		}

		private async Task SynchronizeDataAsync()
		{
			ShowActivityUI (true);

			SQLiteConnection db = App.dbHandler.db;

			//db.DeleteAll<Article> ();
			//db.DeleteAll<ArticleGroup> ();

			var articles = db.Query<Article> ("select * from Article ORDER by SysRevision DESC");
			long maxSysRevArticle = articles.FirstOrDefault () == null ? 0 : articles.FirstOrDefault ().SysRevision;
			//db.Execute("select MAX(SysRevision) from Article");
			var articleGroups = db.Query<Article> ("select * from ArticleGroup ORDER by SysRevision DESC");
			long maxSysRevArticleGroup = articleGroups.FirstOrDefault () == null ? 0 : articleGroups.FirstOrDefault ().SysRevision;
			//= db.Execute("select MAX(SysRevision) from ArticleGroup");

			RegServices.MobRegService service = new RegServices.MobRegService (ProgramVars.URL);
			LoadSyncInput inputObject = new LoadSyncInput () { 
				installationID = App.appSettings.installationID, 
				userID = App.appSettings.loginVars.userID.ToString (), 
				loadSyncList = string.Format("Article={0:d}|ArticleGroup={1:d}", maxSysRevArticle, maxSysRevArticleGroup ) , 
				parameters = ""
			};

			RestSyncResult result = await service.SynchronizeAsync (inputObject);
			if (result.statusCode != 0) {
				ShowActivityUI (false);
				await DisplayAlert (AppResources.Error, result.status, AppResources.Cancel);
			}
			else {
				if (result.Articles != null) {
					foreach (rsArticle a in result.Articles) {
						Article article = db.Find<Article> (a.ID);
						if (article != null) {
							if (a.SysDeleted)
								db.Delete (article);
							else {
								article.SysRevision = a.SysRevision;
								article.Name = a.Name;
								article.GroupID = a.GroupID;
								db.Update (article);
							}
						} else {
							if (!a.SysDeleted) {
								article = new Article (a);
								db.Insert (article);
							}
						}
					}

				}
				if (result.ArticleGroups != null) {
					foreach (rsArticleGroup g in result.ArticleGroups) {
						ArticleGroup artGroup = db.Find<ArticleGroup> (g.ID);
						if (artGroup != null) {
							if (g.SysDeleted)
								db.Delete (artGroup);
							else {
								artGroup.Name = g.Name;
								artGroup.SysRevision = g.SysRevision;
								db.Update (artGroup);
							}
						} else {
							if (!g.SysDeleted) {
								artGroup = new ArticleGroup (g);
								db.Insert (artGroup);
							}
						}
							
					}

				}

				//end foreach
			} // end if null

			ShowActivityUI (false);

		}

		private async Task LoadLookupDataAsync ()
		{
			ShowActivityUI (true);

			RegServices.MobRegService service = new RegServices.MobRegService (ProgramVars.URL);
			LoadSyncInput inputObject = new LoadSyncInput () { 
				installationID = App.appSettings.installationID, 
				userID = App.appSettings.loginVars.userID.ToString (), 
				loadSyncList = "Status|RegistrationType|MobUser", 
				parameters = ""
			};

			RestLoadResult result = await service.LoadAsync (inputObject);
			if (result.statusCode != 0) {
				ShowActivityUI (false);
				await DisplayAlert (AppResources.Error, result.status, AppResources.OK);
			}
			else {
				App.dbHandler.db.DeleteAll<RegistrationType> ();
				App.dbHandler.db.DeleteAll<Status> ();
				//App.dbHandler.db.DeleteAll<MobUser> ();

				if (result.RegistrationTypes != null) {
					foreach (rsRegistrationType rt in result.RegistrationTypes) {
						RegistrationType regType = new RegistrationType () { ID = rt.ID, Name = rt.Name };
						switch (rt.ID) {
						case RegistrationTypes.Text:
							regType.icon = "text.png";
							break;
						case RegistrationTypes.Picture:
							regType.icon = "picture.png";
							break;
						case RegistrationTypes.SingleChoice:
							regType.icon = "single.png";
							break;
						default:
							regType.icon = "questionmark.png";
							break;

						}
						App.dbHandler.db.Insert (regType);
					}
				}

				if (result.Status != null) {
					foreach (rsStatus st in result.Status) {
						Status status = new Status () {
							ID = st.ID,
							Text = st.Text,
							R = st.R,
							G = st.G,
							B = st.B,
							doStopTime = st.doStopTime,
							doStartTime = st.doStartTime
						};
						switch (st.ID) {
						case StatusTypes.Planned:
							status.iconBlack = "planned.png";
							break;
						case StatusTypes.Accepted:
							status.iconBlack = "accepted.png";
							break;
						case StatusTypes.DriveTo:
							status.iconBlack = "driveto.png";
							break;
						case StatusTypes.Started:
							status.iconBlack = "started.png";
							break;
						case StatusTypes.Pauzed:
							status.iconBlack = "pauzed.png";
							break;
						case StatusTypes.Stopped:
							status.iconBlack = "stopped.png";
							break;
						case StatusTypes.DriveFrom:
							status.iconBlack = "drivefrom.png";
							break;
						case StatusTypes.FinishedOK:
							status.iconBlack = "ok.png";
							break;
						case StatusTypes.FinishedNOK:
							status.iconBlack = "nok.png";
							break;
						default:
							status.iconBlack = "questionmark,png";
							break;
						}
						App.dbHandler.db.Insert (status);
					} //end foreach
				} // end if null

				ShowActivityUI (false);

			}

		}

   	}
}
