using RegServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RegServices.Data
{
    public class LoginResult : BaseResult
    {
        public Guid userID { get; set; }
        public string carID { get; set; }
        public long historyID { get; set; }
        public DateTime serverTime { get; set; }

        public LoginResult()
        {
        }
    }
}
