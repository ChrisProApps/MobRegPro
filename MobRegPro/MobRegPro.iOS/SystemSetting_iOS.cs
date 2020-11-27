using System;
using MobRegPro;
using Xamarin.Forms;
using MobRegPro.iOS;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(SystemSetting_iOS))]

namespace MobRegPro.iOS
{
	public class SystemSetting_iOS : ISystemSettings
	{
		public string DeviceId
		{
			get
			{
				return Guid.NewGuid().ToString();
			}
		}

		public string ApplicationName
		{
			get
			{
				NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleDisplayName"];
				return ver.ToString ();
			}
		}

		public string PackageName
		{
			get
			{
				return NSBundle.MainBundle.BundleIdentifier;
			}
		}


		public string AppVersionName
		{
			get
			{
				NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];//CFBundleVersion
				return ver.ToString ();
			}
		}

		public int AppVersionCode
		{
			get
			{
				NSObject ver = NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"];
//				string s = ver.ToString ();
//				int end = s.IndexOf (".");
//				string sub = s.Substring (0, end);
				return Convert.ToInt32 (ver.ToString());
					
			}
		}

		public double DeviceScreenWidth
		{
			get
			{
				return Convert.ToDouble(UIScreen.MainScreen.Bounds.Width);
			}
		}
		public double DeviceScreenHeight
		{
			get
			{
				return Convert.ToDouble(UIScreen.MainScreen.Bounds.Height);
			}
		}


	}
}


