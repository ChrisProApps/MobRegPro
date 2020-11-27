using System;
using RegServices.Data;
using System.Collections.Generic;

namespace RegServices
{
	public class ModuleResult : BaseResult
	{
		public List<ModuleInfo> infoList;
		public ModuleResult() 
		{
			infoList = null;
		}

	}
}

