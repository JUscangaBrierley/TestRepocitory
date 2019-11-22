using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class MergeMembers : OperationProviderBase
	{
		private const string _className = "MergeMembers";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public MergeMembers() : base("MergeMembers") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";

			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for terminate member.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				string eventName = args.ContainsKey("LoyaltyEvent") ? (string)args["LoyaltyEvent"] : string.Empty;
				string typeName = args.ContainsKey("LoyaltyCurrency") ? (string)args["LoyaltyCurrency"] : string.Empty;
				DateTime expDate = args.ContainsKey("ExpirationDate") ? (DateTime)args["ExpirationDate"] : DateTime.Now;
				MemberMergeOptions options = new MemberMergeOptions();
				if (args.ContainsKey("MemberMergeOptions"))
				{
					APIStruct mergeOptions = (APIStruct)args["MemberMergeOptions"];
					if (mergeOptions != null && mergeOptions.Parms != null)
					{
						if (mergeOptions.Parms.ContainsKey("MemberProfile_Name"))
						{
							options.MemberProfile_Name = (bool)mergeOptions.Parms["MemberProfile_Name"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberProfile_MailingAddress"))
						{
							options.MemberProfile_MailingAddress = (bool)mergeOptions.Parms["MemberProfile_MailingAddress"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberProfile_PrimaryPhoneNumber"))
						{
							options.MemberProfile_PrimaryPhoneNumber = (bool)mergeOptions.Parms["MemberProfile_PrimaryPhoneNumber"];
						}
						if (mergeOptions.Parms.ContainsKey("PointBalance"))
						{
							options.PointBalance = (bool)mergeOptions.Parms["PointBalance"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberRewards"))
						{
							options.MemberRewards = (bool)mergeOptions.Parms["MemberRewards"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberTiers"))
						{
							options.MemberTiers = (bool)mergeOptions.Parms["MemberTiers"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberCoupons"))
						{
							options.MemberCoupons = (bool)mergeOptions.Parms["MemberCoupons"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberBonuses"))
						{
							options.MemberBonuses = (bool)mergeOptions.Parms["MemberBonuses"];
						}
						if (mergeOptions.Parms.ContainsKey("MemberPromotions"))
						{
							options.MemberPromotions = (bool)mergeOptions.Parms["MemberPromotions"];
						}
						if (mergeOptions.Parms.ContainsKey("VirtualCards"))
						{
							options.VirtualCards = (bool)mergeOptions.Parms["VirtualCards"];
							if (mergeOptions.Parms.ContainsKey("FromPrimaryIsNewPrimaryVirtualCard"))
							{
								options.FromPrimaryIsNewPrimaryVirtualCard = (bool)mergeOptions.Parms["FromPrimaryIsNewPrimaryVirtualCard"];
							}
						}
					}
				}

				Member memberFrom = LoadMember(args, "MemberIdentity1", "FromMemberSearchType", "FromMemberSearchValue");
				Member memberTo = LoadMember(args, "MemberIdentity2", "ToMemberSearchType", "ToMemberSearchValue");

				PointEvent pe = LoyaltyDataService.GetPointEvent(eventName);
				if (pe == null)
				{
					throw new LWOperationInvocationException(string.Format("Unable to find member loyalty event with name = {0}.", eventName)) { ErrorCode = 3310 };
				}

				PointType pt = LoyaltyDataService.GetPointType(typeName);
				if (pt == null)
				{
					throw new LWOperationInvocationException(string.Format("Unable to find member loyalty currency with name = {0}.", typeName)) { ErrorCode = 3311 };
				}

				IMergeMemberInterceptor intcp = null;
				LWIntegrationDirectives.APIOperationDirective opDirective = Config.GetOperationDirective(Name) as LWIntegrationDirectives.APIOperationDirective;
				if (opDirective.Interceptor != null)
				{
					intcp = InterceptorUtil.GetInterceptor(opDirective.Interceptor) as IMergeMemberInterceptor;
				}

				if (intcp != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking BeforeMerge method of the interceptor.");
						intcp.BeforeMerge(memberFrom, memberTo);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by merge member interceptor.", ex);
						throw;
					}
				}

				Member member = LoyaltyDataService.MergeMember(memberFrom, memberTo, pe, pt, expDate, options);

				if (intcp != null)
				{
					try
					{
						_logger.Debug(_className, methodName, "Invoking AfterMerge method of the interceptor.");
						intcp.AfterMerge(memberFrom, memberTo);
					}
					catch (NotImplementedException)
					{
						// not implemented.
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception thrown by merge member interceptor.", ex);
						throw;
					}
				}

				Dictionary<string, object> context = new Dictionary<string, object>();
				context.Add("survivor", member); // survivor should be first because that is the member passed on to the rules.
				context.Add("victim", memberFrom);
				PostProcessSuccessfullInvocation(context);

				APIArguments resultParams = new APIArguments();
				resultParams.Add("member", member);
				response = SerializationUtils.SerializeResult(Name, Config, resultParams);
				return response;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
