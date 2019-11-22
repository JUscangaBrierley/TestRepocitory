using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.FrameWork.Sms;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class PasswordReset : OperationProviderBase
    {
        private const string _className = "PasswordReset";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private const int _maxmimumMinutes = 60; // As a design decision, we're limiting the max to 60 minutes.

        public PasswordReset() : base("PasswordReset") { }

        public override string Invoke(string source, string parms)
        {
			const string methodName = "Invoke";
            try
            {
                if (string.IsNullOrEmpty(parms))
                {
                    string msg = "No parameters provided for password reset.";
                    _logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string identityTypeString = (string)args["IdentityType"];
                string identity = (string)args["Identity"];
                string channel = (string)args["Channel"];
                // Validate identity type and identity
                if (string.IsNullOrEmpty(identityTypeString))
                {
                    string msg = "No identityType provided for password reset.";
                    _logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
                }
                if (string.IsNullOrEmpty(identity))
                {
                    string msg = "No identity provided for password reset.";
                    _logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
                }
                if (string.IsNullOrEmpty(channel))
                {
                    channel = "email";
                }

                int minutes;
                string minutesParm = GetFunctionParameter("ResetCodeExpirationMinutes");
                if (!int.TryParse(minutesParm, out minutes))
                {
                    string msg = string.Format("Invalid value '{0}' for reset code expiration minutes.", minutesParm);
                    _logger.Error(_className, methodName, msg);
                    throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
                }
                if (minutes > _maxmimumMinutes)
                {
                    string msg = string.Format("Reset code expiration minutes is currently set higher than the maximum allowed value. Defaulting the reset code expiration minutes to {0}. Current: {1}. Maximum: {0}", _maxmimumMinutes, minutes);
                    _logger.Warning(_className, methodName, msg);
                    minutes = _maxmimumMinutes;
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

                Member member = LoyaltyDataService.LoadMemberFromIdentity(identityType, identity);

                // Determine if we're sending an sms or email
                switch (channel)
                {
                    case "sms":
                        // Get the configured sms message name
                        string smsName = GetFunctionParameter("ResetPasswordSmsName");
                        if (!string.IsNullOrEmpty(smsName))
                        {
                            string resetCode = LoyaltyDataService.GenerateMemberResetCode(member, minutes);
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
                            string resetCode = LoyaltyDataService.GenerateMemberResetCode(member, minutes);
                            // Send an email message
                            using (ITriggeredEmail email =  TriggeredEmailFactory.Create(emailName))
                            {
                                Dictionary<string, string> emailFields = new Dictionary<string, string>();
                                email.SendAsync(member, emailFields).Wait();
                            }
                        }
                        break;
                }

                return null;
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
