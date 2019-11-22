using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.EngagementLinks
{
	public partial class ViewEngagementLinks : ModuleControlBase
	{
		private const string _className = "ViewEngagementLinks";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			const string methodName = "OnInit";

			_logger.Debug(_className, methodName, "begin");

			EngagementLinksConfig config = ConfigurationUtil.GetConfiguration<EngagementLinksConfig>(ConfigurationKey);
			if (config == null)
			{
				throw new Exception(string.Format("Missing configuration for module {0}.", ConfigurationKey.ToString()));
			}

			ContextObject co = new ContextObject();

			try
			{
				Member member = null;
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{

					string memberParam = Request.QueryString[config.MemberIdentifierParameterName];
					if (string.IsNullOrWhiteSpace(memberParam))
					{
						_logger.Debug(_className, methodName, "Member identifier parameter is missing. Exiting.");
						return;
					}

					if (!string.IsNullOrEmpty(config.DecryptMemberParameterWith))
					{
						_logger.Debug(_className, methodName, string.Format("decrypting member identifier with {0}", config.DecryptMemberParameterWith));
						memberParam = CryptoUtil.Decrypt(CryptoUtil.EncodeUTF8(config.DecryptMemberParameterWith), memberParam);
					}

					switch (config.MemberIdentifier)
					{
						case AuthenticationFields.AlternateId:
							member = svc.LoadMemberFromAlternateID(memberParam);
							break;
						case AuthenticationFields.LoyaltyIdNumber:
							member = svc.LoadMemberFromLoyaltyID(memberParam);
							break;
						case AuthenticationFields.PrimaryEmailAddress:
							member = svc.LoadMemberFromEmailAddress(memberParam);
							break;
						case AuthenticationFields.Username:
							member = svc.LoadMemberFromEmailAddress(memberParam);
							break;
					}
					co.Environment.Add(EnvironmentKeys.MemberIdentifier, config.MemberIdentifier);

					if (member == null)
					{
						_logger.Debug(_className, methodName, string.Format("Unable to locate load member {0} by identifier {1}. Exiting.", memberParam, config.MemberIdentifier.ToString()));
						return;
					}

					co.Owner = member;

					string promoCode = string.Empty;
					IList<RuleTrigger> promotionRules = null;
					if (!string.IsNullOrEmpty(config.PromotionParameterName))
					{
						promoCode = Request.QueryString[config.PromotionParameterName];
						if (string.IsNullOrWhiteSpace(promoCode))
						{
							_logger.Debug(_className, methodName, "Promotion parameter is required and missing. Exiting.");
							return;
						}

						if (!string.IsNullOrEmpty(config.DecryptPromotionParameterWith))
						{
							_logger.Debug(_className, methodName, string.Format("decrypting promotion identifier with {0}", config.DecryptPromotionParameterWith));
							promoCode = CryptoUtil.Decrypt(CryptoUtil.EncodeUTF8(config.DecryptPromotionParameterWith), promoCode);
						}

						Promotion promo = svc.GetPromotionByCode(promoCode);
						if (promo == null)
						{
							_logger.Debug(_className, methodName, string.Format("Unable to locate promotion by code {0}. Exiting.", promoCode));
							return;
						}

						if (!promo.IsValid())
						{
							_logger.Debug(_className, methodName, string.Format("Promotion {0} is not active. Exiting.", promoCode));
							return;
						}

						if (svc.IsMemberInPromotionList(promoCode, member.IpCode))
						{
							_logger.Debug(_className, methodName, string.Format("Promotion {0} has already been applied to member {1}. Exiting.", promoCode, member.IpCode));
							return;
						}

						svc.CreateMemberPromotion(new MemberPromotion() { Code = promoCode, MemberId = member.IpCode });

						if (config.ExecuteOnlyPromotionRules)
						{
							promotionRules = svc.GetRuleByPromotion(promoCode) ?? new List<RuleTrigger>();
						}
						co.Environment.Add(EnvironmentKeys.PromotionCode, promoCode);
					}

					if (config.ExecuteEventRules != null)
					{
						foreach (string eventName in config.ExecuteEventRules)
						{
							IList<RuleTrigger> rules = svc.GetRuleByObjectName(eventName);
							if (rules != null && rules.Count > 0)
							{
								int count = 0;
								foreach (var rule in rules)
								{
									if (promotionRules != null)
									{
										if (promotionRules.Where(o => o.Id == rule.Id).FirstOrDefault() == null)
										{
											//rule is not part of promotion and we're only supposed to process the promotion's rules. skip.
											_logger.Debug(_className, methodName, string.Format("Rule {0} in event {1} is not part of the promotion {2}. Skipping.", rule.RuleName, eventName, promoCode));
											continue;
										}
									}

									var invocation = (RuleInvocationType)Enum.Parse(typeof(RuleInvocationType), rule.InvocationType);
									if (invocation != RuleInvocationType.Manual)
									{
										_logger.Debug(_className, methodName, string.Format("Rule {0} in event {1} does not have an invocation type of Manual. Skipping.", rule.RuleName, eventName));
										continue;
									}

									svc.Execute(rule, co);
									count++;
								}
								_logger.Debug(_className, methodName, string.Format("{0} rules executed for event {1}.", count, eventName));
							}
							else
							{
								_logger.Debug("No rules have been configured for event " + eventName);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				//log the error, but avoid throwing and causing the user to be redirected to the error page.
				_logger.Error(_className, methodName, "Error processing engagement link", ex);
			}
			finally
			{
				//move the user along to their destination, if possible...
				try
				{
					if (string.IsNullOrEmpty(config.DestinationUrlExpression))
					{
						_logger.Error(_className, methodName, "Destination URL expression is missing");
					}
					else
					{
						Expression exp = new ExpressionFactory().Create(config.DestinationUrlExpression);
						var url = exp.evaluate(co);
						if (url != null)
						{
							Response.Redirect(url.ToString(), false);
						}
					}
				}
				catch (Exception ex)
				{
					//log the error, but again avoid throwing and causing the user to be redirected to the error page.
					_logger.Error(_className, methodName, "Error evaluating redirect URL expression", ex);
				}
				finally
				{
					_logger.Debug(_className, methodName, "end");
				}
			}
		}

		protected override bool ControlRequiresJQueryUI()
		{
			return false;
		}

		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}
	}
}