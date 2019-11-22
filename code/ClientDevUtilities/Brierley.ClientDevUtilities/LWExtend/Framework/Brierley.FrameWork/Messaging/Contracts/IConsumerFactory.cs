using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Messaging.Config;

namespace Brierley.FrameWork.Messaging.Contracts
{
    public interface IConsumerFactory : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfg"></param>
        void Initialize(ConsumerCfg cfg);

        /// <summary>
        /// This operation is invoked to get an instance of IConsumer to consume a message.
        /// </summary>
        /// <returns></returns>
        IConsumer ReserveInstance();
        
        /// <summary>
        /// This is called after the message has been consumed.
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        void ReturnInstance(IConsumer consumer);
    }
}
