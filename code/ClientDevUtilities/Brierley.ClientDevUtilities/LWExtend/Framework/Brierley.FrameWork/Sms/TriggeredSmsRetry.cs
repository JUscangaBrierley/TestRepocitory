using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Dmc;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Sms
{
	public class TriggeredSmsRetry : TriggeredSms
	{
		private const string _className = "TriggeredSmsRetry";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public TriggeredSmsRetry()
			: base(new NullCommunicationLogger())
		{
			Config = LWConfigurationUtil.GetCurrentConfiguration();
		}

		public TriggeredSmsRetry(long smsId)
			: base(new NullCommunicationLogger())
		{
			SmsId = smsId;
		}

		public bool Resend(long smsQueueId)
		{
			const string methodName = "ResendDmc";
			SmsQueue smsQueue = null;
			long thisQueueId = smsQueueId;
			using (var svc = LWDataServiceUtil.SmsServiceInstance())
			{
				try
				{
					_logger.Debug(_className, methodName, string.Format("Resend({0})", smsQueueId));

					LoadConfig();
					//LoadSmsMessage(); <-- not sure we need this for a simple retry

					smsQueue = svc.GetSmsQueue(smsQueueId);
					if (smsQueue == null)
					{
						string msg = "Invalid sms queue ID " + smsQueueId;
						_logger.Error(_className, methodName, msg);
						throw new ArgumentException(msg);
					}

					ExternalId = smsQueue.SmsID;

					//serialNumber = Send(smsQueue.Records, smsQueue, out resendSmsQueueID);
					var data = DeserializePersonalizationsFromQueue(smsQueue.Records);
					if (data.User == null)
					{
						//this should never happen, assuming we control the user at serialization time
						throw new Exception("cannot resend SMS. The user is null.");
					}
					SendDmc(data.User.MobileNumber, data.Personalizations);

					svc.DeleteSmsQueue(thisQueueId);

					return true;
				}
				catch (Exception ex)
				{
					string msg = string.Format("Resend failed for sms queue ID '{0}': {1}", smsQueueId, ex.Message);
					_logger.Error(_className, methodName, msg, ex);

					if (svc != null && smsQueue != null)
					{
						// Update Last_DML_Date, and (in case it changed) EmailFailureType 
						smsQueue.SmsFailureType = GetFailureType(ex);
						smsQueue.SendAttempts++;
						smsQueue.LastSendAttempt = DateTime.Now;
						svc.UpdateSmsQueue(smsQueue);
					}
					return false;
				}
			}
		}

		private CommunicationLogData DeserializePersonalizationsFromQueue(string record)
		{
			if (string.IsNullOrEmpty(record))
			{
				return null;
			}
			try
			{
				return JsonConvert.DeserializeObject<CommunicationLogData>(record);
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to load SMS queue data. The data is invalid", ex);
			}
		}

		private SmsFailureType GetFailureType(Exception ex)
		{
			if (ex is SoapException)
			{
				return SmsFailureType.InvalidContent;
			}
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
