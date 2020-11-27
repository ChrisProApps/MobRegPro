using System;

namespace RegServices
{
	public class rsPlanningValidation
	{
		public Guid PlanningID { get; set; }
		public int StatusID { get; set; }

		// Is set on output, not used on input
		// 0 = OK, 1 = planningID not found, 2 = statusID server different then on client
		public int StatusCode { get; set; }

		public rsPlanningValidation()
		{
			StatusCode = 0;
		}
	}
}

