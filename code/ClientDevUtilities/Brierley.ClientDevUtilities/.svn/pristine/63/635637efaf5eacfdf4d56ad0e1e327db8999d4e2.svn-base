//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Common.Threading
{
    /// <summary>
    /// This is the interface for creating application specific thread objects for the thread pool.
    /// </summary>
    public interface ILWThreadPoolFactory
    {
        /// <summary>
        /// Create a nemd thread object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        LWThread CreateThread(string name);
    }

    /// <summary>
    /// This is the thread pool manager.  An instance of the thread pool contains threads of the 
    /// same type.  These threads can be searched and queried.
    /// </summary>
    public class LWThreadPoolManager
    {
        private const string _className = "LWThreadPoolManager";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private ILWThreadPoolFactory thrdFactory;
        private int requestThreshold;
        private List<LWThread> threadPool = new List<LWThread>();
        private bool initialized = false;
        private bool shuttingDown = false;
        private int sleepInterval = 2000; // 2000 milliseconds
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nthreads"></param>
        /// <param name="requestThreshold"></param>
        /// <param name="sleepInterval"></param>
        /// <param name="factory"></param>
        public LWThreadPoolManager(int nthreads, int requestThreshold, int sleepInterval, ILWThreadPoolFactory factory)
        {
            thrdFactory = factory;
            this.requestThreshold = requestThreshold;
            this.sleepInterval = sleepInterval;
            Initialize(nthreads);
        }

        private void Initialize(int nthreads)
        {
            if (!initialized)
            {
                for (int i = 0; i < nthreads; i++)
                {
                    string name = "LWThread - " + i.ToString();
                    LWThread thrd = thrdFactory.CreateThread(name);
                    threadPool.Add(thrd);
                    thrd.Start();
                }
                initialized = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private LWThread GetThreadFromPool()
        {
            LWThread thrd = null;
            LWThread leastBusy = null;
            lock (threadPool)
            {
                foreach (LWThread t in threadPool)
                {
                    if (t.RequestQueue.Size == 0)
                    {
                        thrd = t;
                        break;
                    }
                    else
                    {
                        if (t.RequestQueue.Size < requestThreshold)
                        {
                            if (leastBusy == null)
                            {
                                leastBusy = t;
                            }
                            else if (t.RequestQueue.Size < leastBusy.RequestQueue.Size)
                            {
                                leastBusy = t;
                            }
                        }
                    }
                }
                thrd = thrd != null ? thrd : leastBusy;
            }                
            return thrd;
        }

        /// <summary>
        /// This method first looks for a free thread.  If it does not find a free thread then
        /// it looks for the least busy thread that is still below the request threshold.
        /// </summary>
        /// <returns></returns>
        private LWThread GetThreadFromPool(int retries)
        {
            string methodName = "GetThreadFromPool";

            LWThread thrd = null;            
            int ntries = 0;
            while (true)
            {
                thrd = GetThreadFromPool();
                ntries++;
                if (thrd == null && ntries < retries)
                {
                    _logger.Debug(_className, methodName, string.Format("Thread Q full.  Waiting for {0} ms.  Number of tries made {1}", sleepInterval, retries));
                    System.Threading.Thread.Sleep(sleepInterval);
                }
                else
                {
                    break;
                }
            }
            return thrd;
        }

        /// <summary>
        /// This method first looks for threads that have the same property as the provided values.
        /// If it finds one, then it schedules the work for it.  if not, then it looks for a free
        /// thread and schedules that thread.  Otherwise it just finds a least busy.
        /// </summary>
        /// <param name="retries"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        private LWThread GetThreadFromPool(int retries,string propName,string propValue)
        {
            string methodName = "GetThreadFromPool";

            LWThread thrd = null;
            int ntries = 0;
            while (true)
            {
                lock (threadPool)
                {
                    foreach (LWThread t in threadPool)
                    {
                        if (t.Query(propName, propValue))
                        {
                            if (t.RequestQueue.Size < requestThreshold)
                            {
                                thrd = t;
                                break;
                            }
                        }
                    }
                }
                if (thrd == null)
                {
                    thrd = GetThreadFromPool();
                    if (thrd != null)
                    {
                        thrd.AddProperty(propName, propValue);
                    }
                }
                ntries++;
                if (thrd == null && ntries < retries)
                {
                    _logger.Debug(_className, methodName, string.Format("Thread Q full.  Waiting for {0} ms.  Number of tries made {1}", sleepInterval, retries));
                    System.Threading.Thread.Sleep(sleepInterval);
                }
                else
                {
                    break;
                }
            }
            return thrd;
        }        

        /// <summary>
        /// This method will pick a thread from the available pool and queue the request to it.
        /// </summary>
        /// <param name="retries"></param>
        /// <param name="request"></param>
        public void QueueRequestToThread(int retries, object request)
        {
            if (initialized && !shuttingDown)
            {
                LWThread thrd = GetThreadFromPool(retries);
                if (thrd != null)
                {
                    thrd.RequestQueue.Add(request);
                }
                else
                {
                    throw new LWException("Unable to allocate a thread for this request.") { ErrorCode = 3229 };
                }                
            }
        }

        /// <summary>
        /// This method queries all the busy threads first and tries to find one that matches the query
        /// parameters.  If it does then the request is queued to it.  
        /// If not then it picks one from the available pool, sets it property and queues work to it.
        /// </summary>
        /// <param name="retries"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <param name="request"></param>
        public void QueueRequestToThread(int retries,string propName, string propValue, object request)
        {
            if (initialized && !shuttingDown)
            {
                LWThread thrd = GetThreadFromPool(retries,propName, propValue);
                if (thrd != null)
                {
                    thrd.RequestQueue.Add(request);
                }
                else
                {
                    throw new LWException("No more threads left in the pool.  All threads are fully loaded.") { ErrorCode = 3229 };
                }                
            }
        }

        public int GetNumberOfThreads()
        {
            return threadPool.Count;
        }

        public LWThread GetThreadByIndex(int index)
        {
            return threadPool[index];
        }

        /// <summary>
        /// Shuts down all threads.
        /// </summary>
        /// <param name="wait">If true then waits for all threads to finish before returning.</param>
        public void ShutDown(bool wait)
        {
            shuttingDown = true;
            lock (threadPool)
            {
                foreach (LWThread thrd in threadPool)
                {
                    thrd.ShutDown();
                }
            }
            if (wait)
            {
                foreach (LWThread thrd in threadPool)
                {
                    thrd.WaitToFinish();
                }
            }
        }

    }
}
