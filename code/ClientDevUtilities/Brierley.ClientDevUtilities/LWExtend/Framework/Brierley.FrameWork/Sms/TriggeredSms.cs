using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Dmc;
using Brierley.FrameWork.Messaging.Messages;

namespace Brierley.FrameWork.Sms
{
	/// <summary>
	/// The triggered sms  class is used to send triggered sms messages via DMC using Loyalty Navigator, or by invoking via the framework.
	/// </summary>
	/// <example>
	/// <code>
	///	using(TriggeredSms sms = new TriggeredSms("SmsNameOrID")) 
	/// {
	///		sms.Send(Member);
	/// }
	/// </code>
	/// <code>
	/// Dictionary<string,string> manualFields = new Dictionary<string,string>();
	/// manualFields.Add("RecipientPhone", "1112223333");
	/// manualFields.Add("Name", "John Smith");
	///	using(TriggeredSms sms = new TriggeredSms("SmsNameOrID")) 
	/// {
	///		sms.Send(manualFields);
	/// }
	/// </code>
	/// </example>
	public class TriggeredSms : IDisposable
	{
		private const string _className = "TriggeredSms";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private bool _dmcUseAlternateMobile = false;
		private DmcService _dmc = null;

		protected LWConfiguration Config { get; set; }
		protected long SmsId { get; set; }
		protected string SmsName { get; set; }
		protected IEnumerable<SmsPersonalization> Personalizations { get; set; }
		protected long? ExternalId { get; set; }

		protected static bool QueueingEnabled { get; private set; }

		static TriggeredSms()
		{
			QueueingEnabled = !LWConfigurationUtil.IsQueueProcessor && Messaging.MessagingBus.CanSend(typeof(SmsMessage));
		}

		protected TriggeredSms(ICommunicationLogger logger = null)
		{
			if (QueueingEnabled)
			{
				//we don't need initialization yet; sms is going to be queued
				return;
			}

			_dmc = new DmcService(logger ?? GetSmsLogger());
			string altMobile = LWConfigurationUtil.GetConfigurationValue(Constants.DmcUseAlternateMobile);
			if (!string.IsNullOrEmpty(altMobile))
			{
				//For alternate mobile, we need an exact boolean. Ignoring a simple mistake like "ttrue" can lead to some 
				//pretty serious problems (members in DMC with no alternate phone mean they'll never receive an SMS and this 
				//is a manual cleanup process), so we'll try to parse the value and throw an exception if we can't get a boolean.
				try
				{
					_dmcUseAlternateMobile = bool.Parse(altMobile);
				}
				catch (Exception ex)
				{
					throw new Exception(
						string.Format(
						"Could not convert configuration value of {0} into a boolean. Please ensure the configuration value is either empty or a valid boolean (true or false)",
						Brierley.FrameWork.Dmc.Constants.DmcUseAlternateMobile),
						ex);
				}
			}
		}

		/// <summary>
		/// Constructs a TriggeredEmail
		/// </summary>
		/// <param name="config">LWConfiguration used to connect to framework database</param>
		/// <param name="server">wrapper for SMS API</param>
		/// <param name="smsName">name of the triggered sms message</param>
		public TriggeredSms(LWConfiguration config, string smsName)
			: this()
		{
			Config = config;
			SmsName = smsName;
			LoadConfig();
			LoadSmsMessage();
		}

		/// <summary>
		/// Constructs a TriggeredSms
		/// </summary>
		/// <param name="config">LWConfiguration used to connect to framework database</param>
		/// <param name="server">wrapper for SMS API</param>
		/// <param name="smsId">ID of the triggered sms message</param>
		public TriggeredSms(LWConfiguration config, long smsId)
			: this()
		{
			Config = config;
			SmsId = smsId;
			LoadConfig();
			LoadSmsMessage();
		}

		/// <summary>
		/// Sends a triggered sms message to a loyalty member.
		/// </summary>
		/// <param name="member">The loyalty member the sms message will be sent to.</param>
		public async Task SendAsync(Member member)
		{
			await SendAsync(member, null);
		}

		[Obsolete("Use SendAsync instead. This method will be removed in a future release.")]
		public void Send(Member member)
		{
			Send(member, null);
		}

		/// <summary>
		/// Sends a triggered sms message to a loyalty member.
		/// </summary>
		/// <param name="member">The loyalty member the sms message will be sent to.</param>
		/// <param name="additionalFields">Name/value pair of fields, which can be used to override the value of any field in the sms message.</param>
		public async Task SendAsync(Member member, Dictionary<string, string> additionalFields)
		{
			string mobileNumber = ResolvePhoneNumber(member, additionalFields);
			if (!ValidateConsent(member, mobileNumber, additionalFields))
			{
				return;
			}
			Dictionary<string, string> updatedPersonalizations = new Dictionary<string, string>();
			BuildDmc(member, mobileNumber, additionalFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				SmsMessage s = new SmsMessage(SmsId, mobileNumber, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<SmsMessage>(s);
			}
			else
			{
				await SendDmcAsync(mobileNumber, updatedPersonalizations);
			}
		}

		[Obsolete("Use SendAsync instead. This method will be removed in a future release.")]
		public void Send(Member member, Dictionary<string, string> additionalFields)
		{
			string mobileNumber = ResolvePhoneNumber(member, additionalFields);
			if (!ValidateConsent(member, mobileNumber, additionalFields))
			{
				return;
			}
			Dictionary<string, string> updatedPersonalizations = new Dictionary<string, string>();
			BuildDmc(member, mobileNumber, additionalFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				SmsMessage s = new SmsMessage(SmsId, mobileNumber, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<SmsMessage>(s);
			}
			else
			{
				SendDmc(mobileNumber, updatedPersonalizations);
			}
		}

		/// <summary>
		/// Sends a triggered sms message.
		/// </summary>
		/// <remarks>
		///	This method can be used to send an sms message in the event that the sms message should not go to a loyalty 
		/// member (e.g., an alert which needs to be send to a customer service rep or administrator).
		/// </remarks>
		/// <param name="recipientPhone">The recipient's selecte mobile phone, if different from MemberDetails.MobilePhone.</param>
		/// <param name="manualFields">Name/value pair of fields.</param>
		public async Task SendAsync(string recipientPhone, Dictionary<string, string> manualFields)
		{
			recipientPhone = ResolvePhoneNumber(recipientPhone, manualFields);
			Dictionary<string, string> updatedPersonalizations = new Dictionary<string, string>();
			BuildDmc(null, recipientPhone, manualFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				SmsMessage s = new SmsMessage(SmsId, recipientPhone, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<SmsMessage>(s);
			}
			else
			{
				await SendDmcAsync(recipientPhone, updatedPersonalizations);
			}
		}

		[Obsolete("Use SendAsync instead. This method will be removed in a future release.")]
		public void Send(string recipientPhone, Dictionary<string, string> manualFields)
		{
			recipientPhone = ResolvePhoneNumber(recipientPhone, manualFields);
			Dictionary<string, string> updatedPersonalizations = new Dictionary<string, string>();
			BuildDmc(null, recipientPhone, manualFields, out updatedPersonalizations);
			if (QueueingEnabled)
			{
				SmsMessage s = new SmsMessage(SmsId, recipientPhone, updatedPersonalizations);
				Messaging.MessagingBus.Instance().Send<SmsMessage>(s);
			}
			else
			{
				SendDmc(recipientPhone, updatedPersonalizations);
			}
		}

		public void Dispose()
		{
			if (_dmc != null)
			{
				_dmc.Dispose();
			}
		}

		protected internal async Task SendDmcAsync(string mobilePhone, Dictionary<string, string> personalizations)
		{
			const string methodName = "SendDmcAsync";
			//bool smsOptIn = false;
			//bool smsDoubleOptIn = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("DmcSmsDoubleOptIn"));

			//determine recipient email address first

			bool caughtObjectExists = false;
			try
			{
				User user = await _dmc.GetUserByMobilePhoneAsync(mobilePhone);
				var attributes = new List<Dmc.Attribute>() { new Dmc.Attribute("sms_consent", "true") };
				if (user == null)
				{
					if (_dmcUseAlternateMobile)
					{
						attributes.Add(new Brierley.FrameWork.Dmc.Attribute("SendSms", mobilePhone));
					}
					user = _dmc.CreateUser(mobilePhone, attributes);
				}
				else
				{
					await _dmc.UpdateProfileByMobileNumberAsync(mobilePhone, attributes);
				}
				await _dmc.SendSingleAsync(ExternalId.Value, user, personalizations);
				_logger.Debug(_className, methodName, string.Format("_dmc.Send('{0}', '{1}')", ExternalId.Value, user.Id));
			}
			catch (Brierley.FrameWork.Dmc.Exceptions.ObjectAlreadyExistsException existsEx)
			{
				caughtObjectExists = true;
				string msg = "Failed to send sms message: " + existsEx.Message;
				_logger.Error(_className, methodName, msg, existsEx);
			}
			catch (Exception ex)
			{
				string msg = "Failed to send sms message: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
			if (caughtObjectExists)
			{
				await SendDmcAsync(mobilePhone, personalizations);
			}
		}

		protected internal void SendDmc(string mobilePhone, Dictionary<string, string> personalizations)
		{
			const string methodName = "SendDmc";

			if (string.IsNullOrEmpty(mobilePhone))
			{
				throw new ArgumentNullException("mobilePhone");
			}

			bool caughtObjectExists = false;
			try
			{
				User user = _dmc.GetUserByMobilePhone(mobilePhone);
				var attributes = new List<Dmc.Attribute>() { new Dmc.Attribute("sms_consent", "true") };
				if (user == null)
				{
					if (_dmcUseAlternateMobile)
					{
						attributes.Add(new Brierley.FrameWork.Dmc.Attribute("SendSms", mobilePhone));
					}
					user = _dmc.CreateUser(mobilePhone, attributes);
				}
				else
				{
					_dmc.UpdateProfileByMobileNumber(mobilePhone, attributes);
				}
				_dmc.SendSingle(ExternalId.Value, user, personalizations);
				_logger.Debug(_className, methodName, string.Format("_dmc.Send('{0}', '{1}')", ExternalId.Value, user.Id));
			}
			catch (Brierley.FrameWork.Dmc.Exceptions.ObjectAlreadyExistsException existsEx)
			{
				caughtObjectExists = true;
				string msg = "Failed to send sms message: " + existsEx.Message;
				_logger.Error(_className, methodName, msg, existsEx);
			}
			catch (Exception ex)
			{
				string msg = "Failed to send sms message: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
			if (caughtObjectExists)
			{
				SendDmc(mobilePhone, personalizations);
			}
		}

		/// <summary>
		/// Helper method for constructors.  Loads framework config values for SMS's API (username, password, url).
		/// </summary>
		protected void LoadConfig()
		{
			const string methodName = "LoadConfig";
			try
			{
				if (_dmc == null)
				{
					_dmc = new DmcService(GetSmsLogger());
				}
			}
			catch (Exception ex)
			{
				string msg = "Failed to load configuration for SMS: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw new Exception(msg, ex);
			}
		}

		/// <summary>
		/// Helper method for constructors.  Loads the sms message from the framework database and puts it into a listening state in DMC.
		/// </summary>
		/// <returns>True if the sms message is successfully loaded, else false</returns>
		protected bool LoadSmsMessage()
		{
			const string methodName = "LoadSms";

			// get the email template fields
			bool smsDocumentExists = false;
			try
			{
				using (var svc = LWDataServiceUtil.SmsServiceInstance())
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					SmsDocument smsDocument = null;
					if (!string.IsNullOrEmpty(SmsName))
					{
						_logger.Debug(_className, methodName, "Loading mailing: " + SmsName);
						smsDocument = svc.GetSmsMessage(SmsName);
					}
					else
					{
						smsDocument = svc.GetSmsMessage(SmsId);
					}

					if (smsDocument != null)
					{
						smsDocumentExists = true;
						SmsName = smsDocument.Name;
						SmsId = smsDocument.Id;

						if (!smsDocument.ExternalId.HasValue)
						{
							throw new Exception(string.Format("The sms message {0} is invalid because no external id has been configured.", string.IsNullOrEmpty(SmsName) ? SmsId.ToString() : SmsName));
						}
						ExternalId = smsDocument.ExternalId;
						Personalizations = svc.GetPersonalizations(smsDocument.Id);
					}
				}
			}
			catch (Exception ex)
			{
				string msg = "Failed to load sms message from Framework database: " + ex.Message;
				_logger.Error(_className, methodName, msg, ex);
				throw new Exception(msg, ex);
			}
			if (!smsDocumentExists)
			{
				string msg = "Failed to load sms message. The provided sms message ID does not exist in the framework database.";
				_logger.Error(_className, methodName, msg);
				throw new Exception(msg);
			}
			return true;
		}


		private string ResolvePhoneNumber(string mobileNumber, Dictionary<string, string> personalizations)
		{
			string ret = mobileNumber;
			bool overwrite = false;

			if (personalizations != null)
			{
				foreach (string k in personalizations.Keys)
				{
					if (k.Equals("recipientMobile", StringComparison.OrdinalIgnoreCase))
					{
						overwrite = true;
						ret = personalizations[k];
						break;
					}
				}
			}
			return ret;
		}

		private string ResolvePhoneNumber(Member member, Dictionary<string, string> personalizations)
		{
			const string methodName = "ResolvePhoneNumber";

			string ret = null;
			bool overwrite = false;

			if (personalizations != null)
			{
				foreach (string k in personalizations.Keys)
				{
					if (k.Equals("recipientMobile", StringComparison.OrdinalIgnoreCase))
					{
						overwrite = true;
						ret = personalizations[k];
					}
				}
			}

			if (!overwrite)
			{
				var details = member.GetChildAttributeSets("MemberDetails", false);
				if (details == null || details.Count == 0)
				{
					throw new Exception("Cannot send message to member. Member has no detail record and no phone number was provided.");
				}

				var detail = details[0];
				var meta = detail.GetMetaData();

				if (meta.Attributes.FirstOrDefault(o => o.Name.Equals("MobilePhone", StringComparison.OrdinalIgnoreCase)) == null)
				{
					//error - no mobile phone attribute defined
					throw new Exception("Cannot send message to member. MobilePhone is not defined in MemberDetails and no phone number was provided.");
				}

				string countryCode = null;
				if (meta.Attributes.FirstOrDefault(o => o.Name.Equals("MobilePhoneCountryCode", StringComparison.OrdinalIgnoreCase)) != null)
				{
					using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
					{
						LWCriterion lwCriteria = new LWCriterion("RefCountry");
						List<IClientDataObject> clientDataObjects = loyalty.GetAttributeSetObjects(null, "RefCountry", lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
						IClientDataObject item = clientDataObjects.Find(obj => obj.RowKey == Convert.ToInt64(detail.GetAttributeValue("MobilePhoneCountryCode")));
						if (item != null)
						{
							countryCode = item.GetAttributeValue("PhoneCode").ToString();
						}
					}
				}
				string phone = detail.GetAttributeValue("MobilePhone").ToString();

				if (string.IsNullOrEmpty(phone))
				{
					throw new Exception("Cannot send message to member. Member has no mobile phone entry and no phone number was provided.");
				}

				if (countryCode != null)
				{
					ret = countryCode + phone;
				}
				else
				{
					ret = phone;
					//this is a warning. It's possible the implementation has an interceptor that adds country code to 
					//the phone number. If the country code is missing from the phone number, then DMC will respond 
					//with an error, which will be logged as an exception.
					_logger.Warning(_className, methodName, "Member detail record has no mobile phone country code entry.");
				}
			}
			return ret;
		}
		
		private bool ValidateConsent(Member member, string mobilePhone, Dictionary<string, string> personalizations)
		{
			if (personalizations != null && personalizations.Count(o => o.Key.Equals("recipientmobile", StringComparison.OrdinalIgnoreCase)) > 0)
			{
				return true;
			}

			var detail = member.GetChildAttributeSets("MemberDetails")[0];
			var meta = detail.GetMetaData();

			bool smsOptIn = Convert.ToBoolean(detail.GetAttributeValue("SmsOptIn"));
			if (!smsOptIn)
			{
				return false;
			}

			bool smsDoubleOptIn = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("DmcSmsDoubleOptIn"));
			if (smsDoubleOptIn)
			{
				bool smsDoubleOptInComplete = detail.GetAttributeValue("SmsConsentChangeDate") != null;
				return smsDoubleOptInComplete;
			}
			return true;
		}
		
		private void BuildDmc(Member member, string mobilePhone, Dictionary<string, string> personalizations, out Dictionary<string, string> updatedPersonalizations)
		{
			const string methodName = "BuildDmc";

			if (string.IsNullOrEmpty(mobilePhone))
			{
				throw new ArgumentNullException("mobilePhone");
			}

			if (personalizations == null)
			{
				personalizations = new Dictionary<string, string>();
			}

			ContextObject context = new ContextObject() { Environment = { { "SmsName", SmsName } } };
			context.Owner = member;

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

			updatedPersonalizations = personalizations;
		}

		private ICommunicationLogger GetSmsLogger()
		{
			if (Config != null)
			{
				return LWDataServiceUtil.GetServiceConfiguration(Config.Organization, Config.Environment).SmsLogger;
			}
			return LWDataServiceUtil.GetServiceConfiguration().SmsLogger;
		}
	}
}
