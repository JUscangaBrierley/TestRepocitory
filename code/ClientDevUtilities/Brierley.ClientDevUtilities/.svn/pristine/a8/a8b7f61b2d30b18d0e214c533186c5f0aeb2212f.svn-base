using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Dmc
{
	public class DmcEmailQueueLogger : ICommunicationLogger
	{
		private static bool? _logSuccess = null;

		private static bool LogSuccess
		{
			get
			{
				if (_logSuccess == null)
				{
					string convert = LWConfigurationUtil.GetConfigurationValue("LogTriggeredEmails");
					if (!string.IsNullOrEmpty(convert))
					{
						_logSuccess = bool.Parse(convert);
					}
				}
				return _logSuccess.GetValueOrDefault();
			}
		}
		
		public void LogMessage(CommunicationLogData data)
		{
			bool queued = false;
			LogMessage(data, out queued);
		}

		public void LogMessage(CommunicationLogData data, out bool queuedForRetry)
		{
			queuedForRetry = false;
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			//log success?
			if (
				data.MessageType == CommunicationType.DmcSendSingle &&
				data.Response != null &&
				data.Response.IsSuccessStatusCode &&
				LogSuccess)
			{
				// queue the successful send
				using (var svc = LWDataServiceUtil.EmailServiceInstance())
				{
					//need to know mailing id
					if (data.MessageId == null && data.QueueId != null)
					{
						var q = svc.GetEmailQueue(data.QueueId.Value);
						if (q != null)
						{
							data.MessageId = q.EmailID;
						}
					}

					EmailQueue emailQueue = new EmailQueue()
					{
						MessageType = data.MessageType,
						EmailID = data.MessageId.GetValueOrDefault(),
						Records = SerializeForQueue(data),
						EmailFailureType = EmailFailureType.SentSuccessfully, //<-- that's right, the failure type is success.
						SendAttempts = 1,
						LastSendAttempt = DateTime.Now
					};
					svc.CreateEmailQueue(emailQueue);
				}
			}


			if (data.Exception != null)
			{
				if (data.Exception is Dmc.Exceptions.InvalidParameterException)
				{
					//invalid parameter. this is a permanent fail, so we won't log it for retry
					return;
				}
				using (var svc = LWDataServiceUtil.EmailServiceInstance())
				{
					EmailQueue emailQueue = new EmailQueue()
					{
						MessageType = data.MessageType,
						EmailID = data.MessageId.GetValueOrDefault(),
						Records = SerializeForQueue(data),
						EmailFailureType = GetFailureType(data.Exception),
						SendAttempts = 1,
						LastSendAttempt = DateTime.Now
					};
					svc.CreateEmailQueue(emailQueue);
				}
				queuedForRetry = true;
			}
		}

		private string SerializeForQueue(CommunicationLogData data)
		{
			return JsonConvert.SerializeObject(data);
		}

		private EmailFailureType GetFailureType(Exception ex)
		{
			if (ex is System.Net.WebException)
			{
				return EmailFailureType.ConnectionFailure;
			}
			if (ex.InnerException != null)
			{
				return GetFailureType(ex.InnerException);
			}
			return EmailFailureType.Unknown;
		}
	}
}
