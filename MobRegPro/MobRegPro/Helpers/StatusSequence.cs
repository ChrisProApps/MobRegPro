using System;
using MobRegPro.ORM;
using System.Collections.Generic;


namespace MobRegPro.Helpers
{
	public class StatusSequence
	{
		public Status current { get; set; }
		public List<Status> next { get; set; }

		public StatusSequence(int size)
		{
			if (size == 0) size = 10;
			next = new List<Status>(size);
		}

	}
}

