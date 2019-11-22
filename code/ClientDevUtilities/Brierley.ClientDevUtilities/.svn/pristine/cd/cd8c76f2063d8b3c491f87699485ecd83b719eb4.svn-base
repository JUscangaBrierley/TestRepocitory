using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Dmc
{
	public class DmcSmsQueueLogger : ICommunicationLogger
	{
		private static bool? _logSuccess = null;

		private static bool LogSuccess
		{
			get
			{
				if (_logSuccess == null)
				{
					string convert = LWConfigurationUtil.GetConfigurationValue("LogTriggeredSmsMessages");
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

			// queue the successful send
			if (
				data.MessageType == CommunicationType.DmcSendSingle &&
				data.Response != null &&
				data.Response.IsSuccessStatusCode &&
				LogSuccess)
			{
				using (var svc = LWDataServiceUtil.SmsServiceInstance())
				{
					//need to know mailing id
					if (data.MessageId == null && data.QueueId != null)
					{
						var q = svc.GetSmsQueue(data.QueueId.Value);
						if (q != null)
						{
							data.MessageId = q.SmsID;
						}
					}

					SmsQueue smsQueue = new SmsQueue()
					{
						MessageType = data.MessageType,
						SmsID = data.MessageId.GetValueOrDefault(),
						Records = SerializeForQueue(data),
						SmsFailureType = SmsFailureType.SentSuccessfully,
						SendAttempts = 1,
						LastSendAttempt = DateTime.Now
					};
					svc.CreateSmsQueue(smsQueue);
					return;
				}
			}


			if (data.Exception != null)
			{
				if (data.Exception is Dmc.Exceptions.InvalidParameterException)
				{
					//invalid parameter. this is a permanent fail, so we won't log it for retry
					return;
				}
				if (data.MessageType == CommunicationType.DmcGetPersonalizations || data.MessageType == CommunicationType.DmcGetUser)
				{
					//get requests do nothing. no reason to log them for retry.
					return;
				}
				using (var svc = LWDataServiceUtil.SmsServiceInstance())
				{
					SmsQueue smsQueue = new SmsQueue()
					{
						MessageType = data.MessageType,
						SmsID = data.MessageId.GetValueOrDefault(),
						Records = SerializeForQueue(data), 
						SmsFailureType = GetFailureType(data.Exception),
						SendAttempts = 1,
						LastSendAttempt = DateTime.Now
					};
					svc.CreateSmsQueue(smsQueue);
				}
				queuedForRetry = true;
			}
		}

		private string SerializeForQueue(CommunicationLogData data)
		{
			return JsonConvert.SerializeObject(data);
		}

		private SmsFailureType GetFailureType(Exception ex)
		{
			if (ex is System.Net.WebException)
			{
				return SmsFailureType.ConnectionFailure;
			}
			if (ex.InnerException != null)
			{
				return GetFailureType(ex.InnerException);
			}
			return SmsFailureType.Unknown;
		}
	}
}
