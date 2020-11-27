using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using MobRegPro.ORM;
using MobRegPro.Helpers;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class ResourcePage : ContentPage
	{
		private Planning planning;
		private int resourcesCount = 0;

		public ResourcePage (Planning planning)
		{
			InitializeComponent ();
			this.planning = planning;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			ShowResources ();
		}

		protected void ItemTapped( object sender, ItemTappedEventArgs e)
		{
			if (resourcesCount > 1) {
				Resource res = e.Item as Resource;
				res.IsPresent = !res.IsPresent;
				App.dbHandler.db.Update (res);
				ShowResources ();
			}
			lvResources.SelectedItem = null;
		}
		
		private void ShowResources()
		{
			var resources = (from r in App.dbHandler.db.Table<Resource> ()
				where r.PlanningID == planning.ID orderby r.IsSeparate orderby r.IsDriver descending
			                 select r).ToList<Resource> ();
			foreach (Resource r in resources) {
				
				r.DriverImage = r.IsDriver ? Common.FormatImageName ("truck.png") : r.DriverImage = Common.FormatImageName ("chair.png");
				r.SeparateImage = r.IsSeparate ? Common.FormatImageName ("singleuser.png") : Common.FormatImageName ("multiuser.png");
				r.IsPresentText = r.IsPresent ? AppResources.Present : AppResources.Absent;
			}
			lvResources.ItemsSource = resources;
			resourcesCount = resources.Count;
		}
	}
}

