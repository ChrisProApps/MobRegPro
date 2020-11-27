using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MobRegPro.ORM;
//
using RegServices;
using MobRegPro.Helpers;
using RegServices.Data;
using System.Threading.Tasks;
using MobRegPro.Resources;
using Xamarin.Essentials;

namespace MobRegPro
{
	public partial class StartPage : ContentPage
	{
		private bool hasAnimationRun = false;

		public StartPage()
		{
			InitializeComponent();
			BackgroundColor = Color.White;
			NavigationPage.SetHasNavigationBar(this, false);
		}

		async private void BtnLoginClicked(object sender, EventArgs e)
		{
			//0. Check if network available
			//
			if (Connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				await DisplayAlert(AppResources.Warning, AppResources.NoNetwork, AppResources.OK);
				return;
			}

			await SetWaitStatus(true);

			// 1. Check license
			//
			LicenseService service = new LicenseService(ProgramVars.LicURL);
			LicenseRequest request = new LicenseRequest()
			{
				Method = "Validate",
				ClientID = App.appSettings.clientID,
				DeviceID = App.appSettings.deviceID,
				languageID = App.appSettings.languageID,
				ServerID = App.appSettings.serverID
			};

			try
			{
				LicenseResult result = await service.ExecuteAsync(request);
				if (result.statusCode != 0)
				{
                    App.appSettings.serverID = result.ServerID;
					await App.SaveAllPropertiesAsync();
					throw new Exception(result.status);
				}
                // Check if license is about to expire
                //
                int nrDaysValid = result.ExpirationDate.Subtract(DateTime.Now).Days;
                if (nrDaysValid < 30)
                    await DisplayAlert(AppResources.Warning, $"License is about to expire in {nrDaysValid} days. Renew license !!!", AppResources.OK);
                //

                App.appSettings.serverID = result.ServerID;
				App.appSettings.installationID = result.InstallationID;
				App.appSettings.isDemo = result.IsDemo;
				App.appSettings.serviceURL = result.URLs[0];
				App.appSettings.imageURL = result.URLs[2];

				ProgramVars.URL = App.appSettings.serviceURL;
				ProgramVars.ImageHandlerUrl = App.appSettings.imageURL;

				//await App.SaveAllPropertiesAsync();
				await SetWaitStatus(false);

			}
			catch (Exception ex)
			{
				await SetWaitStatus(false);
				await DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);
				//lbDemo.IsVisible = true;
				//lbDemo.Text = $"Error:{ex.Message}";
				return;
			}

			// 2. Goto login page
			//
			await App.Current.MainPage.Navigation.PushAsync(new LoginPage(), true);
		}


		private void BtnAboutClicked(object sender, EventArgs e)
		{
			App.Current.MainPage.Navigation.PushAsync(new AboutPage(), true);
		}


		async private void BtnSettingsClicked(object sender, EventArgs e)
		{
			await App.Current.MainPage.Navigation.PushAsync(new SettingsPage(), true);
		}

		async private void BtnActivateClicked(object sender, EventArgs e)
		{
			await App.Current.MainPage.Navigation.PushAsync(new ActivationPage(), true);
		}



		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (string.IsNullOrEmpty(App.appSettings.clientID) || App.appSettings.installationID == 0)
			{
				// doing a PushAsync in an OnAppearing event will block calling this event again after the called paged is popped
				// WORK AROUND : use of BtnActivate
				//await App.Current.MainPage.Navigation.PushAsync(new ActivationPage (), true);
				BtnActivate.IsVisible = true;
				return;
			}
			BtnActivate.IsVisible = false;

			lbDemo.IsVisible = App.appSettings.isDemo;
			if (App.appSettings.isDemo)
				lbDemo.Text = AppResources.DemoLicenseActivated;//"Demo license activated";

			// Show the buttons only if licensing is OK
			if (!hasAnimationRun)
			{
				hasAnimationRun = true;
				foreach (var view in LayoutButtons.Children)
				{
					await view.ScaleTo(0.5, 50, Easing.SpringIn);
					view.IsVisible = true;
					await view.ScaleTo(1, 250, Easing.SpringOut);
				}
			}

		}

		async Task SetWaitStatus(bool showWaitStatus)
		{
			if (showWaitStatus)
			{
				activityIndicator.IsRunning = true;
				statusPanel.IsVisible = true;

				await LayoutButtons.ScaleTo(0.0, 500, null);
				LayoutButtons.IsVisible = false;
			}
			else {
				statusPanel.IsVisible = false;
				activityIndicator.IsRunning = false;

				await LayoutButtons.ScaleTo(0.2, 50, Easing.SpringIn);
				LayoutButtons.IsVisible = true;
				await LayoutButtons.ScaleTo(1, 500, Easing.SpringOut);
			}

		}


	}
}

