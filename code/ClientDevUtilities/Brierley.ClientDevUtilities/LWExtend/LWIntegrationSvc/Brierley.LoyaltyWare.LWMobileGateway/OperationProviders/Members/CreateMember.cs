﻿using System;
using System.Collections.Generic;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
	public class CreateMember : OperationProviderBase
	{
		public enum MemberLoadDirective
		{
			IpCode,
			AlternateId,
			EmailAddress,
			UserName,
			UseInterceptor
		}

		private const string _className = "CreateMember";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public CreateMember()
			: base("CreateMember")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken notoken, object[] parms)
		{
			const string methodName = "Invoke";

			if (parms == null || parms.Length != 5)
			{
				string errMsg = "Invalid parameters provided for CreateMember.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			MGMember member = parms[0] as MGMember;
			string password = (string)parms[1];
			string modeStr = (string)parms[2];
			RuleExecutionMode mode = RuleExecutionMode.Real;
			if (!string.IsNullOrEmpty(modeStr))
			{
				modeStr = modeStr.ToLower();
				if (modeStr != "real" && modeStr != "simulation")
				{
					string err = string.Format("Invalid execution mode {0} specified.  Valid values are real or simulation", modeStr);
					throw new LWIntegrationException(err) { ErrorCode = 3231 };
				}
				if (modeStr == "simulation")
				{
					mode = RuleExecutionMode.Simulation;
				}
			}
			_logger.Debug(_className, methodName, "Execution mdoe = " + mode.ToString());

			//MGMember member = parms[0] as MGMember; // null;
			if (member == null)
			{
				string errMsg = string.Format("No member provided for creating member.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 1 };
			}

			if (string.IsNullOrEmpty(password))
			{
				string errMsg = string.Format("No password provided when creating new member.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 1 };
			}

			MobileGatewayDirectives.APIOperationDirective opDirective = Config.GetOperationDirectiveByName(Name) as MobileGatewayDirectives.APIOperationDirective;
			IInboundMobileInterceptor interceptor = null;
			if (opDirective.Interceptor != null)
			{
				interceptor = InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IInboundMobileInterceptor;
				if (interceptor == null)
				{
					_logger.Error(_className, methodName, "Unable to instantiate interceptor.");
					throw new LWException("Unable to instantiate interceptor.") { ErrorCode = 1 };
				}
			}

			string loadDirectiveStr = GetFunctionParameter("LoadDirective");
			if (string.IsNullOrEmpty(loadDirectiveStr))
			{
				string errMsg = string.Format("No load directives specified to check for existing member.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 1 };
			}

			Member existing = MGMemberUtils.LoadExistingMember(loadDirectiveStr, member, interceptor);
			if (existing != null)
			{
				string errMsg = string.Format("This member already exists.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg) { ErrorCode = 1 };
			}

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				if (interceptor != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking ProcessMemberBeforePopulation on interceptor.");
						member = (MGMember)interceptor.ProcessMemberBeforePopulation(Config, member);
					}
					catch (Exception iex)
					{
						string msg = string.Format("Error generated by {0}.ProcessMemberBeforePopulation", interceptor.GetType().ToString());
						_logger.Error(_className, methodName, msg, iex);
						throw;
					}
				}

				Member newMember = member.PopulateMember(Config, opDirective);
				newMember.MemberCreateDate = DateTime.Now;
				newMember.ChangedBy = "Mobile Gateway Service";

				if (interceptor != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking ProcessMemberBeforeSave on interceptor.");
						interceptor.ProcessMemberBeforeSave(Config, newMember);
					}
					catch (Exception iex)
					{
						string msg = string.Format("Error generated by {0}.ProcessMemberBeforeSave", interceptor.GetType().ToString());
						_logger.Error(_className, methodName, msg, iex);
						throw;
					}
				}

				// Set the member's password
				WcfAuthenticationToken.AuthenticationScheme authScheme = (WcfAuthenticationToken.AuthenticationScheme)Enum.Parse(typeof(WcfAuthenticationToken.AuthenticationScheme), GetFunctionParameter("AuthenticationScheme"));
				loyalty.ChangeMemberPassword(newMember, password, false);

				var results = new List<ContextObject.RuleResult>();
				loyalty.SaveMember(newMember, results, mode);

				if (interceptor != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking ProcessMemberAfterSave on interceptor.");
						interceptor.ProcessMemberAfterSave(Config, newMember, results);
					}
					catch (Exception iex)
					{
						string msg = string.Format("Error generated by {0}.ProcessMemberAfterSave", interceptor.GetType().ToString());
						_logger.Error(_className, methodName, msg, iex);
						throw;
					}
				}
				_logger.Debug(_className, methodName, "Saved member with Ipcode " + newMember.MyKey.ToString());

				WcfAuthenticationToken token = null;
				LoginStatusEnum loginStatus = LoginStatusEnum.Failure;
				if (!string.IsNullOrEmpty(GetFunctionParameter("AutoLogin")))
				{
					bool autoLogin = bool.Parse(GetFunctionParameter("AutoLogin"));
					if (autoLogin)
					{
						string param1 = authScheme == WcfAuthenticationToken.AuthenticationScheme.UsernameAndPassword ? newMember.Username : newMember.Username + "/" + password;
						string deviceType = (string)parms[3];
						string version = (string)parms[4];
						MGMemberUtils.Authenticate(null, deviceType, version, authScheme, newMember.Username, password, string.Empty, ref token, out loginStatus);
						_logger.Debug(_className, methodName, string.Format("Member with credentials '{0}' has been auto logged in with token {1}.", param1, token != null ? token.TokenId : "null"));
					}
				}
				return new MGAuthenticateMember()
				{
					Token = token != null ? token.TokenId : null,
					LoginStatus = LoginStatusEnum.Success,
					StatusText = Enum.GetName(typeof(LoginStatusEnum), loginStatus)
				};
			}
		}
	}
}