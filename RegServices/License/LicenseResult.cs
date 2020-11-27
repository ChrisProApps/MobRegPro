using System;
using System.Collections.Generic;
using RegServices.Data;

namespace RegServices
{
	public class LicenseResult : BaseResult
	{
		public int InstallationID { get; set; }
		public string ServerID { get; set; }
		public List<string> URLs { get; set; }
		public string Comment { get; set; }
		public DateTime ExpirationDate { get; set; }
		public bool IsDemo { get; set; }

		public LicenseResult() {
		}
	}
}

