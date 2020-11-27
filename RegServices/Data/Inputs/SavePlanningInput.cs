using System;
using System.Collections.Generic;

namespace RegServices.Data
{
	public class SavePlanningInput
	{
		public string userIDin { get; set; }
		public int installationID { get; set; }
		public rsPlanning dataIn { get; set; }
		public bool overWriteExecEndDateTime { get; set; }
		public bool includeSignatureData { get; set; }
		public bool sendReport { get; set; }
		public List<rsResource> resources { get; set; }

		public SavePlanningInput() { }
	}
}

