using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

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
using AmericanEagle.SDK.Global;   // AEO Redesign 2015 Begin & End
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.CustomRules
{
    [Serializable]
    public class AEAwardPoints : RuleBase
    {
        [NonSerialized]
        private const string _className = "AEAwardPoints";

        [NonSerialized]
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        //private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        private PointEvent _pointEventType = null;
        private string pointEventTypeName = string.Empty;
        private PointType _pointType = null;
        private string pointTypeName = string.Empty;
        private PointType _negativeAdjPointType = null;
        private string negativeAdjPointTypeName = string.Empty;

        private string _accrualFactor = string.Empty;
        private int AEPtsonholdDaycount = 0;
        private const string _cacheRegion = "AEOCache";
        private string _descriptionExpression;
        private string _expireDate = "Date()";
        private string _pointBankTrxDate = "Date()";
        private string _virtualCardNumber = string.Empty;
        private string _childAttributeSetName = string.Empty;

        private string _locationExpression = string.Empty;
        private string _changedBy = string.Empty;

        // transient property to set
        private string _transientPropertyName = string.Empty;
        private string _transientPropertyExpression;

        private bool _allowZeroPoints = false;
        private bool _allowNegativePointBalance = true;
        private PointExpirationLogic _pointExpirationMethod;
        private long _expirationDays = 0;
        private VirtualCardLocation _virtualCardLocationLogic = VirtualCardLocation.FirstCardInList;
        private PointAwardMethod _pointAwardMethod = PointAwardMethod.Normal;
        private PointBatchingMode _pointBatchMode = PointBatchingMode.PerRecord;

        private string _evaluateTierRule = string.Empty;


        public override string DisplayText
        {
            get { return "Award Loyalty Currency"; }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Negative Adjustment Currency")]
        [Description("Used to set the loyalty currency for negative point adjustments.")]
        [RuleProperty(false, true, false, "PointTypes", false, true)]
        [RulePropertyOrder(19)]
        public string NegativeAdjustmentPointType
        {
            get { return negativeAdjPointTypeName; }
            set { negativeAdjPointTypeName = value; }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Transient Property to Set")]
        [Description("The name of the transient property to set")]
        [RuleProperty(false, false, false, null, false, true)]
        [RulePropertyOrder(20)]
        public string TransientPropertyName
        {
            get { return _transientPropertyName; }
            set { _transientPropertyName = value; }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Transient Property Expression")]
        [Description("The expression to use to set the transient property")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(21)]
        public string TransientPropertyExpression
        {
            get { return _transientPropertyExpression; }
            set { _transientPropertyExpression = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Location Id")]
        [Description("Location id of the store where this transaction took place.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(22)]
        public string LocationIdExpression
        {
            get { return _locationExpression; }
            set { _locationExpression = value; }
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
        [RulePropertyOrder(23)]
        public string ChangedByExpression
        {
            get { return _changedBy; }
            set { _changedBy = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Tier Evaluation Rule")]
        [Description("The rule to be run to evaluate tier")]
        [RuleProperty(false, true, false, "RuleList", false, true)]
        [RulePropertyOrder(24)]
        public string EvaluateTierRule
        {
            get { return _evaluateTierRule; }
            set { _evaluateTierRule = value; }
        }


        /// <summary>
        /// Returns a dictionary of all defined rules
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> RuleList
        {
            get
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    Dictionary<string, string> ruleMap = new Dictionary<string, string>();
                    ruleMap.Add(string.Empty, string.Empty);
                    List<RuleTrigger> rules = service.GetAllRules();
                    foreach (RuleTrigger p in rules)
                    {
                        ruleMap.Add(p.RuleName, p.RuleName);
                    }
                    return ruleMap;
                }
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Point Award Method")]
        [Description("Sets the method used at runtime to award points. If set to Use Child the rule will award points based on the values in a child attribute set.")]
        [RulePropertyOrder(3)]
        [RuleProperty(false, false, false, null, false, true)]
        public PointAwardMethod PointAwardMethod
        {
            get
            {
                return this._pointAwardMethod;
            }
            set
            {
                this._pointAwardMethod = value;
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Child Attribute Set Name")]
        [Description("Defines the name of the child attribute set to use if the rule defines and award point method of UseChild. The name must be the name of a valid child attribute set of the set upon which this rule is dropped.")]
        [RulePropertyOrder(4)]
        [RuleProperty(false, false, false, null, false, true)]
        public string ChildAttributeSetName
        {
            get
            {
                return _childAttributeSetName;
            }
            set
            {
                _childAttributeSetName = value;
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Point Batch Mode")]
        [Description("Defines the batching mode to use when processing point transactions. The default is PerRecord. Under normal conditions when points are awarded based on Rows of data from the attribute set where the rule is dropped the setting has no real effect. Batched and PerRecord are really the same thing. However is the rule is using Child attribute sets to process data then it can either batch the points or award them based on individual records/rows of data.")]
        [RulePropertyOrder(5)]
        [RuleProperty(false, false, false, null, false, true)]
        public PointBatchingMode PointBatchMode
        {
            get
            {
                return _pointBatchMode;
            }
            set
            {
                _pointBatchMode = value;
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Virtual Card Location Logic")]
        [Description("This member determines how the rule locates the appropriate virtual card in cases where the rule is connected to an attribute set that belongs to the memeber.")]
        [RulePropertyOrder(6)]
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


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Virtual Card Number Expression")]
        [Description("This member represents the card number field from data stream. EX: row.loyaltyID")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(7)]
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


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Date to use for expiration date calculation")]
        [Description("This value is used to determine the date to use for point expiration calculations. This value is an expression and it defaults to the current date 'Date()'. If your attribute set defines another date that could be used you may reference that field using the row operator. 'example row.transaction_date'")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(8)]
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


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Point Expiration Logic")]
        [Description("Controls the logic used by the rule in calculating the point expiration date for points granted by this rule.")]
        [RulePropertyOrder(9)]
        [RuleProperty(false, false, false, null, false, true)]
        public PointExpirationLogic PointsExpirationMethod
        {
            get
            {
                return _pointExpirationMethod;
            }
            set
            {
                _pointExpirationMethod = value;
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Expiration Days")]
        [Description("Defines the number of days before points granted by this rule will expire. This value works in conjunction with TrxDatePlusDays and CurrentDatePlusDays point expiration logic values. It is ignored when using other methods of point expiration.")]
        [RulePropertyOrder(10)]
        [RuleProperty(false, false, false, null, false, true)]
        public long ExpirationDays
        {
            get
            {
                return _expirationDays;
            }
            set
            {
                _expirationDays = value;
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Point Bank Transaction Date")]
        [Description("Date to use for point bank transactions generated by this rule")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(11)]
        public string PointBankTransactionDate
        {
            get
            {
                return _pointBankTrxDate;
            }
            set
            {
                this._pointBankTrxDate = value;
            }
        }


        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Loyalty Event")]
        [Description("Sets the loyalty event type used by this instance as defined by the database")]
        [RuleProperty(false, true, false, "PointEventTypes")]
        [RulePropertyOrder(12)]
        public string PointEventType
        {
            get { return pointEventTypeName; }
            set { pointEventTypeName = value; }
        }

        /// <summary>
        /// Returns a dictionary of all defined PointEvents
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> PointEventTypes
        {
            get
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    Dictionary<string, string> _pointEvents = new Dictionary<string, string>();
                    IList<PointEvent> peList = service.GetAllPointEvents();
                    if (peList != null)
                    {
                        foreach (PointEvent p in peList.OrderBy(o => o.Name))
                        {
                            _pointEvents.Add(p.Name, p.Name);
                        }
                    }
                    return _pointEvents;
                }
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Loyalty Currency")]
        [Description("Used to select the type of loyalty currency awarded by this rule")]
        [RuleProperty(false, true, false, "PointTypes")]
        [RulePropertyOrder(13)]
        public string PointType
        {
            get { return pointTypeName; }
            set { pointTypeName = value; }
        }

        [Browsable(false)]
        public Dictionary<string, string> PointTypes
        {
            get
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    Dictionary<string, string> _pointTypes = new Dictionary<string, string>();
                    IList<PointType> ptList = service.GetAllPointTypes();
                    if (ptList != null)
                    {
                        foreach (PointType pt in ptList.OrderBy(o => o.Name))
                        {
                            _pointTypes.Add(pt.Name, pt.Name);
                        }
                    }
                    return _pointTypes;
                }
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Accrual Expression")]
        [Description("Defines the factor used to accrue points")]
        [RuleProperty(true, false, false, null, true)]
        [RulePropertyOrder(14)]
        public string AccrualExpression
        {
            get { return _accrualFactor; }
            set { _accrualFactor = value; }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Allow Zero Point Award")]
        [Description("Defines whether or not the rule will enter point transactions for zero points.")]
        [RulePropertyOrder(15)]
        [RuleProperty(false, false, false, null, false, true)]
        public bool AllowZeroPoints
        {
            get
            {
                return _allowZeroPoints;
            }
            set
            {
                _allowZeroPoints = value;
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Description Expression")]
        [Description("Defines the expression used to generate the description associated with the point transaction")]
        [RuleProperty(true, false, false, null)]
        [RulePropertyOrder(17)]
        public string DescriptionExpression
        {
            get
            {
                return _descriptionExpression;
            }
            set
            {
                _descriptionExpression = value;
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Allow Negative Point Balance")]
        [Description("Defines whether or not the rule will provide an off setting point bank transaction following the rule processing that will prevent negative point balances.")]
        [RulePropertyOrder(18)]
        [RuleProperty(false, false, false, null, false, true)]
        public bool AllowNegativePointBalance
        {
            get
            {
                return _allowNegativePointBalance;
            }
            set
            {
                _allowNegativePointBalance = value;
            }
        }

        public AEAwardPoints()
            : base("AEAwardPoints")
        {
        }

        public override void Validate()
        {
            if (_virtualCardLocationLogic == VirtualCardLocation.UseExpression && string.IsNullOrEmpty(_virtualCardNumber))
            {
                throw new Exception("Virtual Card Number Expression is required when Virtual Card Logic is set to UseExpression.");
            }
            if (string.IsNullOrEmpty(_accrualFactor))
            {
                throw new Exception("No accrual expression.");
            }
        }

        public override void Invoke(ContextObject context)
        {
            string methodName = "Invoke";

            Member lwmember = null;
            VirtualCard lwvirtualCard = null;

            string locationId = null;
            string changeBy = null;

            ExpressionFactory exprF = new ExpressionFactory();

            var env = context.Environment;
            if (env != null && env.ContainsKey("LocationIdExpression"))
            {
                locationId = exprF.Create((string)env["LocationIdExpression"]).evaluate(context).ToString();
            }
            else if (!string.IsNullOrEmpty(_locationExpression))
            {
                locationId = exprF.Create(this._locationExpression).evaluate(context).ToString();
            }

            if (env != null && env.ContainsKey("ChangedByExpression"))
            {
                changeBy = exprF.Create((string)env["ChangedByExpression"]).evaluate(context).ToString();
            }
            else if (!string.IsNullOrEmpty(_changedBy))
            {
                changeBy = exprF.Create(this._changedBy).evaluate(context).ToString();
            }

            // Remember a rule must be generic in nature and agnostic with regard to what
            // type of attribute set it will get dropped on. You never know whether the 
            // invoking row belongs to an attribute set owned by another attribute set,
            // or by a member, or a virtual card. Both Member and Virtual Cards derive from
            // attribute set collection. We must take this into account when setting things
            // up to run the rule. So... We do some basic type checking...

            ResolveOwners(context.Owner, ref lwmember, ref lwvirtualCard);

            if (lwmember == null)
            {
                _logger.Error(_className, methodName, "Unable to resolve a member.");
                throw new LWException("Unable to resolve a member.") { ErrorCode = 3214 };
            }

            long ownerId = context.InvokingRow != null ? context.InvokingRow.GetMetaData().ID : 0;
            long rowKey = context.InvokingRow != null ? context.InvokingRow.RowKey : 0;

            PointTransactionOwnerType ownerType = PointTransactionOwnerType.Unknown;
            if (ownerId != 0)
            {
                ownerType = PointTransactionOwnerType.AttributeSet;
            }
            else
            {
                if (context.Environment.ContainsKey("OwnerType"))
                {
                    ownerType = (PointTransactionOwnerType)context.Environment["OwnerType"];
                }
                if (context.Environment.ContainsKey("OwnerId"))
                {
                    ownerId = (long)context.Environment["OwnerId"];
                }
                if (context.Environment.ContainsKey("RowKey"))
                {
                    rowKey = (long)context.Environment["RowKey"];
                }
            }

            _logger.Trace(_className, methodName, "Invoking AwardPoints for member " + lwmember.MyKey);

            // Notes is nothing more than a container to house an expression that will result
            // in a string value. Notes can be used to concatenate data that might be helpfull
            // later in reconstructing what happened when the rule ran and points were
            // awarded.
            string notes = string.Empty;
            if (!string.IsNullOrEmpty(this._descriptionExpression))
            {
                notes = exprF.Create(this._descriptionExpression).evaluate(context).ToString();
            }

            // The next several chunks of code are concerned with determining how and when
            // the points that are about to be awarded expire. The process starts by figuring out
            // what date we are starting from. This value is a bScript expression and would
            // commonly be something like Date() if you want to start from the day the rule ran
            // and processed the data, or it maybe some expression like Row.TransactionDate if you
            // want to use a date supplied by the data. Regardless once calculated we now know from
            // what date we are starting...
            DateTime pointsExpStartDate = DateTimeUtil.MinValue;
            pointsExpStartDate = (DateTime)exprF.Create(this.ExpireDate).evaluate(context);

            // ... Next we figure out what date should be associated with the points that are credited
            // into the members account. This is the date that the point bank will be given to record the
            // point bank transaction date...
            DateTime pointsTransactionDate = DateTimeUtil.MinValue;
            try
            {
                pointsTransactionDate = (DateTime)exprF.Create(this._pointBankTrxDate).evaluate(context);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error getting TransactionDate from expression '{0}'", this._pointBankTrxDate);
                _logger.Error(_className, methodName, msg, ex);
                throw new LWException(msg, ex);
            }

            // ... Next we figure out what the current quarter and the next quarters ending date is
            DateTime currentQtrEndDate = DateTimeUtil.MinValue;
            DateTime nextQtrEndDate = DateTimeUtil.MinValue;
            if (pointsExpStartDate.Month == 1 || pointsExpStartDate.Month == 2 || pointsExpStartDate.Month == 3)
            {
                currentQtrEndDate = new DateTime(pointsExpStartDate.Year, 3, 31);
                nextQtrEndDate = new DateTime(pointsExpStartDate.Year, 6, 30);
            }
            if (pointsExpStartDate.Month == 4 || pointsExpStartDate.Month == 5 || pointsExpStartDate.Month == 6)
            {
                currentQtrEndDate = new DateTime(pointsExpStartDate.Year, 6, 30);
                nextQtrEndDate = new DateTime(pointsExpStartDate.Year, 9, 30);
            }
            if (pointsExpStartDate.Month == 7 || pointsExpStartDate.Month == 8 || pointsExpStartDate.Month == 9)
            {
                currentQtrEndDate = new DateTime(pointsExpStartDate.Year, 9, 30);
                nextQtrEndDate = new DateTime(pointsExpStartDate.Year, 12, 31);
            }
            if (pointsExpStartDate.Month == 10 || pointsExpStartDate.Month == 11 || pointsExpStartDate.Month == 12)
            {
                currentQtrEndDate = new DateTime(pointsExpStartDate.Year, 12, 31);
                nextQtrEndDate = new DateTime(pointsExpStartDate.Year + 1, 1, 31);
            }

            _logger.Debug(
                _className,
                methodName,
                string.Format(
                    "CurrentQtrEndDate = {0} - NextQtrEndDate = {1}",
                    currentQtrEndDate.ToShortDateString(),
                    nextQtrEndDate.ToShortDateString()));

            //... And now we can finally figure out what on what date these points will expire. The
            //AwardPoints rule knows how to exprie points using several different methods. They are
            // EndOfQtr, EndOfNextQtr, EndOfMonth, EndOfNextMonth,EndOfYear,Days. This chunk of code
            // will use the setting defined in _pointExpriationMethod to figure out the correct date.
            DateTime pointsExpirationDate = DateTimeUtil.MinValue;
            switch (_pointExpirationMethod.ToString())
            {
                case "EndOfQtr":
                    pointsExpirationDate = currentQtrEndDate;
                    break;
                case "EndOfNextQtr":
                    pointsExpirationDate = nextQtrEndDate;
                    break;
                case "EndOfMonth":
                    int nDays = DateTime.DaysInMonth(pointsExpStartDate.Year, pointsExpStartDate.Month);
                    pointsExpirationDate = new DateTime(pointsExpStartDate.Year, pointsExpStartDate.Month, nDays);
                    break;
                case "EndOfNextMonth":
                    if (pointsExpStartDate.Month == 12)
                    {
                        pointsExpirationDate = new DateTime(pointsExpStartDate.Year + 1, 1, 31);
                    }
                    else
                    {
                        int daysInMonth = DateTime.DaysInMonth(pointsExpStartDate.Year, pointsExpStartDate.Month + 1);
                        pointsExpirationDate = new DateTime(pointsExpStartDate.Year, pointsExpStartDate.Month + 1, daysInMonth);
                    }
                    break;
                case "EndOfYear":
                    pointsExpirationDate = new DateTime(pointsExpStartDate.Year, 12, 31);
                    break;
                case "Days":
                    pointsExpirationDate = pointsExpStartDate.AddDays(this._expirationDays);
                    break;
                case "Default":
                    pointsExpirationDate = DateTime.Now.AddYears(100);
                    break;
                default:
                    pointsExpirationDate = DateTime.Now.AddYears(100);
                    break;
            }

            _logger.Debug(_className, methodName, string.Format("PointsExpirationDate = {0}", pointsExpirationDate.ToShortDateString()));

            // We must have a mechanism for locating the virtual card when the rule executes
            // at runtime. The rule defines three ways in which it knows how to locate a virtual
            // card. UseExpression, FirstCardInList, and PrimaryCard. The last two are pretty simple,
            // When VirtualCardLocationLogic is set to FirstCardInList then the rule will use the
            // the virtual card at index 0 in the collection. When set to PrimaryCard, the rule
            // will iterate the list and use the first card that it encounters with it's IsPrimary
            // flag set to true. The UseExpression setting is a little more complex. When using this
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

            //if (lwvirtualCard == null) // LW-759
            //{
            _logger.Debug(_className, methodName, string.Format("AwardPoints is being invoked on a member with ipcode = {0}.  Looking for virtual card to use.", lwmember.IpCode));
            if (this._virtualCardLocationLogic == VirtualCardLocation.UseExpression)
            {
                string cardid = exprF.Create(this._virtualCardNumber).evaluate(context).ToString();
                lwvirtualCard = lwmember.GetLoyaltyCard(cardid);
            }
            if (this._virtualCardLocationLogic == VirtualCardLocation.FirstCardInList && lwmember.LoyaltyCards.Count > 0)
            {
                lwvirtualCard = lwmember.GetFirstCard();
            }
            if (this._virtualCardLocationLogic == VirtualCardLocation.PrimaryCard)
            {
                lwvirtualCard = lwmember.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
            }
            //}
            //else
            //{
            //    _logger.Debug(
            //        _className, 
            //        methodName, 
            //        string.Format(
            //            "Award point is being invoked on a virtual card witk key = {0} and Loyalty id = {1}.  Member Ipcode = {2}.",
            //            lwvirtualCard.VcKey, 
            //            lwvirtualCard.LoyaltyIdNumber, 
            //            lwmember.IpCode));
            //}

            // If the process gets this far and the virtual card is still null we have a problem.
            if (lwvirtualCard == null)
            {
                string msg = "AwardPoints was unable to locate a valid virtual card!";
                _logger.Error(_className, methodName, msg);
                throw new LWException(msg);
            }
            else
            {
                _logger.Debug(_className, methodName, string.Format("Selected virtual card with VCKey {0} for use.", lwvirtualCard.VcKey));
            }

            if (lwvirtualCard.Status != VirtualCardStatusType.Active)
            {
                string err = string.Format("Virtual card with loyalty id {0} does not have valid status.", lwvirtualCard.LoyaltyIdNumber);
                _logger.Error(_className, methodName, err);
                throw new LWRulesException(err);
            }

            //_logger.Debug(_className, methodName,
            //                        string.Format("Virtual card to use = {0}",
            //                        lwvirtualCard.VcKey));

            // This is where the rule actually starts the process of crediting the account with
            // points. The actual point value itself is a bScript expression that gets evaluated
            // at runtime to calculate the points. Exactly how it gets evaluated is another matter.
            // The AwardPoints rule supports two different methods of awarding points. A rule runs
            // within the context of an invoking row of data. By default a rule operates within
            // the context of that row of data only. However, the AwardPoints rule supports the
            // ability to iterate over child rows of data for the purpose of calculating and awarding
            // points. It uses the point award method property to control this behavior. If set to
            // UseChild then the rule will attempt to obtain the child rows of data for the attribute
            // set name specified by the ChildAttributeSetName property. If set to use a child
            // attribute set for the purposes of awarding points then the rule must know how to
            // process those transactions into the point bank. The PointBatchingMode property is used
            // to define this. If the batch mode is PerRecord then an entry will be made in the point
            // bank system for each child row encountered. If the mode is Batched then the sum of points
            // as calculated for each child row will be credited.

            AwardPointsRuleResult result = null;
            decimal points = 0;
            decimal totalPoints = 0;
            string promoCode = context.Environment.ContainsKey("PromotionCode") ? (string)context.Environment["PromotionCode"] : string.Empty;
            if (!string.IsNullOrEmpty(this._accrualFactor))
            {
                _logger.Debug(_className, methodName, "Accrual factor expression: " + this._accrualFactor);
                if (this.PointAwardMethod == PointAwardMethod.UseChild)
                {
                    _logger.Debug(
                        _className,
                        methodName,
                        "Using child award method");

                    if (context.InvokingRow != null)
                    {
                        IList<IClientDataObject> childAsetList = context.InvokingRow.GetChildAttributeSets(ChildAttributeSetName);
                        foreach (IClientDataObject aset in childAsetList)
                        {
                            ContextObject co = new ContextObject();
                            co.Owner = context.Owner;
                            co.InvokingRow = aset;
                            object retVal = null;
                            try
                            {
                                retVal = exprF.Create(this._accrualFactor).evaluate(co);
                            }
                            catch (Exception ex)
                            {
                                string msg = string.Format("Error getting accrual factor from expression '{0}'", this._accrualFactor);
                                _logger.Error(_className, methodName, msg, ex);
                                if (ex.InnerException != null)
                                {
                                    msg = string.Format("Inner exception for accrual factor error: {0}", ex.InnerException.Message);
                                    _logger.Error(_className, methodName, msg, ex.InnerException);
                                }
                                throw new LWException(msg, ex);
                            }
                            points = decimal.Parse(retVal.ToString());
                            if (this.PointBatchMode == PointBatchingMode.PerRecord)
                            {
                                result = CreatePointTransaction(context.Name, context.Mode, lwvirtualCard, points, promoCode, pointsTransactionDate, pointsExpirationDate, PointTransactionOwnerType.AttributeSet, aset.GetMetaData().ID, aset.RowKey, notes);
                            }
                            else
                            {
                                totalPoints += points;
                            }
                        }
                    }
                    else
                    {
                        string msg = string.Format("Point award method used is UseChild but there is no invoking row");
                        _logger.Error(_className, methodName, msg);
                        throw new CRMException(msg);
                    }
                    if (this.PointBatchMode == PointBatchingMode.Batched)
                    {
                        result = CreatePointTransaction(context.Name, context.Mode, lwvirtualCard, totalPoints, promoCode, pointsTransactionDate, pointsExpirationDate, ownerType, ownerId, rowKey, notes);
                    }
                    else
                    {
                        _logger.Debug(_className, methodName, "Award point method was batched but no points could be calculated.");
                    }
                }
                else
                {
                    object retVal = null;
                    try
                    {
                        retVal = exprF.Create(this._accrualFactor).evaluate(context);
                        if (lwvirtualCard == null)
                        {
                            _logger.Error(_className, methodName, "How did the virtual card get set to null - after accrual factor evaluation?");
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = string.Format("Error getting accrual factor from expression '{0}'", this._accrualFactor);
                        _logger.Error(_className, methodName, msg, ex);
                        if (ex.InnerException != null)
                        {
                            msg = string.Format("Inner exception for accrual factor error: {0}", ex.InnerException.Message);
                            _logger.Error(_className, methodName, msg, ex.InnerException);
                        }
                        throw new LWException(msg, ex) { ErrorCode = 9972 };
                    }
                    if (retVal == null)
                    {
                        string msg = string.Format("Error getting accrual factor from expression '{0}'.  Possibly no value found.", this._accrualFactor);
                        _logger.Error(_className, methodName, msg);
                        throw new LWException(msg) { ErrorCode = 9972 };
                    }
                    points = decimal.Parse(retVal.ToString());
                    result = CreatePointTransaction(context.Name, context.Mode, lwvirtualCard, points, promoCode, pointsTransactionDate, pointsExpirationDate, ownerType, ownerId, rowKey, notes);
                }
            }
            else
            {
                _logger.Error(_className, methodName, "Invalid point accrual expression in " + this.RuleName + ".");
                throw new System.Exception("AwardPoints: invalid point accrual expression in " + RuleName + ".");
            }

            if (!string.IsNullOrEmpty(TransientPropertyName) && !string.IsNullOrEmpty(TransientPropertyExpression))
            {
                object v = null;
                try
                {
                    _logger.Debug(_className, methodName, "Evaluating transient expression: " + TransientPropertyExpression);
                    v = exprF.Create(TransientPropertyExpression).evaluate(context);
                    if (context.InvokingRow != null)
                    {
                        _logger.Debug(
                            _className,
                            methodName,
                            string.Format("Setting transient property {0} to {1} on the invoking row.", TransientPropertyName, v.ToString()));

                        context.InvokingRow.UpdateTransientProperty(TransientPropertyName, v);
                    }
                    else
                    {
                        _logger.Debug(
                            _className,
                            methodName,
                            string.Format("Setting transient property {0} to {1} on the member.", TransientPropertyName, v.ToString()));

                        lwmember.UpdateTransientProperty(TransientPropertyName, v);
                    }
                }
                catch (Exception e)
                {
                    string errMsg = "Error setting transient property " + TransientPropertyName + ".";
                    _logger.Error(_className, methodName, errMsg, e);
                    throw new LWRulesException(errMsg);
                }
            }

            using (var loyalty = _dataUtil.LoyaltyDataServiceInstance())
            {
                // We must have a way of handling client requirements with regard to allowing a members
                // point balance to fall below zero. The AllowNegativePointBalance property is used for
                // that purpose. If set to False, then the rule will attempt to offset the negative
                // balance by processing an additional transaction on the members account equal to the
                // absolute value of the negative amount, thereby bringing the balance back to zero. The
                // rule also provides a property for specifying the point type for this special adjustment
                // transaction. Performance NOTE: Keep in mind that rules run at the row level. Using this
                // property to control negative balances may not be the best approach for systems processing
                // large amounts of data.
                if (this._allowNegativePointBalance == false)
                {
                    long[] vckeys = new long[] { lwvirtualCard.VcKey };
                    //PointBalance += service.GetPointBalance(vckeys, ptIds, peIds, null, StartDate, EndDate, null, null, null, null, null, null, null);
                    decimal currentBalance = loyalty.GetPointBalance(vckeys, null, null, null, null, null, null, null, null, null, null, null, null);
                    if (currentBalance < 0)
                    {
                        _logger.Debug(
                            _className,
                            methodName,
                            string.Format(
                                "Current balance for virtual card = {0} is {1}.",
                                lwvirtualCard.VcKey,
                                currentBalance));

                        loyalty.Credit(lwvirtualCard, GetNegativeAdjustmentPointType(), GetPointEventType(), Math.Abs(currentBalance), string.Empty, pointsTransactionDate, pointsExpirationDate, null, null);
                        result.NegativeBalanceOffset = Math.Abs(currentBalance);
                    }
                }

                // add the result of this transaction
                AddRuleResult(context, result);

                // Now all the processing associated with Award Point rule has finished.  next step is to
                // invoke the rule for tier evaluation, i fone is defined.            
                if (!string.IsNullOrEmpty(EvaluateTierRule))
                {
                    _logger.Debug(_className, methodName, "Executing rule to evaluate tier.");
                    try
                    {
                        RuleTrigger rt = loyalty.GetRuleByName(EvaluateTierRule);
                        if (rt != null)
                        {
                            loyalty.Execute(rt, context);
                        }
                    }
                    catch
                    {
                        _logger.Error(_className, methodName, string.Format("Error invoking rule {0} for tier evaluation.", EvaluateTierRule));
                        throw;
                    }
                }
                else
                {
                    _logger.Warning(_className, methodName, string.Format("No rule specified for tier evaluation."));
                }

                _logger.Debug(_className, methodName, "Finished invoking award point rule for member " + lwmember.IpCode);
                return;
            }
        }

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
            if (!string.IsNullOrWhiteSpace(this.PointBankTransactionDate) && ExpressionUtil.IsLibraryExpression(this.PointBankTransactionDate))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.PointBankTransactionDate));
            }
            if (!string.IsNullOrWhiteSpace(this.AccrualExpression) && ExpressionUtil.IsLibraryExpression(this.AccrualExpression))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.AccrualExpression));
            }
            if (!string.IsNullOrWhiteSpace(this.DescriptionExpression) && ExpressionUtil.IsLibraryExpression(this.DescriptionExpression))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.DescriptionExpression));
            }
            if (!string.IsNullOrWhiteSpace(this.TransientPropertyExpression) && ExpressionUtil.IsLibraryExpression(this.TransientPropertyExpression))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.TransientPropertyExpression));
            }
            if (!string.IsNullOrWhiteSpace(this.LocationIdExpression) && ExpressionUtil.IsLibraryExpression(this.LocationIdExpression))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.LocationIdExpression));
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

            _logger.Trace(_className, methodName, "Migrating AwardPoints rule.");

            AEAwardPoints src = (AEAwardPoints)source;
            //PromotionCode = src.PromotionCode;
            PointAwardMethod = src.PointAwardMethod;
            ChildAttributeSetName = src.ChildAttributeSetName;
            PointBatchMode = src.PointBatchMode;
            VirtualCardLocationLogic = src.VirtualCardLocationLogic;
            VirtualCardNumber = src.VirtualCardNumber;
            ExpireDate = src.ExpireDate;
            PointsExpirationMethod = src.PointsExpirationMethod;
            ExpirationDays = src.ExpirationDays;
            PointBankTransactionDate = src.PointBankTransactionDate;
            PointEventType = src.PointEventType;
            PointType = src.PointType;
            AccrualExpression = src.AccrualExpression;
            AllowZeroPoints = src.AllowZeroPoints;
            DescriptionExpression = src.DescriptionExpression;
            AllowNegativePointBalance = src.AllowNegativePointBalance;
            NegativeAdjustmentPointType = src.NegativeAdjustmentPointType;
            TransientPropertyName = src.TransientPropertyName;
            TransientPropertyExpression = src.TransientPropertyExpression;
            LocationIdExpression = src.LocationIdExpression;
            ChangedByExpression = src.ChangedByExpression;
            EvaluateTierRule = src.EvaluateTierRule;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;
            return this;
        }


        private PointType GetPointType()
        {
            if (_pointType != null)
            {
                return _pointType;
            }
            else if (!string.IsNullOrEmpty(pointTypeName))
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    _pointType = service.GetPointType(pointTypeName);
                    return _pointType;
                }
            }
            else
            {
                return null;
            }
        }

        private PointEvent GetPointEventType()
        {
            if (_pointEventType != null)
            {
                return _pointEventType;
            }
            else if (!string.IsNullOrEmpty(pointEventTypeName))
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    _pointEventType = service.GetPointEvent(pointEventTypeName);
                    return _pointEventType;
                }
            }
            else
            {
                return null;
            }
        }

        private PointType GetNegativeAdjustmentPointType()
        {
            if (_negativeAdjPointType != null)
            {
                return _negativeAdjPointType;
            }
            else if (!string.IsNullOrEmpty(negativeAdjPointTypeName))
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    _negativeAdjPointType = service.GetPointType(negativeAdjPointTypeName);
                    return _negativeAdjPointType;
                }
            }
            else
            {
                return null;
            }
        }

        private string GetAEPtsOnHoldDayCount()
        {
            const string cacheKey = "PtsOnHoldDayCount";
            string DayCount = String.Empty;
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                DayCount = (string)service.CacheManager.Get(_cacheRegion, cacheKey) == null ? String.Empty : (string)service.CacheManager.Get(_cacheRegion, cacheKey);
                if (DayCount == string.Empty)
                {
                    using (var ldService = _dataUtil.DataServiceInstance())
                    {
                        ClientConfiguration clientConfiguration = ldService.GetClientConfiguration("AEPtsonholdDaycount");
                        if (clientConfiguration == null)
                        {
                            _logger.Error(_className, "GetAEPtsOnHoldDayCount", " Variable not found: AEPtsonholdDaycount");
                            throw new Exception("Variable not found AEPtsonholdDaycount");
                        }
                        DayCount = clientConfiguration.Value;
                        service.CacheManager.Update(_cacheRegion, cacheKey, DayCount);
                    }
                }
                return DayCount;
            }
        }


        private AwardPointsRuleResult CreatePointTransaction(string name, RuleExecutionMode mode, VirtualCard lwvirtualCard, decimal points, string promoCode, DateTime txnDate, DateTime expirationDate, PointTransactionOwnerType ownerType, long ownerId, long rowKey, string notes)
        {
            string methodName = "CreatePointTransaction";

            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {

                AwardPointsRuleResult result = new AwardPointsRuleResult()
                {
                    Name = !string.IsNullOrWhiteSpace(name) ? name : this.RuleName,
                    Mode = mode,
                    RuleType = this.GetType(),
                    PromotionCode = promoCode,
                    PointsAwarded = points,
                    PointType = GetPointType().Name,
                    PointEvent = GetPointEventType().Name,
                    RowKey = rowKey,
                    OwnerType = ownerType,
                    OwnerId = ownerId
                };

                IList<PointTransaction> txns = null;
                if (lwvirtualCard != null)
                {
                    result.MemberId = lwvirtualCard.IpCode;

                    if (string.IsNullOrEmpty(notes) && !string.IsNullOrEmpty(promoCode))
                    {
                        notes = string.Format("Points being awarded for Promotion {0}.", promoCode);
                    }
                    if (points > 0)
                    {
                        _logger.Debug(
                            _className,
                            methodName,
                            string.Format("Creating a Credit transaction for {0} points.  Virtual card key being used is {1}.", points, lwvirtualCard.VcKey));

                        if (mode == RuleExecutionMode.Real)
                        {
                            PointType pt = GetPointType();
                            PointEvent ev = GetPointEventType();
                            txns = service.Credit(lwvirtualCard, GetPointType(), GetPointEventType(), points, promoCode, txnDate, expirationDate, ownerType, ownerId, rowKey, notes, null, null);
                            result.Detail = string.Format("{0} {1} points awarded for {2}.", points, pt != null ? pt.Name : "Unknown", ev != null ? ev.Name : "Unknown");
                        }
                    }
                    else if (points < 0)
                    {
                        points = Math.Abs(points); // debit will make the points negative.
                        _logger.Debug(
                            _className,
                            methodName,
                            string.Format("Creating a Debit transaction for {0} points.  Virtual card key being used is {1}.", points, lwvirtualCard.VcKey));

                        if (mode == RuleExecutionMode.Real)
                        {
                            PointType pt = GetPointType();
                            PointEvent ev = GetPointEventType();
                            txns = service.Debit(lwvirtualCard, GetPointType(), GetPointEventType(), points, txnDate, expirationDate, ownerType, ownerId, rowKey, notes, null, null);
                            result.Detail = string.Format("{0} {1} points awarded for {2}.", points, pt != null ? pt.Name : "Unknown", ev != null ? ev.Name : "Unknown");
                        }
                    }
                    else
                    {
                        if (_allowZeroPoints)
                        {
                            if (mode == RuleExecutionMode.Real)
                            {
                                PointType pt = GetPointType();
                                PointEvent ev = GetPointEventType();
                                txns = service.Credit(lwvirtualCard, GetPointType(), GetPointEventType(), points, string.Empty, txnDate, expirationDate, ownerType, ownerId, rowKey, notes, null, null);
                                result.Detail = string.Format("{0} {1} points awarded for {2}.", points, pt != null ? pt.Name : "Unknown", ev != null ? ev.Name : "Unknown");
                            }
                        }
                        else
                        {
                            _logger.Debug(_className, methodName, "Accrual factor resulted in 0 points to be awarded.  Skipping.");
                        }
                    }

                    var ptsDayCount = GetAEPtsOnHoldDayCount();
                    AEPtsonholdDaycount = ptsDayCount != String.Empty ? Convert.ToInt32(ptsDayCount) : 0;

                    if (AEPtsonholdDaycount > 0 && mode == RuleExecutionMode.Real)
                    {
                        _logger.Debug(
                            _className,
                            methodName,
                            string.Format("Putting {0} points on hold.  Virtual card key being used is {1}.", points, lwvirtualCard.VcKey));

                        // now put the unconsumed transaction points on hold.
                        foreach (PointTransaction txn in txns)
                        {
                            if (txn.TransactionDate.Date >= DateTime.Now.AddDays(-AEPtsonholdDaycount).Date)
                            {
                                if (txn.PointsConsumed < txn.Points)
                                {
                                    txn.PointsOnHold = txn.Points - txn.PointsConsumed;
                                    service.UpdatePointTransaction(txn);
                                    PointTransaction htxn = new PointTransaction();
                                    htxn.VcKey = txn.VcKey;
                                    htxn.PointTypeId = txn.PointTypeId;
                                    htxn.PointEventId = txn.PointEventId;
                                    htxn.TransactionType = PointBankTransactionType.Hold;
                                    htxn.TransactionDate = txn.TransactionDate;
                                    htxn.PointAwardDate = txn.PointAwardDate;
                                    htxn.Points = txn.Points;
                                    htxn.ExpirationDate = txn.ExpirationDate;
                                    htxn.Notes = txn.Notes;
                                    htxn.OwnerType = txn.OwnerType;
                                    htxn.OwnerId = txn.OwnerId;
                                    htxn.RowKey = txn.RowKey;
                                    htxn.ParentTransactionId = txn.Id;
                                    if (mode == RuleExecutionMode.Real)
                                    {
                                        service.CreatePointTransaction(htxn);
                                        result.Detail = string.Format("{0} points put on hold.", points);
                                        //result.Detail = string.Format("{0} points of currency {1} and for event {2} put on hold.", points, pt != null ? pt.Name : "Unknown", ev != null ? ev.Name : "Unknown");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Debug(_className, methodName, "Rule Execution was a simulation and creation point transacion will be skip.");
                    }
                }
                else
                {
                    _logger.Error(_className, methodName, "No virtual card available.");
                    throw new LWException("No virtual card available.");
                }

                return result;
            }
        }
    }
}
