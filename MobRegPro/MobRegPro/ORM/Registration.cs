using System;
using SQLite;
using Xamarin.Forms;

namespace MobRegPro.ORM
{
	[Table("Registration")]
	public class Registration
	{
		[PrimaryKey, MaxLength(48)]
		public string ID { get; set; }
		[Ignore]
		public Guid IDguid { get { return Guid.Parse(ID); } }
		[MaxLength(48)]
		public string PlanningID { get; set; }
		[Ignore]
		public Guid PlanningIDguid { get { return Guid.Parse(PlanningID); } }
		[MaxLength(48)]
		public string OrderID { get; set; }
		[Ignore]
		public Guid OrderIDguid { get { return Guid.Parse(OrderID); } }
		[MaxLength(48)]
		public string UserID { get; set; }
		[Ignore]
		public Guid UserIDguid { get { return Guid.Parse(UserID); } }
		public int RegTypeID { get; set;}
		public int Priority { get; set;}
		public bool IsDisplayed { get; set;}
		public bool IsOnReport { get; set;}
		public bool IsRequired { get; set;}
		public bool IsReadingOnly { get; set;}
		[MaxLength(256)]
		public string Caption { get; set; }
		[MaxLength(512)]
		public string Result { get; set; }
		[MaxLength(512)]
		public string PathName { get; set; }
		[MaxLength(512)]
		public string Input { get; set; }
		public DateTime? Date { get; set; }
		public bool IsChanged { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsClientReg { get; set; }
		//
		// for displaying
		[Ignore]
		public string RegTypeIcon {get;set;}
		[Ignore]
		public Color RegColor { get; set;}
	}
}

