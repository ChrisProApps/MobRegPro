using System;

namespace MobRegPro
{
	public class PAResult{
		public string status {get;set;}
		public int statusCode { get; set;}
		public object result { get; set;}

		public PAResult()
		{
			result = null;
			statusCode = 0;
			status = "OK";
		}

	}

}

