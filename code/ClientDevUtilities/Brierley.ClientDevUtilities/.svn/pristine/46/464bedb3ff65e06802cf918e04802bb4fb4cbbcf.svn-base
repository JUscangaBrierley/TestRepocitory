using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules.UIDesign;
using Brierley.FrameWork.Sms;

namespace Brierley.FrameWork.Rules
{
	[Serializable]
	public class TriggeredSmsRule : RuleBase
	{
		private const string _className = "TriggeredSmsRule";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private string _smsName = string.Empty;
		private string _recipientPhone = string.Empty;

		public override string DisplayText
		{
			get { return "Send Sms"; }
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("The mobile phone of the recipient")]
		[Description("This is a bScript expression that provides the recipient mobile phone number.")]
		[RuleProperty(true, false, false, null, false, true)]
		[RulePropertyOrder(1)]
		public string RecipientSms
		{
			get
			{
				return _recipientPhone;
			}
			set
			{
				_recipientPhone = value;
			}
		}

		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Triggered Sms")]
		[Description("Used to select the triggered sms message to be used for this rule.")]
		[RuleProperty(false, true, false, "AvailableTriggeredSmsMessages")]
		[RulePropertyOrder(2)]
		public string TriggeredSmsName
		{
			get
			{
				if (_smsName != null)
				{
					return _smsName;
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				_smsName = value;
			}
		}

		/// <summary>
		/// Returns list of all available triggered sms messages
		/// </summary>
		[Browsable(false)]
		public Dictionary<string, string> AvailableTriggeredSmsMessages
		{
			get
			{
				using (var svc = LWDataServiceUtil.SmsServiceInstance())
				{
					Dictionary<string, string> smsMessages = new Dictionary<string, string>();
					IList<SmsDocument> list = svc.GetSmsMessages();
					if (list != null && list.Count > 0)
					{
						foreach (SmsDocument sms in list)
						{
							smsMessages.Add(sms.Name, sms.Name);
						}
					}
					return smsMessages;
				}
			}
		}

		public TriggeredSmsRule()
			: base("TriggeredSmsRule")
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="PreviousResultCode"></param>
		/// <returns></returns>
		public override void Invoke(ContextObject context)
		{
			string method = "Invoke";

			Member lwmember = null;
			VirtualCard lwvirtualcard = null;

			TriggeredSmsRuleResult result = new TriggeredSmsRuleResult()
			{
				Name = !string.IsNullOrWhiteSpace(context.Name) ? context.Name : this.RuleName,
				Mode = context.Mode,
				RuleType = this.GetType()
			};
			AddRuleResult(context, result);

			_logger.Debug(_className, method, "Invoking TriggeredSms rule");
			ResolveOwners(context.Owner, ref lwmember, ref lwvirtualcard);

			if (lwmember != null)
			{
				result.MemberId = lwmember.IpCode;
			}

            using (TriggeredSms sms = new TriggeredSms(LWConfigurationUtil.GetCurrentConfiguration(), TriggeredSmsName))
            {
                Dictionary<string, string> additionalFields = null;
                if (context.Environment != null)
                {
                    //additionalFields = Context.Environment as Dictionary<string, string>;
                    additionalFields = new Dictionary<string, string>();
                    foreach (var field in context.Environment)
                    {
                        additionalFields.Add(field.Key, field.Value.ToString());
                    }
                }
                /*
                 * WS - 07/11/2012
                 * In order of priority, if the incoming additional fields contain a field called recipientphone then that 
                 * phone number is used.  If the icoming fields do not contain a recipientphone but the rule is configured
                 * with a bscript expressino to provide the recipientphone then that is used.  If none of the two above
                 * applies then the member's MobilePhone (MemberDetails) is used.
                 * */
                if (!string.IsNullOrEmpty(_recipientPhone) && !ContainsRecipientPhone(additionalFields))
                {
                    // the additional fields do not contain a recipient email but it is specified in the rule property
                    ExpressionFactory exprF = new ExpressionFactory();
                    string recipientPhone = (string)exprF.Create(_recipientPhone).evaluate(context);
                    if (additionalFields == null)
                    {
                        additionalFields = new Dictionary<string, string>();
                    }
                    additionalFields.Add("recipientphone", recipientPhone);
                    _logger.Debug(
                        _className,
                        method,
                        string.Format("Adding mobile phone recipient {0} to fields.", recipientPhone));
                }
                if (additionalFields != null)
                {
                    sms.Send(lwmember, additionalFields);
                }
                else
                {
                    sms.Send(lwmember);
                }
            }
			return;
		}

		public override List<string> GetBscriptsToMove()
		{
			List<string> bscriptList = new List<string>();
			if (!string.IsNullOrWhiteSpace(this.RecipientSms) && ExpressionUtil.IsLibraryExpression(this.RecipientSms))
			{
				bscriptList.Add(ExpressionUtil.GetLibraryName(this.RecipientSms));
			}
			return bscriptList;
		}

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
		{
			string methodName = "MigrateRuleInstance";

			_logger.Trace(_className, methodName, "Migrating TriggeredSmsRule rule.");

			TriggeredSmsRule src = (TriggeredSmsRule)source;

			RecipientSms = src.RecipientSms;
			TriggeredSmsName = src.TriggeredSmsName;
			
            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

			return this;
		}

		private bool ContainsRecipientPhone(Dictionary<string, string> fields)
		{
			bool result = false;
			if (fields != null)
			{
				foreach (string k in fields.Keys)
				{
					if (k.ToLower() == "recipientphone")
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
