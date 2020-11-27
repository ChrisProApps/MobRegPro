using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceHandler;
using RegServices.Data;

namespace RegServices.Data
{
    public class RegResult : StatusResult
    {
        public List<rsRegistration> Registrations { get; set; }
        public List<rsArticleReg> Articles { get; set; }

        public RegResult()
        {
            Registrations = null;
            Articles = null;
        }
    }
}
