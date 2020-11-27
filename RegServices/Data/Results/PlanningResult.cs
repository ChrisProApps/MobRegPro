using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RegServices.Data;
using RegServices;

namespace RegServices.Data
{
    public class PlanningResult : StatusResult
    {
        public List<rsPlanning> Plannings { get; set; }

        public PlanningResult() { }

    }
}
