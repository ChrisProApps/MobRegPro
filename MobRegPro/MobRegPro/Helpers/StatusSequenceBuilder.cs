using System;
using System.Collections.Generic;

namespace MobRegPro.Helpers
{
	public class StatusSequenceBuilder
	{
		public StatusSequenceBuilder()
		{
		}


		static public List<StatusSequence> Run(bool isDemoMode)
		{
			List<StatusSequence> statusSequences = new List<StatusSequence>(10);

			StatusSequence s;

			s = new StatusSequence(2);
			s.current = Common.GetStatus(StatusTypes.Planned);
			s.next.Add(Common.GetStatus(StatusTypes.Accepted));
			s.next.Add(Common.GetStatus(StatusTypes.DriveTo));
			statusSequences.Add(s);

			s = new StatusSequence(2);
			s.current = Common.GetStatus(StatusTypes.Accepted);
			s.next.Add(Common.GetStatus(StatusTypes.DriveTo));
			if(isDemoMode) s.next.Add(Common.GetStatus(StatusTypes.Planned));
			statusSequences.Add(s);

			s = new StatusSequence(2);
			s.current = Common.GetStatus(StatusTypes.DriveTo);
			s.next.Add(Common.GetStatus(StatusTypes.Started));
			//s.next.Add(Common.GetStatus(StatusTypes.Accepted));
			statusSequences.Add(s);

			s = new StatusSequence(3);
			s.current = Common.GetStatus(StatusTypes.Started);
			s.next.Add(Common.GetStatus(StatusTypes.Pauzed));
			s.next.Add(Common.GetStatus(StatusTypes.Stopped));
			if (isDemoMode) s.next.Add(Common.GetStatus(StatusTypes.Planned));
			statusSequences.Add(s);

			s = new StatusSequence(2);
			s.current = Common.GetStatus(StatusTypes.Pauzed);
			s.next.Add(Common.GetStatus(StatusTypes.Started));
			s.next.Add(Common.GetStatus(StatusTypes.Stopped));
			statusSequences.Add(s);

			s = new StatusSequence(3);
			s.current = Common.GetStatus(StatusTypes.Stopped);
			s.next.Add(Common.GetStatus(StatusTypes.DriveFrom));
			//s.next.Add(new Status(StatusTypes.FinishedOK));
			//s.next.Add(new Status(StatusTypes.FinishedNOK));
			statusSequences.Add(s);

			s = new StatusSequence(2);
			s.current = Common.GetStatus(StatusTypes.DriveFrom);
			s.next.Add(Common.GetStatus(StatusTypes.FinishedOK));
			s.next.Add(Common.GetStatus(StatusTypes.FinishedNOK));
			statusSequences.Add(s);

			return statusSequences;
		}
	}

}

