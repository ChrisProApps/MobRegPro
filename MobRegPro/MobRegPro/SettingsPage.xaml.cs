using MobRegPro.Helpers;
using MobRegPro.Resources;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MobRegPro
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			swDefaultInUse.On = App.appSettings.defaultInUse;
			tbDefaultUserName.Text = App.appSettings.defaultUserName;
			tbDefaultCarID.Text = App.appSettings.defaultCarID;
			for (int i=0; i < App.appSettings.defaultPassword.Length; i++)
				tbDefaultPassword.Text += "*";
		}

		async protected override void OnDisappearing()
		{
			App.appSettings.defaultInUse = swDefaultInUse.On;
			App.appSettings.defaultUserName = tbDefaultUserName.Text;
			App.appSettings.defaultCarID = tbDefaultCarID.Text;
			if (!tbDefaultPassword.Text.Contains ("*"))
				App.appSettings.defaultPassword = tbDefaultPassword.Text;
			await App.SaveAllPropertiesAsync ();

			base.OnDisappearing ();
		}

        async private void BtnClearTables_Clicked(object sender, EventArgs e)
        {
            try
            {
                Setting setting = Common.ReadApplicationSettingsFromDatabase();
                App.dbHandler.DeleteDatabase();
                App.dbHandler.CreateTables();
                App.dbHandler.db.Insert(setting);
                await DisplayAlert(AppResources.Info, AppResources.txtTablesCleared, AppResources.OK);
            }
            catch(Exception ex)
            {
                await DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);

            }
        }
    }
}

