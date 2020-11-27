using System;
using System.Collections.Generic;


namespace RegServices.Data
{
	public class UpdateStatusInput
	{
		public string userID {get;set;}
		public int installationID { get; set; }
		public string planningID { get; set; }
		public int newStatus { get; set; }
		public List<rsResource> resources { get; set; }

		public UpdateStatusInput() { }
	}
}

