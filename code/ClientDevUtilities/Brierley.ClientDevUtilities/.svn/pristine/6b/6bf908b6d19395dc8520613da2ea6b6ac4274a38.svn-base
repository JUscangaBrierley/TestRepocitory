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
using Brierley.FrameWork.Messaging.Messages;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Email
{
	/// <summary>
	/// The triggered email class is used to send triggered emails via DMC using Loyalty Navigator, or by invoking via the framework.
	/// </summary>
	/// <example>
	/// <code>
	///	using(TriggeredEmail email = new TriggeredEmail("EmailNameOrID")) 
	/// {
	///		email.Send(Member);
	/// }
	/// </code>
	/// <code>
	/// Dictionary<string,string> manualFields = new Dictionary<string,string>();
	/// manualFields.Add("RecipientEmail", "jsmith@brierley.com");
	/// manualFields.Add("Name", "John Smith");
	///	using(TriggeredEmail email = new TriggeredEmail("EmailNameOrID")) 
	/// {
	///		email.Send(manualFields);
	/// }
	/// </code>
	/// </example>
	public class DmcTriggeredEmail : TriggeredEmail, ITriggeredEmail
	{
		private const string _className = "TriggeredEmail";
		private ICommunicationLogger _messageLogger;
		private bool _dmcUseAlternateEmail = false;
		private Dmc.DmcService _dmc = null;

		protected long? ExternalId { get; set; }

		public DmcTriggeredEmail()
		{
		}

		public DmcTriggeredEmail(ICommunicationLogger logger = null)
		{
			_messageLogger = logger;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config">LWConfiguration used to connect to framework database</param>
		/// <param name="server">wrapper for email API</param>
		/// <param name="emailName">name of the triggered email</param>
		public void Init(string emailName, LWConfiguration config = null)
		{
			Config = config ?? LWConfigurationUtil.GetCurrentConfiguration();
			MailingName = emailName;
			LoadConfig();
			LoadEmail();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config">LWConfiguration used to connect to framework database</param>
		/// <param name="server">wrapper for email API</param>
		/// <param name="mailingName">name of the triggered email</param>
		public void Init(long emailId, LWConfiguration config = null)
		{
			Config = config ?? LWConfigurationUtil.GetCurrentConfiguration();
			MailingId = emailId;
			LoadConfig();
			LoadEmail();
		}

		/// <summary>
		/// Sends a triggered email to a loyalty member.
		/// </summary>
		/// <param name="Member">The loyalty member the email will be sent to.</param>
		public override async Task SendAsync(Member member)
		{
			await SendAsync(member, null);
		}

		/// <summary>
		/// Sends a triggered email to a loyalty member.
		/// </summary>
		/// <param name="member">The loyalty member the email will be sent to.</param>
		/// <param name="additionalFields">Name/value pair of fields, which can be used to override the value of any field in the email.</param>
		public override async Task SendAsync(Member member, Dictionary<string, string> additionalFields)
		{
			string recipientEmail = string.Empty;
			Dictionary<string, string> updatedPersonalizations = new Dictionary<string, string>();
			BuildDmc(member, ref recipientEmail, additionalFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				var m = new EmailMessage(MailingId, recipientEmail, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<EmailMessage>(m);
			}
			else
			{
				await SendDmcAsync(recipientEmail, updatedPersonalizations);
			}
		}

		/// <summary>
		/// Sends a triggered email.
		/// </summary>
		/// <remarks>
		///	This method can be used to send an email in the event that the email should not go to a loyalty 
		/// member (e.g., an alert which needs to be send to a customer service rep or administrator).
		/// </remarks>
		/// <param name="recipientEmail">The recipient's email address, if different from Member.PrimaryEmailAddress.</param>
		/// <param name="manualFields">Name/value pair of fields.</param>
		/// <param name="emailQueueID">The ID of the send record in the EmailQueue table.</param>
		/// <returns>Serial number of the mailing.</returns>
		public override async Task SendAsync(string recipientEmail, Dictionary<string, string> manualFields)
		{
			Dictionary<string, string> updatedPersonalizations = new Dictionary<string, string>();
			BuildDmc(null, ref recipientEmail, manualFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				var m = new EmailMessage(MailingId, recipientEmail, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<EmailMessage>(m);
			}
			else
			{
				await SendDmcAsync(recipientEmail, updatedPersonalizations);
			}
		}

		protected internal async Task SendDmcAsync(string emailAddress, Dictionary<string, string> personalizations)
		{
			const string methodName = "SendDmcAsync";

			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			bool caughtObjectExists = false;
			try
			{
				User user = await _dmc.GetUserAsync(emailAddress);
				if (user == null)
				{
					List<Dmc.Attribute> attributes = null;
					if (_dmcUseAlternateEmail)
					{
						attributes = new List<Dmc.Attribute>() { new Dmc.Attribute("SendEmail", emailAddress) };
					}
					user = await _dmc.CreateUserAsync(emailAddress, null, attributes);
				}
				await _dmc.SendSingleAsync(ExternalId.Value, user, personalizations);
				_logger.Debug(_className, methodName, string.Format("_dmc.Send('{0}', '{1}')", ExternalId.Value, user.Id));
			}
			catch (Dmc.Exceptions.ObjectAlreadyExistsException existsEx)
			{
				caughtObjectExists = true;
				string msg = "Failed to send email: " + existsEx.Message;
				_logger.Error(_className, methodName, msg, existsEx);
			}
			catch (Exception ex)
			{
				string msg = "Failed to send email: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
			if (caughtObjectExists)
			{
				//todo: catching object exists and retrying has potential for infinite loop if somehow a bug in DMC continues to send null 
				//user (get) followed by object exists (create) we may want to limit the number of times this can happen before we give up
				await SendDmcAsync(emailAddress, personalizations);
			}
		}

		private void BuildDmc(Member member, ref string emailAddress, Dictionary<string, string> personalizations, out Dictionary<string, string> updatedPersonalizations)
		{
			const string methodName = "BuildDmc";

			if (personalizations == null)
			{
				personalizations = new Dictionary<string, string>();
			}

			ContextObject context = new ContextObject() { Environment = { { "EmailName", MailingName } } };
			context.Owner = member;

			bool overwriteRecipient = false;

			if (personalizations != null)
			{
				foreach (string key in personalizations.Keys)
				{
					context.Environment.Add(key, personalizations[key]);

					if (key.Equals("recipientemail", StringComparison.OrdinalIgnoreCase))
					{
						overwriteRecipient = true;
						emailAddress = personalizations[key];
					}
				}
			}

			if (!overwriteRecipient && member != null)
			{
				emailAddress = member.PrimaryEmailAddress;
			}

			if (Personalizations != null)
			{
				foreach (var p in Personalizations)
				{
					if (!personalizations.ContainsKey(p.Name))
					{
						//personalization has not been overridden. Add it
						string expression = p.Expression;
						string value = string.Empty;
						if (!string.IsNullOrEmpty(expression))
						{
							Expression e = new ExpressionFactory().Create(expression);
							try
							{
								value = e.evaluate(context).ToString();
							}
							catch (Exception ex)
							{
								//we used to insert the default value for the field when an exception was caught. We probably should 
								//let this exception happen, but we also need to preserve functionalizty as much as possible in order
								//to prevent client teams from having to make code changes, so for now we'll eat the exception.
								//record.Append(field.DefaultValue);
								_logger.Error(_className, methodName, "Error evaluating bScript expression " + expression, ex);
							}
						}
						personalizations.Add(p.Name, value);
					}
				}
			}
			updatedPersonalizations = personalizations;
		}


		/// <summary>
		/// Helper method for constructors.  Loads framework config values for email API (username, password, url).
		/// </summary>
		protected void LoadConfig()
		{
			const string methodName = "LoadConfig";
			try
			{
				if (_dmc == null)
				{
					_dmc = new Dmc.DmcService(_messageLogger ?? GetEmailLogger());
				}
				
				string altEmail = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Dmc.Constants.DmcUseAlternateEmail);
				if (!string.IsNullOrEmpty(altEmail))
				{
					//_enableDmc (above) is fine as =="true". If someone puts "asdf" as the value, it will become obvious at send time that 
					//something is wrong. For alternate email, we need an exact boolean. Ignoring a simple mistake like "ttrue" can lead to 
					//some pretty serious problems (members in DMC with no alternate email mean they'll never receive an email and this is 
					//a manual cleanup process), so we'll try to parse the value and throw an exception if we can't get a boolean.
					try
					{
						_dmcUseAlternateEmail = bool.Parse(altEmail);
					}
					catch (Exception ex)
					{
						throw new Exception(
							string.Format(
							"Could not convert configuration value of {0} into a boolean. Please ensure the configuration value is either empty or a valid boolean (true or false)",
							Brierley.FrameWork.Dmc.Constants.DmcUseAlternateEmail),
							ex);
					}
				}
			}
			catch (Exception ex)
			{
				string msg = "Failed to load configuration for email: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw new Exception(msg, ex);
			}
		}

		/// <summary>
		/// Helper method for constructors.  Loads the email from the framework database and puts it into a listening state.
		/// </summary>
		/// <returns>True if the email is successfully loaded, else false</returns>
		protected bool LoadEmail()
		{
			const string methodName = "LoadEmail";

			// get the email template fields
			bool emailDocumentExists = false;
			try
			{
				using (var svc = LWDataServiceUtil.EmailServiceInstance(Config.Organization, Config.Environment))
				using (var content = LWDataServiceUtil.ContentServiceInstance(Config.Organization, Config.Environment))
				{

					EmailDocument emailDocument = null;
					if (!string.IsNullOrEmpty(MailingName))
					{
						_logger.Debug(_className, methodName, "Loading mailing: " + MailingName);
						emailDocument = svc.GetEmail(MailingName);
					}
					else
					{
						emailDocument = svc.GetEmail(MailingId);
					}

					if (emailDocument != null)
					{
						emailDocumentExists = true;
						MailingName = emailDocument.Name;
						MailingId = emailDocument.Id;

						if (!emailDocument.ExternalId.HasValue)
						{
							throw new Exception(string.Format("The email {0} is invalid because no external id has been configured.", string.IsNullOrEmpty(MailingName) ? MailingId.ToString() : MailingName));
						}
						ExternalId = emailDocument.ExternalId;
						Personalizations = svc.GetPersonalizations(emailDocument.Id);
					}
				}
			}
			catch (Exception ex)
			{
				string msg = "Failed to load email from Framework database: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw new Exception(msg, ex);
			}
			if (!emailDocumentExists)
			{
				string msg = "Failed to load email. The provided email ID does not exist in the framework database.";
				_logger.Error(_className, methodName, msg);
				throw new Exception(msg);
			}


			return true;
		}

		public override void Dispose()
		{
			if (_dmc != null)
			{
				_dmc.Dispose();
			}
		}

		private ICommunicationLogger GetEmailLogger()
		{
			if (Config != null)
			{
				return LWDataServiceUtil.GetServiceConfiguration(Config.Organization, Config.Environment).EmailLogger;
			}
			return LWDataServiceUtil.GetServiceConfiguration().EmailLogger;
		}
	}
}
