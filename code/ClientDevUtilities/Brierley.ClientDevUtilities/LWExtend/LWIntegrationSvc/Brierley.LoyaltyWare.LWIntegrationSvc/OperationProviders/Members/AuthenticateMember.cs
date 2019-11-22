using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class AuthenticateMember : OperationProviderBase
    {
        #region Fields
        private const string _className = "AuthenticateMember";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public AuthenticateMember() : base("AuthenticateMember") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
			const string methodName = "Invoke";
            try
            {
                if (string.IsNullOrEmpty(parms))
                {
					string msg = "No parameters provided for authenticate member.";
					_logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string identityTypeString = (string)args["IdentityType"];
				string identity = (string)args["Identity"];
				string password = args.ContainsKey("Password") ? (string)args["Password"] : string.Empty;
                string resetCode = args.ContainsKey("ResetCode") ? (string)args["ResetCode"] : string.Empty;
				if (string.IsNullOrEmpty(identityTypeString))
                {
					string msg = "No identityType provided for authenticate member.";
					_logger.Error(_className, methodName, msg);
					throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
                }
				if (string.IsNullOrEmpty(identity))
				{
					string msg = "No identity provided for authenticate member.";
					_logger.Error(_className, methodName, msg);
					throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
				}

				AuthenticationFields identityType = AuthenticationFields.Username;
				if (!string.IsNullOrEmpty(identityTypeString) && "emailaddress".Equals(identityTypeString.ToLower()))
				{
					identityType = AuthenticationFields.PrimaryEmailAddress;
				}
				else
				{
					try
					{
						identityType = (AuthenticationFields)Enum.Parse(typeof(AuthenticationFields), identityTypeString, true);
					}
					catch (Exception ex)
					{
						string msg = string.Format("Invalid identityType '{0}' provided for authenticate member.", identityTypeString);
						_logger.Error(_className, methodName, msg + ": " + ex.Message, ex);
						throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
					}
				}

				IAuthenticateMemberInterceptor interceptor = null;
				LWIntegrationDirectives.APIOperationDirective opDirective = Config.GetOperationDirective(Name) as LWIntegrationDirectives.APIOperationDirective;
				if (opDirective.Interceptor != null)
				{
					interceptor = InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IAuthenticateMemberInterceptor;
				}

				if (interceptor != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking BeforeAuthenticate method of the interceptor.");
						interceptor.BeforeAuthenticate(identityType, identity, password, resetCode);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by BeforeAuthenticate interceptor.", ex);
						throw;
					}
				}

                bool unlockMember = StringUtils.FriendlyBool(opDirective.FunctionProviderParms["UnlockLockedMember"], false);

                LoginStatusEnum loginStatus = LoginStatusEnum.Failure;
				bool authenticated = false;
				Member member = null;
                APIArguments resultParams = new APIArguments();
				try
				{
                    member = LoyaltyDataService.LoginMember(identityType, identity, password, resetCode, ref loginStatus, unlockMember);
                    authenticated = (loginStatus == LoginStatusEnum.Success || loginStatus == LoginStatusEnum.PasswordResetRequired);
                    string msg = loginStatus == LoginStatusEnum.PasswordResetRequired ? "Password reset required" : string.Empty;
                    resultParams.Add("StatusText", msg);
				}
				catch (AuthenticationException ex)
				{
					if (interceptor != null)
					{
						try
						{
							_logger.Debug(_className, methodName, "Invoking HandleAuthenticationException method of the interceptor.");
							authenticated = interceptor.HandleAuthenticationException(member, loginStatus, ex);
						}
						catch (NotImplementedException)
						{
							// not implemented.
						}
						catch (Exception ex2)
						{
							_logger.Error(_className, methodName, "Exception thrown by HandleAuthenticationException interceptor.", ex2);
							throw;
						}
					}

					if (!authenticated)
					{
						string msg = "Authentication error:" + ex.Message;
						_logger.Error(_className, methodName, msg, ex);
                        resultParams.Add("StatusText", msg);
					}
				}

				if (interceptor != null && authenticated)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking AfterAuthenticateOK method of the interceptor.");
						interceptor.AfterAuthenticateOK(member, loginStatus);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by AfterAuthenticateOK interceptor.", ex);
						throw;
					}
				}

				resultParams.Add("Authenticated", authenticated);
                resultParams.Add("LoginStatus", loginStatus);
				string response = SerializationUtils.SerializeResult(Name, Config, resultParams);
				return response;
            }
            catch (LWOperationInvocationException)
            {
                throw;
            }
            catch (Exception ex)
            {
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
                throw new LWOperationInvocationException(ex.Message);
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
