using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public class RfeUtil
	{
		private const string _className = "RfeUtil";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public static MemberBonusStatus ProcessBonusAction(long memberBonusId, MemberBonusStatus? newStatus, string language, string channel, string bonusCompletedEventName, string awardPointsRuleName, bool completingReferral = false)
		{
			RuleTrigger rule = null;
			if (!string.IsNullOrEmpty(awardPointsRuleName))
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					rule = loyalty.GetRuleByName(awardPointsRuleName);
				}
			}
			return ProcessBonusAction(memberBonusId, newStatus, language, channel, bonusCompletedEventName, rule, completingReferral);
		}


		public static MemberBonusStatus ProcessBonusAction(long memberBonusId, MemberBonusStatus? newStatus, string language, string channel, string bonusCompletedEventName, long awardPointsRuleId, bool completingReferral = false)
		{
			RuleTrigger rule = null;
			if (awardPointsRuleId > 0)
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					rule = loyalty.GetRuleById(awardPointsRuleId);
				}
			}
			return ProcessBonusAction(memberBonusId, newStatus, language, channel, bonusCompletedEventName, rule, completingReferral);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bonusId"></param>
		public static MemberBonusStatus ProcessBonusAction(long memberBonusId, MemberBonusStatus? newStatus, string language, string channel, string bonusCompletedEventName, RuleTrigger awardPointsRule, bool completingReferral = false)
		{
			const string methodName = "ProcessBonusAction";
			MemberBonusStatus ret = MemberBonusStatus.Completed;
			try
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					MemberBonus memberBonus = loyalty.GetMemberOffer(memberBonusId);
					BonusDef bonus = content.GetBonusDef(memberBonus.BonusDefId);
					decimal points = 0;
					bool isCompleting = false;

					if (newStatus.HasValue)
					{
						//make sure new status is valid, if provided. Otherwise ignore and return.
						switch (memberBonus.Status)
						{
							case MemberBonusStatus.Issued:
								if (newStatus.Value != MemberBonusStatus.Viewed)
								{
									return memberBonus.Status;
								}
								break;
							case MemberBonusStatus.Saved:
								return memberBonus.Status;
								break;
							case MemberBonusStatus.Viewed:
								if (newStatus.Value != MemberBonusStatus.Completed)
								{
									return memberBonus.Status;
								}
								break;
							case MemberBonusStatus.Completed:
								if (!completingReferral)
								{
									return memberBonus.Status;
								}
								break;
						}
					}

					switch (memberBonus.Status)
					{
						case MemberBonusStatus.Issued:
							//first action - no points for viewing the reward, only on survey and referral completion
							ret = memberBonus.Status = MemberBonusStatus.Viewed;
							break;
						case MemberBonusStatus.Viewed:
							if (!bonus.ApplyQuotaToReferral.GetValueOrDefault())
							{
								try
								{
									long count = content.CheckAndUpdateBonusQuotaCount(memberBonus.BonusDefId, 1);
								}
								catch (LWDataServiceException ex)
								{
									//nothing to do except quietly log the error
									_logger.Error(_className, "ProcessBonusAction", "Unexpected error incrementing bonus quota count", ex);
								}
							}
							ret = memberBonus.Status = MemberBonusStatus.Completed;
							isCompleting = true;
							break;
						case MemberBonusStatus.Completed:
							if (!memberBonus.ReferralCompleted && completingReferral)
							{
								if (bonus.ApplyQuotaToReferral.GetValueOrDefault())
								{
									try
									{
										long count = content.CheckAndUpdateBonusQuotaCount(memberBonus.BonusDefId, 1);
									}
									catch (LWDataServiceException ex)
									{
										//nothing to do except quietly log the error
										_logger.Error(_className, "ProcessBonusAction", "Unexpected error incrementing bonus quota count", ex);
									}
								}
								memberBonus.ReferralCompleted = true;
								BonusDef bonusDef = content.GetBonusDef(memberBonus.BonusDefId);
								points = bonusDef.Points.GetValueOrDefault();
							}
							break;
					}

					memberBonus.TimesClicked++;
					loyalty.UpdateMemberOffer(memberBonus);

					if (isCompleting)
					{
						if (!string.IsNullOrEmpty(bonusCompletedEventName))
						{
							var member = loyalty.LoadMemberFromIPCode(memberBonus.MemberId);
							ContextObject co = new ContextObject() { Owner = member };
							co.Environment.Add(EnvironmentKeys.Channel, channel);
							co.Environment.Add(EnvironmentKeys.Language, language);
							co.Environment.Add(EnvironmentKeys.BonusName, bonus.Name);
							co.Environment.Add(EnvironmentKeys.OwnerType, PointTransactionOwnerType.Bonus);
							co.Environment.Add(EnvironmentKeys.OwnerId, memberBonus.ID);
							co.Environment.Add(EnvironmentKeys.RowKey, memberBonus.BonusDefId);
							loyalty.ExecuteEventRules(co, bonusCompletedEventName, Brierley.FrameWork.Common.RuleInvocationType.Manual);
						}
					}

					if (points > 0)
					{
						AwardOfferPoints(memberBonus.MemberId, points, memberBonus.BonusDefId, memberBonus.ID, awardPointsRule);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
			return ret;
		}

		private static void AwardOfferPoints(long ipCode, decimal points, long bonusDefId, long bonusId, RuleTrigger trigger)
		{
			const string methodName = "AwardPoints";

			if (trigger != null)
			{

				System.Reflection.PropertyInfo pInfo = trigger.Rule.GetType().GetProperty("AccrualExpression");
				if (pInfo == null)
				{
					string msg = string.Format("Unable to lookup property 'AccrualExpression' in the rule configured to award points.  The type of award point rule is {0}", trigger.Rule.GetType().Name);
					_logger.Critical(_className, methodName, msg);
					throw new Exception(msg);
				}
				pInfo.SetValue(trigger.Rule, points.ToString(), null);
				pInfo = trigger.Rule.GetType().GetProperty("DescriptionExpression");
				if (pInfo == null)
				{
					string msg = string.Format("Unable to lookup property 'DescriptionExpression' in the rule configured to award points.  The type of award point rule is {0}", trigger.Rule.GetType().Name);
					_logger.Error(_className, methodName, msg);
				}
				else
				{
					string notes = string.Format("'Member {0} consumed bonus {1}'", ipCode, bonusId);
					if (notes.Length > 500)
					{
						notes = notes.Substring(0, 500);
					}
					pInfo.SetValue(trigger.Rule, notes, null);
				}
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					ContextObject ctx = new ContextObject();
					var member = loyalty.LoadMemberFromIPCode(ipCode);
					ctx.Owner = member;
					ctx.Environment.Add(EnvironmentKeys.OwnerType, PointTransactionOwnerType.Bonus);
					ctx.Environment.Add(EnvironmentKeys.OwnerId, bonusDefId);
					ctx.Environment.Add(EnvironmentKeys.RowKey, bonusId);
					loyalty.Execute(trigger, ctx);
				}
			}
			else
			{
				_logger.Trace(_className, methodName, "No business rule defined for awarding points.");
			}
		}
	}
}
