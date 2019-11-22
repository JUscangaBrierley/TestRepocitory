using System;
using System.Transactions;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Email;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Messages;

namespace Brierley.FrameWork.Messaging.Consumers
{
	public class EmailQueueConsumer : IConsumer
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_QueueProcessor);

		public void Consume(object msg)
		{
			var message = (EmailMessage)msg;
			_logger.Debug("EmailQueueConsumer", "Consume", string.Format("consuming message: {0}", message.EmailId));

			using (var email = TriggeredEmailFactory.Create(message.EmailId))
			{
				email.SendAsync(message.RecipientEmail, message.Personalizations).Wait();
			}
		}

		public void Dispose()
		{
		}
	}
}
