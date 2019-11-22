using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging
{
	/// <summary>
	/// Used to monitor message bus processing
	/// </summary>
	public interface IMessagingMonitor
	{
		/// <summary>
		/// Logs the successful processing of a message
		/// </summary>
		/// <param name="message"></param>
		void Success(string messageName, IMessage message);

		/// <summary>
		/// Logs the failed processing of a message
		/// </summary>
		/// <param name="message">The message that failed to process</param>
		/// <param name="ex">The exception that was thrown during processing</param>
		/// <param name="moved">Indicates whether or not the message was moved to the errors subqueue.</param>
		void Failure(string messageName, IMessage message, Exception ex, bool moved = false);

		void BeginMonitoring();

		void Shutdown();
	}
}
