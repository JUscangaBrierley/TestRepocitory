using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Rules.UIDesign;

using Brierley.FrameWork.Push;
using Brierley.FrameWork.Common.Config;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class IssueNotification : RuleBase
    {
        #region Results
        public class IssueNotificationRuleResult : ContextObject.RuleResult
        {
            public List<string> ids = new List<string>();
            public List<long> mobileDeviceIds = new List<long>();
            public List<long> sessionIds = new List<long>();
        }
        #endregion

        #region Private Variables

        [NonSerialized]
        private string _className = "IssueNotification";

        [NonSerialized]
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private string notificationDefName = string.Empty;
        private string expirationDateLogic = string.Empty;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public IssueNotification()
            : base("IssueNotification")
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
            get { return "Send Push Notification"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Notification Name")]
        [Description("Used to select the type of notification to be issued by this rule.")]
        [RuleProperty(false, true, false, "NotificationDefs")]
        [RulePropertyOrder(1)]
        public string NotificationDefType
        {
            get
            {
                return notificationDefName;
            }
            set
            {
                notificationDefName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, string> NotificationDefs
        {
            get
            {
                using (var content = LWDataServiceUtil.ContentServiceInstance())
                {
                    Dictionary<string, string> notificationDefs = new Dictionary<string, string>();
                    IList<NotificationDef> notifications = content.GetAllNotificationDefs(null);
                    notificationDefs.Add("-- Select --", string.Empty);
                    foreach (NotificationDef msg in notifications)
                    {
                        notificationDefs.Add(msg.Name, msg.Name);
                    }
                    return notificationDefs;
                }
            }
        }
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
            bool hasContextResults = false;
            List<string> contextResultIds = new List<string>();

            if (Context.Results.Count > 0)
            {
                foreach (ContextObject.RuleResult ruleResult in Context.Results)
                {
                    if (ruleResult is IssueRewardRuleResult)
                    {
                        contextResultIds.Add(((IssueRewardRuleResult)ruleResult).RewardId.ToString());
                    }
                    else if (ruleResult is IssueCoupon.IssueCouponRuleResult)
                    {
                        contextResultIds.Add(((IssueCoupon.IssueCouponRuleResult)ruleResult).Id.ToString());
                    }
                    else if (ruleResult is IssueMessage.IssueMessageRuleResult)
                    {
                        contextResultIds.Add(((IssueMessage.IssueMessageRuleResult)ruleResult).MemberMessageId.ToString());
                    }
                    else if (ruleResult is CampaignResult)
                    {
                        contextResultIds.Add(((CampaignResult)ruleResult).MemberReferenceId.ToString());
                    }
                }

                hasContextResults = contextResultIds.Count > 0 ? true : false;
            }

            Member lwmember = null;
            VirtualCard lwvirtualCard = null;

            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);
            if (lwmember == null)
            {
                string errMsg = "No member could be resolved for issue notification rule.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3214 };
            }
            else
            {
                _logger.Trace(_className, methodName, "Invoking issue notification rule for member with Ipcode = " + lwmember.IpCode + ".");
            }

            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (var content = LWDataServiceUtil.ContentServiceInstance())
            {
                IssueNotificationRuleResult result = new IssueNotificationRuleResult()
                {
                    Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
                    Mode = Context.Mode,
                    RuleType = this.GetType(),
                    MemberId = lwmember.IpCode,
                    OwnerType = PointTransactionOwnerType.Message
                };

                var env = Context.Environment;

                /*
                 * If the environment specifies a message name, then use that.  Otherwise use the message
                 * name defined in the rule configuration.  If none, then consider it an error.
                 * */
                string notificationName = notificationDefName;

                if (env != null && env.ContainsKey("NotificationName"))
                {
                    notificationName = (string)env["NotificationName"];
                }
                if (string.IsNullOrEmpty(notificationName))
                {
                    string errMsg = "No notification specified to issue.";
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3220 };
                }
                NotificationDef notification = content.GetNotificationDef(notificationName);
                if (notification == null)
                {
                    string errMsg = string.Format("No notification exists with name {0}.", notificationName);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3221 };
                }
                result.Name = notification.Name;
                result.OwnerId = notification.Id;

                if (!hasContextResults)
                {
                    contextResultIds.Add(string.Empty);
                }
                foreach (string id in contextResultIds)
                {
                    using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
                    {
                        List<MobileDevice> mobileDevices = loyaltyService.GetMobileDevices(lwmember, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = 1000 });
                        if (mobileDevices.Count <= 0)
                            _logger.Error(_className, methodName, string.Format("Member with ipcode {0} does not have any associated devices.", lwmember.IpCode));

                        foreach (MobileDevice mobileDevice in mobileDevices)
                        {
                            // Does Device accept push
                            if (!mobileDevice.AcceptsPush)
                            {
                                _logger.Trace(_className, methodName, string.Format("Member: {0}, MobileDevice: {1}, does not accept push notifications.", lwmember.IpCode, mobileDevice.Id));
                                continue;
                            }

                            //Does Device have an active session
                            PushSession activeSession = loyaltyService.GetActivePushSessions(mobileDevice.Id);
                            if (activeSession == null)
                            {
                                _logger.Error(_className, methodName, string.Format("Member: {0}, MobileDevice: {1}, does not have an active session.", lwmember.IpCode, mobileDevice.Id));
                                continue;
                            }

                            try
                            {
                                using (var push = LWDataServiceUtil.PushServiceInstance())
                                {
                                    push.Send(lwmember, notification.Id, mobileDevice.Id, id);
                                    if (!result.ids.Contains(id))
                                        result.ids.Add(id);
                                    result.mobileDeviceIds.Add(mobileDevice.Id);
                                    result.sessionIds.Add(activeSession.Id);
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = string.Format(
                                    "Error invoking Push Service rule using notification {0} for member with ipcode {1}",
                                    notification.Name,
                                    lwmember.MyKey);
                                _logger.Error(_className, methodName, msg, ex);
                            }
                        }

                    }
                    AddRuleResult(Context, result);
                }
            }
        }

        #region Migrartion Helpers
        public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating IssueNotification rule.");

            IssueNotification src = (IssueNotification)source;

            NotificationDefType = src.NotificationDefType;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
        #endregion
    }
}
