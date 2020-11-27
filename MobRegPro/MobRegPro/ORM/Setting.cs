using System;
using SQLite;

namespace MobRegPro
{
	[Table("Setting")]
	public class Setting
	{
		[PrimaryKey,MaxLength(128)]
		public string ID { get; set;}
		[MaxLength(4096)]
		public string Content {get;set;}
		public bool IsActiv { get; set;}


		public Setting ()
		{
			IsActiv = false;
			Content = string.Empty;
		}
	}
}

