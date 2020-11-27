using System;
using SQLite;
using RegServices.Data;

namespace MobRegPro.ORM
{
	[Table("Resource")]
	public class Resource
	{
		[MaxLength(48),PrimaryKey]
		public string ID { get; set;}
		[MaxLength(16)]
		public string PlanningID {get;set;}
		[Ignore]
		public Guid PlanningIDGuid {get{ return Guid.Parse(PlanningID); }}
		[MaxLength(48)]
		public string UserID {get;set;}
		[Ignore]
		public Guid UserIDGuid {get{ return Guid.Parse(UserID); }}
		[MaxLength(128)]
		public string FriendlyName {get;set;}
		[MaxLength(48)]
		public string OwnPlanningID {get;set;}
		[Ignore]
		public Guid OwnPlanningIDGuid {get{ return Guid.Parse(OwnPlanningID); }}
		public bool IsDriver {get;set;}
		public bool IsSeparate { get; set;}
		public bool IsPresent {get;set;}
		public DateTime? StartDate {get;set;}
		public DateTime? EndDate {get;set;}
		//Required for displaying 
		[MaxLength(64)]
		public string DriverImage {get;set;}
		[MaxLength(64)]
		public string SeparateImage {get;set;}
		[Ignore]
		public string IsPresentText { get; set; }

		public rsResource TOrsResource()
		{
			rsResource result = new rsResource () {
				UserID = UserIDGuid,
				OwnPlanningID = OwnPlanningIDGuid,
				FriendlyName = FriendlyName,
				IsDriver = IsDriver,
				IsSeparate = IsSeparate,
				IsPresent = IsPresent,
				StartDate = StartDate,
				EndDate = EndDate
			};
			return result;
		}
	}
}

