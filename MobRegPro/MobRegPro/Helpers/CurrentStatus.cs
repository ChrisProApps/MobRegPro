using System;

namespace MobRegPro
{
	public class CurrentStatus
	{
		public int ID { get; set;}
		public DateTime TimeStamp { get; set;}
		public bool IsValid { get; set;}

		public CurrentStatus ()
		{
			TimeStamp = DateTime.Now;
			IsValid = false;
		}
	}
}

