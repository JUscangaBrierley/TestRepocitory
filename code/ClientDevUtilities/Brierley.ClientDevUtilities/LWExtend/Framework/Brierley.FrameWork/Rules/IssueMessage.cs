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
    public class IssueMessage : RuleBase
    {
        public class IssueMessageRuleResult : ContextObject.RuleResult
        {
            public long MemberMessageId { get; set; }
            public long MessageId { get; set; }
        }

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private const string _className = "IssueMessage";

        public IssueMessage()
            : base("IssueMessage")
        {
        }

        public override string DisplayText
        {
            get { return "Issue Message"; }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Message Name")]
        [Description("Used to select the type of message to be issued by this rule.")]
        [RuleProperty(false, true, false, "MessageDefs")]
        [RulePropertyOrder(1)]
        public string MessageDefType { get; set; }

        [Browsable(false)]
        public Dictionary<string, string> MessageDefs
        {
            get
            {
                using (var content = LWDataServiceUtil.ContentServiceInstance())
                {
                    Dictionary<string, string> messageDefs = new Dictionary<string, string>();
                    IList<MessageDef> messsages = content.GetAllMessageDefs(null);
                    messageDefs.Add("-- Select --", string.Empty);
                    foreach (MessageDef msg in messsages)
                    {
                        messageDefs.Add(msg.Name, msg.Name);
                    }
                    return messageDefs;
                }
            }
        }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Start Date")]
        [Description("Defines the expression used to calculate the start date of these messages.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(2)]
        public string StartDateExpression { get; set; }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Expiration Date")]
        [Description("Defines the expression used to calculate the expiration date of these messages.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(3)]
        public string ExpiryDateExpression { get; set; }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Allow Multiple Messages")]
        [Description("Issue message even if the member already has it.")]
        [RulePropertyOrder(13)]
        [RuleProperty(false, false, false, null, false, true)]
        public bool AllowDuplicate { get; set; }

        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [Category("General")]
        [DisplayName("Make Transient")]
        [Description("When checked, the message will not be permanently assigned to the member. Use this option to assign single-use messages that will not appear in the member's inbox.")]
        [RulePropertyOrder(14)]
        [RuleProperty(false, false, false, null, false, true)]
        public bool Transient { get; set; }

        public override void Invoke(ContextObject Context)
        {
            string methodName = "Invoke";

            Member lwmember = null;
            VirtualCard lwvirtualCard = null;

            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);
            if (lwmember == null)
            {
                string errMsg = "No member could be resolved for issue message rule.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3214 };
            }
            else
            {
                _logger.Trace(_className, methodName, "Invoking issue message rule for member with Ipcode = " + lwmember.IpCode + ".");
            }

            using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (var content = LWDataServiceUtil.ContentServiceInstance())
            {
                IssueMessageRuleResult result = new IssueMessageRuleResult()
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
                string messageName = MessageDefType;

                if (env != null && env.ContainsKey("MessageName"))
                {
                    messageName = (string)env["MessageName"];
                }
                if (string.IsNullOrEmpty(messageName))
                {
                    string errMsg = "No message specified to issue.";
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3220 };
                }
                MessageDef message = content.GetMessageDef(messageName);
                if (message == null)
                {
                    string errMsg = string.Format("No message exists with name {0}.", messageName);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWRulesException(errMsg) { ErrorCode = 3221 };
                }
                result.Name = message.Name;
                result.OwnerId = message.Id;

                /*
				 Calculate the start date.  Precedence is:
                    Environment context: StartDate
                    Start date bScript expression
                    definition's start date
				 */
                DateTime startDate = message.StartDate;
                if (env.ContainsKey("StartDate"))
                {
                    startDate = (DateTime)env["StartDate"];
                }
                else
                {
                    try
                    {
                        startDate = EvaluateDateExpression(StartDateExpression, message.StartDate, Context);
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
                DateTime expiryDate = message.ExpiryDate != null ? (DateTime)message.ExpiryDate : DateTimeUtil.MaxValue;
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

                bool issueMessage = false;
                if (!AllowDuplicate)
                {
                    IList<MemberMessage> messages = service.GetMemberMessagesByMemberAndType(lwmember.IpCode, message.Id);
                    if (messages == null || messages.Count == 0)
                    {
                        issueMessage = true;
                    }
                    else
                    {
                        result.Detail = string.Format("Message {0} not issued because the member already has this message.", message.Name);
                    }
                }
                else
                {
                    issueMessage = true;
                }

                if (issueMessage)
                {
                    MemberMessage mc = new MemberMessage()
                    {
                        MemberId = lwmember.IpCode,
                        MessageDefId = message.Id,
                        DateIssued = DateTime.Now,
                        StartDate = startDate,
                        ExpiryDate = expiryDate,
                        DisplayOrder = message.DisplayOrder
                    };
                    if (Context.Mode == RuleExecutionMode.Real && !Transient)
                    {
                        service.CreateMemberMessage(mc);
                        result.MemberMessageId = mc.Id;
                        result.RowKey = mc.Id;
                    }
                    result.MessageId = message.Id;
                    result.Detail = string.Format("Issued message {0} with id {1}.", message.Name, result.RowKey);

                    //Send Push notification if one is attached to the message definition
                    if (message.PushNotificationId != null)
                    {
                        using (var push = LWDataServiceUtil.PushServiceInstance())
                        {
                            Task.Run(() => push.SendAsync((long)message.PushNotificationId, lwmember));
                        }
                    }
                }

                AddRuleResult(Context, result);
            }
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

            _logger.Trace(_className, methodName, "Migrating IssueMessage rule.");

            IssueMessage src = (IssueMessage)source;

            AllowDuplicate = src.AllowDuplicate;
            MessageDefType = src.MessageDefType;
            StartDateExpression = src.StartDateExpression;
            ExpiryDateExpression = src.ExpiryDateExpression;
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
