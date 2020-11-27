using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using MobRegPro.ORM;
using MobRegPro.Helpers;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class RegSummaryPage : ContentPage
	{
		private Planning planning;
		private Registration editedReg;
		private List<Registration> registrations;

		public RegSummaryPage (Planning planning)
		{
			InitializeComponent ();
			this.planning = planning;
			this.Title = string.Format ("{0}-{1}", planning.Reference, planning.Customer);
			lvRegistrations.ItemTapped += LvRegistrations_ItemTapped;

			registrations = App.dbHandler.db.Query<Registration> ("select * from Registration where PlanningID=? Order by Priority", planning.ID);
			editedReg = null;

			ToolbarItem addItem = new ToolbarItem () { Text = AppResources.itemAdd };
			addItem.Clicked += BtnAddRegistrationClicked;
			ToolbarItems.Add (addItem);

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					edResult.BackgroundColor = Color.FromHex("#e6e6e6");
					break;
				case Device.Android:
					edResult.BackgroundColor = Color.LightGray;//  Color.FromHex("#2c3e50");
					edResult.TextColor = Color.Black;
					break;
				case Device.UWP:
					edResult.BackgroundColor = Color.FromHex("#2c3e50");
					break;
			}
		}
		 
		void ShowRegistrations(List<Registration> registrations)
		{
			lvRegistrations.ItemsSource = null;
			foreach(Registration r in registrations)
			{
				r.RegColor = (string.IsNullOrEmpty (r.Result) && r.IsRequired) ? Color.Red : Color.Black;

				RegistrationType regType =  App.dbHandler.db.Query<RegistrationType> ("select * from RegistrationType where ID=?", r.RegTypeID).FirstOrDefault ();;
				string iconName;
				if (regType != null)
					iconName = regType.icon;
				else
					iconName = "questionmark.png";
				r.RegTypeIcon = Common.FormatImageName (iconName);//  Device.OS==TargetPlatform.iOS ? string.Format("Images/{0}", iconName) : iconName;
			}

			lvRegistrations.ItemsSource = registrations.Where(r => !r.IsDeleted).OrderBy(r=> r.Priority).ToList<Registration>();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			if (editedReg != null) {
				if (editedReg.IsClientReg && editedReg.IsDeleted) {
					App.dbHandler.db.Delete (editedReg);
				}
				if (editedReg.IsDeleted && editedReg.RegTypeID == RegistrationTypes.Picture)
					Common.DeletePictureFromRegistration  (editedReg); 
				editedReg = null;
			}

			//registrations = App.dbHandler.db.Query<Registration> ("select * from Registration where PlanningID=? Order by Priority", planning.ID);
			ShowRegistrations (registrations);
		}

		void LvRegistrations_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			editedReg = e.Item as Registration;
			App.Current.MainPage.Navigation.PushAsync (new RegEditPage (editedReg));
		}

		void BtnAddRegistrationClicked (object sender, EventArgs e)
		{
			if (!panelAdd.IsVisible) {
				enCaption.Text = "";
				edResult.Text = "";
				swPicture.IsToggled = false;
				swOnReport.IsToggled = true;
				var highestPriority = (from r in registrations
				                       where !r.IsDeleted
				                       orderby r.Priority
				                       select r.Priority).Last ();
				enPriority.Text = (highestPriority + 1).ToString ();
				AddUI (true);
			}
		}

		void AddUI (bool showAddInput)
		{
			lvRegistrations.IsVisible = !showAddInput;
			panelAdd.IsVisible = showAddInput;
		}

		void BtnAddConfirmClicked (object sender, EventArgs e)
		{
			if (enCaption.Text.Trim () == string.Empty) {
                this.DisplayAlert (AppResources.Error, AppResources.CaptionIsEmpty, AppResources.OK);
                return;
			}
			Registration reg = registrations [0];
			Registration newReg = new Registration () {
				ID = Guid.NewGuid ().ToString (),
				OrderID = reg.OrderID,
				UserID = reg.UserID,
				IsClientReg = true,
				IsChanged = true,
				IsDeleted = false,
				IsDisplayed = false,
				IsReadingOnly = false,
				Input = "",
				IsRequired = false,
				PlanningID = reg.PlanningID,
				Caption = enCaption.Text,
				Result = edResult.Text,
				RegTypeID = swPicture.IsToggled ? RegistrationTypes.Picture : RegistrationTypes.Text,
				IsOnReport = swOnReport.IsToggled,
				Priority = Convert.ToInt32(enPriority.Text),
				Date = DateTime.Now
			};
			registrations.Add (newReg);
			if(enPriority.IsFocused) enPriority.Unfocus ();
			if(enCaption.IsFocused) enCaption.Unfocus ();
			App.dbHandler.db.Insert (newReg);

			AddUI (false);
			ShowRegistrations (registrations);
		}

		void BtnAddCancelClicked (object sender, EventArgs e)
		{
			AddUI (false);
		}
	}
}

