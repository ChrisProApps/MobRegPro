using System;
using System.Collections.Generic;
using System.Linq;


namespace RegServices.Data
{
    public class ClientRegistrationInput
    {
        public string clientID {get;set;}
        public int installationID { get; set; }
        public string userID { get; set; }

        public ClientRegistrationInput() { }
    }
}