using System;
using System.Collections.Generic;

using Xamarin.Forms;
using RegServices;
using MobRegPro.Helpers;
using System.Text;
using System.Reflection;
using MobRegPro.Resources;

namespace MobRegPro
{
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			InitializeComponent ();
		}

		async protected override void OnAppearing ()
		{
			base.OnAppearing ();
			int maxWidth = Convert.ToInt32 (this.Bounds.Width - 20.0);

			ISystemSettings system = DependencyService.Get<ISystemSettings> ();
			string applicationName = system.ApplicationName;
			string versionName = system.AppVersionName;

			lbRevsion.Text = $"{applicationName} Rev:{versionName}";
			await lbRevsion.RotateTo (360, 500, Easing.CubicInOut);

			MobRegService service = new MobRegService (ProgramVars.URL);
			ModuleResult result = await service.GetRevisionAsync ();

			await imgLogo.ScaleTo (0.5, 50, Easing.SpringIn);
			imgLogo.IsVisible = true;
			await imgLogo.ScaleTo (1, 500, Easing.SpringOut);
			await imgLogo.ScaleTo (0.5, 250, Easing.SpringIn);
			await imgLogo.ScaleTo (1, 500, Easing.SpringOut);

			if (result.statusCode == 0) {
				StringBuilder sb = new StringBuilder ();
				//sb.Append ("<html><body>");
				sb.Append("<html>");

				if (Device.RuntimePlatform == Device.iOS)
				{
					sb.Append("<head>");
					sb.Append("<style>");
					if (Device.Idiom == TargetIdiom.Tablet) sb.Append("table {font-size:30px;border-spacing: 10px;}");
					else sb.Append("table {font-size:50px; border-spacing: 10px;}");
					sb.Append("</style>");
					sb.Append("</head>");
				}
				sb.Append("<body>");

				sb.Append (string.Format ("<table align=\"center\" style=\"width:{0}px;\">", maxWidth*2/3));
				sb.AppendFormat ("<tr><td><b>{0}</b></td><td><b>{1}</b></td></tr>", AppResources.Service, AppResources.Revision);
				foreach (ModuleInfo mi in result.infoList) {
					sb.AppendFormat ("<tr><td>{0}</td><td>{1}</td></tr>", mi.name, mi.version);
				}
				sb.Append ("</body></html>");

				webView.Source = new HtmlWebViewSource(){Html= sb.ToString ()};
			}

		}
	}
}

