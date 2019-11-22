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
	/// The triggered email class is used to send triggered emails via <email provider> using Loyalty Navigator, or by invoking via the framework.
	/// </summary>
	/// <example>
	/// <code>
	///	using(TriggeredEmail email = TriggeredEmailFactory.Create("EmailNameOrID")) 
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
	public abstract class TriggeredEmail : IDisposable
	{
		private const string _className = "TriggeredEmail";
		protected static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		protected LWConfiguration Config { get; set; }
		protected long MailingId { get; set; }
		protected string MailingName { get; set; }
		protected IEnumerable<EmailPersonalization> Personalizations { get; set; }

		protected static bool QueueingEnabled { get; private set; }

		static TriggeredEmail()
		{
			QueueingEnabled = !LWConfigurationUtil.IsQueueProcessor && Messaging.MessagingBus.CanSend(typeof(EmailMessage));
		}

		/// <summary>
		/// Sends a triggered email to a loyalty member.
		/// </summary>
		/// <param name="Member">The loyalty member the email will be sent to.</param>
		public abstract Task SendAsync(Member member);

		/// <summary>
		/// Sends a triggered email to a loyalty member.
		/// </summary>
		/// <param name="member">The loyalty member the email will be sent to.</param>
		/// <param name="additionalFields">Name/value pair of fields, which can be used to override the value of any field in the email.</param>
		public abstract Task SendAsync(Member member, Dictionary<string, string> additionalFields);

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
		public abstract Task SendAsync(string recipientEmail, Dictionary<string, string> manualFields);

		/// <summary>
		/// Helper method for constructors.  Loads the email from the framework database and puts it into a listening state.
		/// </summary>
		/// <returns>True if the email is successfully loaded, else false</returns>
		protected virtual bool LoadEmail()
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

		public virtual void Dispose()
		{
		}
	}
}
