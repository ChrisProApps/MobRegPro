using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RegServices.Data;

namespace MobRegPro.ORM
{
    [Table("Planning")]
    public class Planning
    {
        [PrimaryKey, MaxLength(48)]
        public string ID { get; set; }
        [Ignore]
        public Guid IDguid { get { return Guid.Parse(ID); } }
		[MaxLength(48)]
		public string OrderID { get; set; }
		[Ignore]
		public Guid OrderIDguid { get { return Guid.Parse(OrderID); } }
		public int StatusID { get; set; }
		[MaxLength(32)]
		public string CarID { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		[MaxLength(128)]
		public string Project { get; set; }
		[MaxLength(128)]
		public string Alias { get; set; }
		[MaxLength(128)]
		public string OrderType { get; set; }
		[MaxLength(512)]
		public string Description { get; set; }
		[MaxLength(512)]
		public string Comment { get; set; }
		[MaxLength(64)]
		public string Reference { get; set; }
		[MaxLength(128)]
		public string Customer { get; set; }
		[MaxLength(128)]
		public string ContactName { get; set; }
		[MaxLength(64)]
		public string Phone { get; set; }
		[MaxLength(128)]
		public string Street { get; set; }
		[MaxLength(32)]
		public string HouseNr { get; set; }
		[MaxLength(32)]
		public string Zip { get; set; }
		[MaxLength(128)]
		public string City { get; set; }
		[MaxLength(128)]
		public string Country { get; set; }
		[MaxLength(48)]
		public string UserID { get; set; }
		[Ignore]
		public Guid UserIDguid { get { return Guid.Parse(UserID); } }
		public long PauzeTime { get; set; }
		public DateTime ExecStartDateTime { get; set; }
		public DateTime ExecEndDateTime { get; set; }
		[MaxLength(256)]
		public string Email { get; set; }
		[MaxLength(128)]
		public string SignatureName { get; set; }
		public byte[] Signature { get; set; }

		// Extra fields
		//

		//Required for status time
		public DateTime StatusDateTime {get;set;}
		//Required for displaying images
		[MaxLength(64)]
		public string StatusImage {get;set;}
		//Required for storing signature
		[MaxLength(2048000)]
		public string SignaturePoints { get; set; }

		//
		//
		public rsPlanning TOrsPlanning()
		{
			rsPlanning result = new rsPlanning () {
				ID = IDguid,
				OrderID = OrderIDguid,
				StatusID = StatusID,
				CarID = CarID,
				UserID = UserIDguid,
				StartDateTime = StartTime,
				EndDateTime = EndTime,
				PauzeTime =PauzeTime,
				ExecStartDateTime = ExecStartDateTime,
				ExecEndDateTime = ExecEndDateTime,
				// From Order
				Project = Project,
				Alias = Alias,
				OrderType = OrderType,
				Description = Description,
				Comment = Comment,
				Reference = Reference,
				Customer = Customer,
				ContactName = ContactName,
				Email = Email,
				Phone = Phone,
				Street = Street,
				HouseNr = HouseNr,
				Zip = Zip,
				City = City,
				Country = Country,
				// From Planning
				Resources = null,
				SignatureName = SignatureName,
				Signature = null
				
			};
			return result;
		}
    }
}
