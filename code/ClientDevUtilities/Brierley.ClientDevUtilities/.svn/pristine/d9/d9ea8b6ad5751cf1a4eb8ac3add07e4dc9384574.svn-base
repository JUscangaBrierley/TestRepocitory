//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using System.Threading;

namespace Brierley.FrameWork.Common.Threading
{
	/// <summary>
	/// This is a wrapper around system thread.
	/// </summary>
	public abstract class LWThread : IDisposable
	{
		private const string _className = "LWThread";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		/// <summary>
		/// This flag determines if the thread needs to shut down or not.
		/// </summary>
		protected bool keepRunning = true;
		protected string name = "";
		private int sleepInterval = 2000;
		protected Thread thrd = null;
		private LWSyncQueue requestQ;
		private LWSyncQueue outputQ;
		private Hashtable properties = new Hashtable();
		private Timer timer = null;
		private bool disposed = false;

		/// <summary>
		/// This property provides back the Request Q for this thread.
		/// </summary>
		public LWSyncQueue RequestQueue
		{
			get { return requestQ; }
		}

		public LWSyncQueue OutputQueue
		{
			get { return outputQ; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		protected LWThread(string name)
		{
			this.name = name;
			requestQ = new LWSyncQueue();
			outputQ = new LWSyncQueue();
		}

		#region Thread Management
		/// <summary>
		/// This method starts the work for the thread.
		/// </summary>
		public virtual void Start()
		{
			if (thrd == null)
			{
				thrd = new Thread(this.Run); // use method group conversion 
				thrd.Name = name;
				thrd.Start(); // start the thread
			}
		}

		/// <summary>
		/// This method instructs the thread to shut down.
		/// </summary>
		public virtual void ShutDown()
		{
			string methodName = "ShutDown";

			keepRunning = false;
			_logger.Trace(_className, methodName, string.Format("Thread {0} is being shutdown.", Name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsAlive()
		{
			if (thrd == null)
			{
				return false;
			}
			else
			{
				return thrd.IsAlive;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void WaitToFinish()
		{
			string methodName = "WaitToFinish";
			if (thrd != null && thrd.IsAlive)
			{
				_logger.Debug(_className, methodName, string.Format("Beginning wait for thread {0} to finish", Name));
				thrd.Join();
				// this is unreachable.
				_logger.Debug(_className, methodName, string.Format("Thread {0} just finished.", Name));
			}
			else
			{
				_logger.Debug(_className, methodName, string.Format("Thread {0} is already finished.", Name));
			}
		}
		#endregion

		#region Property Management
		/// <summary>
		/// Add a property.  If the property already exists then changes its existign value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AddProperty(string name, string value)
		{
			if (properties.Contains(name))
			{
				properties[name] = value;
			}
			else
			{
				properties.Add(name, value);
			}
		}

		public string GetProperty(string name)
		{
			if (properties.Contains(name))
			{
				return (string)properties[name];
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Checks to see if the named property has this value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Query(string name, string value)
		{
			bool result = properties.Contains(name) && (string)properties[name] == value ? true : false;
			return result;
		}

		/// <summary>
		/// Clears all property values.
		/// </summary>
		public void ClearProperties()
		{
			properties.Clear();
		}
		#endregion

		#region Timer Methods
		public void RegisterCallBackTimer(int interval, object stateInfo)
		{
			if (timer == null)
			{
				TimerCallback timerDelegate = new TimerCallback(PrivateTimerCallBackFunction);
				timer = new Timer(timerDelegate, stateInfo, interval, interval);
			}
			else
			{
				timer.Change(interval, interval);
			}
		}

		public void UnregisterCallBackTimer()
		{
			if (timer != null)
			{
				timer.Dispose();
				timer = null;
			}
		}

		private void PrivateTimerCallBackFunction(object stateInfo)
		{
			UnregisterCallBackTimer();
			TimerCallBackFunction(stateInfo);
		}

		#endregion

		void Run()
		{
			while (true)
			{
				if (keepRunning || requestQ.Size > 0)
				{
					try
					{
						Object obj = requestQ.Remove(sleepInterval);
						if (obj != null)
						{
							// process the message.
							ProcessRequest(obj);
						}
						else
						{
							Cleanup();
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					break;
				}
			}
			Dispose();
		}

		/// <summary>
		/// Abstract function that is provided by the derived class.  This function does the actual work.
		/// </summary>
		/// <param name="workItem"></param>
		public abstract void ProcessRequest(object workItem);

		/// <summary>
		/// 
		/// </summary>
		public abstract void Cleanup();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateInfo"></param>
		public virtual void TimerCallBackFunction(object stateInfo)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			string methodName = "Dispose";
			_logger.Debug(_className, methodName, string.Format("Thread {0} is being disposed.", Name));
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					ChildDispose();
					if (timer != null)
					{
						timer.Dispose();
					}
				}
				disposed = true;
			}
		}

		/// <summary>
		/// Derived classes should override this method to do their cleanup.
		/// </summary>
		protected virtual void ChildDispose()
		{
		}
	}
}
