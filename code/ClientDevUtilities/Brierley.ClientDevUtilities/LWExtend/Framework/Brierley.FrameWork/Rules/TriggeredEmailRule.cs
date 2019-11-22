using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using System.ComponentModel;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.bScript;

using Brierley.FrameWork.Rules.UIDesign;

using Brierley.FrameWork.Email;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TriggeredEmailRule:RuleBase
    {
        #region Results
        public class TriggeredEmailRuleResult : ContextObject.RuleResult
        {
            public long Id = -1;
            //public string Name;
        }
        #endregion

        #region Fields
        private const string _className = "TriggeredEmailRule";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private string _emailName = string.Empty;
        private string _recipientEmail = string.Empty;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public TriggeredEmailRule()
            : base("TriggeredEmailRule")
        {
        }

        #region Private Helpers        
        private bool ContainsRecipientEmail(Dictionary<string, string> fields)
        {
            bool result = false;
            if (fields != null)
            {
                foreach (string k in fields.Keys)
                {
                    if (k.ToLower() == "recipientemail")
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Properties

        public override string DisplayText
        {
            get { return "Send Triggered Email"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("The email of the recipient")]
        [Description("This is a bScript expression that provides the recipient email address.")]
        [RuleProperty(true, false, false, null, false, true)]
        [RulePropertyOrder(1)]
        public string RecipientEmail
        {
            get
            {
                return _recipientEmail;
            }
            set
            {
                _recipientEmail = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Triggered Email")]
        [Description("Used to select the triggered email to be used for this rule.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails")]
        [RulePropertyOrder(2)]
        public string TriggeredEmailName
        {
            get
            {
                if (_emailName != null)
                {
                    return _emailName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                _emailName = value;
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
				using (var svc = LWDataServiceUtil.EmailServiceInstance())
				{
					Dictionary<string, string> emails = new Dictionary<string, string>();
					IList<EmailDocument> list = svc.GetEmails();
					if (list != null && list.Count > 0)
					{
						foreach (EmailDocument email in list)
						{
							emails.Add(email.Name, email.Name);
						}
					}
					return emails;
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
            string method = "Invoke";

			Member lwmember = null;
			VirtualCard lwvirtualcard = null;

            TriggeredEmailRuleResult result = new TriggeredEmailRuleResult() 
            {
                Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
                Mode = Context.Mode,
                RuleType = this.GetType() 
            };
            AddRuleResult(Context, result);

            _logger.Debug(_className, method, "Invoking TriggeredEmail rule");
            ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualcard);

            if (lwmember != null)
            {
                result.MemberId = lwmember.IpCode;
            }

            /*
             * We have to use reflection here because we cannot create a reference to this dll from the
             * framework because the email assembly has a reference to the framework assembly.
             * */
            //ILWEmailService svc = LWDataServiceUtil.EmailServiceInstance();
            using (ITriggeredEmail email = TriggeredEmailFactory.Create(TriggeredEmailName))
            {
                Dictionary<string, string> additionalFields = null;
                if (Context.Environment != null)
                {
                    //additionalFields = Context.Environment as Dictionary<string, string>;
                    additionalFields = new Dictionary<string, string>();
                    foreach (var field in Context.Environment)
                    {
                        additionalFields.Add(field.Key, field.Value.ToString());
                    }
                }
                /*
                 * WS - 07/11/2012
                 * In order of priority, if the incoming additional fields contain a field called recipientemail then that 
                 * email address is used.  If the icoming fields do not contain a recipientemail but the rule is configured
                 * with a bscript expressino to provide the recipientemail then that is used.  If none of the two above
                 * applies then the member's primary email address is used.
                 * */
                if (!string.IsNullOrEmpty(_recipientEmail) && !ContainsRecipientEmail(additionalFields))
                {
                    // the additional fields do not contain a recipient email but it is specified in the rule property
                    ExpressionFactory exprF = new ExpressionFactory();
                    string recipientEmail = (string)exprF.Create(_recipientEmail).evaluate(Context);
                    if (additionalFields == null)
                    {
                        additionalFields = new Dictionary<string, string>();
                    }
                    additionalFields.Add("recipientemail", recipientEmail);
                    _logger.Debug(_className, method,
                        string.Format("Adding email recipient {0} to fields.", recipientEmail));
                }
                if (additionalFields != null)
                {
                    email.SendAsync(lwmember, additionalFields).Wait();
                }
                else
                {
                    email.SendAsync(lwmember).Wait();
                }
            }
            return;
        }

        #region Migrartion Helpers

        public override List<string> GetBscriptsToMove()
        {
            List<string> bscriptList = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.RecipientEmail) && ExpressionUtil.IsLibraryExpression(this.RecipientEmail))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.RecipientEmail));
            }            
            return bscriptList;
        }

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating TriggeredEmailRule rule.");

            TriggeredEmailRule src = (TriggeredEmailRule)source;

            RecipientEmail = src.RecipientEmail;
            TriggeredEmailName = src.TriggeredEmailName;            
            
            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
        #endregion
    }
}
