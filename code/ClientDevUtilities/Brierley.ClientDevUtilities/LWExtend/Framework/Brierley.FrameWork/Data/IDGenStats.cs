using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data
{
	public class IDGenStats
	{
		public long CurrentId { get; set; }
		public long LastId { get; set; }

		public IDGenStats()
		{
			CurrentId = -1;
			LastId = -1;
		}

		public bool hasEnoughIDs(int howMany)
		{
			if (CurrentId < 0) return false;
			if ((CurrentId + howMany - 1) > LastId) return false;
			return true;
		}
	}
}
