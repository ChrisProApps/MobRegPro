using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MobRegPro.ORM;
using SQLite;
using ZXing;
using MobRegPro.Resources;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;

namespace MobRegPro
{
	public partial class ArtSummaryPage : ContentPage
	{
		private Planning planning;
		private List<ArticleReg> artRegs;
		private SQLiteConnection db;
		private Article article;
		private ZXingScannerPage scanPage;

        public ArtSummaryPage()
        {
            InitializeComponent();
        }

		public ArtSummaryPage(Planning planning)
		{
			InitializeComponent();
			this.planning = planning;

			ToolbarItem searchItem = new ToolbarItem() { Text = AppResources.itemSearch };
			this.ToolbarItems.Add(searchItem);
			searchItem.Clicked += BtnSearch_Clicked;
            tbArticleID.Completed += TbArticleID_Completed;
            tbQty.Completed += TbQty_Completed;
		
			article = null;
			ShowArticleName("", false);
		}

        private void TbQty_Completed(object sender, EventArgs e)
        {
			if (!string.IsNullOrEmpty(tbQty.Text)) BtnAdd_Clicked(sender, e);
        }

        private void TbArticleID_Completed(object sender, EventArgs e)
        {
			tbQty.Focus();
        }

        async void BtnScan_Clicked (object sender, EventArgs e)
		{
	
			scanPage = new ZXingScannerPage(MobileBarcodeScanningOptions.Default);
			scanPage.Title = this.Title;

			ZXingDefaultOverlay overlay = (scanPage.Overlay as ZXingDefaultOverlay);
			overlay.BottomText = AppResources.ScanBarcode;
			overlay.ShowFlashButton = true;
			scanPage.IsScanning = true;

			scanPage.OnScanResult += (result) =>
			{
				scanPage.IsScanning = false;

				//ZXing.BarcodeFormat barcodeFormat = result.BarcodeFormat;
				//string type = barcodeFormat.ToString();
				Device.BeginInvokeOnMainThread(() =>
				{
					//DisplayAlert("The Barcode type is : " + type, "The text is : " + result.Text, "OK");
					if (result != null)
					{
						tbArticleID.Text = result.Text;
						article = App.dbHandler.db.Find<Article>(tbArticleID.Text);
					}
					Navigation.PopAsync(); //will trigger on appearing

				});
			};
			await Navigation.PushAsync(scanPage);
		}

		void TbArticleID_Unfocused (object sender, FocusEventArgs e)
		{
			var artFound = App.dbHandler.db.Find<Article> (tbArticleID.Text);
			if (artFound != null)
				ShowArticleName (artFound.Name, true);
			else
				ShowArticleName ("", false);
		}

		void BtnAdd_Clicked (object sender, EventArgs e)
		{
			var artFound = App.dbHandler.db.Find<Article> (tbArticleID.Text);

			if (string.IsNullOrEmpty (tbArticleID.Text) || artFound==null) {
                DisplayAlert (AppResources.Warning, AppResources.EnterValidArticleNr, AppResources.OK);
                return;
			}

			decimal qty = 0;
			Decimal.TryParse (tbQty.Text, out qty);

			if (string.IsNullOrEmpty (tbQty.Text) || qty <= 0) {
                DisplayAlert (AppResources.Warning, AppResources.EnterValidQty, AppResources.OK);
                return;
			}

			//artFound and qty are valid
			//
			var artRegFound = App.dbHandler.db.FindWithQuery<ArticleReg>("select * from ArticleReg where PlanningID=? and ArticleID=?", planning.ID, artFound.ID);
			if (artRegFound != null) {
				artRegFound.IsChanged = true;
				artRegFound.IsDeleted = false;
				artRegFound.Qty = qty;
				App.dbHandler.db.Update (artRegFound);
			} else {
				artRegFound = new ArticleReg () {
                    ID  = ArticleReg.CreateID(tbArticleID.Text, planning.ID), 
					ArticleID = tbArticleID.Text,
					PlanningID = planning.ID,
					OrderID = planning.OrderID,
					Qty = qty,
					IsChanged = true,
					IsDeleted = false
				};
				App.dbHandler.db.Insert (artRegFound);
			}
			tbQty.Text = tbArticleID.Text = string.Empty;
			article = null;
			ShowArticleName ("", false);
			ShowArticleRegs ();
			tbQty.Unfocus ();
			
		}

		void BtnSearch_Clicked (object sender, EventArgs e)
		{
			article = new Article ();
			App.Current.MainPage.Navigation.PushAsync (new ArtGroupPage (article), true);
		}

		void ShowArticleName (string name, bool isVisible)
		{
			lbArticleName.IsVisible = isVisible;
			inputGrid.RowDefinitions [1].Height = isVisible ? new GridLength(0,GridUnitType.Auto) : new GridLength (0, GridUnitType.Absolute);
			if (isVisible)
				lbArticleName.Text = name;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			ShowArticleRegs ();
			if (article != null) {
				tbArticleID.Text = article.ID;
				ShowArticleName (article.Name, true);
				if (!string.IsNullOrEmpty (article.ID))
					tbQty.Focus ();
				else
					tbQty.Unfocus ();
			} else
				tbQty.Unfocus ();

		}

		async protected void ItemTapped(object sender, ItemTappedEventArgs e)
		{
			ArticleReg ar = e.Item as ArticleReg;

            bool isOKToDelete = await DisplayAlert (AppResources.Question, AppResources.DeleteArticle + $" : {ar.ArticleID} ?", AppResources.Delete, AppResources.Cancel);
            if (isOKToDelete) {
				ar.IsDeleted = true;
				ar.IsChanged = true;
				App.dbHandler.db.Update (ar);
				ShowArticleRegs ();
			}
			lvRegs.SelectedItem = null;
				
		}

		protected void ShowArticleRegs()
		{
			db = App.dbHandler.db;
			artRegs = db.Query<ArticleReg> ("select * from ArticleReg where IsDeleted=0 and PlanningID=?", planning.ID);
			foreach (ArticleReg ar in artRegs) {
				Article article = db.Find<Article> (ar.ArticleID);
				ar.Name = article == null ? "???" : article.Name;
			}

			lvRegs.ItemsSource = artRegs;	
		}

	}
}

