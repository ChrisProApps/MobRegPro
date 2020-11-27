using System;
using System.Collections.Generic;
using System.Linq;


namespace RegServices.Data
{
    public class LoadSyncInput
    {
        public string userID {get;set;}
        public int installationID {get;set;}
        public string loadSyncList {get;set;}
        public string parameters { get; set; }

        public LoadSyncInput() { }

    }
}