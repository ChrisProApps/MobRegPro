using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ZXing;
using RegServices;
using MobRegPro.Helpers;
using MobRegPro.Resources;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;

namespace MobRegPro
{
	public partial class ActivationPage : ContentPage
	{
		private ZXingScannerPage scanPage;

		public ActivationPage ()
		{
			InitializeComponent ();
			tbLicenseKey.TextChanged += (object sender, TextChangedEventArgs e) => {
				if(e.NewTextValue.Length >= 36) BtnActivate.IsVisible = true;
				else BtnActivate.IsVisible = false;
				if(e.NewTextValue.Length > 0) BtnMissing.IsVisible = false;
				else BtnMissing.IsVisible = true;
			};
		}

		async private void BtnScanClicked (object sender, EventArgs e)
		{
			scanPage = new ZXingScannerPage(MobileBarcodeScanningOptions.Default);
			scanPage.Title = this.Title;

			ZXingDefaultOverlay overlay = (scanPage.Overlay as ZXingDefaultOverlay);
			overlay.BottomText = AppResources.ScanLicenseCode;
			overlay.ShowFlashButton = true;
			scanPage.IsScanning = true;

			scanPage.OnScanResult += (result) =>
			{
				scanPage.IsScanning = false;

				//ZXing.BarcodeFormat barcodeFormat = result.BarcodeFormat;
				//string type = barcodeFormat.ToString();
				Device.BeginInvokeOnMainThread(() =>
				{
					Navigation.PopAsync();
					if (result?.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
						tbLicenseKey.Text = result.Text;

				});
			};
			await Navigation.PushAsync(scanPage);
			
		}

		async private void BtnActivateClicked (object sender, EventArgs e)
		{
			UpdateUI (true);
			LicenseService service = new LicenseService (ProgramVars.LicURL);
			LicenseRequest request = new LicenseRequest () {
				Method = "Register",
				ClientID = tbLicenseKey.Text,
				DeviceID = App.appSettings.deviceID,
				languageID = App.appSettings.languageID,
				ServerID = "",
				Email = ""
			};

			try {
				LicenseResult result = await service.ExecuteAsync (request);
				if(result.statusCode != 0) throw new Exception(result.status);
				App.appSettings.clientID = request.ClientID;
				App.appSettings.serverID = result.ServerID;
				App.appSettings.installationID = result.InstallationID;
				App.appSettings.isDemo = result.IsDemo;
				App.appSettings.serviceURL = result.URLs[0];
				App.appSettings.imageURL = result.URLs[2];

				ProgramVars.URL = App.appSettings.serviceURL;
				ProgramVars.ImageHandlerUrl = App.appSettings.imageURL;

				UpdateUI(false);
				await App.SaveAllPropertiesAsync();
				await App.Current.MainPage.Navigation.PopAsync(true);
			}
			catch(Exception ex) {
				UpdateUI (false);
				tbLicenseKey.Text = "";
				await DisplayAlert (AppResources.Error, ex.Message, AppResources.OK);
			}

		}

		private void BtnMissingClicked (object sender, EventArgs e)
		{
			licensePanel.IsVisible = false;
			 emailPanel.IsVisible = true;

		}

		async private void BtnEmailClicked (object sender, EventArgs e)
		{
			UpdateUI (true);
			LicenseService service = new LicenseService (ProgramVars.LicURL);
			LicenseRequest request = new LicenseRequest () {
				Method = "Request",
				ClientID = "",
				DeviceID = "",
				languageID = App.appSettings.languageID,
				ServerID = "",
				Email = tbEmail.Text
			};

			try {
				LicenseResult result = await service.ExecuteAsync (request);
				if(result.statusCode != 0) throw new Exception(result.status);

				tbEmail.Text = "";
				UpdateUI(false);
				licensePanel.IsVisible = true;
				emailPanel.IsVisible = false;

				await DisplayAlert (AppResources.Info, AppResources.LicenseRequested, AppResources.OK);
			}
			catch(Exception ex) {
				
				UpdateUI (false);
				await DisplayAlert (AppResources.Error, ex.Message, AppResources.OK);
			}

		}

		protected void UpdateUI(bool isBusy)
		{
			if (licensePanel.IsVisible) {
				activityIndicator.IsRunning = activityIndicator.IsVisible = isBusy;
				BtnScan.IsVisible = BtnActivate.IsVisible = !isBusy;
			}

			if (emailPanel.IsVisible) {
				activityIndicator2.IsRunning = activityIndicator2.IsVisible = isBusy;
				BtnEmail.IsVisible = !isBusy;
			}

		}

	}
}

