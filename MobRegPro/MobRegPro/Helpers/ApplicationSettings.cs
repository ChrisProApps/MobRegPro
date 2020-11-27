using System;
using MobRegPro.Helpers;
using Xamarin.Forms;

namespace MobRegPro
{
	public class ApplicationSettings
	{
		public bool defaultInUse { get; set;} = false;
		public string defaultUserName { get; set; } = "";
		public string defaultCarID { get; set;} = "";
		public string defaultPassword {get;set;} ="";

		public LoginVars loginVars { get; set;}
		public double maxHeigth { get; set;}
		public int installationID { get; set;} = 0;
		public string languageID{ get; set;} = "en-US";
		public string deviceID{ get; set;}
		public string clientID{ get; set;} = "";
		public string serverID{ get; set;} = "";
		public string serviceURL{ get; set;} = "";
		public string imageURL{ get; set;} = "";
		public bool isDemo{ get; set;} = true;
	
		public ApplicationSettings ()
		{
			// Set defaults
			//
			loginVars = new LoginVars ();
			ISystemSettings system = DependencyService.Get < ISystemSettings> ();
			deviceID = system.DeviceId;
		}
	}
}

