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
    public class PasswordResetOptions : OperationProviderBase
    {
        private const string _className = "PasswordResetOptions";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public PasswordResetOptions() : base("PasswordResetOptions") { }

        public override string Invoke(string source, string parms)
        {
			const string methodName = "Invoke";
            try
            {
                if (string.IsNullOrEmpty(parms))
                {
					string msg = "No parameters provided for password reset options.";
					_logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string identityTypeString = (string)args["IdentityType"];
				string identity = (string)args["Identity"];
                // Validate identity type and identity
				if (string.IsNullOrEmpty(identityTypeString))
                {
                    string msg = "No identityType provided for password reset options.";
					_logger.Error(_className, methodName, msg);
					throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
                }
				if (string.IsNullOrEmpty(identity))
				{
                    string msg = "No identity provided for password reset options.";
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
						string msg = string.Format("Invalid identityType '{0}' provided for password reset options.", identityTypeString);
						_logger.Error(_className, methodName, msg + ": " + ex.Message, ex);
						throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
					}
				}

				LWIntegrationDirectives.APIOperationDirective opDirective = Config.GetOperationDirective("PasswordReset") as LWIntegrationDirectives.APIOperationDirective;
                string emailName = opDirective.FunctionProviderParms["ResetPasswordEmailName"];
                string smsName = opDirective.FunctionProviderParms["ResetPasswordSmsName"];
                if (string.IsNullOrEmpty(emailName) && string.IsNullOrEmpty(smsName))
                    throw new LWIntegrationException("No channels have been configured for sending reset codes.");

				Member member = LoyaltyDataService.LoadMemberFromIdentity(identityType, identity);

                Dictionary<string, string> resetOptions = LoyaltyDataService.GetPasswordResetOptions(member, emailName, smsName);

				APIArguments resultParams = new APIArguments();
                if (resetOptions.ContainsKey("email"))
                    resultParams.Add("Email", resetOptions["email"]);
                if (resetOptions.ContainsKey("sms"))
                    resultParams.Add("SMS", resetOptions["sms"]);
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
    }
}
