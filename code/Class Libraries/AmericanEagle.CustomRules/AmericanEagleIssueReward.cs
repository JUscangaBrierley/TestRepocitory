using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

using Brierley.FrameWork.Rules.UIDesign;
using Brierley.FrameWork.Rules;
using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.Clients.AmericanEagle.DataModel;  // AEO Redesign 2015 Begin & End

using Brierley.FrameWork.bScript;
using AmericanEagle.SDK.Global;
using System.Collections;   // AEO Redesign 2015 Begin & End
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.CustomRules
{
    /// <summary>
    /// Custom IssueRule rule for AmericanEagle.  
    /// </summary>
    [Serializable]
    public class AmericanEagleIssueReward : RuleBase
    {


        #region Private Fields

        LWLogger _logger = LWLoggerManager.GetLogger("AmericanEagleIssueReward"); // AEO Redesign 2015 Begin & End
        private string className = "AmericanEagleIssueReward"; // AEO Redesign 2015 Begin & End
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private string rewardDefName = string.Empty;
        private string promotionCode = string.Empty;
        private string expirationDateLogic = string.Empty;
        private IssueRewardType issuedRewardType = IssueRewardType.Earned;
        private AmericanEagleIssueRewardUtil util = new AmericanEagleIssueRewardUtil();
        private DateTime processDate = DateTime.Now;
        private const string rewardType = "5D";
        private string rewardTypeCode = string.Empty; 

        #endregion

        #region Public Fields
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Promotion")]
        [Description("Used to select the the promotion to be used for this rule.")]
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //[RuleProperty(false, true, "AvailablePromotions")]
        [RuleProperty(false, true, false, "AvailablePromotions")]
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        [RulePropertyOrder(1)]
        public string PromotionCode
        {
            get
            {
                if (promotionCode != null)
                {
                    return promotionCode;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                promotionCode = value;
            }
        }

        /// <summary>
        /// Returns list of all available promotion codes
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> AvailablePromotions
        {
            get
            {
                Dictionary<string, string> promotions = new Dictionary<string, string>();
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //IList<Promotion> promoList = LWDataServiceUtil.DataServiceInstance(true).GetAllPromotions();
                //IList<Promotion> promoList = LWDataServiceUtil.DataServiceInstance(true).GetAllPromotions(new LWQueryBatchInfo() { BatchSize = 10, StartIndex = 0 });
                using (var contService = _dataUtil.ContentServiceInstance())
                {
                    IList<Promotion> promoList = contService.GetAllPromotions(null);
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    promotions.Add(string.Empty, string.Empty);
                    if (promoList != null && promoList.Count > 0)
                    {
                        foreach (Promotion promo in promoList)
                        {
                            promotions.Add(promo.Name, promo.Code);
                        }
                    }
                    return promotions;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Reward Type")]
        [Description("This rule will consume points.")]
        [RulePropertyOrder(2)]
        public IssueRewardType IssuedRewardType
        {
            get
            {
                return issuedRewardType;
            }
            set
            {
                issuedRewardType = value;
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Points Consumed When Issued")]
        [Description("Points are consumed when reward is issued to the account.")]
        [RulePropertyOrder(3)]
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
        [DisplayName("Reward Name")]
        [Description("Used to select the type of reward to be issued by this rule.")]
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //[RuleProperty(false, true, "RewardTypes")]
        [RuleProperty(false, true, false, "RewardTypes")]
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        [RulePropertyOrder(4)]
        public string RewardType
        {
            get
            {
                return rewardDefName;
            }
            set
            {
                rewardDefName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> RewardTypes
        {
            get
            {
                using (var contService = _dataUtil.ContentServiceInstance())
                {
                    Dictionary<string, string> rewardTypes = new Dictionary<string, string>();
                    IList<RewardDef> rewards = contService.GetAllRewardDefs();
                    foreach (RewardDef rdef in rewards)
                    {
                        rewardTypes.Add(rdef.Name, rdef.Name);
                    }
                    return rewardTypes;
                }
            }
        }

        [Browsable(false)]
        public RewardFulfillmentOption FulfillmentOption
        {
            get
            {
                return RewardFulfillmentOption.Electronic;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Expiration Date")]
        [Description("Defines the Expression used to calculate the expiration date of these rewards.")]
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //[RuleProperty(true, false, null)]
        [RuleProperty(true, false, false, null)]
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        [RulePropertyOrder(6)]
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
        [DisplayName("Reward Certificate Bucket")]
        [Description("This member provides the global attribute set that contains the valid reward certificate numbers.")]
        [RulePropertyOrder(8)]
        public string RewardCertificateBucket
        {
            get { return util.RewardCertificateBucket; }
            set { util.RewardCertificateBucket = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Certificate Type Code Attribute")]
        [Description("This property provides the name of the attribute in the above attribute set that contains the certificate typecode to be used for the reward.")]
        [RulePropertyOrder(9)]
        public string CertificateTypeCodeAttribute
        {
            get { return util.CertificateTypeCodeAttribute; }
            set { util.CertificateTypeCodeAttribute = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Certificate Number Attribute")]
        [Description("This property provides the name of the attribute in the above attribute set that contains the certificate number to be used for the reward.")]
        [RulePropertyOrder(10)]
        public string CertificateNmbrAttribute
        {
            get
            {
                return util.CertificateNmbrAttribute;
            }
            set
            {
                util.CertificateNmbrAttribute = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Certificate Status Attribute")]
        [Description("This property provides the name of the attribute in the above attribute set that contains the certificate status. As a certificate is used, its status is marked to 1.")]
        [RulePropertyOrder(11)]
        public string CertificateStatusAttribute
        {
            get
            {
                return util.CertificateStatusAttribute;
            }
            set
            {
                util.CertificateStatusAttribute = value;
            }
        }


        #endregion

        #region Public Methods
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        // public override void Invoke(Brierley.FrameWork.ContextObject Context, long PreviousResultCode)
        public override void Invoke(Brierley.FrameWork.ContextObject Context)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {

            Member lwmember = null;
            VirtualCard lwvirtualcard = null;
            string methodName = "Invoke";
            decimal rewardAmount = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            long rewardId = 0;
            decimal points = 0;
            string certificateNumber = string.Empty;
            string offerCode = string.Empty;
            string typeCode = string.Empty;
            DateTime expirationDate = DateTime.MinValue;

            // AEO-Redesign 2015 Begin
            Context.Results = new List<ContextObject.RuleResult>();
            // AEO-Redesign 2015 End

            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualcard);

            if (lwvirtualcard == null)
                lwvirtualcard = lwmember.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);

            using (var contService = _dataUtil.ContentServiceInstance())
            {
                RewardDef rdef = contService.GetRewardDef(RewardType);


                PointType pt = null;
                PointEvent pe = null;

                // AEO-592 begin
                if (!rdef.Name.ToUpper().Equals("AEO REWARDS BIRTHDAY REWARD"))
                {
                    points = util.GetPoints(rdef, pt, pe, lwmember, lwvirtualcard);
                }
                // AEO-592 end

                //


                _logger.Trace(string.Format("Processing member {0} with a current Point balance of {1}", lwmember.IpCode.ToString(), points.ToString()));

                bool issueReward = true;

                if (IssuedRewardType == IssueRewardType.Earned)
                {

                    // check to make sure that the member has enough points for this reward to be issued.

                    if (points < rdef.HowManyPointsToEarn)
                    {
                        string msg = string.Format("The reward requires {0} points.  Member with ipcode {1} only has {2} points.", rdef.HowManyPointsToEarn, lwmember.IpCode, points);
                        _logger.Trace(className, methodName, msg);
                        issueReward = false;
                    }
                    else
                    {

                        issueReward = true;
                        /* AEO-1441 begin
                        try
                        {
                            IClientDataObject memberDetails = lwmember.GetChildAttributeSets("MemberDetails")[0];

                            if (memberDetails == null)
                                issueReward = false;
                        }
                        catch
                        {
                            issueReward = false;
                        }
                        AEO-1441 end*/
                    }

                }

                if (issueReward)
                {
                    // Calculate the expiration date
                    DateTime expiryDate = DateTimeUtil.MaxValue;
                    if (!string.IsNullOrEmpty(this.expirationDateLogic))
                    {
                        try
                        {
                            ExpressionFactory exprF = new ExpressionFactory();
                            expiryDate = DateTime.Parse(exprF.Create(this.expirationDateLogic).evaluate(Context).ToString());
                            expirationDate = expiryDate;
                        }
                        catch (Exception ex)
                        {
                            string errMsg = string.Format("Error while calculating expiry date using Expression {0}", expirationDateLogic);
                            _logger.Error(className, methodName, errMsg, ex);
                            throw new LWRulesException(errMsg, ex);
                        }
                    }

                    if (IssuedRewardType == IssueRewardType.Entitlement)
                    {
                        // AEO-592 Begin
                        string rewardAmountString = "0";

                        if (!rdef.Name.ToUpper().Equals("AEO REWARDS BIRTHDAY REWARD"))
                        {
                            rewardAmountString = rdef.Name.Substring(1, 2);

                            rewardAmount = decimal.Parse(rewardAmountString);
                            rewardId = util.IssueRewardCertificate(lwmember, rdef, expiryDate, RewardFulfillmentOption.Electronic,
                                string.Empty, string.Empty, -1, string.Empty, rewardAmount);

                        }
                        else
                        {
                            try
                            {
                                if ((Context.Environment != null) && (Context.Environment.Count > 0))
                                {
                                    Dictionary<string, object> additionalFields = Context.Environment;
                                    object TypeCodeObject;
                                    object OfferTypeObject;
                                    object MonthOffsetObject;
                                    additionalFields.TryGetValue("TypeCode", out TypeCodeObject);
                                    additionalFields.TryGetValue("OfferType", out OfferTypeObject);
                                    // AEO-592 Fix
                                    additionalFields.TryGetValue("MonthOffset", out MonthOffsetObject);

                                    string rewardTypeCode = TypeCodeObject.ToString();
                                    string offerType = OfferTypeObject.ToString();
                                    int MonthOffset;

                                    if (MonthOffsetObject == null || !int.TryParse(MonthOffsetObject.ToString(), out MonthOffset))
                                    { // AEO-648

                                        MonthOffset = 0;
                                    }

                                    int tmpMonth = processDate.Month + MonthOffset; // AEO-648
                                    if (processDate.Month == 12)
                                    {
                                        tmpMonth = 1;
                                    }

                                }

                                _logger.Trace(string.Format("Issue Reward Certificate"));
                                rewardId = util.IssueRewardCertificate(lwmember, rdef, expirationDate,
                                    RewardFulfillmentOption.Electronic, certificateNumber, offerCode, -1, string.Empty, 0);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.StartsWith("No reward bar code valid"))
                                {
                                    _logger.Trace(string.Format("No reward bar codes for type: " + typeCode));

                                    throw new Exception("No reward bar codes for type: " + typeCode);
                                }
                                throw new Exception(ex.Message);
                            }

                        }


                        // AEO-592 End
                        IssueRewardRuleResult result = new IssueRewardRuleResult();
                        result.MemberId = lwmember.IpCode;
                        result.RewardId = rewardId;
                        result.Detail = "id=" + rewardId;
                        Context.Results.Add(result);
                    }
                    else
                    {
                        _logger.Trace(string.Format("member {0} has Points {1} for a reward", lwmember.IpCode.ToString(), points.ToString()));


                        MemberDetails mbrDetails = lwmember.GetChildAttributeSets("MemberDetails")[0] as MemberDetails;

                        bool isPilot = false;

                        isPilot = ((mbrDetails != null) && (Utilities.isInPilot(mbrDetails.ExtendedPlayCode))); // point converiso



                        while (points >= rdef.HowManyPointsToEarn)
                        {
                            decimal totalPoints = rdef.HowManyPointsToEarn;      // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ

                            try
                            {
                                if ((Context.Environment != null) && (Context.Environment.Count > 0))
                                {
                                    Dictionary<string, object> additionalFields = Context.Environment;
                                    object TypeCodeObject;
                                    bool isDM = false;
                                    if (additionalFields.TryGetValue("TypeCode", out TypeCodeObject))
                                    {
                                        isDM = TypeCodeObject.ToString() == "5DDM";
                                    }

                                    // check if the reward is the $10 or the $5
                                    object rewType;
                                    bool is10Reward = false;
                                    if (additionalFields.TryGetValue("RewardType", out rewType))
                                    {
                                        is10Reward = rewType.ToString() == "$10";
                                    }


                                    string rewardTypeCode = TypeCodeObject.ToString();

                                }
                                else
                                {
                                    Dictionary<string, object> additionalFields = Context.Environment;

                                    bool isDM = false;
                                    object TypeCodeObject;
                                    if (additionalFields.TryGetValue("TypeCode", out TypeCodeObject))
                                    {
                                        isDM = TypeCodeObject.ToString() == "5DDM";
                                    }


                                    // check if the reward is the $10 or the $5
                                    object rewType;
                                    bool is10Reward = false;
                                    if (additionalFields.TryGetValue("RewardType", out rewType))
                                    {
                                        is10Reward = rewType.ToString() == "$10";
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.StartsWith("No reward bar code valid"))
                                {
                                    _logger.Trace(string.Format("No reward bar codes for type: " + typeCode));

                                    throw new Exception("No reward bar codes for type: " + typeCode);
                                }
                                throw new Exception(ex.Message);
                            }


                            _logger.Trace(string.Format("Issue Reward Certificate"));
                            rewardId = util.IssueRewardCertificate(lwmember, rdef, expirationDate, RewardFulfillmentOption.Electronic, certificateNumber, offerCode, -1, string.Empty, 0);
                            // AEO-Redesig-2015 Begin
                            IssueRewardRuleResult result = new IssueRewardRuleResult();
                            result.MemberId = lwmember.IpCode;
                            result.RewardId = rewardId;
                            result.Detail = rewardId.ToString();
                            Context.Results.Add(result);


                            // Issue reward
                            if (IssuedRewardType == IssueRewardType.Earned)
                            {
                                switch (PointsConsumption)
                                {
                                    case PointsConsumptionOnIssueReward.Consume:
                                        _logger.Trace(string.Format("Consume Points {0} for reward {1}", totalPoints, rewardId.ToString()));
                                        util.ConsumePoints(lwmember, lwvirtualcard, rdef, pt, rewardId, totalPoints);
                                        break;
                                    case PointsConsumptionOnIssueReward.NoAction:
                                    default:
                                        _logger.Debug(className, methodName, "Points consumption being skipped.");
                                        break;
                                }
                            }

                            points -= totalPoints;


                        }

                    }

                }

                //  return rcode;
                contService.Dispose(); // AEO-2114
            }
        }
        public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceDataSvc, ServiceConfig targetDataSvc)
        {
            throw new NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public override string DisplayText { get { return " "; } }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ

        #endregion

        #region Constructors
        public AmericanEagleIssueReward()
            : base("AmericanEagleIssueReward")
        {
            _logger.Debug("AmericanEagleIssueReward has been instantiated.");
        }
        #endregion
    }
}
