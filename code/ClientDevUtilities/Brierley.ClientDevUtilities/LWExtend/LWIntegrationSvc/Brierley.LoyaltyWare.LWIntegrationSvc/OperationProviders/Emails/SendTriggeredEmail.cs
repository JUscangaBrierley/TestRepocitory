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
using Brierley.FrameWork.Email;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Emails
{
	public class SendTriggeredEmail : OperationProviderBase
	{
		private class EmailState
		{
			public string Organization { get; set; }
			public string Environment { get; set; }
			public string EmailName { get; set; }
			public string RecepientEmail { get; set; }
			public Member member { get; set; }
			public Dictionary<string, string> AdditionalFields { get; set; }
		}

		private const string _className = "SendTriggeredEmail";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public SendTriggeredEmail() : base("SendTriggeredEmail") { }

		public override string Invoke(string source, string parms)
		{
			const string methodName = "Invoke";
			try
			{
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for sending triggered email.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string emailName = args.ContainsKey("EmailName") ? (string)args["EmailName"] : string.Empty;
				string recipientEmail = args.ContainsKey("RecipientEmail") ? (string)args["RecipientEmail"] : string.Empty;
				bool asynchStr = args.ContainsKey("Asynchronous") ? (bool)args["Asynchronous"] : false;

				if (string.IsNullOrEmpty(emailName))
				{
					throw new LWOperationInvocationException("No email name provided.") { ErrorCode = 3108 };
				}

				EmailDocument emailDocument;
				using (EmailService emailService = LWDataServiceUtil.EmailServiceInstance())
				{
					emailDocument = emailService.GetEmail(emailName);
				}
				if (emailDocument == null)
				{
					throw new LWOperationInvocationException("The email name provided does not exist.") { ErrorCode = 3394 };
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
				EmailState state = new EmailState()
				{
					Organization = ctx.Organization,
					Environment = ctx.Environment,
					EmailName = emailName,
					AdditionalFields = additionalFields
				};
				if (!string.IsNullOrEmpty(recipientEmail))
				{
					state.RecepientEmail = recipientEmail;
				}
				else
				{
					Member member = LoadMember(args);

					if (string.IsNullOrEmpty(member.PrimaryEmailAddress) && string.IsNullOrEmpty(recipientEmail))
					{
						throw new LWOperationInvocationException("No recipient to send triggered email to.") { ErrorCode = 3366 };
					}

					if (!string.IsNullOrEmpty(recipientEmail) && !additionalFields.ContainsKey("RecipientEmail"))
					{
						additionalFields.Add("RecipientEmail", recipientEmail);
					}
					state.member = member;
				}
				if (asynchStr)
				{
                    Task t = new Task(() => SendEmail(state));
                    t.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                    t.Start();
				}
				else
				{
					SendEmail(state);
				}
				_logger.Trace(_className, methodName, "Triggered email sent.");
				return string.Empty;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
		
		protected void SendEmail(object state)
		{
			EmailState emailState = (EmailState)state;
			LWConfigurationUtil.SetCurrentEnvironmentContext(emailState.Organization, emailState.Environment);

			using (ITriggeredEmail mailing = TriggeredEmailFactory.Create(emailState.EmailName))
			{
				if (emailState.member != null)
				{
					mailing.SendAsync(emailState.member, emailState.AdditionalFields).Wait();
				}
				else
				{
					mailing.SendAsync(emailState.RecepientEmail, emailState.AdditionalFields).Wait();
				}
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
                        _logger.Error(_className, "SendEmail", "Error sending email", exception);
                    }
                }
            }
            catch { } // Don't let the thread crash
        }
	}
}
