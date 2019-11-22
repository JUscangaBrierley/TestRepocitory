using System;
using System.Collections.Generic;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Bonuses;
using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons;
using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Messages;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class TriggerUserEvent : OperationProviderBase
	{
		private Brierley.FrameWork.CampaignManagement.CampaignRuleUtil _campaignUtil = null;

		public TriggerUserEvent() : base("TriggerUserEvent") { }

		public List<long> ExtractOutput(string type, object result)
		{
			InitializeCampaignUtility();
			return _campaignUtil.ExtractResultContent(type, result);
		}

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided for trigger user event.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				string eventName = (string)args["EventName"];
				string channel = (string)args["Channel"];
				string language = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();

				bool returnCoupons = args.ContainsKey("ReturnCoupons") ? (bool)args["ReturnCoupons"] : false;
				bool returnAllCoupons = args.ContainsKey("ReturnAllCoupons") ? (bool)args["ReturnAllCoupons"] : false;
				bool returnBonuses = args.ContainsKey("ReturnBonuses") ? (bool)args["ReturnBonuses"] : false;
				bool returnAllBonuses = args.ContainsKey("ReturnAllBonuses") ? (bool)args["ReturnAllBonuses"] : false;
				bool returnPromotions = args.ContainsKey("ReturnPromotions") ? (bool)args["ReturnPromotions"] : false;
				bool returnAllPromotions = args.ContainsKey("ReturnAllPromotions") ? (bool)args["ReturnAllPromotions"] : false;
				bool returnMessages = args.ContainsKey("ReturnMessages") ? (bool)args["ReturnMessages"] : false;
				bool returnAllMessages = args.ContainsKey("ReturnAllMessages") ? (bool)args["ReturnAllMessages"] : false;

				Member member = LoadMember(args);

				LWEvent lwEvent = LoyaltyDataService.GetLWEventByName(eventName);
				if (lwEvent == null)
				{
					throw new LWOperationInvocationException(string.Format("Unable to find user defined event name = {0}.", eventName)) { ErrorCode = 3382 };
				}

				Dictionary<string, object> contextMap = new Dictionary<string, object>();
				contextMap.Add("Channel", channel);
				contextMap.Add("Language", language);
				if (args.ContainsKey("ClientContext"))
				{
					APIStruct[] attList = (APIStruct[])args["ClientContext"];
					foreach (APIStruct att in attList)
					{
						contextMap.Add((string)att.Parms["ParmName"], att.Parms["ParmValue"]);
					}
				}

				ContextObject context = new ContextObject()
				{
					Owner = member,
					Environment = contextMap,
				};
				LoyaltyDataService.ExecuteEventRules(context, eventName);

				APIArguments resultParams = new APIArguments();

				decimal balance = member.GetPoints(null, null, null, null);
				resultParams.Add("CurrencyBalance", balance);
				resultParams.Add("MemberStatus", Enum.GetName(typeof(MemberStatusEnum), member.MemberStatus));
				MemberTier tier = member.GetTier(DateTime.Now);
				if (tier != null)
				{
					TierDef def = LoyaltyDataService.GetTierDef(tier.TierDefId);
					resultParams.Add("CurrentTierName", def.Name);
					resultParams.Add("CurrentTierExpirationDate", tier.ToDate);
				}
				resultParams.Add("CurrencyToNextTier", member.GetPointsToNextTier());
				if (member.LastActivityDate != null)
				{
					resultParams.Add("LastActivityDate", member.LastActivityDate);
				}

				IList<MemberMessage> messages = null;
				IList<MemberCoupon> coupons = null;
				IList<MemberBonus> bonuses = null;
				IList<MemberPromotion> promotions = null;

				string xmlResult = null;

				TriggerUserEventUtil.ExtractTriggerUserEventOutput(
					returnMessages, 
					returnAllMessages, 
					out messages,
					returnCoupons, 
					returnAllCoupons, 
					out coupons,
					returnBonuses, 
					returnAllBonuses, 
					out bonuses,
					returnPromotions, 
					returnAllPromotions, 
					out promotions,
					member, 
					language, 
					channel, 
					context.Results, 
					out xmlResult);

				if (coupons != null && coupons.Count > 0)
				{
					APIStruct[] memberCoupons = new APIStruct[coupons.Count];
					int msgIdx = 0;
					foreach (MemberCoupon coupon in coupons)
					{
						memberCoupons[msgIdx++] = CouponHelper.SerializeMemberCoupon(member, language, channel, coupon, false);
					}
					resultParams.Add("MemberCoupon", memberCoupons);
				}

				if (bonuses != null && bonuses.Count > 0)
				{
					APIStruct[] memberBonuses = new APIStruct[bonuses.Count];
					int msgIdx = 0;
					foreach (MemberBonus bonus in bonuses)
					{
						memberBonuses[msgIdx++] = BonusUtil.SerializeMemberBonus(language, channel, bonus);
					}
					resultParams.Add("MemberBonus", memberBonuses);
				}

				if (promotions != null && promotions.Count > 0)
				{
					APIStruct[] memberPromotions = new APIStruct[promotions.Count];
					//int msgIdx = 0;
					//foreach (MemberPromotion bonus in promotions)
					//{
					//    //memberPromotions[msgIdx++] = BonusUtil.SerializeMemberBonus(language, channel, bonus);
					//}
					resultParams.Add("MemberBonus", memberPromotions);
				}

				if (messages != null && messages.Count > 0)
				{
					APIStruct[] memberMessages = new APIStruct[messages.Count];
					int msgIdx = 0;
					foreach (MemberMessage message in messages)
					{
						memberMessages[msgIdx++] = MessageUtil.SerializeMemberMessage(member, language, channel, message);
					}
					resultParams.Add("MemberMessage", memberMessages);
				}

				response = SerializationUtils.SerializeResult(Name, Config, resultParams);

				TriggerUserEventLog log = new TriggerUserEventLog()
				{
					MemberId = member.IpCode,
					EventName = eventName,
					Channel = channel,
				};

				log.Context = log.SerializeContext(contextMap);
				log.RulesExecuted = log.SerailizeRuleExecutionLogIds(context.Results);
				log.Result = xmlResult;

				EventLogger.RequestQueue.Add(log);

				Dictionary<string, object> postContext = new Dictionary<string, object>();
				postContext.Add("member", member);
				if (coupons != null && coupons.Count > 0)
				{
					postContext.Add("coupons", coupons);
				}
				if (bonuses != null && bonuses.Count > 0)
				{
					postContext.Add("bonuses", bonuses);
				}
				if (promotions != null && promotions.Count > 0)
				{
					postContext.Add("promotions", promotions);
				}
				if (messages != null && messages.Count > 0)
				{
					postContext.Add("messages", messages);
				}
				PostProcessSuccessfullInvocation(postContext);

				return response;
			}
			catch (LWOperationInvocationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message);
			}
		}

		private void InitializeCampaignUtility()
		{
			if (_campaignUtil == null)
			{
				_campaignUtil = new Brierley.FrameWork.CampaignManagement.CampaignRuleUtil();
			}
		}
	}
}
