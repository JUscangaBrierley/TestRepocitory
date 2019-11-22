using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class ChangePassword : OperationProviderBase
	{
		public enum MemberLoadDirective { IpCode, AlternateId, EmailAddress, UserName, UseInterceptor }

		private const string _className = "ChangePassword";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public ChangePassword()
			: base("ChangePassword")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length != 3)
			{
				string errMsg = "Invalid parameters provided for ChangePassword.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string username = (string)parms[0];
			string oldPassword = (string)parms[1];
			string newPassword = (string)parms[2];

			_logger.Trace(_className, methodName, "Changing password for member: " + username);

			WcfAuthenticationToken.AuthenticationScheme authScheme = (WcfAuthenticationToken.AuthenticationScheme)Enum.Parse(typeof(WcfAuthenticationToken.AuthenticationScheme), GetFunctionParameter("AuthenticationScheme"));

			AuthenticationFields identityType = AuthenticationFields.Username;

			switch (authScheme)
			{
				case WcfAuthenticationToken.AuthenticationScheme.UsernameAndPassword:
					identityType = AuthenticationFields.Username;
					break;
				case WcfAuthenticationToken.AuthenticationScheme.EmailAndLoyaltyId:
					identityType = AuthenticationFields.LoyaltyIdNumber;
					break;
			}

			LoyaltyService.ChangeMemberPassword(identityType, username, oldPassword, newPassword, token.PasswordResetRequired);
			Member member = token.CachedMember;
			if (member.IpCode > 0)
			{
				string passwordChangedEmailName = GetFunctionParameter("PasswordChangedEmailName");
				if (!string.IsNullOrEmpty(passwordChangedEmailName))
				{
					try
					{
						// send password changed email
						using (var emailService = LWDataServiceUtil.EmailServiceInstance())
						{
							EmailDocument emailDoc = emailService.GetEmail(passwordChangedEmailName);
							if (emailDoc != null)
							{
								using (var email = TriggeredEmailFactory.Create(emailDoc.Id))
								{
									email.SendAsync(member).Wait();
									_logger.Debug(_className, methodName, "Password change email was sent to: " + member.PrimaryEmailAddress);
								}
							}
							else
							{
								_logger.Error(_className, methodName, "Unable to resolve emailDoc for name: " + passwordChangedEmailName);
							}
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error sending password changed email:" + ex.Message, ex);
					}
				}
				else
				{
					_logger.Debug(_className, methodName, "No password change email is configured.");
				}
			}

			string deviceType = token.MobileDeviceType;
			string version = token.MobileDeviceVersion;

			_logger.Debug(_className, methodName, string.Format("Removing existing authentication token {0} for username {1}.", token.TokenId, username));
			WcfAuthenticationToken.RemoveAuthTokenFromCache(token);

			if (!token.PasswordResetRequired)
			{
				// Member logged in normally
				LoginStatusEnum loginStatus;
				MGMemberUtils.Authenticate(null, deviceType, version, authScheme, username, newPassword, string.Empty, ref token, out loginStatus);
				string authToken = token.TokenId;
				string param1 = authScheme == WcfAuthenticationToken.AuthenticationScheme.UsernameAndPassword ? username : username + "/" + newPassword;
				_logger.Debug(_className, methodName, string.Format("Member with credentials '{0}' has been auto logged in with token {1}.", param1, token.TokenId));

				return authToken;
			}
			else
			{
				// Member logged in via temporary password or reset code
				return string.Empty;
			}
		}
	}
}