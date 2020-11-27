using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsArticleGroup
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public long SysRevision { get; set; }
        public bool SysDeleted { get; set; }

        public rsArticleGroup() { }
    }
}
