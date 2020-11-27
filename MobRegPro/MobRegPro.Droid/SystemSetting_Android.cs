using System;
using MobRegPro.Droid;
using MobRegPro;
using Android.Provider;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(SystemSetting_Android))]
namespace MobRegPro.Droid
{
	public class SystemSetting_Android : ISystemSettings
	{
			public string DeviceId
			{
				get
				{
                return Settings.Secure.GetString(Android.App.Application.Context.ContentResolver,
						Settings.Secure.AndroidId);
				}
			}

			public string PackageName
			{
				get
				{
                return Android.App.Application.Context.PackageName;
				}
			}


			public string ApplicationName 
			{
				get 
				{
                var lApplicationInfo = Android.App.Application.Context.PackageManager.GetApplicationInfo (PackageName, 0);
                return (string)(lApplicationInfo != null ? Android.App.Application.Context.PackageManager.GetApplicationLabel (lApplicationInfo) : "Unknown");
				}
			}


			public string AppVersionName
			{
				get
				{
                var context = Android.App.Application.Context; 
					return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
				}
			}

			public int AppVersionCode
			{
				get
				{
                var context = Android.App.Application.Context; 
					return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
				}
			}

			public double DeviceScreenWidth
			{
				get
				{
                var displayMetrics = Android.App.Application.Context.Resources.DisplayMetrics;
					return displayMetrics.WidthPixels / displayMetrics.Density;
				}
			}
			public double DeviceScreenHeight
			{
				get
				{
                var displayMetrics = Android.App.Application.Context.Resources.DisplayMetrics;
					return displayMetrics.HeightPixels / displayMetrics.Density;
				}
			}
				

	}
}

