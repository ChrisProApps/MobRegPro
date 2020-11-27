using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegServices.Data;

namespace RegServices.Data
{
    public class RestLoadResult : StatusResult
    {
        public List<rsStatus> Status { get; set; }
        public List<rsUser> Users { get; set; }
        public List<rsRegistrationType> RegistrationTypes { get; set; }
        public RestLoadResult() { }
    }
}
