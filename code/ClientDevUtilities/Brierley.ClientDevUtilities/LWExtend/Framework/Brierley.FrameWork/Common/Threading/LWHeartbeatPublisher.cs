//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Threading
{
	public class LWHeartbeatPublisher : IHeartbeatPublisher
	{
		#region Fields
		private const string _className = "LWHeartbeatPublisher";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string _serviceName = string.Empty;
		#endregion

		public string ServiceName
		{
			set { _serviceName = value; }
		}

		public virtual void PublishPulse()
		{
			string method = "PublishPulse";
			_logger.Trace(_className, method, _serviceName + " is alive.");
		}
	}
}
