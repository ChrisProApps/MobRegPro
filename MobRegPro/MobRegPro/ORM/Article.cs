using System;
using SQLite;
using RegServices.Data;

namespace MobRegPro.ORM
{
	[Table("Article")]
	public class Article 
	{
		[PrimaryKey,MaxLength(128)]
		public string ID { get; set; }
		[MaxLength(128)]
		public string Name { get; set; }
		[MaxLength(128)]
		public string GroupID { get; set; }

		public long SysRevision { get; set; }

		public Article()
		{
		}

		public Article(rsArticle a)
		{
			ID = a.ID;
			Name = a.Name;
			GroupID = a.GroupID;
			SysRevision = a.SysRevision;
		}
	
	}
}


