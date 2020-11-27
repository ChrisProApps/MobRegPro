using System;
using System.Collections.Generic;
using System.Linq;


namespace RegServices.Data
{
    public class LoginUserInput
    {
        public int installationID {get;set;}
        public string userName { get; set; }
        public string password { get; set; }
        public string carID { get; set; }
        public string deviceLangID { get; set; }
        public long loginHistoryID { get; set; }
        public bool login { get; set; }

        public LoginUserInput() { }
    }
}