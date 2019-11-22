//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Threading
{
	public class LWHeartbeatManager
	{
		private const string _className = "LWHeartbeatManager";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private Thread _thrd = null;
		private bool _keepRunning = true;
		private int _pulseRate = 1;  // in minutes
		private IHeartbeatPublisher _publisher = null;
		private string _serviceName = string.Empty;

		public LWHeartbeatManager(string serviceName, int pulseRate, IHeartbeatPublisher publisher)
		{
			_serviceName = serviceName;
			_pulseRate = pulseRate;
			if (publisher == null)
			{
				// instantiate the default publisher.
				_publisher = new LWHeartbeatPublisher();
			}
			else
			{
				_publisher = publisher;
			}
			_publisher.ServiceName = serviceName;
		}

		private bool KeepRunning()
		{
			lock (this)
			{
				return _keepRunning;
			}
		}

		private void Run()
		{
			int pulseRate = _pulseRate * 60 * 1000;
			int elapsed = 0;
			int sleepInterval = 2000; // milliseconds
			while (true)
			{
				if (KeepRunning())
				{
					Thread.Sleep(sleepInterval);
					elapsed += sleepInterval;
					if (elapsed >= pulseRate)
					{
						_publisher.PublishPulse();
						elapsed = 0;
					}
				}
				else
				{
					break;
				}
			}
		}


		public void Start()
		{
			string method = "Start";

			if (_thrd != null)
			{
				// raise exception that the monitor is already running.
				_logger.Error(_className, method, string.Format("Heartbeat monitor for {0} is already running.", _serviceName));
			}
			else
			{
				_thrd = new Thread(this.Run);
				_thrd.Name = "Heartbeat monitor";
				_thrd.Start();
				_logger.Trace(_className, method, string.Format("Heartbeat monitor for {0} has been started.", _serviceName));
			}
		}

		public void Stop(bool wait)
		{
			string methodName = "Stop";
			lock (this)
			{
				_keepRunning = false;
			}
			_logger.Trace(_className, methodName, string.Format("Heartbeat monitor for {0} is being shutdown.", _serviceName));
			if (wait && _thrd != null && _thrd.IsAlive)
			{
				_thrd.Join();
				// unreachable
			}
		}

	}
}
