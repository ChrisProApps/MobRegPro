using System;
using System.Collections.Generic;

namespace RegServices
{
	public class ValidatePlanningInput
	{
		public string userIDin {get;set;} 
		public int installationID {get;set;}
		public List<rsPlanningValidation> plannings { get; set; }

		public ValidatePlanningInput()
		{
			plannings = null;
		}
	}
}

