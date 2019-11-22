using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class AuthenticateMember : OperationProviderBase
	{
		private const string _className = "AuthenticateMember";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        private static MobileGatewayDirectives.APIOperationDirective opDirective;
        private static WcfAuthenticationToken.AuthenticationScheme authScheme;

		public AuthenticateMember()
			: base("AuthenticateMember")
		{
		}

		public override void Initialize(string opName, MobileGatewayDirectives config, System.Collections.Specialized.NameValueCollection functionProviderParms)
		{
			base.Initialize(opName, config, functionProviderParms);
		}

		public override object Invoke(string source, WcfAuthenticationToken notoken, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length < 5)
			{
				string errMsg = "Invalid parameters provided for authentication.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string deviceType = (string)parms[0];
			string version = (string)parms[1];
			string username = (string)parms[2];
			string password = (string)parms[3];
			string resetCode = (string)parms[4];

			if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(resetCode))
			{
				string errMsg = "Invalid parameters provided for authentication. You must provide either password or reset code, but not both.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			authScheme = (WcfAuthenticationToken.AuthenticationScheme)Enum.Parse(typeof(WcfAuthenticationToken.AuthenticationScheme), GetFunctionParameter("AuthenticationScheme"));

			WcfAuthenticationToken token = WcfAuthenticationToken.GetAuthTokenFromCache(authScheme, username, string.IsNullOrEmpty(password) ? resetCode : password);

            if (opDirective == null)
                opDirective = Config.GetOperationDirectiveByName(Name) as MobileGatewayDirectives.APIOperationDirective;

            LoginStatusEnum loginStatus = LoginStatusEnum.Success;
			if (token == null)
			{
				IAuthenticateMemberInterceptor interceptor = null;
				if (opDirective.Interceptor != null)
				{
					interceptor = InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IAuthenticateMemberInterceptor;
				}

                bool unlockMember = StringUtils.FriendlyBool(GetFunctionParameter("UnlockLockedMember"), false);

                MGMemberUtils.Authenticate(interceptor, deviceType, version, authScheme, username, password, resetCode, ref token, out loginStatus, unlockMember);
				if (token != null)
				{
					token.MobileDeviceType = deviceType;
					token.MobileDeviceVersion = version;

					_logger.Debug(
						_className,
						methodName,
						string.Format(
							"User with credentials '{0}' has been authenticated and token {1} has been cached.",
							token.CachedMember != null ? token.CachedMember.IpCode.ToString() : "unknown member",
							token.TokenId));
				}
			}
			else
			{
				_logger.Debug(
					_className,
					methodName,
					string.Format(
						"Cached authentication token for '{0}' found",
						token.CachedMember != null ? token.CachedMember.IpCode.ToString() : "unknown member"));

                if (token.CachedMember != null)
                {
                    Member member = MGMemberUtils.LoadExistingMember(authScheme, token.CachedMember, InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IInboundMobileInterceptor);
                    if (member != null)
                    {
                        if (member.MemberStatus != MemberStatusEnum.Locked)
                        {
                            if (member.FailedPasswordAttemptCount > 0)
                            {
                                member.FailedPasswordAttemptCount = 0;
                                MGMemberUtils.SaveMember(member);
                            }
                        }
                        else
                        {
                            loginStatus = LoginStatusEnum.LockedOut;
                        }
                    }

                    if (token.CachedMember.PasswordChangeRequired)
                    {
                        loginStatus = LoginStatusEnum.PasswordResetRequired;
                    }
                }
			}
			return new MGAuthenticateMember()
			{
				Token = token != null ? token.TokenId : null,
				LoginStatus = loginStatus,
				StatusText = Enum.GetName(typeof(LoginStatusEnum), loginStatus)
			};
		}
	}
}