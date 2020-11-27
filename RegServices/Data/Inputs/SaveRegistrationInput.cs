using System;
using System.Collections.Generic;

namespace RegServices.Data
{
	public class SaveRegistrationInput
	{
		public string userIDin {get;set;}
		public int installationID {get;set;} 
		public List<rsRegistration> registrations;
		public List<rsArticleReg> articles;

		public SaveRegistrationInput()
		{
		}
	}
}

