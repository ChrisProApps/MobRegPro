using System;
using System.Collections.Generic;

using Xamarin.Forms;
using MobRegPro.ORM;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class ArticlePage : ContentPage
	{
		private Article article;

		public ArticlePage (Article article)
		{
			InitializeComponent();
			this.Title = AppResources.cpArticle; ;
			this.article = article;
			lvItems.ItemTapped += LvItems_ItemTapped;
		}

        async void LvItems_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			Article a = e.Item as Article;
			article.ID = a.ID;
			article.Name = a.Name;
		
			Page artGroupPage = null;

			foreach (Page p in App.Current.MainPage.Navigation.NavigationStack)
			{
				if (p.GetType () == typeof(ArtGroupPage)) {
					artGroupPage = p;
					break;
				}
			}
			App.Current.MainPage.Navigation.RemovePage (artGroupPage);
			await App.Current.MainPage.Navigation.PopAsync (true);
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			var articles = App.dbHandler.db.Query<Article>("select * from Article where GroupID=?", article.GroupID);
			lvItems.ItemsSource = articles;
			
		}
	}
}

