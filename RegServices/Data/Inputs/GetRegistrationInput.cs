using System;
using System.Collections.Generic;
using System.Linq;


namespace RegServices.Data
{
    public class GetRegistrationInput
    {
        public string userID { get; set; }
        public int installationID { get; set; }
        public string planningID { get; set; }

        public GetRegistrationInput() { }
    }
}