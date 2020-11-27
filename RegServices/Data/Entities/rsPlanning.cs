using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsPlanning
    {
        // From Planning
        public Guid ID { get; set; }
        public Guid OrderID { get; set; }
        public int StatusID { get; set; }
        public string CarID { get; set; }
        public Guid UserID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public long PauzeTime { get; set; }
        public DateTime ExecStartDateTime { get; set; }
        public DateTime ExecEndDateTime { get; set; }
        // From Order
        public string Project { get; set; }
        public string Alias { get; set; }
        public string OrderType { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string Reference { get; set; }
        public string Customer { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string HouseNr { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        // From Planning
        public List<rsResource> Resources { get; set; }
        public string SignatureName { get; set; }
        public byte[] Signature { get; set; }

		public rsPlanning()
        {
            Resources = null;
			Signature = null;
        }
    }
}
