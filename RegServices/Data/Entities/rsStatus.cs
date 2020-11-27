using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsStatus
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public bool doStopTime { get; set; }
        public bool doStartTime { get; set; }

        public rsStatus() { }
    }
}
