using System;
using System.Collections.Generic;


namespace RegServices.Data
{
    public class ClientRevisionInput
    {
        public int major { get; set; }
        public int minor { get; set; }
        public int subminor { get; set; }
        public int lower { get; set; }
        public string languageID { get; set; }

        public ClientRevisionInput() { }
    }
}