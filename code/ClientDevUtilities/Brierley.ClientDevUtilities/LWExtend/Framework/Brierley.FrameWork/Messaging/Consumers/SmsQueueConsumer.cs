using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Messages;
using Brierley.FrameWork.Sms;

namespace Brierley.FrameWork.Messaging.Consumers
{
	public class SmsQueueConsumer : IConsumer
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_QueueProcessor);

		public void Consume(object msg)
		{
			var message = (SmsMessage)msg;
			_logger.Debug("SmsQueueConsumer", "Consume", string.Format("consuming email message: {0}", message.SmsId));

			using (var sms = new TriggeredSms(LWConfigurationUtil.GetCurrentConfiguration(), message.SmsId))
			{
				sms.SendDmc(message.MobileNumber, message.Personalizations);
			}
		}

		public void Dispose()
		{
		}
	}
}
