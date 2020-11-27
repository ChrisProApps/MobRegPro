using System;
using SQLite;

namespace MobRegPro.ORM
{
	[Table("PlanningHistory")]
	public class PlanningHistory
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set;}
		[MaxLength(48)]
		public string PlanningID { get; set; }
		[Ignore]
		public Guid PlanningIDguid { get { return Guid.Parse(PlanningID); } }
		public int StatusID { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }

	}
}

