using System;
using RegServices.Data;

namespace RegServices
{
	public class RestResult : StatusResult
	{
		public string RestResponse { get; set; }

		public RestResult() : base()
		{
		}
	}


}

