using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MobRegPro.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class SignaturePage : ContentPage
	{
		private Summary summary;
		public static bool IsStillrunning = false;
		public bool IsReturningFromSummary = false;

		public SignaturePage (Summary summary )
		{
			InitializeComponent ();
			this.summary = summary;
			//padView.CaptionTextColor = padView.BackgroundColor;
			ToolbarItem summaryItem = new ToolbarItem () { Text = AppResources.itemSummary };
			summaryItem.Clicked += SummaryItem_Clicked;
			ToolbarItems.Add (summaryItem);
			IsReturningFromSummary = false;
			// TO TEST on TC55 and iOS device when using scrollview signature control isnt working properly
			//
			ISystemSettings systemSettings = DependencyService.Get<ISystemSettings>();
			if(this.Content.Height > systemSettings.DeviceScreenHeight || Device.RuntimePlatform == Device.iOS)
				this.Content = slMain; //skip scrollview

			signaturePad.PromptText = "Signature here";
			signaturePad.PromptTextColor = Color.Black;
			signaturePad.ClearText = "Clear";
			signaturePad.ClearTextColor = Color.Red;
			signaturePad.SignatureLineColor = Color.Blue;
			signaturePad.SignatureLineWidth = 3.0;



		}

		async void SummaryItem_Clicked (object sender, EventArgs e)
		{
			IsReturningFromSummary = true;
			await App.Current.MainPage.Navigation.PushAsync (new InfoPage (summary));
		}

		//		protected override bool OnBackButtonPressed()
		//		{
		//			return base.OnBackButtonPressed ();
		//		}

		async protected override void OnAppearing()
		{
			base.OnAppearing();

			if (!IsReturningFromSummary)
			{
				SignaturePage.IsStillrunning = true;
				if (!string.IsNullOrWhiteSpace(summary.Planning.SignaturePoints))
				{
					await Task.Factory.StartNew(() =>
					{
						try
						{
							signaturePad.Strokes = JsonConvert.DeserializeObject<IEnumerable<IEnumerable<Point>>>(summary.Planning.SignaturePoints);
						}
						catch (Exception ex) { }
					}    
			
					);
				}
				tbName.Text = string.IsNullOrEmpty(summary.Planning.SignatureName) ? summary.Planning.ContactName ?? "" : summary.Planning.SignatureName;
				tbMail.Text = summary.Planning.Email ?? "";
			}
			else IsReturningFromSummary = false;

		}

		async protected override void OnDisappearing ()
		{
			if (!IsReturningFromSummary)
			{
				string jsonSignature = signaturePad.IsBlank ? "" : JsonConvert.SerializeObject(signaturePad.Strokes);
				if (jsonSignature.Length > 2048000)
					await DisplayAlert(AppResources.Info, AppResources.SignatureTooLong, AppResources.OK);
				else
				{
					// Save DB
					summary.Planning.SignaturePoints = jsonSignature;
					summary.Planning.SignatureName = tbName.Text;
					summary.Planning.Email = tbMail.Text;
					App.dbHandler.db.Update(summary.Planning);
					// Save signature
					if (!signaturePad.IsBlank)
					{
						IFileSystem fs = DependencyService.Get<IFileSystem>();
						using (Stream file = fs.CreateFileStream(fs.GetDataPath($"sig{summary.Planning.ID}.png")))
						{
							Stream imgStream = await signaturePad.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png, true, true);
							imgStream.Seek(0L, SeekOrigin.Begin);
							await imgStream.CopyToAsync(file);
							await file.FlushAsync();

						}
					}

				}
			}
			base.OnDisappearing ();
			SignaturePage.IsStillrunning = false;
		}

        void signaturePad_Saved(System.Object sender, System.EventArgs e)
        {
        }
    }
}

