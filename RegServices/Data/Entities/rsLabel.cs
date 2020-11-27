using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsLabel
    {
        public string lbForm { get; set; }
        public string lbControl { get; set; }
        public string lbLangID { get; set; }
        public string lbText { get; set; }
        public long SysRevision { get; set; }
        public bool SysDeleted { get; set; }

        public rsLabel() { }
    }
}
