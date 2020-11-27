using System;
using SQLite;
using RegServices.Data;

namespace MobRegPro.ORM
{
	[Table("ArticleGroup")]
	public class ArticleGroup
	{
		[PrimaryKey,MaxLength(128)]
		public string ID { get; set;}
		[MaxLength(128)]
		public string Name { get; set; }
		public long SysRevision { get; set; }

		public ArticleGroup(){}

		public ArticleGroup(rsArticleGroup g)
		{
			ID = g.ID;
			Name = g.Name;
			SysRevision = g.SysRevision;
		}
	}
}


