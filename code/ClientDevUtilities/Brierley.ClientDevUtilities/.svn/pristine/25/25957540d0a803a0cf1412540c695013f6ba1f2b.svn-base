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
using Dmc = Brierley.FrameWork.Dmc;

namespace Brierley.FrameWork.Sms
{
	//todo: this class is intended to be used for resending any resendable DMC message, but the way the queues are constructed 
	//leaves the design in question. Updating a user's profile is tied to the SMS queue when it really shouldn't be. We should 
	//consider logging all failed DMC messages to a single queue (e.g., LW_DmcQueue). No time for that in 4.6.2. Maybe 4.6.3.
	public class DmcMessageRetry //: TriggeredSms
	{
		private const string _className = "DmcMessageRetry";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private long _queueId = 0;

		public DmcMessageRetry()
		{
		}

		public bool Resend(long queueId)
		{
			const string methodName = "Resend";
			SmsQueue queue = null;
			using (var svc = LWDataServiceUtil.SmsServiceInstance())
			using (var dmc = new DmcService(new NullCommunicationLogger()))
			{
				try
				{
					_logger.Debug(_className, methodName, string.Format("Resend({0})", queueId));

					//LoadConfig();
					//LoadSmsMessage();

					queue = svc.GetSmsQueue(queueId);
					if (queue == null)
					{
						string msg = "Invalid sms queue ID " + queueId;
						_logger.Error(_className, methodName, msg);
						throw new ArgumentException(msg);
					}

					//serialNumber = Send(smsQueue.Records, smsQueue, out resendSmsQueueID);
					var data = DeserializePersonalizationsFromQueue(queue.Records);
					if (data.User == null)
					{
						//this should never happen, assuming we control the user at serialization time
						throw new Exception("cannot resend SMS. The user is null.");
					}

					dmc.UpdateProfileByMobileNumber(data.User.MobileNumber, data.Attributes);

					svc.DeleteSmsQueue(queueId);

					return true;
				}
				catch (Exception ex)
				{
					string msg = string.Format("Resend failed for sms queue ID '{0}': {1}", queueId, ex.Message);
					_logger.Error(_className, methodName, msg, ex);

					if (svc != null && queue != null)
					{
						// Update Last_DML_Date, and (in case it changed) EmailFailureType 
						queue.SmsFailureType = GetFailureType(ex);
						queue.SendAttempts++;
						queue.LastSendAttempt = DateTime.Now;
						svc.UpdateSmsQueue(queue);
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
				throw new Exception("Failed to load queue data. The data is invalid", ex);
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
