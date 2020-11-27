using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RegServices.Data
{
    public class ClientRegResult : StatusResult
    {
        public Guid defaultUserID { get; set; }
        public int installationID { get; set; }

        public ClientRegResult()
        {
        }
    }
}
