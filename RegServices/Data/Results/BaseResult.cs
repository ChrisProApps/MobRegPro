using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegServices.Data
{
    public class BaseResult
    {
        public int statusCode { get; set; }
        public string status { get; set; }
        public int version { get; set; }
        public string language { get; set; }

        public BaseResult()
        {
            version = 1;
            statusCode = 0;
            status = "OK";
            language = "en-US";
        }
    }
}
