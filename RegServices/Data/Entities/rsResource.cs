using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsResource
    {
        public Guid UserID { get; set; }
        public Guid OwnPlanningID { get; set; }     //planning ID from planning that this resource was coupled to (on Client PlanningID is added to group this to the master planning(IsDriver|| !IsDrive && IsSeparate)
        public string FriendlyName { get; set; }
        public bool IsDriver { get; set; }
        public bool IsSeparate { get; set; }
         
        // on client
        public bool IsPresent { get; set; }         //used on client to indicate if resource is present (and status registration will be done)
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public rsResource()
        {
            IsDriver = false;
            IsPresent = true;
            StartDate = EndDate = null;
        }
    }
}
