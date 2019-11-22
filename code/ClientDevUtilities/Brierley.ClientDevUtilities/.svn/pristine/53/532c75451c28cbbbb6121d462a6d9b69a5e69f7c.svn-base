using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork.Email;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Rules.UIDesign;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EvaluateTier : RuleBase
    {

        #region Results
        public class EvaluateTierRuleResult : ContextObject.RuleResult
        {
            public string   PreviousTier;
            public string   NewTier;
            //public string   Reason;
        }
        #endregion

        #region Private Variables

        public static string INITIAL_TIER_ENROLLMENT = "InitialTierEnrollment";
        public static string TIER_DOWNGRADE = "Downgraded";
        public static string TIER_UPGRADE = "Upgraded";
        public static string TIER_MEMBERMERGE = "MembersMerged";
        public static string TIER_REQUALIFIED = "Requalified";
        public static string TIER_OVERRIDE = "Overridden";

        [NonSerialized]
        private string _className = "EvaluateTier";

        [NonSerialized]
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
                                
        private string _virtualCardNumber = string.Empty;
        private string _tiers = string.Empty;
        private string _expireDate = string.Empty;
        private bool _overridePointLogic = false;
        private string _overrideTier = string.Empty;
        private bool _includeExpiredPoints = false;
        private string _tierUpgradeEmailName = string.Empty;
        private string _tierDowngradeEmailName = string.Empty;
                        
        private VirtualCardLocation _virtualCardLocationLogic = VirtualCardLocation.AllCards;        

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public EvaluateTier()
            : base("EvaluateTier")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Validate()
        {
            if (_virtualCardLocationLogic == VirtualCardLocation.UseExpression && string.IsNullOrEmpty(_virtualCardNumber))
            {
                throw new Exception("Virtual Card Number Expression is required when Virtual Card Logic is set to UseExpression.");
            }
            //if (string.IsNullOrEmpty(Tiers))
            //{
            //    throw new Exception("Please select the tiers that should be checked by this rule.");
            //}
        }

        #region Properties

        public override string DisplayText
        {
            get { return "Evaluate Tier"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        public bool OverridePointsLogic
        {
            get
            {
                return _overridePointLogic;
            }
            set
            {
                _overridePointLogic = value;
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Override point basis")]
        [Description("Defines whether or not the rule will use point basis to determine the new tier.")]
        [RulePropertyOrder(1)]
        [RuleProperty(false, true, false, "TierNamesWithBlank", false, true)]
        public string OverrideTier
        {
            get
            {
                if (string.IsNullOrEmpty(_overrideTier) && _overridePointLogic)
                {
                    _overrideTier = !string.IsNullOrEmpty(Tiers) ? Tiers.Split(';')[0] : string.Empty;
                    _overridePointLogic = false;
                }
                return _overrideTier;
            }
            set
            {
                _overrideTier = value;
            }
        }

        [Browsable(false)]
        public Dictionary<string, string> TierNamesWithBlank
        {
            get
            {
                return (new Dictionary<string, string>() { { string.Empty, string.Empty } }).Concat(TierNames).ToDictionary(k => k.Key, v => v.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Include expired points")]
        [Description("Defines whether expired points should also be used in tier calculation or not.")]
        [RulePropertyOrder(2)]
        [RuleProperty(false, false, false, null, false, true)]
        public bool IncludeExpiredPoints
        {
            get
            {
                return _includeExpiredPoints;
            }
            set
            {
                _includeExpiredPoints = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Virtual Card Location Logic")]
        [Description("This member determines how the rule locates the appropriate virtual card in cases where the rule is connected to an attribute set that belongs to the memeber.")]
        [RulePropertyOrder(3)]
        [RuleProperty(false, false, false, null, false, true)]
        public VirtualCardLocation VirtualCardLocationLogic
        {
            get
            {
                return _virtualCardLocationLogic;
            }
            set
            {
                _virtualCardLocationLogic = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Virtual Card Number Expression")]
        [Description("This member represents the card number field from data stream. EX: row.loyaltyID")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(4)]
        public string VirtualCardNumber
        {
            get
            {
                return _virtualCardNumber;
            }
            set
            {
                _virtualCardNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Tiers")]
        [Description("The tiers that should be considered for tier evaluation.")]
        [RuleProperty(false, true, true, "TierNames")]
        [RulePropertyOrder(5)]
        public string Tiers
        {
            get { return _tiers; }
            set { _tiers = value; }
        }


        /// <summary>
        /// Returns a dictionary of all defined Tiers
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> TierNames
        {
            get
            {
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					Dictionary<string, string> tierMap = new Dictionary<string, string>();
					List<TierDef> tiers = svc.GetAllTierDefs();
					foreach (TierDef p in tiers)
					{
						tierMap.Add(p.Name, p.DisplayText);
					}
					return tierMap;
				}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Date to use for expiration date calculation")]
        [Description("This value is used to determine the date to use for point expiration calculations. This value is an expression and it defaults to the current date 'Date()'. If your attribute set defines another date that could be used you may reference that field using the row operator. 'example row.transaction_date'")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(6)]
        public string ExpireDate
        {
            get
            {
                return _expireDate;
            }
            set
            {
                _expireDate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Tier Upgrade Email")]
        [Description("Used to select the triggered email that will be sent out when a member's is initially put in a tier or upgraded.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails")]
        [RulePropertyOrder(7)]
        public string TierUpgradeEmailName
        {
            get { return _tierUpgradeEmailName; }
            set { _tierUpgradeEmailName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Tier Downgrade Email")]
        [Description("Used to select the triggered email that will be sent out when a member's tier is downgraded.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails")]
        [RulePropertyOrder(8)]
        public string TierDowngradeEmailName
        {
            get { return _tierDowngradeEmailName; }
            set { _tierDowngradeEmailName = value; }
        }

        /// <summary>
        /// Returns list of all available triggered emails
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> AvailableTriggeredEmails
        {
            get
            {
				using (var svc = LWDataServiceUtil.EmailServiceInstance())
				{
					Dictionary<string, string> emails = new Dictionary<string, string>();
					List<EmailDocument> emailList = svc.GetEmails();
					if (emailList != null && emailList.Count > 0)
					{
						emails.Add(string.Empty, string.Empty);
						foreach (EmailDocument email in emailList)
						{
							emails.Add(email.Name, email.Name);
						}
					}
					return emails;
				}
            }
        }
        #endregion

        #region Helper Methods
                
        private decimal GetCumulativePoints(ExpressionFactory exprF,ContextObject context, Member member, string [] pointTypes, string [] pointEvents, DateTime from, DateTime to, bool includeExpiredPoints)
        {
            string methodName = "GetCumulativePoints";

            decimal points = 0;
			
            if (VirtualCardLocationLogic == VirtualCardLocation.AllCards)
            {                
                var cardList = (from v in member.LoyaltyCards where v.Status == VirtualCardStatusType.Active select v);
                if (cardList.Count() > 0)
                {
                    points = TierUtil.GetCumulativePoints(member, cardList.ToList<VirtualCard>(), pointTypes, pointEvents, from, to, includeExpiredPoints);
                }
                else
                {
                    _logger.Trace(_className, methodName,
                       string.Format("There is no active card for the member ipcode {0} for calculating points to next tier", member.IpCode));
                }                
            }
            else
            {
                VirtualCard vc = null;
                if (VirtualCardLocationLogic == VirtualCardLocation.UseExpression)
                {
                    // When using this
                    // setting rule will use the expression defined in VirtualCardNumber, the rule
                    // will evaulate this expression to get the card number. For example: lets say we are
                    // processing transactions on a file. The program as defined allows members to use any
                    // valid card they may have in their possession as long as the card is registered. There
                    // really is no concept of a primary card defined in the program. The transaction summary
                    // record data contains the loyalty id number of the card used in the purchase in a field
                    // called 'loyaltyidnumber'. In this scenario we might use an expression of the form
                    // Row.Loyaltyidnumber to obtain the card number. Keep in mind that the rule executes
                    // within the context of the invoking row, so each time it executes it will have the
                    // current card number in that row.
                    string cardid = exprF.Create(this._virtualCardNumber).evaluate(context).ToString();
                    vc = member.GetLoyaltyCard(cardid);
                }
                if (VirtualCardLocationLogic == VirtualCardLocation.FirstCardInList)
                {
                    vc = member.LoyaltyCards[0];
                }
                if (VirtualCardLocationLogic == VirtualCardLocation.PrimaryCard)
                {
                    foreach (VirtualCard virtualCard in member.LoyaltyCards)
                    {
                        if (virtualCard.IsPrimary == true)
                        {
                            vc = virtualCard;
                        }
                    }
                }
                if (vc == null || vc.Status != VirtualCardStatusType.Active)
                {
                    _logger.Trace(_className, methodName,
                       string.Format("No active card located for the member ipcode {0}.", member.IpCode));
                }
                else
                {
                    List<VirtualCard> vcs = new List<VirtualCard>();
                    vcs.Add(vc);
                    points = TierUtil.GetCumulativePoints(member, vcs, pointTypes, pointEvents, from, to, includeExpiredPoints);                    
                }
            }
            return points;
        }

        private List<TierDef> GetTiersToCheck()
        {
            if (string.IsNullOrEmpty(Tiers))
            {
                return new List<TierDef>();
            }
            else
            {
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					string[] tokens = Tiers.Split(';');
					List<TierDef> tiers = new List<TierDef>();
					foreach (string tierName in tokens)
					{
						TierDef tier = svc.GetTierDef(tierName);
						tiers.Add(tier);
					}
					return tiers;
				}
            }
        }

        public void SendTriggeredEmail(Member member, string emailToSend, Dictionary<string, string> fields)
        {
            string method = "SendTriggeredEmail";

            try
            {
                using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailToSend))
                {
                    if (fields != null)
                    {
                        email.SendAsync(member, fields).Wait();
                    }
                    else
                    {
                        email.SendAsync(member).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error sending TriggeredEmail using mailing {0} for member with ipcode {1}",
                    emailToSend, member.MyKey);
                _logger.Error(_className, method, msg, ex);
            }
        }

        public void SendTierChangedEmail(Member member, string emailName, TierDef newTier)
        {
            if (!string.IsNullOrEmpty(emailName))
            {                
                Dictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add("TierName", newTier.Name);                
                SendTriggeredEmail(member, emailName, fields);
            }
        }

        private string[] GetPointTypes(TierDef tier, LoyaltyDataService svc)
        {
            string[] pointTypes = tier.GetPointTypes();
            if (pointTypes == null || pointTypes.Length < 1)
            {
                List<PointType> listPointTypes = svc.GetAllPointTypes();
                pointTypes = listPointTypes.Select(s => s.Name).ToArray();
            }
            return pointTypes;
        }

        private string[] GetPointEvents(TierDef tier, LoyaltyDataService svc)
        {
            string[] pointEvents = tier.GetPointEvents();
            if (pointEvents == null || pointEvents.Length < 1)
            {
                List<PointEvent> listPointEvents = svc.GetAllPointEvents();
                pointEvents = listPointEvents.Select(s => s.Name).ToArray();
            }
            return pointEvents;
        }

        private bool MemberQualifiesForTier(ContextObject Context, LoyaltyDataService svc, ExpressionFactory exprF, Member lwmember, TierDef rtier)
        {
            const string methodName = "IsMemberOutsideTierPointsRange";
            DateTime from = DateTime.Now;
            DateTime to = DateTime.Now;
            TierUtil.CalculateTierActivityDates(exprF, Context, ref from, ref to, rtier);
            string[] pointTypes = this.GetPointTypes(rtier, svc);
            string[] pointEvents = this.GetPointEvents(rtier, svc);
            decimal points = GetCumulativePoints(exprF, Context, lwmember, pointTypes, pointEvents, from, to, _includeExpiredPoints);
            _logger.Debug(_className, methodName, string.Format("Cumulative points of member {0} for tier '{1}' are {2}", lwmember.IpCode, rtier.Name, points));
            return rtier.Qualifies(points);
        }

        #endregion

        public override void Invoke(ContextObject Context)
        {
            string methodName = "Invoke";

			ExpressionFactory exprF = new ExpressionFactory();

			Member lwmember = null;
			VirtualCard lwvirtualcard = null;

            EvaluateTierRuleResult result = new EvaluateTierRuleResult() 
            {
                Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
                Mode = Context.Mode, 
                RuleType = this.GetType(),
                Detail = "No tier changes made."
            };

			ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualcard);
            if (lwmember == null)
            {
                string errMsg = "No member could be resolved for evaluate tier rule.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3214 };
            }

            result.MemberId = lwmember.IpCode;

            _logger.Trace(_className, methodName, "Invoking Evaluate tier for member " + lwmember.IpCode);

			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				DateTime? expirationDate = null;
				if (!string.IsNullOrEmpty(ExpireDate))
				{
					expirationDate = (DateTime)exprF.Create(ExpireDate).evaluate(Context);
				}

				MemberTier tier = svc.GetMemberTier(lwmember, DateTime.Now);
                // If the tier expired before now, then it came back null so we need to check if they were ever in a tier
                // It could be that the member never had a tier or it could be that they simply expired out of an old one
                bool tierExpired = false;
                if(tier == null)
                {
                    var tierHistory = svc.GetMemberTiers(lwmember);
                    if (tierHistory != null && tierHistory.Count > 0) // Else the member never had a tier
                    {
                        tierHistory.Sort((t1, t2) => t2.FromDate.CompareTo(t1.FromDate)); // Sorting it descending so we get the latest one first
                        tier = tierHistory[0];
                        tierExpired = DateTimeUtil.GreaterEqual(DateTime.Now, tier.ToDate); // Extremely likely true, but just to be sure
                    }
                }
                TierDef currentTierDef = tier != null ? svc.GetTierDef(tier.TierDefId) : null;

                if (!string.IsNullOrEmpty(OverrideTier) && (tier == null || (currentTierDef != null && currentTierDef.Name != OverrideTier))) // Override flag set
                {
                    _logger.Debug(_className, methodName, string.Format("Overriding tier logic to {0}", OverrideTier));
                    TierDef rtier = svc.GetTierDef(OverrideTier);
                    // just move the member in the first tier checked on the rule.
                    result.PreviousTier = tier != null ? svc.GetTierDef(tier.TierDefId).Name : null;
                    result.NewTier = rtier.Name;
                    result.Detail = TIER_OVERRIDE;

                    if (Context.Mode == RuleExecutionMode.Real)
                    {
                        lwmember.AddTier(rtier.Name, DateTime.Now, expirationDate, result.Detail);
                        SendTierChangedEmail(lwmember, _tierUpgradeEmailName, rtier);
                    }
                }
                else if (tier == null || // Member has no tier
                         tierExpired || // Current tier expired
                         !MemberQualifiesForTier(Context, svc, exprF, lwmember, currentTierDef)) // Member's points are < entry points or > exit points
                {
                    // Maintaining old logging strings
                    if (tier == null)
                        _logger.Debug(_className, methodName, "Member is currently not in a tier.");
                    else if (tierExpired)
                        _logger.Debug(_className, methodName, string.Format("{0} has expired.  Member is not in a tier.", currentTierDef.Name));
                    else
                        _logger.Debug(_className, methodName, string.Format("{0} is currently in {1} tier.", lwmember.IpCode, currentTierDef.Name));

                    List<TierDef> allTiers = GetTiersToCheck();

                    foreach (TierDef rtier in allTiers)
                    {
                        if (MemberQualifiesForTier(Context, svc, exprF, lwmember, rtier))
                        {
                            string tierChangedEmailName = _tierUpgradeEmailName;

                            result.PreviousTier = tier != null ? svc.GetTierDef(tier.TierDefId).Name : null;
                            result.NewTier = rtier.Name;

                            // Determine the result detail and new member tier description
                            if (tier == null)
                                result.Detail = INITIAL_TIER_ENROLLMENT;
                            else if (tierExpired && currentTierDef.Id == rtier.Id)
                            {
                                result.Detail = TIER_REQUALIFIED;
                                tierChangedEmailName = _tierDowngradeEmailName;
                            }
                            else if (currentTierDef.EntryPoints < rtier.EntryPoints || currentTierDef.ExitPoints < rtier.ExitPoints)
                                result.Detail = TIER_UPGRADE;
                            else
                            {
                                result.Detail = TIER_DOWNGRADE;
                                tierChangedEmailName = _tierDowngradeEmailName;
                            }

                            if (Context.Mode == RuleExecutionMode.Real)
                            {
                                lwmember.AddTier(rtier.Name, DateTime.Now, expirationDate, result.Detail);
                                SendTierChangedEmail(lwmember, tierChangedEmailName, rtier);
                            }
                            break;
                        }
                    }
                }
                // else Member is currently in a valid tier that they are eligible for
			}

            AddRuleResult(Context, result);

            return;
        }

        #region Migrartion Helpers

        public override List<string> GetBscriptsToMove()
        {
            List<string> bscriptList = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.VirtualCardNumber) && ExpressionUtil.IsLibraryExpression(this.VirtualCardNumber))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.VirtualCardNumber));
            }
            if (!string.IsNullOrWhiteSpace(this.ExpireDate) && ExpressionUtil.IsLibraryExpression(this.ExpireDate))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.ExpireDate));
            }
            return bscriptList;
        }

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating EvaluateTier rule.");

            EvaluateTier src = (EvaluateTier)source;

            ExpireDate = src.ExpireDate;
            IncludeExpiredPoints = src.IncludeExpiredPoints;
            OverridePointsLogic = src.OverridePointsLogic;
            OverrideTier = src.OverrideTier;
            VirtualCardLocationLogic = src.VirtualCardLocationLogic;
            VirtualCardNumber = src.VirtualCardNumber;
            Tiers = src.Tiers;
            TierUpgradeEmailName = src.TierUpgradeEmailName;
            TierDowngradeEmailName = src.TierDowngradeEmailName;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
        #endregion
    }
}
