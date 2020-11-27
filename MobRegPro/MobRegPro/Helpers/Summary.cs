using System;
using MobRegPro.ORM;

namespace MobRegPro.Helpers
{
	public enum SummaryTypes {Display, Completed, Info};

	public class Summary
	{
		public Planning Planning { get; set; }
		public DateTime StopTime { get; set; }
		public long PauzeTime { get; set; }
		public SummaryTypes SummaryType { get; set; }

		public Summary()
		{
			SummaryType = SummaryTypes.Info;
			PauzeTime = 0L;
		}
	}
}

