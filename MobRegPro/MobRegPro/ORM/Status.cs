using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobRegPro.ORM
{
    [Table("Status")]
    public class Status
    {
        [PrimaryKey]
        public int ID { get; set; }
        [MaxLength(64)]
        public string Text { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public bool doStopTime { get; set; }
        public bool doStartTime { get; set; }
		// added for displaying status icons
		//
		[MaxLength(64)]
		public string iconBlack {get;set;}
		[MaxLength(64)]
		public string iconwhite { get; set; }
    }
}
