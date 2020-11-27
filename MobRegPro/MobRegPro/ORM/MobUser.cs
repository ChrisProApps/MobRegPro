using System;
using SQLite;

namespace MobRegPro.ORM
{
		[Table("MobUser")]
		public class MobUser
		{
			[PrimaryKey, MaxLength(48)]
			public string ID { get; set; }
			[Ignore]
			public Guid IDguid { get { return Guid.Parse(ID); } }
			[MaxLength(50)]
			public string LoginName { get; set; }
			[MaxLength(50)]
			public string Password { get; set; }
			[MaxLength(128)]
			public string FriendlyName { get; set; }
			public bool IsLockedOut { get; set; }
			[MaxLength(16)]
			public string LanguageID { get; set; }
		}
}


