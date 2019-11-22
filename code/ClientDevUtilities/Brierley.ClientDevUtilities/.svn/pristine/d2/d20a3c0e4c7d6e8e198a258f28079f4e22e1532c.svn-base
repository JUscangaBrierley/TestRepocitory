using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Dmc;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Email
{
	public class AwsTriggeredEmailRetry : AwsTriggeredEmail
	{
		private const string _className = "AwsTriggeredEmailRetry";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public AwsTriggeredEmailRetry()
			: base(new NullCommunicationLogger())
		{
			Config = LWConfigurationUtil.GetCurrentConfiguration();
		}

		public AwsTriggeredEmailRetry(long emailId)
			: base(new NullCommunicationLogger())
		{
			MailingId = emailId;
		}
		
		public bool Resend(long emailQueueId)
		{
			const string methodName = "Resend";
			EmailQueue emailQueue = null;
			long thisQueueId = emailQueueId;
			using (var svc = LWDataServiceUtil.EmailServiceInstance(Config.Organization, Config.Environment))
			{
				try
				{
					_logger.Debug(_className, methodName, string.Format("Resend({0})", emailQueueId));

					LoadConfig();
					LoadEmail();

					emailQueue = svc.GetEmailQueue(emailQueueId);
					if (emailQueue == null)
					{
						string msg = "Invalid email queue ID " + emailQueueId;
						_logger.Error(_className, methodName, msg);
						throw new ArgumentException(msg);
					}

					var data = DeserializePersonalizationsFromQueue(emailQueue.Records);
					if (data.User == null)
					{
						//this should never happen, assuming we control the user at serialization time
						throw new Exception("cannot resend SMS. The user is null.");
					}
					SendAwsAsync(data.User.Email, data.Personalizations).Wait();
					svc.DeleteEmailQueue(thisQueueId);
					return true;
				}
				catch (Exception ex)
				{
					string msg = string.Format("Resend failed for email queue ID '{0}': {1}", emailQueueId, ex.Message);
					_logger.Error(_className, methodName, msg, ex);

					if (svc != null && emailQueue != null)
					{
						// Update Last_DML_Date, and (in case it changed) EmailFailureType 
						emailQueue.EmailFailureType = GetFailureType(ex);
						emailQueue.SendAttempts++;
						emailQueue.LastSendAttempt = DateTime.Now;
						svc.UpdateEmailQueue(emailQueue);
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
				throw new Exception("Failed to load Email queue data. The data is invalid", ex);
			}
		}


		private EmailFailureType GetFailureType(Exception ex)
		{
			if (ex is SoapException)
			{
				return EmailFailureType.InvalidContent;
			}
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
