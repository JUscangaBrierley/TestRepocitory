using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules.UIDesign;

namespace Brierley.FrameWork.Rules
{
	[Serializable]
	public class IssueReward : RuleBase
	{
		private const string _className = "IssueReward";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private RewardRuleUtil _util = new RewardRuleUtil();

		public IssueReward()
			: base("IssueReward")
		{
			FulfillmentOption = RewardFulfillmentOption.Electronic;
			IssuedRewardType = IssueRewardType.Earned;
		}

		public override void Validate()
		{
			//todo: Rule validation requires an exception to be thrown and we cannot get into the habit of throwing exceptions in multiple languages.
			//Fixing this is beyond the current scope of work, but this needs to be addressed. JIRA LW-3185 created to eventually address this.

			if (string.IsNullOrEmpty(RewardType) && !IssueMemberRewardChoice)
			{
				throw new Exception("You must choose either a specific reward or to issue the member's choice.");
			}

			if (!string.IsNullOrEmpty(RewardType) && IssueMemberRewardChoice)
			{
				throw new Exception("Please choose either a specific reward or to issue the member's choice (cannot select both).");
			}
		}

		public override string DisplayText
		{
			get { return "Issue Reward"; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[Category("General")]
		[DisplayName("Fulfillment Method")]
		[Description("Defines the fulfillment for this reward.")]
		[RulePropertyOrder(1)]
		public RewardFulfillmentOption FulfillmentOption { get; set; }

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[Category("General")]
		[DisplayName("Reward Type")]
		[Description("This rule will consume points.")]
		[RulePropertyOrder(2)]
		[RuleProperty(false, false, false, null, false, true)]
		public IssueRewardType IssuedRewardType { get; set; }

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[Category("General")]
		[DisplayName("Points Consumed When Issued")]
		[Description("Points are consumed when reward is issued to the account.")]
		[RulePropertyOrder(3)]
		[RuleProperty(false, false, false, null, false, true)]
		public PointsConsumptionOnIssueReward PointsConsumption
		{
			get
			{
				return _util.PointsConsumption;
			}
			set
			{
				_util.PointsConsumption = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Offer Code")]
		[Description("This member provides the offer code to be used for the issued member.")]
		[RuleProperty(false, false, false, null, false, true)]
		[RulePropertyOrder(4)]
		public string OfferCode
		{
			get
			{
				return _util.OfferCode;
			}
			set
			{
				_util.OfferCode = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Reward Name")]
		[Description("Used to select the type of reward to be issued by this rule.")]
		[RuleProperty(false, true, false, "RewardTypes", false)]
		[RulePropertyOrder(5)]
		public string RewardType { get; set; }

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Issue the Member's Reward Choice")]
		[Description("Instead of issuing a specific reward, the rule will issue the member's reward choice when checked.")]
		[RuleProperty(false, false, false, null, false, false)]
		[RulePropertyOrder(6)]
		public bool IssueMemberRewardChoice { get; set; }

		[Browsable(false)]
		public Dictionary<string, string> RewardTypes
		{
			get
			{
				using (var service = LWDataServiceUtil.ContentServiceInstance())
				{
					Dictionary<string, string> rewardTypes = new Dictionary<string, string>() { { string.Empty, string.Empty } }; //terrible - we have no resources in framework
					IList<RewardDef> rewards = service.GetAllRewardDefs();
					foreach (RewardDef rdef in rewards)
					{
                        if (rdef.RewardType != Common.RewardType.Payment)
                            rewardTypes.Add(rdef.Name, rdef.Name);
					}
					return rewardTypes;
				}
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Expiration Date")]
		[Description("Defines the expression used to calculate the expiration date of these rewards.")]
		[RuleProperty(true, false, false, null, false, true)]
		[RulePropertyOrder(7)]
		public string ExpiryDateExpression { get; set; }

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Changed by")]
		[Description("Id of the entity that changed cause this transaction.")]
		[RuleProperty(true, false, false, null, false, true)]
		[RulePropertyOrder(8)]
		public string ChangedByExpression
		{
			get { return _util.ChangedByExpression; }
			set { _util.ChangedByExpression = value; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Exhaust Points")]
		[Description("If set to true, the rule will continue issuing rewards until there are enough points in the point bank..")]
		[RulePropertyOrder(9)]
		[RuleProperty(false, false, false, null, false, true)]
		public bool ExhaustPoints { get; set; }

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Mark as Redeemed")]
		[Description("If set to true, the rule will set the redemption date of the reward.")]
		[RulePropertyOrder(10)]
		[RuleProperty(false, false, false, null, false, true)]
		public bool MarkAsRedeemed
		{
			get { return _util.MarkAsRedeemed; }
			set { _util.MarkAsRedeemed = value; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Reward Issued Email")]
		[Description("Used to select the triggered email that will be sent out once the reward is issued.")]
		[RuleProperty(false, true, false, "AvailableTriggeredEmails")]
		[RulePropertyOrder(11)]
		public string TriggeredEmailName
		{
			get { return _util.TriggeredEmailName; }
			set { _util.TriggeredEmailName = value; }
		}

		[Browsable(false)]
		public Dictionary<string, string> AvailableTriggeredEmails
		{
			get
			{
				return _util.AvailableTriggeredEmails;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Assign LoyaltyWare Certificate")]
		[Description("Assign certificate from LoyaltyWare's certificate bank.")]
		[RulePropertyOrder(12)]
		public bool AssignLWCertificate
		{
			get { return _util.AssignLWCertificate; }
			set { _util.AssignLWCertificate = value; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Low Certificates Email")]
		[Description("Used to select the triggered email that will be sent out when the number of certificates reach the threshold value.")]
		[RuleProperty(false, true, false, "AvailableTriggeredEmails", false, true)]
		[RulePropertyOrder(13)]
		public string LowThresholdEmailName
		{
			get
			{
				return _util.LowThresholdEmailName;
			}
			set
			{
				_util.LowThresholdEmailName = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Low Certificates Email Recepient")]
		[Description("This is the email of the recepient for low threshold email.")]
		[RulePropertyOrder(14)]
		[RuleProperty(false, false, false, null, false, true)]
		public string LowCertificatesEmailRecepient
		{
			get { return _util.LowThresholdEmailRecepient; }
			set { _util.LowThresholdEmailRecepient = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="PreviousResultCode">
		///     0 - The rule was successfull
		///     1 - The rule did not run because a promotion list was specified and the member was not part of it.
		///     2 - The rule did not run because the member did not have enough points.
		///     3 - The rule did not run because this was not a member.
		/// </param>
		/// <returns>
		/// If the ExhaustPoints option is set to false then the id of the reward awarded is returned.  If that
		/// option is set to true then 0 is returned.
		/// </returns>
		public override void Invoke(ContextObject context)
		{
			/*
             * A lot of this logic needs to be synchronized with AddMemberReward API.
             * */
			string methodName = "Invoke";

			bool useAllPoints = ExhaustPoints;

			// Ignore the value of ExhaustPoints if the points consumption policy is not Consume
			// or the reward type is not earned.
			if (PointsConsumption != PointsConsumptionOnIssueReward.Consume || IssuedRewardType != IssueRewardType.Earned)
			{
				useAllPoints = false;
			}

			Member member = null;
			VirtualCard card = null;

			var result = new IssueRewardRuleResult()
			{
				Name = !string.IsNullOrWhiteSpace(context.Name) ? context.Name : this.RuleName,
				Mode = context.Mode,
				RuleType = this.GetType()
			};
			AddRuleResult(context, result);

			ResolveOwners(context.Owner, ref member, ref card);

			if (member == null)
			{
				string errMsg = "No member could be resolved for issue reward rule.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWRulesException(errMsg) { ErrorCode = 3214 };
			}

			_logger.Trace(_className, methodName, "Invoking issue reward rule for member with Ipcode = " + member.IpCode + ".");

			result.MemberId = member.IpCode;

			if (member.MemberStatus == MemberStatusEnum.NonMember)
			{
				string msg = string.Format("Cannot issue reward to non-member with ipcode {0}. Skipping the rule.", member.IpCode);
				_logger.Trace(_className, methodName, msg);
				result.ResultCode = 3;
				return;
			}

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				long rewardId = -1;

				//BEGIN - Extract Order Information
				var env = context.Environment;
				string rewardName = string.Empty;
				long variantId = -1;
				long? fulfillmentProviderId = null;
				MemberOrder order = null;
				string fporderNumber = string.Empty;
				string lworderNumber = string.Empty;
				if (env != null && env.ContainsKey("RewardName"))
				{
					rewardName = (string)env["RewardName"];
				}
				if (env != null && env.ContainsKey("LWOrder"))
				{
					order = (MemberOrder)env["LWOrder"];
					lworderNumber = order.OrderNumber;
				}
				else if (env != null && env.ContainsKey("LWOrderNumber"))
				{
					lworderNumber = (string)env["LWOrderNumber"];
					order = loyalty.GetMemberOrder(lworderNumber);
				}
				if (env != null && env.ContainsKey("FPOrderNumber"))
				{
					fporderNumber = (string)env["FPOrderNumber"];
				}
				if (env != null && env.ContainsKey("ProductVariantId"))
				{
					variantId = (long)env["ProductVariantId"];
				}
				if (env != null && env.ContainsKey("FulfillmentProviderId"))
				{
					fulfillmentProviderId = (long?)env["FulfillmentProviderId"];
				}

				if (order == null && !string.IsNullOrWhiteSpace(fporderNumber))
				{
					IList<MemberReward> mrList = loyalty.GetMemberRewardsByFPOrderNumber(fporderNumber);
					if (mrList.Count > 0)
					{
						order = loyalty.GetMemberOrder(mrList[0].LWOrderNumber);
					}
				}
				//END - Extract Order Information

				RewardDef reward = null;

				if (IssueMemberRewardChoice)
				{
                    var choice = loyalty.GetCurrentRewardChoiceOrDefault(member);
                    if(choice == null)
                    {
                        throw new LWRulesException(string.Format("Reward choice not found for member {0}. Cannot issue rewards.", member.IpCode));
                    }
					rewardId = choice.Id;
                    reward = choice;
				}
                else
				{
					reward = content.GetRewardDef(RewardType);
				}

                if (!reward.Active || reward.CatalogStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.Now || reward.CatalogEndDate.GetValueOrDefault(DateTime.MaxValue) <= DateTime.Now)
                {
					throw new LWRulesException(string.Format("Cannot issue reward {0} because it is not active.", reward.Name)) { ErrorCode = 3232 };
				}

				result.RewardIssued = reward.Name;

				string[] pointTypes = reward.GetPointTypes();
				string[] pointEvents = reward.GetPointEvents();

				int nRewardsAwarded = 0;
				while (true)
				{
					bool issueReward = true;
					if (IssuedRewardType == IssueRewardType.Earned)
					{
						// check to make sure that the member has enough points for this reward to be issued.
						decimal pointsBalance = _util.GetPoints(reward, pointTypes, pointEvents, member, card);
						decimal pointsOnHold = _util.GetOnHoldPoints(reward, pointTypes, pointEvents, member, card);
						decimal points = pointsBalance - pointsOnHold;
						if (points < reward.HowManyPointsToEarn)
						{
							if (nRewardsAwarded == 0)
							{
								string msg = string.Format("The reward requires {0} points.  Member with ipcode {1} only has {2} points.", reward.HowManyPointsToEarn, member.IpCode, points);
								_logger.Trace(_className, methodName, msg);
								rewardId = -1;
								result.RewardId = rewardId;
								return;
							}
							else
							{
								string msg = string.Format("{0} rewards awarded to member with ipcode {1}.", nRewardsAwarded, member.IpCode);
								_logger.Trace(_className, methodName, msg);
								rewardId = nRewardsAwarded == 1 ? rewardId : 0;
								break;
							}
						}
					}

					if (issueReward)
					{
						// Calculate the expiration date
						DateTime expiryDate = DateTimeUtil.MaxValue;
						if (!string.IsNullOrEmpty(this.ExpiryDateExpression))
						{
							try
							{
								ExpressionFactory exprF = new ExpressionFactory();
								expiryDate = (DateTime)exprF.Create(this.ExpiryDateExpression).evaluate(context);
							}
							catch (Exception ex)
							{
								string errMsg = string.Format("Error while calculating expiry date using expression {0}", ExpiryDateExpression);
								_logger.Error(_className, methodName, errMsg, ex);
								throw new LWRulesException(errMsg, ex);
							}
						}
						// Issue reward
						string certNmbr = string.Empty;
						if (context.Environment.ContainsKey("CertificateNumber"))
						{
							certNmbr = (string)context.Environment["CertificateNumber"];
						}
						else if (context.Environment.ContainsKey("CertContainer"))
						{
							PromoCertContainer certContainer = (PromoCertContainer)context.Environment["CertContainer"];
							certNmbr = certContainer.GetNextAvailableCert(ContentObjType.Reward, reward.CertificateTypeCode);
						}

						string changedBy = order != null ? order.ChangedBy : string.Empty;
						if (string.IsNullOrWhiteSpace(changedBy) && env.ContainsKey("ChangedBy"))
						{
							changedBy = (string)env["ChangedBy"];
						}

						rewardId = _util.IssueRewardCertificate(context, member, reward, expiryDate, FulfillmentOption, null, ref certNmbr, variantId, lworderNumber, fporderNumber, changedBy);
						result.CertNmbr = certNmbr;

						if (rewardId != -1)
						{
							if (IssuedRewardType == IssueRewardType.Earned)
							{
								switch (PointsConsumption)
								{
									case PointsConsumptionOnIssueReward.Consume:
										result = _util.ConsumePoints(member, card, reward, rewardId, null, result);
										break;
									case PointsConsumptionOnIssueReward.Hold:
										result = _util.HoldPoints(member, card, reward, rewardId, result);
										break;
									case PointsConsumptionOnIssueReward.NoAction:
									default:
										_logger.Debug(_className, methodName, "Points consumption being skipped.");
										break;
								}
							}

							if (!string.IsNullOrEmpty(TriggeredEmailName))
							{
								// send triggered email.				
								MemberReward memberReward = loyalty.GetMemberReward(rewardId);
								_util.SendIssueRewardEmail(member, reward, memberReward, null, memberReward.CertificateNmbr);
							}

                            Product p = content.GetProduct(reward.ProductId);
                            if (variantId != -1)
                            {
                                ProductVariant pv = content.GetProductVariant(variantId);
                                if (pv.ProductId != p.Id)
                                {
                                    string msg = "Incorrect product variant id specified for reward " + rewardName;
                                    _logger.Error(_className, methodName, msg);
                                    throw new LWRulesException(msg) { ErrorCode = 3207 };
                                }
                                else
                                {
                                    if (pv.Quantity != null)
                                    {
                                        content.UpdateProductVariantQuantity(pv.ID, -1);
                                    }
                                }
                            }
                            else
                            {
                                if (p.Quantity != null)
                                {
                                    content.UpdateProductQuantity(p.Id, -1);
                                }
                            }
							nRewardsAwarded++;
						}

						//Send Push notification if one is attached to the message definition
						if (reward.PushNotificationId != null)
						{
							using (var push = LWDataServiceUtil.PushServiceInstance())
							{
								Task.Run(() => push.SendAsync((long)reward.PushNotificationId, member));
							}
						}
					}
					if (!useAllPoints)
					{
						break;
					}
				}
				_util.CheckLowCertificateThreshold(member, reward);
				result.RewardId = rewardId;
				if (string.IsNullOrEmpty(result.CertNmbr))
				{
					result.Detail = string.Format("Issued reward {0} with id {1}.", reward.Name, result.RewardId);
				}
				else
				{
					result.Detail = string.Format("Issued reward {0} with Cert number {1}.", reward.Name, result.CertNmbr);
				}
			}
		}

		public override List<string> GetBscriptsToMove()
		{
			List<string> bscriptList = new List<string>();
			if (!string.IsNullOrWhiteSpace(this.ExpiryDateExpression) && ExpressionUtil.IsLibraryExpression(this.ExpiryDateExpression))
			{
				bscriptList.Add(ExpressionUtil.GetLibraryName(this.ExpiryDateExpression));
			}
			if (!string.IsNullOrWhiteSpace(this.ChangedByExpression) && ExpressionUtil.IsLibraryExpression(this.ChangedByExpression))
			{
				bscriptList.Add(ExpressionUtil.GetLibraryName(this.ChangedByExpression));
			}
			return bscriptList;
		}

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
		{
			string methodName = "MigrateRuleInstance";

			_logger.Trace(_className, methodName, "Migrating Issuereward rule.");

			IssueReward src = (IssueReward)source;

			IssuedRewardType = src.IssuedRewardType;
			IssueMemberRewardChoice = src.IssueMemberRewardChoice;
			PointsConsumption = src.PointsConsumption;
			ExhaustPoints = src.ExhaustPoints;
			OfferCode = src.OfferCode;
			RewardType = src.RewardType;
			ExpiryDateExpression = src.ExpiryDateExpression;
			ChangedByExpression = src.ChangedByExpression;
			TriggeredEmailName = src.TriggeredEmailName;
			AssignLWCertificate = src.AssignLWCertificate;
			LowThresholdEmailName = src.LowThresholdEmailName;
			LowCertificatesEmailRecepient = src.LowCertificatesEmailRecepient;
			FulfillmentOption = src.FulfillmentOption;
			MarkAsRedeemed = src.MarkAsRedeemed;

			RuleVersion = src.RuleVersion;
			RuleDescription = src.RuleDescription;

			return this;
		}
	}
}
