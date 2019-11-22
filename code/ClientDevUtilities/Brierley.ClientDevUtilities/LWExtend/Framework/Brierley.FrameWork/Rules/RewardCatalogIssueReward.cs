using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Data;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Rules.UIDesign;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RewardCatalogIssueReward : RuleBase
    {        
        #region Private Variables
        [NonSerialized]
		private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string className = "RewardCatalogIssueReward";
        private string expirationDateLogic = string.Empty;
        [NonSerialized]
		private RewardRuleUtil util = new RewardRuleUtil();        
        private RewardFulfillmentOption fulfillmentOption = RewardFulfillmentOption.Printed;		
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public RewardCatalogIssueReward()
            : base("RewardCatalogIssueReward")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Validate()
        {            
        }

        #region Properties

        public override string DisplayText
        {
            get { return "Issue Reward from catalog"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Fulfillment Method")]
        [Description("Defines the fulfillment for this reward.")]
		[RulePropertyOrder(1)]
        public RewardFulfillmentOption FulfillmentOption
        {
            get
            {
                return fulfillmentOption;
            }
            set
            {
                fulfillmentOption = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Points Consumed When Issued")]
		[Description("Points are consumed when reward is issued to the account.")]
		[RulePropertyOrder(2)]
        [RuleProperty(false, false, false, null, false, true)]
		public PointsConsumptionOnIssueReward PointsConsumption
		{
			get
			{
				return util.PointsConsumption;
			}
			set
			{
				util.PointsConsumption = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Expiration Date")]
        [Description("Defines the expression used to calculate the expiration date of these rewards.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(3)]
        public string ExpiryDateExpression
        {
            get
            {
                return expirationDateLogic;
            }
            set
            {
                expirationDateLogic = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Changed by")]
        [Description("Id of the entity that changed cause this transaction.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(4)]
        public string ChangedByExpression
        {
            get { return util.ChangedByExpression; }
            set { util.ChangedByExpression = value; }
        }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Reward Issued Email")]
		[Description("Used to select the triggered email that will be sent out once the reward is issued.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails")]
		[RulePropertyOrder(5)]
		public string TriggeredEmailName
		{
			get
			{
				return util.TriggeredEmailName;
			}
			set
			{
				util.TriggeredEmailName = value;
			}
		}

		/// <summary>
		/// Returns list of all available triggered emails
		/// </summary>
		[Browsable(false)]
		public Dictionary<string, string> AvailableTriggeredEmails
		{
			get
			{
				return util.AvailableTriggeredEmails;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Assign LoyaltyWare Certificate")]
        [Description("Assign certificate from LoyaltyWare's certificate bank.")]
        [RulePropertyOrder(10)]
        public bool AssignLWCertificate
        {
            get { return util.AssignLWCertificate; }
            set { util.AssignLWCertificate = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Mark as Redeemed")]
        [Description("If set to true, the rule will set the redemption date of the reward.")]
        [RulePropertyOrder(11)]
        [RuleProperty(false, false, false, null, false, true)]
        public bool MarkAsRedeemed
        {
            get { return util.MarkAsRedeemed; }
            set { util.MarkAsRedeemed = value; }
        }
        
		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Low Certificates Email")]
		[Description("Used to select the triggered email that will be sent out when the number of certificates reach the threshold value.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails", false, true)]
		[RulePropertyOrder(12)]
		public string LowThresholdEmailName
		{
			get
			{
				return util.LowThresholdEmailName;
			}
			set
			{
				util.LowThresholdEmailName = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Low Certificates Email Recipient")]
        [Description("This is the email of the recipient for low threshold email.")]
        [RulePropertyOrder(13)]
        [RuleProperty(false, false, false, null, false, true)]
        public string LowCertificatesEmailRecepient
        {
            get { return util.LowThresholdEmailRecepient; }
            set { util.LowThresholdEmailRecepient = value; }
        }
        #endregion

        #region Helper Methods                        
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="PreviousResultCode"></param>
        /// <returns></returns>
        public override void Invoke(ContextObject Context)
        {
            string methodName = "Invoke";

			Member lwmember = null;
			VirtualCard lwvirtualCard = null;

            IssueRewardRuleResult result = new IssueRewardRuleResult() 
            {
                Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
                Mode = Context.Mode,
                RuleType = this.GetType() 
            };
            AddRuleResult(Context, result);

			ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);

            result.MemberId = lwmember.IpCode;

            if (lwmember.MemberStatus == MemberStatusEnum.NonMember)
            {
                string msg = string.Format("Cannot issue reward to non-member with ipcode {0}. Skipping the rule.", lwmember.IpCode);
                logger.Trace(className, methodName, msg);
                result.ResultCode = 3;                
            }

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{

				#region Extract Order Information
				var env = Context.Environment;
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
				#endregion

				logger.Trace(className, methodName, "Invoking RewardCatalogIssueReward rule for reward name " + rewardName);
				RewardDef rdef = content.GetRewardDef(rewardName);
				if (rdef == null)
				{
					string msg = string.Format("Reward {0} does not exist.", rewardName);
					LWRulesException ex = new LWRulesException(msg);
					logger.Error(className, methodName, "Error invoking rule.", ex);
					throw ex;
				}
				if (!rdef.Active)
				{
					throw new LWRulesException(string.Format("Cannot issue reward {0} because it is not active.", rdef.Name)) { ErrorCode = 3232 };
				}

				result.RewardIssued = rdef.Name;

				string[] pointTypes = rdef.GetPointTypes();
				string[] pointEvents = rdef.GetPointEvents();

				if (PointsConsumption != PointsConsumptionOnIssueReward.NoAction)
				{
					decimal pointsBalance = util.GetPoints(rdef, pointTypes, pointEvents, lwmember, lwvirtualCard);
					decimal pointsOnHold = util.GetOnHoldPoints(rdef, pointTypes, pointEvents, lwmember, lwvirtualCard);
					decimal points = pointsBalance - pointsOnHold;
					if (points < rdef.HowManyPointsToEarn)
					{
						string msg = string.Format("The reward requires {0} points.  Member with ipcode {1} only has {2} points.",
							rdef.HowManyPointsToEarn, lwmember.IpCode, points);
						logger.Trace(className, methodName, msg);
						throw new LWRulesException(msg) { ErrorCode = 3205 };
					}
					logger.Debug(className, methodName, string.Format("There are {0} points in bank for member {1}", points, lwmember.IpCode));
				}



				string certNmbr = string.Empty;
				long rewardId = -1;
				// Calculate the expiration date
				DateTime expiryDate = DateTimeUtil.MaxValue;
				if (env.ContainsKey("ExpirationDate"))
				{
					expiryDate = (DateTime)env["ExpirationDate"];
				}
				else if (!string.IsNullOrEmpty(this.expirationDateLogic))
				{
					try
					{
						ExpressionFactory exprF = new ExpressionFactory();
						expiryDate = (DateTime)exprF.Create(this.expirationDateLogic).evaluate(Context);
					}
					catch (Exception ex)
					{
						string errMsg = string.Format("Error while calculating expiry date using expression {0}", expirationDateLogic);
						logger.Error(className, methodName, errMsg, ex);
						throw new LWRulesException(errMsg, ex) { ErrorCode = 3206 };
					}
				}
				// issue reward.				
				if (env.ContainsKey("CertificateNumber"))
				{
					certNmbr = (string)env["CertificateNumber"];
				}
				else if (Context.Environment.ContainsKey("CertContainer"))
				{
					PromoCertContainer certContainer = (PromoCertContainer)Context.Environment["CertContainer"];
					certNmbr = certContainer.GetNextAvailableCert(ContentObjType.Reward, rdef.CertificateTypeCode);
				}

				string changedBy = order != null ? order.ChangedBy : string.Empty;
				if (string.IsNullOrWhiteSpace(changedBy) && env.ContainsKey("ChangedBy"))
				{
					changedBy = (string)env["ChangedBy"];
				}

				rewardId = util.IssueRewardCertificate(Context, lwmember, rdef, expiryDate, fulfillmentOption, fulfillmentProviderId, ref certNmbr, variantId, lworderNumber, fporderNumber, changedBy);

				result.CertNmbr = certNmbr;
				result.RewardId = rewardId;

				// consume points.
				switch (PointsConsumption)
				{
					case PointsConsumptionOnIssueReward.Consume:
						result = util.ConsumePoints(lwmember, lwvirtualCard, rdef, rewardId, order, result);
						break;
					case PointsConsumptionOnIssueReward.Hold:
						result = util.HoldPoints(lwmember, lwvirtualCard, rdef, rewardId, result);
						break;
					case PointsConsumptionOnIssueReward.NoAction:
					default:
						logger.Debug(className, methodName, "Points consumption being skipped.");
						break;
				}

				if (!string.IsNullOrEmpty(TriggeredEmailName))
				{
					// send triggered email.				
					MemberReward reward = loyalty.GetMemberReward(rewardId);
					util.SendIssueRewardEmail(lwmember, rdef, reward, order, reward.CertificateNmbr);
				}

				// decrement the product quantity.
				if (rewardId != -1)
				{
					Product p = content.GetProduct(rdef.ProductId);
					if (variantId != -1)
					{
						ProductVariant pv = content.GetProductVariant(variantId);
						if (pv.ProductId != p.Id)
						{
							string msg = "Incorrect product variant id specified for reward " + rewardName;
							logger.Error(className, methodName, msg);
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
				}
				if (string.IsNullOrEmpty(result.CertNmbr))
				{
					result.Detail = string.Format("Issued reward {0} with id {1}.", rdef.Name, result.RewardId);
				}
				else
				{
					result.Detail = string.Format("Issued reward {0} with Cert number {1}.", rdef.Name, result.CertNmbr);
				}
				if (string.IsNullOrEmpty(certNmbr))
				{
					util.CheckLowCertificateThreshold(lwmember, rdef);
				}
				return;
			}
        }

        #region Migrartion Helpers

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

            logger.Trace(className, methodName, "Migrating RewardCatalogIssueReward rule.");

            RewardCatalogIssueReward src = (RewardCatalogIssueReward)source;

            FulfillmentOption = src.FulfillmentOption;
            PointsConsumption = src.PointsConsumption;            
            ExpiryDateExpression = src.ExpiryDateExpression;
            ChangedByExpression = src.ChangedByExpression;
            TriggeredEmailName = src.TriggeredEmailName;
            AssignLWCertificate = src.AssignLWCertificate;
            LowThresholdEmailName = src.LowThresholdEmailName;
            LowCertificatesEmailRecepient = src.LowCertificatesEmailRecepient;
            MarkAsRedeemed = src.MarkAsRedeemed;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
        #endregion
    }
}
