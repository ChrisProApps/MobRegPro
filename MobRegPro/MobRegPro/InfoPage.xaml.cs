using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Text;
using MobRegPro.ORM;
using MobRegPro.Helpers;
using SQLite;
using System.Linq;
using MobRegPro.Resources;
using Xamarin.Essentials;

namespace MobRegPro
{
	public partial class InfoPage : ContentPage
	{
		private Summary summary;

		public InfoPage (Summary summary)
		{
			InitializeComponent ();

			this.summary = summary;
			//webView.Source = new HtmlWebViewSource ().Html

			switch (summary.SummaryType) {
			case SummaryTypes.Completed:
                    this.Title = AppResources.SummaryTitle;
                    break;

			case SummaryTypes.Display:
                    this.Title = AppResources.InstructionsTitle;
                    break;
			
			case SummaryTypes.Info:
                    this.Title = AppResources.InformationTitle;
                    break;
			}
			//if(App.dbHandler.db.Table<ArticleReg>().Count() == 0)
			//	AddArticles (); //TEST routine

		}

		protected override void OnAppearing()
		{
			base.OnAppearing ();
			webView.Source = BuildHtml ();
		}

//		void AddArticles()
//		{
//			var article = App.dbHandler.db.Get<Article> ("ART100");
//			ArticleReg ar = new ArticleReg () { ArticleID = article.ID, PlanningID = summary.Planning.ID, OrderID = summary.Planning.OrderID, Qty = 10,
//				IsChanged = true, IsDeleted = false
//			};
//			App.dbHandler.db.Insert (ar);
//		}
	
		private HtmlWebViewSource BuildHtml ()
		{
			HtmlWebViewSource htmlSource = new HtmlWebViewSource ();

			try {

				// Create HtmlWebViewSource and set base url(for local resources : images, css, ....
				//
				IFileSystem fs = DependencyService.Get<IFileSystem> ();
				string basePath = fs.GetLocalFolder () + "/";
				//if (Device.OS == TargetPlatform.Android)
                if(Device.RuntimePlatform == Device.Android)
					basePath = @"file:///" + basePath;
				htmlSource.BaseUrl = basePath;
				int maxWidth = Convert.ToInt32 (this.Bounds.Width - 20.0);
				if (Device.RuntimePlatform == Device.iOS)
				{
					var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
					maxWidth = Convert.ToInt32(mainDisplayInfo.Width) - 20;
				}

				// Set db again else db is cleared after power down/up, resulting in program crash
				//
				SQLiteConnection db = App.dbHandler.db;

				// Get data 
				//
				string dateTimeFormat = "dd:MMM:yyyy HH:mm";
				var resource = (from r in db.Table<Resource> ()
				               where r.UserID == summary.Planning.UserID
				               select r).FirstOrDefault ();
				string techName = (resource == null) ? "???" : resource.FriendlyName;
				List<Registration> registrations = null;

				StringBuilder sb = new StringBuilder ();
				//sb.Append ("<html><body>");

				sb.Append("<html>");

				if (Device.RuntimePlatform == Device.iOS)
				{
					sb.Append("<head>");
					sb.Append("<style>");
					if (Device.Idiom == TargetIdiom.Tablet)
					{
						sb.Append("body {font-size:75px;}");
						sb.Append("table {font-size:75px;border-spacing: 10px;}");
					}
					else
					{
						sb.Append("body {font-size:50px;}");
						sb.Append("table {font-size:50px; border-spacing: 10px;}");
					}
					sb.Append("</style>");
					sb.Append("</head>");
				}
				sb.Append("<body>");

				// Order Info
				//
				if (summary.SummaryType == SummaryTypes.Info || summary.SummaryType == SummaryTypes.Display) {
					sb.AppendFormat ("<p><font color=\"blue\"><u><b>{0}</b></u></font></br>", AppResources.OrderInfo);
					sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Customer, summary.Planning.Customer);
					if (summary.SummaryType == SummaryTypes.Info) {
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Car, summary.Planning.CarID);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Technician, techName);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Project, summary.Planning.Project);
					}
					sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Reference, summary.Planning.Reference);
					sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Alias, summary.Planning.Alias);
					sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.StartTime, summary.Planning.StartTime.ToString (dateTimeFormat));
					if (summary.SummaryType == SummaryTypes.Info) {
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.StopTime, summary.Planning.EndTime.ToString (dateTimeFormat));
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Status, Common.GetStatus (summary.Planning.StatusID).Text);
					}
					string wrappedLine = summary.Planning.Description.Replace ("\r", "</br>");
					sb.AppendFormat ("<b>{0}&nbsp:</b></br>{1}</br>", AppResources.Description, wrappedLine);
					wrappedLine = summary.Planning.Comment.Replace ("\r", "</br>");
					sb.AppendFormat ("<b>{0}&nbsp:</b></br>{1}", AppResources.Comments, wrappedLine);
					sb.Append ("</p>");

					if (summary.SummaryType == SummaryTypes.Info) {
						// Customer info
						//
						sb.AppendFormat ("<p><font color=\"blue\"><u><b>{0}</b></u></font></br>", AppResources.CustomerInfo);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Customer, summary.Planning.Customer);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Contact, summary.Planning.ContactName);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.PhoneNr, summary.Planning.Phone);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Street, summary.Planning.Street);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.HouseNr, summary.Planning.HouseNr);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.Zipcode, summary.Planning.Zip);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}</br>", AppResources.City, summary.Planning.City);
						sb.AppendFormat ("<b>{0}&nbsp:&nbsp</b>{1}", AppResources.Country, summary.Planning.Country);
						sb.Append ("</p>");

						//Resources info
						//
						var resources = (from r in App.dbHandler.db.Table<Resource> ()
						                where r.PlanningID == summary.Planning.ID orderby r.IsDriver descending
						                select r).ToList<Resource> ();
						if (resources.Count () > 0) {
							sb.AppendFormat ("<p><font color=\"blue\"><u><b>{0}</b></u></font></br>", AppResources.Technicians);
							sb.Append (string.Format ("<table style=\"width:{0}px;border:1px solid black\">", maxWidth));
							sb.AppendFormat ("<tr><td><b>{0}</b></td><td><b>{1}</b></td><td><b>{2}</b></td><td><b>{3}</b></td></tr>", AppResources.Name, AppResources.Driver, AppResources.Present, AppResources.InGroup);

							foreach (Resource r in resources) {
								sb.AppendFormat ("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", r.FriendlyName, r.IsDriver ? "x" : "-", r.IsPresent ? "x" : "-", r.IsSeparate ? "-" : "X");
							}
							sb.Append ("</table>");
							sb.Append ("</p>");
						}
						//Get registrations for Info
						//
						registrations = (from r in db.Table<Registration> ()
						                where r.PlanningID == summary.Planning.ID && !r.IsDeleted
						                orderby r.Priority
						                select r).ToList<Registration> ();
					}
				}
				if (summary.SummaryType == SummaryTypes.Display) {
					registrations = (from r in db.Table<Registration> ()
					                where
					                    r.PlanningID == summary.Planning.ID
					                    && !r.IsDeleted
					                    && r.IsDisplayed
					                orderby r.Priority
					                select r).ToList<Registration> ();
				}
				if (summary.SummaryType == SummaryTypes.Completed) {
					sb.AppendFormat ("<p><font color=\"blue\"><u><b>{0}</b></u></font></br>", AppResources.InstallationReport);
					int resCount = 0;
					//it's me in resources where PlanningID == OwnPlanningID
					var me = (from r in db.Table<Resource> ()
					         where r.OwnPlanningID == r.PlanningID && r.UserID == summary.Planning.UserID && summary.Planning.ID == r.PlanningID
					         select r).SingleOrDefault ();
					if (me.IsSeparate)
						resCount = 1; //if me is separate count is allways 1
				else
						resCount = (from r in db.Table<Resource> ()
						           where r.PlanningID == summary.Planning.ID && r.IsPresent
						           select r).Count ();
					var start = (from h in db.Table<PlanningHistory> ()
					            where h.PlanningID == summary.Planning.ID && h.StatusID == StatusTypes.Started
					            select h).FirstOrDefault ();
					//var end = (from h in data.PlanningHistory where h.PlanningID == planning.ID && h.StatusID == StatusID.Stopped select h).LastOrDefault();
					// no stop in History since history is updated after all is done and shown, then use stopTime
					string pauzeString;
					TimeSpan pauzeTimeSpan = new TimeSpan (summary.PauzeTime);
					pauzeString = pauzeTimeSpan.Days == 0 ?
					string.Format ("{0:00}:{1:00}:{2:00}", pauzeTimeSpan.Hours, pauzeTimeSpan.Minutes, pauzeTimeSpan.Seconds)
					:
					string.Format ("{0:d}.{1:00}:{2:00}:{3:00}", pauzeTimeSpan.Days, pauzeTimeSpan.Hours, pauzeTimeSpan.Minutes, pauzeTimeSpan.Seconds);
				
					sb.Append (string.Format ("<table style=\"width:{0:d}px\">", maxWidth));
					;
					sb.AppendFormat ("<tr><td><b>{0}</b></td><td>{1}</td></tr>", AppResources.Customer, summary.Planning.Customer);
					sb.AppendFormat ("<tr><td><b>{0}</b></td><td>{1}</td></tr>", AppResources.Start, start.StartTime.ToString ());
					sb.AppendFormat ("<tr><td><b>{0}</b></td><td>{1}</td></tr>", AppResources.Stop, summary.StopTime.ToString ());
					sb.AppendFormat ("<tr><td><b>{0}</b></td><td>{1}</td></tr>", AppResources.Pauze, pauzeString);
					sb.AppendFormat ("<tr><td><b>{0}</b></td><td>{1:d}</td></tr>", AppResources.TechCount, resCount);
					sb.Append ("</table>");
			
					registrations = (from r in db.Table<Registration> ()
					                where
					                    r.PlanningID == summary.Planning.ID
					                    && !r.IsDeleted
					                    && r.IsOnReport
					                orderby r.Priority
					                select r).ToList<Registration> ();
				}

				// Show Registrations
				//
				if(summary.SummaryType != SummaryTypes.Display)
					 sb.AppendFormat ("<p><font color=\"blue\"><u><b>{0}</b></u></font></br>", AppResources.RegistrationInfo);
				else sb.AppendFormat ("<p><font color=\"blue\"><u><b>{0}</b></u></font></br>", string.Format("!!! {0} !!!", AppResources.ReadInstructionsFirst));
				foreach (Registration r in registrations) {
					sb.AppendFormat (@"<p><b>{0}</b>", r.Caption);
					sb.AppendFormat (@"</br>{0}</br>", r.Result ?? "");
					if (r.RegTypeID == RegistrationTypes.Picture)
						sb.AppendFormat ("<img src=\"{0}.jpg\" style=\"max-width:{1}px;\" />", r.ID, maxWidth);
					sb.Append ("</p>");
				}
				var artregs = from a in db.Table<ArticleReg> ()
				             where a.PlanningID == summary.Planning.ID && !a.IsDeleted
				             orderby a.ArticleID
				             select a;
				if (artregs.Count () > 0) {
					sb.AppendFormat ("<font color=\"blue\"><p></br><u><b>{0}</b></u></font>", AppResources.ArticleUsage);
					sb.Append (string.Format ("<table style=\"width:{0}px;border:1px solid black\">", maxWidth));
					sb.AppendFormat ("<tr><td><b>{0}</b></td><td><b>{1}</b></td><td><b>{2}</b></td></tr>", AppResources.ArticleNr, AppResources.Quantity, AppResources.Description);
					foreach (ArticleReg a in artregs) {
						Article article = db.Find<Article> (a.ArticleID);
						string artName = article == null ? "???" : article.Name;
						sb.AppendFormat ("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", a.ArticleID, a.Qty.ToString ("0.##"), artName);
					}
					sb.Append ("</table>");
					sb.Append ("</p>");
				}

				sb.Append ("</br></body></html>");
				htmlSource.Html = sb.ToString ();
			} catch (Exception ex) {
				DisplayAlert (AppResources.Error, ex.Message, AppResources.OK);
			}
			return htmlSource;

		} //end method

	}//end class
}//end namespace

