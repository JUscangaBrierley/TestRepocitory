using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Used to determine whether or not an item has been issued to the member within the current rule event's execution context.
	/// </summary>
	/// <example>
	///     Usage : IsItemIssued('Item type', 'Item name')
	/// </example>
	/// <remarks>
	/// This function's intention is to be used to determine whether or not an item was issued in the current execution context, meaning withing the
	/// event that triggered a series of rules, being a manual event or a insert/update of an attribute set row. It cannot be used to determine
	/// whether or not an item was issued in the distant past (e.g., was a reward issued last week). Once the current context is cleared, knowledge
	/// of any issuance is also lost. An example of this function's usage:
	/// 
	///		A transaction row insert triggers a series of rules. One rule will execute a real-time campaign, which may issue any number of coupons
	///		and messages.
	///		
	///		A second rule will issue a coupon, but should only issue the coupon if no other coupons or messages have been issued by other rules in
	///		the current event. The issue coupon rule's execution expression is set to:
	///		
	///			!IsItemIssued('Coupon') & !IsItemIssued('Message')
	///			
	///		This prevents the rule from executing if any coupons or messages have been issued.
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Used to determine whether or not an item has been issued to the member within the current execution context.",
		DisplayName = "IsItemIssued",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,
		WizardDescription = "Has an item been issued?",
		AdvancedWizard = true,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "ItemType", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "What type of item?", Helpers = ParameterHelpers.RuleTriggerIssueType)]
	[ExpressionParameter(Order = 1, Name = "Name", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "What name?")]
	public class IsItemIssued : UnaryOperation
	{
		private Expression _itemType = null;
		private Expression _itemName = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public IsItemIssued()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="expression">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal IsItemIssued(Expression expression)
			: base("IsItemIssued", expression)
		{
			if (expression == null)
			{
				return;
			}

			if (expression is ParameterList)
			{
				ParameterList plist = (ParameterList)expression;
				if (plist.Expressions.Length > 0)
				{
					_itemType = plist.Expressions[0];
				}

				if (plist.Expressions.Length > 1)
				{
					_itemName = plist.Expressions[1];
				}
			}
			else
			{
				_itemType = expression;
			}			
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "IsItemIssued('<Bonus|Message|Reward>', '<item name>')";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			//todo: campaign results currently do not track surveys, emails and SMS issued (possibly others, too). This function will not work completely until
			//we have full tracking of what's issued by CampaignWare. This should work fine for LoD, though, which is why this function exists to begin with.

			Member member = ResolveMember(contextObject.Owner);
			if (member == null)
			{
				throw new CRMException("IsItemIssued must be evaluated in the context of a loyalty member.");
			}

			RuleTriggerIssueType? itemType = null;
			string itemName = null;
			long itemId = 0;

			if (_itemType != null)
			{
				object type = _itemType.evaluate(contextObject);
				if (type != null)
				{
					string t = type.ToString();
					RuleTriggerIssueType issueType = RuleTriggerIssueType.Points;
					if (!string.IsNullOrEmpty(t))
					{
						if (!Enum.TryParse<RuleTriggerIssueType>(t, out issueType))
						{
							throw new Exception(string.Format("Invalid item type '{0}' passed to IsItemIssued.", t));
						}
						itemType = issueType;
					}
				}
			}
						
			if (_itemName != null)
			{
				itemName = (_itemName.evaluate(contextObject) ?? string.Empty).ToString();
				if (!string.IsNullOrEmpty(itemName))
				{
					if (itemType == 0)
					{
						throw new Exception(string.Format("Attempted to search for item with name '{0}' but no type identified. When looking for items by name, a valid type must be specified", itemName));
					}

					switch (itemType)
					{
						case RuleTriggerIssueType.Coupon:
							using (var content = LWDataServiceUtil.ContentServiceInstance())
							{
								var coupon = content.GetCouponDef(itemName);
								if (coupon == null)
								{
									throw new Exception(string.Format("Coupon '{0}' does not exist.", itemName));
								}
								itemId = coupon.Id;
							}
							break;
						case RuleTriggerIssueType.Email:
							using (var svc = LWDataServiceUtil.EmailServiceInstance())
							{
								var email = svc.GetEmail(itemName);
								if (email == null)
								{
									throw new Exception(string.Format("Email '{0}' does not exist.", itemName));
								}
								itemId = email.Id;
							}
							break;
						case RuleTriggerIssueType.Message:
							using (var svc = LWDataServiceUtil.ContentServiceInstance())
							{
								var message = svc.GetMessageDef(itemName);
								if (message == null)
								{
									throw new Exception(string.Format("Message '{0}' does not exist.", itemName));
								}
								itemId = message.Id;
							}
							break;
                        case RuleTriggerIssueType.Notification:
                            using (var svc = LWDataServiceUtil.ContentServiceInstance())
                            {
                                var notification = svc.GetNotificationDef(itemName);
                                if (notification == null)
                                {
                                    throw new Exception(string.Format("Notification '{0}' does not exist.", itemName));
                                }
                                itemId = notification.Id;
                            }
                            break;
                        case RuleTriggerIssueType.Points:
							throw new NotSupportedException("Cannot specify an item name when searching for awarded points. Use IsItemIssued('Points') instead.");
						case RuleTriggerIssueType.Promotion:
							using (var svc = LWDataServiceUtil.ContentServiceInstance())
							{
								var promotion = svc.GetPromotionByName(itemName);
								if (promotion == null)
								{
									throw new Exception(string.Format("Promotion '{0}' does not exist.", itemName));
								}
								itemId = promotion.Id;
							}
							break;
						case RuleTriggerIssueType.Reward:
							// reward result sets the reward name, so there's no need to get the id
							break;
						case RuleTriggerIssueType.Sms:
							using (var svc = LWDataServiceUtil.SmsServiceInstance())
							{
								var sms = svc.GetSmsMessage(itemName);
								if (sms == null)
								{
									throw new Exception(string.Format("SMS '{0}' does not exist.", itemName));
								}
								itemId = sms.Id;
							}
							break;
						case RuleTriggerIssueType.Survey:
							using (var svc = LWDataServiceUtil.SurveyManagerInstance())
							{
								var survey = svc.RetrieveSurvey(itemName);
								if (survey == null)
								{
									throw new Exception(string.Format("Survey '{0}' does not exist.", itemName));
								}

								itemId = survey.ID;
							}
							break;
						case RuleTriggerIssueType.Tier:
							//itemName can be used to specify the name of the issued tier. EvaluateTierRuleResult stores the new tier as a string. 
							//a supplied name can be used (later) to check the tier name.
							break;
						case RuleTriggerIssueType.VirtualCard:
							throw new NotSupportedException("Canot specify an item name when searching for virtual cards. Use IsItemIssued('VirtualCard') instead.");
						case RuleTriggerIssueType.Bonus:
							using (var svc = LWDataServiceUtil.ContentServiceInstance())
							{
								var bonus = svc.GetBonusDef(itemName);
								if (bonus == null)
								{
									throw new Exception(string.Format("Bonus '{0}' does not exist.", itemName));
								}
								itemId = bonus.Id;
							}
							break;
					}
				}
			}

			//now find issued items...

			if (contextObject.Results == null || contextObject.Results.Count == 0)
			{
				return false;	//because nothing has been issued
			}

			if (!itemType.HasValue)
			{
				return true;	//because something has been issued and the caller doesn't care what it is
			}

			switch (itemType)
			{
				case RuleTriggerIssueType.Coupon:
					return HasCoupon(contextObject, itemId, itemName);
				case RuleTriggerIssueType.Email:
					return HasEmail(contextObject, itemId);
				case RuleTriggerIssueType.Message:
					return HasMessage(contextObject, itemId);
				case RuleTriggerIssueType.Notification:
					return HasNotification(contextObject, itemId);
				case RuleTriggerIssueType.Points:
					return HasPoints(contextObject);
				case RuleTriggerIssueType.Promotion:
					return HasPromotion(contextObject, itemId);
				case RuleTriggerIssueType.Reward:
					return HasReward(contextObject, itemId, itemName);
				case RuleTriggerIssueType.Sms:
					return HasSms(contextObject, itemId);
				case RuleTriggerIssueType.Survey:
					return HasSurvey(contextObject, itemId);
				case RuleTriggerIssueType.Tier:
					return HasTier(contextObject, itemName);
				case RuleTriggerIssueType.VirtualCard:
					return HasVirtualCard(contextObject);
				default:
					return false;
			}
		}

		private bool HasCoupon(ContextObject context, long id = 0, string name = null)
		{
			foreach (var r in context.Results)
			{
				if (r is IssueCoupon.IssueCouponRuleResult)
				{
					if (id == 0 || ((IssueCoupon.IssueCouponRuleResult)r).Id == id)
					{
						return true;
					}
				}
				else if (r is CampaignResult)
				{
					var campaign = (CampaignResult)r;
					if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Coupon && (id == 0 || id == campaign.ReferenceId))
					{
						return true;
					}
				}
				else if (r is AwardDefaultContent.AwardDefaultContentRuleResult)
				{
					var content = (AwardDefaultContent.AwardDefaultContentRuleResult)r;
					if (content.CouponAssigned != null && content.CouponAssigned.Count > 0)
					{
						if (string.IsNullOrEmpty(name) || content.CouponAssigned.Contains(name))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool HasEmail(ContextObject context, long id = 0)
		{
			foreach (var r in context.Results)
			{
				if (r is TriggeredEmailRule.TriggeredEmailRuleResult)
				{
					if (id == 0 || ((TriggeredEmailRule.TriggeredEmailRuleResult)r).Id == id)
					{
						return true;
					}
				}
				//todo: we need tracking of triggered emails in RealtimeCampaignResult
				//else if (r is RealtimeCampaign.RealtimeCampaignResult)
				//{
				//	var realtime = (RealtimeCampaign.RealtimeCampaignResult)r;
				//	if (realtime.Coupons != null && realtime.Emails.Count > 0)
				//	{
				//		if (id == 0 || realtime.Emails.Contains(id))
				//		{
				//			return true;
				//		}
				//	}
				//}
				else if (r is CampaignResult)
				{
					var campaign = (CampaignResult)r;
					if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Email && (id == 0 || id == campaign.ReferenceId))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasMessage(ContextObject context, long id = 0)
		{
			foreach (var r in context.Results)
			{
				if (r is IssueMessage.IssueMessageRuleResult)
				{
					if (id == 0 || ((IssueMessage.IssueMessageRuleResult)r).MemberMessageId == id)
					{
						return true;
					}
				}
				else if (r is CampaignResult)
				{
					var campaign = (CampaignResult)r;
					if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Message && (id == 0 || id == campaign.ReferenceId))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasNotification(ContextObject context, long id = 0)
		{
            foreach (var r in context.Results)
            {
                if (r is IssueNotification.IssueNotificationRuleResult)
                {
                    if (id == 0 || ((IssueNotification.IssueNotificationRuleResult)r).OwnerId == id)
                    {
                        return true;
                    }
                }
                //todo: need notification tracking in RealtimeCampaignResult 
                //else if (r is RealtimeCampaign.RealtimeCampaignResult)
                //{
                //	var realtime = (RealtimeCampaign.RealtimeCampaignResult)r;
                //	if (realtime.Messages != null && realtime.Notifications.Count > 0)
                //	{
                //		if (id == 0 || realtime.Notifications.Contains(id))
                //		{
                //			return true;
                //		}
                //	}
                //}
                else if (r is CampaignResult)
                {
                    var campaign = (CampaignResult)r;
                    if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Notification && (id == 0 || id == campaign.ReferenceId))
                    {
                        return true;
                    }
                }
            }
            return false;
		}

		private bool HasPoints(ContextObject context)
		{
			return context.Results.Count(r => r is AwardPointsRuleResult && ((AwardPointsRuleResult)r).PointsAwarded > 0) > 0;
		}

		private bool HasPromotion(ContextObject context, long id = 0)
		{
			foreach (var r in context.Results)
			{
				if (r is CampaignResult)
				{
					var campaign = (CampaignResult)r;
					if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Promotion && (id == 0 || id == campaign.ReferenceId))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasReward(ContextObject context, long id = 0, string name = null)
		{
			//r.ResultCode != 3 &&   <-- this gets set to 3 if the member is in non-member status. The reward will not be issued
			//r.RewardId > 0 &&      <-- this can be 0 or -2 if no reward was issued or it can be the id of the member reward.
			foreach (var r in context.Results.Where(r => r is IssueRewardRuleResult && r.ResultCode != 3).Select(r => (IssueRewardRuleResult)r))
			{
				if (r.RewardId >= 0 &&
					(string.IsNullOrEmpty(name) || name.Equals(r.RewardIssued, StringComparison.OrdinalIgnoreCase)))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasSms(ContextObject context, long id = 0)
		{
			foreach (var r in context.Results)
			{
				if (r is TriggeredSmsRuleResult)
				{
					if (id == 0 || ((TriggeredSmsRuleResult)r).Id == id)
					{
						return true;
					}
				}
				//todo: we need tracking of triggered sms in RealtimeCampaignResult
				//else if (r is RealtimeCampaign.RealtimeCampaignResult)
				//{
				//	var realtime = (RealtimeCampaign.RealtimeCampaignResult)r;
				//	if (realtime.Sms != null && realtime.Sms.Count > 0)
				//	{
				//		if (id == 0 || realtime.Sms.Contains(id))
				//		{
				//			return true;
				//		}
				//	}
				//}
				else if (r is CampaignResult)
				{
					var campaign = (CampaignResult)r;
					if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Sms && (id == 0 || id == campaign.ReferenceId))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasSurvey(ContextObject context, long id = 0)
		{
			//todo: the only way a survey is issued is through a bonus. Rather than throwing a NotImplementedException...
			//a method to get bonuses by survey id would also be nice to have <-- can you delete a survey that's 
			//tied to a bonus? Yes, you can and the SurveyId remains in LW_BonusDef. That should probably be fixed, too
			using (var svc = LWDataServiceUtil.ContentServiceInstance())
			{
				IEnumerable<BonusDef> bonuses = null;
				if (id == 0)
				{
					//can be any bonus, as long as it has a survey
					bonuses = svc.GetAllBonusDefs().Where(o => o.SurveyId.GetValueOrDefault(0) > 0);
				}
				else
				{
					//can only be any bonus with a matching survey id
					bonuses = svc.GetAllBonusDefs().Where(o => o.SurveyId.GetValueOrDefault(0) == id);
				}

				foreach (var bonus in bonuses)
				{
					if (HasBonus(context, bonus.Id))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasTier(ContextObject context, string tierName)
		{
			foreach (var r in context.Results)
			{
				if (r is EvaluateTier.EvaluateTierRuleResult)
				{
					var tier = (EvaluateTier.EvaluateTierRuleResult)r;
					if (string.IsNullOrEmpty(tierName) || tier.NewTier.Equals(tierName, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasVirtualCard(ContextObject context)
		{
			foreach (var r in context.Results)
			{
				if (r is CreateVirtualCardRule.CreateVirtualCardRuleResult)
				{
					return true;
				}
			}
			return false;
		}

		private bool HasBonus(ContextObject context, long id = 0, string name = null)
		{
			foreach (var r in context.Results)
			{
                if (r is CampaignResult)
				{
					var campaign = (CampaignResult)r;
					if (campaign.OutputType.HasValue && campaign.OutputType.Value == OutputType.Offer && (id == 0 || id == campaign.ReferenceId))
					{
						return true;
					}
				}
				else if (r is AwardDefaultContent.AwardDefaultContentRuleResult)
				{
					var content = (AwardDefaultContent.AwardDefaultContentRuleResult)r;
					if (content.BonusAssigned != null && content.BonusAssigned.Count > 0)
					{
						if (string.IsNullOrEmpty(name) || content.BonusAssigned.Contains(name))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
