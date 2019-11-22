using System;

namespace Brierley.FrameWork.Messaging.Contracts
{
    public interface IConsumer : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        // This method has to be thread safe.
        /// </remarks>
        /// <param name="msg"></param>
        void Consume(object msg);
	}
}
