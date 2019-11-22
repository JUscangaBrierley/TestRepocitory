using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Common.Logging
{
	public class NullCommunicationLogger : ICommunicationLogger
	{
		public void LogMessage(CommunicationLogData data)
		{
			//do nothing
		}

		public void LogMessage(CommunicationLogData data, out bool queuedForRetry)
		{
			//do nothing
			queuedForRetry = false;
		}
	}
}
