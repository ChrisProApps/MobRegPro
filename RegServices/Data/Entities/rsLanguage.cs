using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsLanguage
    {
        public string lgID { get; set; }
        public string lgText { get; set; }
        public long SysRevision { get; set; }
        public bool SysDeleted { get; set; }

        public rsLanguage() { }
    }
}
