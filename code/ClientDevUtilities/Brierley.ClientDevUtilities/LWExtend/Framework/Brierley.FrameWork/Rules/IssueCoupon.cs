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
    public class IssueCoupon : RuleBase
    {
        public class IssueCouponRuleResult : ContextObject.RuleResult
        {
            public long Id = -1;
            public string CertNmbr;
        }

        private const string _className = "IssueCoupon";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private CertificateUtil _util = new CertificateUtil();

        public IssueCoupon()
            : base("IssueCoupon")
        {
        }

        public override string DisplayText
        {
            get { return "Issue Coupon"; }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Offer Code")]
        [Description("This member provides the offer code to be used for the issued member.")]
        [RulePropertyOrder(1)]
        public string OfferCode
        {
            get { return _util.OfferCode; }
            set { _util.OfferCode = value; }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Coupon Name")]
        [Description("Used to select the type of coupon to be issued by this rule.")]
        [RuleProperty(false, true, false, "CouponTypes")]
        [RulePropertyOrder(2)]
        public string CouponType { get; set; }

        [Browsable(false)]
        public Dictionary<string, string> CouponTypes
        {
            get
            {
                using (var service = LWDataServiceUtil.ContentServiceInstance())
                {
                    Dictionary<string, string> couponTypes = new Dictionary<string, string>();
                    couponTypes.Add("-- Select --", string.Empty);
                    var coupons = service.GetUnexpiredCouponDefs();
                    foreach (CouponDef coupon in coupons)
                    {
                        couponTypes.Add(coupon.Name, coupon.Name);
                    }
                    return couponTypes;
                }
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Start Date")]
        [Description("Defines the expression used to calculate the start date of these coupons.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(3)]
        public string StartDateExpression { get; set; }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Expiration Date")]
        [Description("Defines the expression used to calculate the expiration date of these coupons.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(4)]
        public string ExpiryDateExpression { get; set; }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Assign LoyaltyWare Certificate")]
        [Description("Assign certificate from LoyaltyWare's certificate bank.")]
        [RuleProperty(false, false, false, null, false, false)]
        [RulePropertyOrder(5)]
        public bool AssignLWCertificate
        {
            get { return _util.AssignLWCertificate; }
            set { _util.AssignLWCertificate = value; }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Low Certificates Email")]
        [Description("Used to select the triggered email that will be sent out when the number of certificates reach the threshold value.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails", false, true)]
        [RulePropertyOrder(10)]
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

        /// <summary>
        /// Returns list of all available triggered emails
        /// </summary>
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
        [Category("General")]
        [DisplayName("Low Certificates Email Recipient")]
        [Description("This is the email of the recipient for low threshold email.")]
        [RulePropertyOrder(11)]
        [RuleProperty(false, false, false, null, false, true)]
        public string LowCertificatesEmailRecepient
        {
            get { return _util.LowThresholdEmailRecepient; }
            set { _util.LowThresholdEmailRecepient = value; }
        }

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

            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);
            if (lwmember == null)
            {
                string errMsg = "No member could be resolved for issue coupon rule.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3214 };
            }
            else
            {
                _logger.Trace(_className, methodName, "Invoking issue coupon rule for member with Ipcode = " + lwmember.IpCode + ".");
            }

            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (var content = LWDataServiceUtil.ContentServiceInstance())
            {

                IssueCouponRuleResult result = new IssueCouponRuleResult()
                {
                    Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
                    Mode = Context.Mode,
                    RuleType = this.GetType(),
                    MemberId = lwmember.IpCode,
                    OwnerType = PointTransactionOwnerType.Coupon
                };

                var env = Context.Environment;

                /*
				 * If the environment specifies a coupon name, then use that.  Otherwise use the coupon
				 * name defined in the rule configuration.  If none, then consider it an error.
				 * */
                string couponName = CouponType;
                string certNmbr = string.Empty;
                if (env != null && env.ContainsKey("CouponName"))
                {
                    couponName = (string)env["CouponName"];
                }
                if (string.IsNullOrEmpty(couponName))
                {
                    string errMsg = "No coupon specified to issue.";
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3218 };
                }
                CouponDef coupon = content.GetCouponDef(couponName);
                if (coupon == null)
                {
                    string errMsg = string.Format("No coupon exists with name {0}.", couponName);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3219 };
                }

                if (!coupon.IsActive())
                {
                    string errMsg = string.Format("Cannot issue coupon {0} because it is not active.", couponName);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3234 };
                }

                result.OwnerId = coupon.Id;


                /*
				 Calculate the start date.  Precedence is:
                    Environment context: StartDate
                    Start date bScript expression
                    definition's start date
				 */
                DateTime startDate = coupon.StartDate;
                if (env.ContainsKey("StartDate"))
                {
                    startDate = (DateTime)env["StartDate"];
                }
                else
                {
                    try
                    {
                        startDate = EvaluateDateExpression(StartDateExpression, coupon.StartDate, Context);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = string.Format("Error while calculating start date using expression {0}", StartDateExpression);
                        _logger.Error(_className, methodName, errMsg, ex);
                        throw new LWRulesException(errMsg, ex);
                    }
                }

                /*
                 Calculate the expiration date.  Precedence is:
                    Environment context: ExpirationDate
                    Expiration date bScript expression
                    definition's expiration date
                 */
                DateTime expiryDate = coupon.ExpiryDate != null ? (DateTime)coupon.ExpiryDate : DateTimeUtil.MaxValue;
                if (env.ContainsKey("ExpirationDate"))
                {
                    expiryDate = (DateTime)env["ExpirationDate"];
                }
                else
                {
                    try
                    {
                        expiryDate = EvaluateDateExpression(ExpiryDateExpression, expiryDate, Context);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = string.Format("Error while calculating expiry date using expression {0}", ExpiryDateExpression);
                        _logger.Error(_className, methodName, errMsg, ex);
                        throw new LWRulesException(errMsg, ex);
                    }
                }

                MemberCoupon mc = new MemberCoupon()
                {
                    MemberId = lwmember.IpCode,
                    CouponDefId = coupon.Id,
                    TimesUsed = 0,
                    DateIssued = DateTime.Now,
                    StartDate = startDate, 
                    ExpiryDate = expiryDate,
                    DisplayOrder = coupon.DisplayOrder
                };
                result.Name = coupon.Name;

                /*
                 * Calculate the certificate.
                 * If the incoming environment context has the certificate number then use it.  Otherwise 
                 * we try to get a certificate number. 
                 * */
                if (env.ContainsKey("CertificateNumber"))
                {
                    certNmbr = (string)env["CertificateNumber"];
                }
                if (string.IsNullOrEmpty(certNmbr))
                {
                    PromotionCertificate cert = null;
                    if (Context.Mode == RuleExecutionMode.Real)
                    {
                        cert = _util.IssueCertificate(ContentObjType.Coupon, coupon.CouponTypeCode);
                    }
                    if (!string.IsNullOrEmpty(coupon.CouponTypeCode) && Context.Mode == RuleExecutionMode.Simulation && AssignLWCertificate)
                    {
                        result.CertNmbr = "No certs issued in simulation mode.";
                    }
                    if (cert != null)
                    {
                        mc.CertificateNmbr = cert.CertNmbr;
                        cert.Available = false;
                        string msg = string.Format("Using certificate number {0} for coupon.", mc.CertificateNmbr);
                        _logger.Trace(_className, methodName, msg);
                        result.CertNmbr = cert.CertNmbr;
                        if (Context.Mode == RuleExecutionMode.Real)
                        {
                            service.CreateMemberCoupon(mc);
                        }
                    }
                    else
                    {
                        // create without a certificate.
                        if (Context.Mode == RuleExecutionMode.Real)
                        {
                            service.CreateMemberCoupon(mc);
                        }
                    }
                    result.RowKey = mc.ID;
                }
                else
                {
                    string msg = string.Format("Using certificate number {0} as provided.", certNmbr);
                    _logger.Trace(_className, methodName, msg);
                    mc.CertificateNmbr = certNmbr;
                    result.CertNmbr = certNmbr;
                    if (Context.Mode == RuleExecutionMode.Real)
                    {
                        service.CreateMemberCoupon(mc);
                    }
                }

                result.Id = mc.ID;
                if (string.IsNullOrEmpty(result.CertNmbr))
                {
                    result.Detail = string.Format("Issued coupon {0} with id {1}.", coupon.Name, result.Id);
                }
                else
                {
                    result.Detail = string.Format("Issued coupon {0} with Cert number {1}.", coupon.Name, result.CertNmbr);
                }

                //Send Push notification if one is attached to the coupon definition
                if (coupon.PushNotificationId != null)
                {
                    using (var push = LWDataServiceUtil.PushServiceInstance())
                    {
                        Task.Run(() => push.SendAsync((long)coupon.PushNotificationId, lwmember));
                    }
                }

                AddRuleResult(Context, result);
            }
            return;
        }

        public override List<string> GetBscriptsToMove()
        {
            List<string> bscriptList = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.ExpiryDateExpression) && ExpressionUtil.IsLibraryExpression(this.ExpiryDateExpression))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.ExpiryDateExpression));
            }
            return bscriptList;
        }

        public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating IssueCoupon rule.");

            IssueCoupon src = (IssueCoupon)source;

            OfferCode = src.OfferCode;
            CouponType = src.CouponType;
            StartDateExpression = src.StartDateExpression;
            ExpiryDateExpression = src.ExpiryDateExpression;
            AssignLWCertificate = src.AssignLWCertificate;
            LowThresholdEmailName = src.LowThresholdEmailName;
            LowCertificatesEmailRecepient = src.LowCertificatesEmailRecepient;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }


        private DateTime EvaluateDateExpression(string expression, DateTime defaultDate, ContextObject context = null)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                Expression e = new ExpressionFactory().Create(expression);
                object result = e.evaluate(context);
                if (result != null)
                {
                    if (result is DateTime)
                    {
                        return (DateTime)result;
                    }
                    else if (result is IConvertible)
                    {
                        return Convert.ToDateTime(result);
                    }
                    else
                    {
                        return DateTime.Parse(result.ToString());
                    }
                }
            }
            return defaultDate;
        }
    }
}
