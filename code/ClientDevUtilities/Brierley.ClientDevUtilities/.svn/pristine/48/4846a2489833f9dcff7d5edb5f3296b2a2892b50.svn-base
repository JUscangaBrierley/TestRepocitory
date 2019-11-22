using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Messaging.Messages;

namespace Brierley.FrameWork.Email
{
	public class AwsTriggeredEmail : TriggeredEmail, ITriggeredEmail
	{
		private const string _className = "AwsTriggeredEmail";
		private EmailDocument _emailDocument = null;
		private RegionEndpoint _region = RegionEndpoint.USWest2;
		private AWSCredentials _credentials = null;
		private ICommunicationLogger _messageLogger;

		protected long? ExternalId { get; set; }

		public AwsTriggeredEmail()
		{
		}

		public AwsTriggeredEmail(ICommunicationLogger logger = null)
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
			BuildAws(member, ref recipientEmail, additionalFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				var m = new EmailMessage(MailingId, recipientEmail, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<EmailMessage>(m);
			}
			else
			{
				await SendAwsAsync(recipientEmail, updatedPersonalizations);
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
			BuildAws(null, ref recipientEmail, manualFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				var m = new EmailMessage(MailingId, recipientEmail, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<EmailMessage>(m);
			}
			else
			{
				await SendAwsAsync(recipientEmail, updatedPersonalizations);
			}
		}

		protected internal async Task SendAwsAsync(string emailAddress, Dictionary<string, string> personalizations)
		{
			const string methodName = "SendAwsAsync";

			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}

			//check for suppression. If the address is suppressed, for any reason, a silent exit
			using (var svc = LWDataServiceUtil.EmailServiceInstance())
			{
				if (svc.IsEmailSuppressed(emailAddress))
				{
					return;
				}
			}

			string subject = _emailDocument.Subject;

			//todo: isPreview has strange effects. We may not need it now that StrongMail is gone:
			XsltDocument doc = new XsltDocument(_emailDocument.DocumentId.GetValueOrDefault(), false, true);

			string xslHtml = doc.XslHtml;
			foreach (var p in personalizations)
			{
				xslHtml = Regex.Replace(xslHtml, "##" + p.Key + "##", p.Value, RegexOptions.IgnoreCase);
				subject = Regex.Replace(subject, "##" + p.Key + "##", p.Value, RegexOptions.IgnoreCase);
			}

			//html
			StringReader sr = new StringReader(xslHtml);
			XmlTextReader xrdr = new XmlTextReader(sr);
			XslCompiledTransform xsltC = new XslCompiledTransform();
			xsltC.Load(xrdr);

			StringWriter sw = new StringWriter();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.ConformanceLevel = ConformanceLevel.Auto;
			XmlWriter xw = XmlWriter.Create(sw, settings);

			XmlDocument recip = new XmlDocument();
			recip.LoadXml(GetRecipientXml(personalizations));

			xsltC.Transform(recip, xw);

			string html = sw.ToString();

			//text
			string text = null;
			if (!string.IsNullOrEmpty(doc.XslText))
			{
				string xslText = doc.XslText;
				foreach (var p in personalizations)
				{
					xslText = Regex.Replace(xslText, "##" + p.Key + "##", p.Value, RegexOptions.IgnoreCase);
				}

				sr = new StringReader(xslText);
				xrdr = new XmlTextReader(sr);
				xsltC = new XslCompiledTransform();
				xsltC.Load(xrdr);

				sw = new StringWriter();
				xw = XmlWriter.Create(sw, settings);

				xsltC.Transform(recip, xw);

				text = sw.ToString();
			}

			var destination = new Destination()
			{
				ToAddresses = new List<string>() { emailAddress }
			};

			// Create the subject and body of the message.
			Content subj = new Content(subject);
			Content textBody = new Content(text);
			Content htmlBody = new Content(html);
			Body emailBody = new Body()
			{
				Html = htmlBody,
				Text = textBody
			};

			// Create a message with the specified subject and body.
			Message message = new Message(subj, emailBody);

			// Assemble the email.
			SendEmailRequest request = new SendEmailRequest(_emailDocument.FromEmail, destination, message);
			if (!string.IsNullOrEmpty(_emailDocument.BounceEmail))
			{
				request.ReturnPath = _emailDocument.BounceEmail;
			}

            // Instantiate an Amazon SES client, which will make the service call.
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_credentials, _region);

			try
			{
				// Send the email.
				SendEmailResponse response = await client.SendEmailAsync(request);
				if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
				{
					throw new Exception("Unexpected response from AWS: " + response.HttpStatusCode.ToString());
				}
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_messageLogger != null)
				{
					var data = new CommunicationLogData(CommunicationType.AwsSendEmail)
					{
						Exception = ex,
						User = new Dmc.User() { Email = emailAddress },
						MessageId = _emailDocument.Id,
						Personalizations = personalizations
					};

					_messageLogger.LogMessage(data, out queued);

				}
				if (queued)
				{
					string msg = "Failed to send email: " + ex.Message;
					_logger.Warning(_className, methodName, string.Format("Failed to send email {0} due to unexpected error. Email has been queued for retry", _emailDocument.Name), ex);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Helper method for constructors.  Loads framework config values for email API (username, password, url).
		/// </summary>
		protected void LoadConfig()
		{
			const string methodName = "LoadConfig";

			try
			{
				if (_messageLogger == null)
				{
					_messageLogger = GetEmailLogger();
				}

				string region = LWConfigurationUtil.GetConfigurationValue("AwsRegion");
				if (!string.IsNullOrEmpty(region))
				{
					try
					{
						_region = RegionEndpoint.GetBySystemName(region);
					}
					catch (Exception ex)
					{
						throw new Exception(
							string.Format("could not map configured AWS region to a valid region ({0}). Please check the configured AwsRegion and ensure it is set to a valid region.", region),
							ex);
					}
					_logger.Debug(_className, methodName, string.Format("region endpoint set to {0}", _region.SystemName));
				}
				else
				{
					_logger.Debug(_className, methodName, string.Format("region not specified. using default {0}", _region.SystemName));
				}

				string awsAccessKey = LWConfigurationUtil.GetConfigurationValue("AwsEmailAccessKey");
				if (string.IsNullOrEmpty(awsAccessKey))
				{
					throw new Exception("Could not load AWS email configuration. Configuration setting AwsEmailAccessKey has not been provided.");
				}

				string awsSecretKey = LWConfigurationUtil.GetConfigurationValue("AwsEmailSecretKey");
				if (string.IsNullOrEmpty(awsSecretKey))
				{
					throw new Exception("Could not load AWS email configuration. Configuration setting AwsEmailSecretKey has not been provided.");
				}

				_credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
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
		protected override bool LoadEmail()
		{
			const string methodName = "LoadEmail";

			// get the email template fields
			try
			{
				//todo: it would be nice if instead of pulling objects from the database sequentially (email, document, template) just to get 
				//the template fields, if we either maintain a cache OR create a method to grab all of the relevant fields at once.

				using (var svc = LWDataServiceUtil.EmailServiceInstance(Config.Organization, Config.Environment))
				using (var content = LWDataServiceUtil.ContentServiceInstance(Config.Organization, Config.Environment))
				{
					if (!string.IsNullOrEmpty(MailingName))
					{
						_logger.Debug(_className, methodName, "Loading mailing: " + MailingName);
						_emailDocument = svc.GetEmail(MailingName);
					}
					else
					{
						_emailDocument = svc.GetEmail(MailingId);
					}

					if (_emailDocument == null)
					{
						throw new Exception(string.Format("The email {0} does not exist.", string.IsNullOrEmpty(MailingName) ? MailingId.ToString() : MailingName));
					}

					if (!_emailDocument.DocumentId.HasValue)
					{
						throw new Exception(string.Format("The email {0} is invalid because no document has been configured.", string.IsNullOrEmpty(MailingName) ? MailingId.ToString() : MailingName));
					}

					var document = content.GetDocument(_emailDocument.DocumentId.Value);
					if (document == null)
					{
						throw new Exception(string.Format("Cannot send email {0} because it does not have an associated document.", string.IsNullOrEmpty(MailingName) ? MailingId.ToString() : MailingName));
					}

					var template = content.GetTemplate(document.TemplateID);

					if (template == null)
					{
						throw new Exception(string.Format("Cannot send email {0}. The template {1} does not exist.", string.IsNullOrEmpty(MailingName) ? MailingId.ToString() : MailingName, document.TemplateID));
					}

					MailingName = _emailDocument.Name;
					MailingId = _emailDocument.Id;

					//template.Fields.N
					var personalizations = new List<EmailPersonalization>();
					var fields = new FieldCollection(template.Fields);
					foreach (var field in fields.Fields)
					{
						personalizations.Add(new EmailPersonalization(MailingId, field.Name, field.Expression));
					}
					Personalizations = personalizations;
				}
			}
			catch (Exception ex)
			{
				string msg = "Failed to load email from Framework database: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw new Exception(msg, ex);
			}

			return true;
		}

		private string GetRecipientXml(Dictionary<string, string> recipient)
		{
			return new XElement("recipient", recipient.Select(o => new XElement(o.Key, o.Value))).ToString();
		}

		private void BuildAws(Member member, ref string emailAddress, Dictionary<string, string> personalizations, out Dictionary<string, string> updatedPersonalizations)
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
