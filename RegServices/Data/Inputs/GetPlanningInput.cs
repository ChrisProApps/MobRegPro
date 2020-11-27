using System;
using System.Collections.Generic;
using System.Linq;


namespace RegServices.Data
{
    public class GetPlanningInput
    {
        public string userID { get; set; }
        public int installationID { get; set; }
        public string carID { get; set; }

        public GetPlanningInput() { }
    }
}