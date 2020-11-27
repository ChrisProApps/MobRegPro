using System;
using SQLite;

namespace MobRegPro.ORM
{
	[Table("RegistrationType")]
	public class RegistrationType
	{
		[PrimaryKey]
		public int ID { get; set; }
		[MaxLength(64)]
		public string Name { get; set; }
		// added for displaying status icons
		//
		[MaxLength(64)]
		public string icon {get;set;}
	}
}

