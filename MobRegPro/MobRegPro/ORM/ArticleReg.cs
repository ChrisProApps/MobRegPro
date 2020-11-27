using System;
using SQLite;

namespace MobRegPro.ORM
{
	[Table("ArticleReg")]
	public class ArticleReg
	{
		[PrimaryKey,MaxLength(128)]
        public string ID { get; set; } //complex key = ArticleID|PlanningID
        [MaxLength(128)]
        public string ArticleID { get; set;}
		[MaxLength(48)]
		public string PlanningID { get; set; }
		[Ignore]
		public Guid PlanningIDguid { get { return Guid.Parse(PlanningID); } }
		[MaxLength(48)]
		public string OrderID { get; set; }
		[Ignore]
		public Guid OrderIDguid { get { return Guid.Parse(OrderID); } }
		public decimal Qty { get; set; }
		public decimal? PriceIn { get; set; }
		public decimal? PriceOut { get; set; }
		public bool IsChanged { get; set; }
		public bool IsDeleted { get; set; }
		// Referenced fields
		[Ignore]
		public string Name {get;set;}	//from Article
		[Ignore]
		public string Info {get { return string.Format("{0} ({1})", ArticleID, Qty.ToString("0.####")); } }

        public string CreateID()
        {
            return $"{ArticleID}|{PlanningID}";
        }
        public static string CreateID(string articleID, string planningID)
        {
            return $"{articleID}|{planningID}";
        }
    }
}

