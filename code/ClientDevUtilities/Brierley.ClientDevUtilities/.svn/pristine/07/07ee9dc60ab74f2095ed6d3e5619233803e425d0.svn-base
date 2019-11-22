using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Sms;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Sms
{
	public class SendSms : OperationProviderBase
	{
		private class SmsState
		{
			public string Organization { get; set; }
			public string Environment { get; set; }
			public string SmsName { get; set; }
			public Member member { get; set; }
			public Dictionary<string, string> AdditionalFields { get; set; }
		}

		private const string _className = "SendSms";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public SendSms() : base("SendSms") { }

		public override string Invoke(string source, string parms)
		{
			const string methodName = "Invoke";
			try
			{
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for sending SMS.") { ErrorCode = 11300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string smsName = args.ContainsKey("SmsName") ? (string)args["SmsName"] : string.Empty;
				bool asynchStr = args.ContainsKey("Asynchronous") ? (bool)args["Asynchronous"] : false;

				if (string.IsNullOrEmpty(smsName))
				{
					throw new LWOperationInvocationException("No SMS name provided.") { ErrorCode = 11108 };
				}

				SmsDocument smsDocument;
				using (SmsService smsService = LWDataServiceUtil.SmsServiceInstance())
				{
					smsDocument = smsService.GetSmsMessage(smsName);
				}
				if (smsDocument == null)
				{
					throw new LWOperationInvocationException("The SMS name provided does not exist.") { ErrorCode = 11394 };
				}

				Dictionary<string, string> additionalFields = new Dictionary<string, string>();
				if (args.ContainsKey("AdditionalFields"))
				{
					APIStruct[] attList = (APIStruct[])args["AdditionalFields"];
					foreach (APIStruct att in attList)
					{
						additionalFields.Add((string)att.Parms["FieldName"], (string)att.Parms["FieldValue"]);
					}
				}

				LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
				SmsState state = new SmsState()
				{
					Organization = ctx.Organization,
					Environment = ctx.Environment,
					SmsName = smsName,
					AdditionalFields = additionalFields
				};

				Member member = LoadMember(args);
				state.member = member;				
				
				if (asynchStr)
				{
                    Task t = new Task(() => SendSmsMessage(state));
                    t.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                    t.Start();
                }
				else
				{
					SendSmsMessage(state);
				}
				_logger.Trace(_className, methodName, "SMS message sent.");
				return string.Empty;
			}
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message, ex) { ErrorCode = 1 };
			}
		}

		protected void SendSmsMessage(object state)
		{
			SmsState smsState = (SmsState)state;
			LWConfigurationUtil.SetCurrentEnvironmentContext(smsState.Organization, smsState.Environment);

			LWConfiguration cfg = LWConfigurationUtil.GetCurrentConfiguration();

			using (TriggeredSms mailing = new TriggeredSms(cfg, smsState.SmsName))
			{
				mailing.Send(smsState.member, smsState.AdditionalFields);
			}
		}

        protected void ExceptionHandler(Task task)
        {
            try
            {
                if (task.Exception != null)
                {
                    foreach (var exception in task.Exception.InnerExceptions)
                    {
                        _logger.Error(_className, "SendSmsMessage", "Error sending SMS", exception);
                    }
                }
            }
            catch { } // Don't let the thread crash
        }
    }
}
