//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;

using System.Threading;

namespace Brierley.FrameWork.Common.Threading
{
    /// <summary>
    /// This class provides a synchronized Queue.  It is most useful to pass data back between 2 threads in a 
    /// synchronized manner.  The Queue implements a FIFO strategy.  Messages are retrieved the order in which 
    /// they are put.
    /// </summary>
    public class LWSyncQueue
    {
        private IList queue = new ArrayList();        

        /// <summary>
        /// 
        /// </summary>
        public LWSyncQueue()
        {            
        }

        /// <summary>
        /// Returns the number of messages waiting in the Queue to be retrieved.
        /// </summary>
        public int Size
        {
            get { return queue.Count; }
        }

        /// <summary>
        /// This method removes a message from the Queue.
        /// </summary>
        /// <returns></returns>
        private object RemoveMessage()
        {
            object msg = null;
            try
            {
                if (queue.Count <= 0)
                    return null;
                msg = queue[0];
                queue.Remove(msg);                
            }
            catch (Exception)
            {
                msg = null;                
            }
            return msg;
        }

        /// <summary>
        /// This method adds a new message and if any other threads are waiting for messages to arrive,
        /// then they are woken up.
        /// </summary>
        /// <param name="msg"></param>
        public void Add(object msg)
        {
            lock (queue)
            {                
                queue.Add(msg);
                Monitor.PulseAll(queue);                
            }
        }

        /// <summary>
        /// This method removes a msg object from the message queue.  In case the Queue is empty, a timeout 
        /// parameter specifies whether the call should block for a message to arrive or not.
        /// </summary>
        /// <param name="timeout">A timeout value when the message queue is emty</param>
        /// <returns>the message</returns>
        /// <remarks>
        /// The timeout value specifies what to do when there are no messages in the queue.  
        /// timeout = 0 - Return immediately if the queue is empty..
        /// timeout = -1 - Block until a message arrives in the queue.
        /// timout  = value in milliseconds.  The call will block for that much time.
        /// 
        /// When a message arrives, and there are multiple threads blockign on this queue, a race
        /// condition is caused.  All the blockign threads are woken up and they rush to get the
        /// new message.  One of them gets it and the others go back to waiting or returning.  It is not
        /// gauranteed that the first thread to start waiting will be the one that gets the message.
        /// </remarks>
        public object Remove(int timeout)
        {
            object msg = null;
            lock (queue)
            {
                while (msg == null)
                {
                    try
                    {
                        msg = RemoveMessage();
                        if (msg == null)
                        {
                            if (timeout == 0)
                            {
                                break;
                            }
                            else if (timeout == -1)
                            {
                                Monitor.Wait(queue);
                            }
                            else
                            {
                                Monitor.Wait(queue, timeout);
                                return RemoveMessage();
                            }
                        }                        
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            return msg;
        }
    }
}
