using System;

namespace MobRegPro
{
	public interface ISystemSettings
	{
		string DeviceId { get; }
		string PackageName { get; }
		string ApplicationName { get; }
		string AppVersionName { get; }
		int AppVersionCode { get; }
		double DeviceScreenWidth { get;}
		double DeviceScreenHeight { get;}
	}
}

