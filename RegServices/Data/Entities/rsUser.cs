using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsUser
    {
        public Guid ID { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string FriendlyName { get; set; }
        public string LanguageID { get; set; }
        public bool IsLockedOut { get; set; }
        public long SysRevision { get; set; }
        public bool SysDeleted { get; set; }

        public rsUser() { }
    }
}
