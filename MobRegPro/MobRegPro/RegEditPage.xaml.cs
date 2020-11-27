using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MobRegPro.ORM;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class RegEditPage : ContentPage
	{
		private Registration registration;
		private bool isChanged = false;

		public RegEditPage (Registration registration)
		{
			InitializeComponent ();
			this.registration = registration;
			//this.registration.Result = this.registration.Result ?? string.Empty;

			BindingContext = this.registration;
			piInput.SelectedIndexChanged += piInput_SelectedIndexChanged;
			piInput.Unfocused += piInput_Unfocused;
			edResult.TextChanged += edResult_TextChanged;
			edResult.Completed += EdResult_Completed;

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
			imgPicture.HeightRequest = App.appSettings.maxHeigth;
				
		}

		void EdResult_Completed (object sender, EventArgs e)
		{
			if (isChanged) {
				BindingContext = null;
				BindingContext = registration;
			}
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			App.dbHandler.db.Update (registration);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing ();

			if (!registration.IsClientReg) {
				swIsReadOnly.IsEnabled = false;
				swOnReport.IsEnabled = false;
				enPriority.IsEnabled = false;
			}

			if (!registration.IsRequired && !registration.IsReadingOnly)
				BtnDelete.IsVisible = true;

			if(registration.RegTypeID == RegistrationTypes.SingleChoice)
			{
				if (!string.IsNullOrEmpty (registration.Result)) {
					lbChoice.Text = string.Format ("  {0}", registration.Result);
					lbChoice.IsVisible = true;
				}

				string[] answers = registration.Input.Split(new char[]{'|'});
				foreach(string s in answers)
					piInput.Items.Add(s);
				piInput.Title = registration.Caption;
				BtnInput.IsVisible = true;
			}
			if (registration.RegTypeID == RegistrationTypes.Text) 
			{
				edResult.IsVisible = true;
				edResult.IsEnabled = !registration.IsReadingOnly;
				//edResult.Focus();
			}
			if (registration.RegTypeID == RegistrationTypes.Picture) {
				ShowPicture ();
				edResult.IsEnabled = !registration.IsReadingOnly;
				edResult.IsVisible = true;
				BtnTakePicture.IsVisible = !registration.IsReadingOnly;
			}

			if (registration.IsReadingOnly) {
				swIsReadOnly.IsEnabled = false;
				swOnReport.IsEnabled = false;
				edResult.IsEnabled = false;
			}

		}

		void ShowPicture ()
		{
			IFileSystem fs = DependencyService.Get<IFileSystem> ();
			string path = fs.GetDataPath (registration.ID + ".jpg");
			if (fs.ExistsFile (path).statusCode == 0) {
				imgPicture.Source = FileImageSource.FromFile (path);
				imgPicture.IsVisible = true;
			} else {
				imgPicture.Source = null;
				imgPicture.IsVisible = false;
			}

		}

		void ShowChange ()
		{
			registration.Date = DateTime.Now;
			registration.IsChanged = true;
			isChanged = true;
			this.BindingContext = null;
			this.BindingContext = registration;
		}

		void edResult_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (!isChanged) {
				registration.Date = DateTime.Now;
				registration.IsChanged = true;
				isChanged = true;
			}
		}

		void piInput_Unfocused (object sender, FocusEventArgs e)
		{
			BtnInput.IsVisible = true;
			piInput.IsVisible = false;
		}

		void piInput_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (registration.RegTypeID == RegistrationTypes.SingleChoice) {
				registration.Result = piInput.Items [piInput.SelectedIndex];
				lbChoice.Text = string.Format ("  {0}", registration.Result);
				lbChoice.IsVisible = true;
				registration.Date = DateTime.Now;
			}

			if (!isChanged)
				ShowChange ();

		}

		void BtnInputClicked (object sender, EventArgs e)
		{
			BtnInput.IsVisible = false;
			piInput.IsVisible = true;
			piInput.Title = AppResources.piInput;
			piInput.Focus ();
		}

		void BtnDeleteClicked (object sender, EventArgs e)
		{
			registration.IsDeleted = true;
			App.Current.MainPage.Navigation.PopAsync (true);

		}

		async void BtnTakePictureClicked (object sender, EventArgs e)
		{
			Stream sourceStream = null;

            await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
                await DisplayAlert(AppResources.NoCameraTitle, AppResources.NoCameraText, AppResources.OK);
				return;
			}

            //Consider using properties : CompressionQuality, CustomPhotoSize to scale the picture
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Camera",
                Name = "test.jpg",
                DefaultCamera = CameraDevice.Rear,
                AllowCropping = true
			});
					

			try {
				if (file == null)
					throw new Exception (AppResources.CameraFailed);
				
				//Copy picture to main
				IFileSystem fs = DependencyService.Get<IFileSystem> ();
				string destination = fs.GetDataPath (registration.ID + ".jpg");
				string source = file.Path;
				sourceStream = file.GetStream ();

				//NEW FROM HERE....
				byte[] buffer = new byte[sourceStream.Length];
				await sourceStream.ReadAsync(buffer, 0, (int)sourceStream.Length);
                //float scale = Device.OS == TargetPlatform.Android ? 2 : 8;
                float scale = Device.RuntimePlatform == Device.Android ? 2 : 8;

				byte[]resisedBuffer = fs.ResizeImage(buffer, 0, 0, scale, 50);
				MemoryStream memStream = new MemoryStream(resisedBuffer);

				PAResult result = await fs.SaveStreamAsync (destination, memStream);

				memStream.Dispose();
				buffer = null;
				// ....TILL HERE
				//

				//OLD code
				//PAResult result = await fs.SaveStreamAsync (destination, sourceStream);

				fs.DeleteFilesWithExtention (source, ".jpg");
				///storage/emulated/0/Android/data/be.proapps.mobregpro/files/Pictures/Camera/test.jpg
				if (result.statusCode == 0) {
					ShowPicture ();
					ShowChange ();
					//await DisplayAlert ("File Location", file.Path, "OK");
				}
			} catch (Exception ex) {
                await DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);
			}
			// Clean-up resources
			//
			if (sourceStream != null) {
				sourceStream.Dispose ();
				sourceStream = null;
			}
			if (file != null) {
				file.Dispose ();
				file = null;
			}

		}

	}
}

