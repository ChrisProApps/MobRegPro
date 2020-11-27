using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsRegistration
    {
        public Guid ID { get; set; }
        public Guid PlanningID { get; set; }
        public Guid OrderID { get; set; }
        public Guid? UserID { get; set; } //can be null
        public int RegTypeID { get; set; }
        public int Priority { get; set; }
        public bool IsDisplayed { get; set; }
        public bool IsOnReport { get; set; }
        public bool IsRequired { get; set; }
        public bool IsReadingOnly { get; set; }
        public string Caption { get; set; }
        public string Result { get; set; } //can be null
        public string PathName { get; set; } //can be null
        public string Input { get; set; } //can be null
        public DateTime? Date { get; set; } //can be null
        // Data management
        public bool IsClientReg { get; set; }
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }


        public rsRegistration()
        {
            IsChanged = false;
            IsDeleted = false;
            IsClientReg = false;
        }

    }
}
