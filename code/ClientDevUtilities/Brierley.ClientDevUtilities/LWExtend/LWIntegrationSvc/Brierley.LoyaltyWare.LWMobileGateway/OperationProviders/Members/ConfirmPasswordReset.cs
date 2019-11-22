using System;
using System.Collections.Generic;
using System.Net;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.FrameWork.Sms;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;


namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class ConfirmPasswordReset : OperationProviderBase
	{
		private const string _className = "ConfirmPasswordReset";
		private const int _maxmimumMinutes = 60; // As a design decision, we're limiting the max to 60 minutes.
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public ConfirmPasswordReset() : base("ConfirmPasswordReset") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			string username = (string)parms[0];

			string channel = ((string)parms[1] ?? string.Empty).ToLower();
			if (string.IsNullOrEmpty(channel))
			{
				channel = "email";
			}
			else if (!channel.Equals("email") && !channel.Equals("sms"))
			{
				throw new LWIntegrationException(string.Format("Unknown channel value: {0}", channel));
			}

			int minutes;
			string minutesParm = GetFunctionParameter("ResetCodeExpirationMinutes");
			if (!int.TryParse(minutesParm, out minutes))
			{
				string msg = string.Format("Invalid value '{0}' for reset code expiration minutes.", minutesParm);
				_logger.Error(_className, methodName, msg);
				throw new LWIntegrationException(msg);
			}
			if (minutes > _maxmimumMinutes)
			{
				string msg = string.Format("Reset code expiration minutes is currently set higher than the maximum allowed value. Defaulting the reset code expiration minutes to {0}. Current: {1}. Maximum: {0}", _maxmimumMinutes, minutes);
				_logger.Warning(_className, methodName, msg);
				minutes = _maxmimumMinutes;
			}

			WcfAuthenticationToken.AuthenticationScheme authScheme =
				(WcfAuthenticationToken.AuthenticationScheme)Enum.Parse(typeof(WcfAuthenticationToken.AuthenticationScheme), GetFunctionParameter("AuthenticationScheme"));

			// Look up member
			Member member = null;
			switch (authScheme)
			{
				case WcfAuthenticationToken.AuthenticationScheme.UsernameAndPassword:
					member = LoyaltyService.LoadMemberFromUserName(username);
					break;
				case WcfAuthenticationToken.AuthenticationScheme.EmailAndLoyaltyId:
					member = LoyaltyService.LoadMemberFromLoyaltyID(username);
					break;
			}

			if (member == null)
			{
				SetResponseCode(HttpStatusCode.NotFound);
				return null;
			}

			// Determine if we're sending an sms or email
			switch (channel)
			{
				case "sms":
					// Get the configured sms message name
					string smsName = GetFunctionParameter("ResetPasswordSmsName");
					if (!string.IsNullOrEmpty(smsName))
					{
						string resetCode = LoyaltyService.GenerateMemberResetCode(member, minutes);
						// Send an sms message
						using (TriggeredSms sms = new TriggeredSms(LWConfigurationUtil.GetCurrentConfiguration(), smsName))
						{
							Dictionary<string, string> smsFields = new Dictionary<string, string>();
							sms.Send(member, smsFields);
						}
					}
					break;

				case "email":
					// Get the configurated email message name
					string emailName = GetFunctionParameter("ResetPasswordEmailName");
					if (!string.IsNullOrEmpty(emailName))
					{
						string resetCode = LoyaltyService.GenerateMemberResetCode(member, minutes);
						// Send an email message
						using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailName))
						{
							Dictionary<string, string> emailFields = new Dictionary<string, string>();
							email.SendAsync(member, emailFields).Wait();
						}
					}
					break;
			}

			return null;
		}
	}
}