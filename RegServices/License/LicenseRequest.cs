﻿using System;

namespace RegServices
{
	public class LicenseRequest
	{
		//Method name : 'Validate', 'Register'
		public string Method { get; set; }
		//License key unique per client, is defined by ProApps
		public string ClientID { get; set; }
		//Device ID generated by client
		public string DeviceID { get; set; }
		//Unique ID generated by server after each call 
		public string ServerID { get; set; }
		//"en-US" style input
		public string languageID { get; set; }
		//Email address for registration only, else can be empty
		public string Email { get; set; }

		public LicenseRequest()
		{ }
	}
}

