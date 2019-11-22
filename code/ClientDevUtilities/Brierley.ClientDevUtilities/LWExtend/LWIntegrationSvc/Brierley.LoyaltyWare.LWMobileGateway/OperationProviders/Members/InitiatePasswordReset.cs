using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;


namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class InitiatePasswordReset : OperationProviderBase
	{
		private const string _className = "InitiatePasswordReset";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public InitiatePasswordReset() : base("InitiatePasswordReset") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string username = (string)parms[0];
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

			OperationProviderBase op = (OperationProviderBase)OperationProviderBase.GetOperationProvider(Config, "ConfirmPasswordReset");
			string emailName = op.GetFunctionParameter("ResetPasswordEmailName");
			string smsName = op.GetFunctionParameter("ResetPasswordSmsName");
			if (string.IsNullOrEmpty(emailName) && string.IsNullOrEmpty(smsName))
			{
				throw new LWIntegrationException("No channels have been configured for sending reset codes.");
			}

			Dictionary<string, string> resetOptions = LoyaltyService.GetPasswordResetOptions(member, emailName, smsName);

			// Return as JSON: {"email":"asdf@asdf.com", "sms":"1234567890"}
			var ret = new PasswordResetOptions();
			if (resetOptions.ContainsKey("email"))
			{
				ret.email = resetOptions["email"];
			}
			if (resetOptions.ContainsKey("sms"))
			{
				ret.sms = resetOptions["sms"];
			}

			return ret;
		}
	}
}