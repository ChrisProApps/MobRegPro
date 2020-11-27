using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using MobRegPro.ORM;
using MobRegPro.Helpers;
using MobRegPro.Resources;
namespace MobRegPro
{
    public partial class MainMenuPage : ContentPage
    {
		private Planning planning;
		 
        public MainMenuPage(Planning planning)
        {
            InitializeComponent();
			this.planning = planning;

			this.Title = string.Format ("{0} - {1}", planning.Reference, planning.Customer);
			this.Appearing += MainMenuPage_Appearing;

            if (Device.RuntimePlatform == Device.iOS)
            {
                this.BtnStatusChange.Text = $"{this.BtnStatusChange.Text,-32}";
                this.BtnInfo.Text = $"{this.BtnInfo.Text,-32}";
                this.BtnRegistrations.Text = $"{this.BtnRegistrations.Text,-32}";
                this.BtnArticles.Text = $"{this.BtnArticles.Text,-32}";
                this.BtnResources.Text = $"{this.BtnResources.Text,-32}";
            }

        }

		async private void MainMenuPage_Appearing(object sender, EventArgs e)
		{
			//bool done = false;
			Status status = Common.GetStatus (planning.StatusID);
			if (status.ID == StatusTypes.FinishedOK || status.ID == StatusTypes.FinishedNOK) {
				buttonsPanel.IsVisible = false;
				//if (Device.OS == TargetPlatform.Android) {
				if (Device.RuntimePlatform == Device.Android)
				{
					await Task.Run(() =>
					{
						do
						{
						} while (StatusChangePage.IsStillRunning);
					});
					await App.Current.MainPage.Navigation.PopAsync(true);
				}
				else await App.Current.MainPage.Navigation.PopAsync(true);
			}
			else {
				imgStatus.Source = Common.FormatImageName (status.iconBlack);
                lbStatus.Text = string.Format ("{0} : {1}", AppResources.lbStatus, status.Text);
				lbStatusTime.Text = planning.StatusDateTime.ToString ("dd-MMM-yyyy HH:mm:ss");
				panelStatus.BackgroundColor = Color.FromRgb (status.R, status.G, status.B);
				UpdateUI (status);
			}
		}
			
		private void BtnStatusChangeClicked(object sender, EventArgs e)
		{
			App.Current.MainPage.Navigation.PushAsync (new StatusChangePage (planning));
		}

		private void BtnRegistrationsClicked(object sender, EventArgs e)
		{ 
			App.Current.MainPage.Navigation.PushAsync (new RegSummaryPage (planning),true);
		}

		private void BtnInfoClicked(object sender, EventArgs e)
		{
			Summary summary = new Summary (){ Planning = planning, SummaryType=SummaryTypes.Info, StopTime = DateTime.Now, PauzeTime = 0 };
			App.Current.MainPage.Navigation.PushAsync (new InfoPage (summary),true);
		}

		private void BtnArticlesClicked(object sender, EventArgs e)
		{
			App.Current.MainPage.Navigation.PushAsync (new ArtSummaryPage (planning),true);
		}

		private void BtnResourcesClicked(object sender, EventArgs e)
		{
			App.Current.MainPage.Navigation.PushAsync (new ResourcePage (planning), true);
		}

		private void UpdateUI(Status status)
		{
			if (status.ID == StatusTypes.Started)
				BtnArticles.IsVisible = BtnRegistrations.IsVisible =  true;
			else
				BtnArticles.IsVisible = BtnRegistrations.IsVisible = false;
		}

    }
}
