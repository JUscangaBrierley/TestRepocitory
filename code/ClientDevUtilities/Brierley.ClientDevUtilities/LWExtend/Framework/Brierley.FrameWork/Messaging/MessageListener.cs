using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;

namespace Brierley.FrameWork.Messaging
{
	/// <summary>
	/// This is called by the main listener thread to listen to incoming messages and consume them.
	/// It starts its own threadpool. Each thread in the pool listens on the queue and whichever
	/// thread gets the message processes it completely.
	/// </summary>
	public abstract class MessageListener
	{
		private const string _className = "MessageListener";

        private static object _root = new object();
        private static Dictionary<string, object> _endpointMutex = new Dictionary<string, object>();

        protected static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        protected bool _haveStarted { get; set; }
        protected bool _keepRunning { get; set; }
        protected Thread[] _threads { get; set; }
        protected EndPointCfg _endpoinCfg { get; set; }
        protected IMessageSerializer _serializer { get; set; }
        protected ITransportProvider _messagingProvider { get; set; }
        protected IMessagingMonitor _monitor { get; set; }

		public MessageListener(EndPointCfg endpointCfg, string orgName, string envName, ITransportProvider messagingProvider, IMessagingMonitor monitor = null)
		{
			_endpoinCfg = endpointCfg;
			if (!string.IsNullOrEmpty(orgName) && !string.IsNullOrEmpty(envName))
			{
				LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
			}
			ConsumerCfg cfg = _endpoinCfg.MessageCfg.ConsumerCfg;
			_serializer = messagingProvider.Serializer;
			_messagingProvider = messagingProvider;
			_monitor = monitor;
		}

		//hack? (also see MsmqTransportProvider.ReceiveAndConsume()) Due to transaction handling and the lack of a good API for moving queue messages (must do a PInvoke), 
		//we have to lock other threads to prevent them from accessing a failed message. We have to return the failed message to the queue in order to rollback the transaction
		//(the transaction has to be terminated, otherwise the PInvoke to move the message fails) and during this time, the message can be picked up by other threads. This
		//function will return an object to be locked, exclusive to the endpoint configuration (endpoint specific, because we don't want to block threads that are processing
		//other message types, which would bring everything to a halt every time a message fails).
		//The longer term solution would be to get MSMQ to move the message to the errors subqueue under the transaction that we started, which may not be possible.
		public static object GetMutex(EndPointCfg cfg)
		{
			string key = cfg.Uri;
			if (!_endpointMutex.ContainsKey(key))
			{
				lock (_endpointMutex)
				{
					if (!_endpointMutex.ContainsKey(key))
					{
						_endpointMutex.Add(key, new object());
					}
				}
			}
			return _endpointMutex[key];
		}

		public virtual void Start()
		{
			const string methodName = "Start";
			if (_haveStarted)
			{
				return;
			}
			_keepRunning = true;
			_threads = new Thread[_endpoinCfg.NumberOfThreads];
			_logger.Debug(_className, methodName, string.Format("Starting transport on: {0}.  Configured number of threads: {1}.", _endpoinCfg.Name, _endpoinCfg.NumberOfThreads));

			for (var t = 0; t < _endpoinCfg.NumberOfThreads; t++)
			{
				var thread = new Thread(ConsumeMessages)
				{
					Name = string.Format("{0}-{1}", _endpoinCfg.Name, t + 1),
					IsBackground = true
				};
				_logger.Trace(_className, methodName, string.Format("Starting thread {0} for listening.", thread.Name));
				_threads[t] = thread;
				thread.Start();
			}
			_haveStarted = true;
		}

		public virtual void ShutDown()
		{
			const string methodName = "ShutDown";
			_logger.Trace(_className, methodName, "Shutting down listener threads for " + _endpoinCfg.Name);
			_keepRunning = false;
            _endpoinCfg.PollingTimeout = 0;
			WaitForProcessingToEnd();
			_endpoinCfg.ConsumerFactory.Dispose();
			_haveStarted = false;
			_logger.Trace(_className, methodName, string.Format("Listener threads shutdown for {0} completed.", _endpoinCfg.Name));
		}

        protected abstract void ConsumeMessages(object state);
		
		private void WaitForProcessingToEnd()
		{
			const string methodName = "WaitForProcessingToEnd";

			if (!_haveStarted)
			{
				return;
			}

			foreach (var thread in _threads)
			{
				_logger.Trace(_className, methodName, string.Format("Shutting down background thread {0} for endpoint listener {1}.", thread.Name, _endpoinCfg.Name));
				thread.Join();
				_logger.Trace(_className, methodName, string.Format("Shutdown of thread {0} for endpoint listener {1} complete.", thread.Name, _endpoinCfg.Name));
			}
		}
	}
}
