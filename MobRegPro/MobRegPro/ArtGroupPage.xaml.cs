using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MobRegPro.ORM;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class ArtGroupPage : ContentPage
	{
		private Article article;

		public ArtGroupPage (Article article)
		{
			InitializeComponent ();
			this.Title = AppResources.cpArtGroup;
			this.article = article;
			lvItems.ItemTapped += LvItems_ItemTapped;
		}

		async void LvItems_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			ArticleGroup g = e.Item as ArticleGroup;
			article.GroupID = g.ID;
			await App.Current.MainPage.Navigation.PushAsync (new ArticlePage (article));
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var groups = App.dbHandler.db.Query<ArticleGroup> ("select * from ArticleGroup");
			lvItems.ItemsSource = groups;
		}
	}
}

