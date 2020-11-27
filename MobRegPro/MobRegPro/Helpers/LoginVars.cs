using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobRegPro.Helpers
{
    public class LoginVars
    {
        public long loginHistory;
        public Guid userID;
        public string userName;
        public string passWord;
        public string carID;

        public LoginVars()
        {
            loginHistory = -1;
            userID = Guid.Empty;
        }
    }
}
