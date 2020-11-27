using System;
using RegServices.Data;
using System.Collections.Generic;

namespace RegServices
{
	public class ValidatePlanningResult : StatusResult
	{
		public List<rsPlanningValidation> PlanningResults { get; set; }

		public ValidatePlanningResult() { }
	}
}

