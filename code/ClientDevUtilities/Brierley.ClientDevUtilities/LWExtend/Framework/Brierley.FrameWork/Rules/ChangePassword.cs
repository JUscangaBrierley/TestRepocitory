using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Rules.UIDesign;

using Brierley.FrameWork.Email;

namespace Brierley.FrameWork.Rules
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ChangePassword : RuleBase
    {
        #region Results
        public class ChangePasswordRuleResult : ContextObject.RuleResult
        {
            public string   NewPassword;
            public bool     PasswordChangeRequiredFlag = false;
            public bool     EmailSent = false;
        }
        #endregion

        #region Private Variables

		private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private string className = "ChangePassword";
		
        private string passwordChangeExpression = string.Empty;
        private bool pwdChangeRequired = false;        
        private string emailName = string.Empty;		        
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public ChangePassword()
            : base("ChangePassword")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Validate()
        {
            if (string.IsNullOrEmpty(PasswordExpression))
            {
                throw new Exception("Please provide a password expression.");
            }
        }

        #region Properties

        public override string DisplayText
        {
            get { return "Change Member's Password"; }
        }
       
		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Password Expression")]
		[Description("This bScript expression will be used to set the member's password.")]
		[RulePropertyOrder(1)]
		[RuleProperty(true, false, false, null, true, false)]
		public string PasswordExpression
		{
			get
			{
                return passwordChangeExpression;
			}
			set
			{
                passwordChangeExpression = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Password Change Required")]
        [Description("If set to true, the member will be required to change their password next time they login.")]
        [RulePropertyOrder(2)]
        [RuleProperty(false, false, false, null, false)]
        public bool PasswordChangeRequired
        {
            get { return pwdChangeRequired; }
            set { pwdChangeRequired = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Password Notification Email")]
        [Description("Used to select the triggered email that will be sent out to the member providing them their credentials.")]
        [RuleProperty(false, true, false, "AvailableTriggeredEmails")]
        [RulePropertyOrder(3)]
        public string TriggeredEmailName
        {
            get { return emailName; }
            set { emailName = value; }
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
					IList<EmailDocument> emailList = svc.GetEmails();
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
                
        #endregion
        
        public override void Invoke(ContextObject Context)
        {
            string methodName = "Invoke";

			Member lwmember = null;
			VirtualCard lwvirtualCard = null;

            ChangePasswordRuleResult result = new ChangePasswordRuleResult()
            {
                Name = !string.IsNullOrWhiteSpace(Context.Name) ? Context.Name : this.RuleName,
                Mode = Context.Mode,
                RuleType = this.GetType(),
                PasswordChangeRequiredFlag = pwdChangeRequired
            };

            AddRuleResult(Context, result);

			ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);

            if (lwmember == null)
            {
                string errMsg = "No member could be resolved for change password rule.";
                logger.Error(className, methodName, errMsg);
                throw new LWRulesException(errMsg) { ErrorCode = 3214 };
            }
            else
            {
                logger.Trace(className, methodName, "Invoking change password rule for member with Ipcode = " + lwmember.IpCode + ".");
            }

            result.MemberId = lwmember.IpCode;

			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{

				string newPassword = string.Empty;
				if (!string.IsNullOrEmpty(passwordChangeExpression))
				{
					ExpressionFactory exprF = new ExpressionFactory();
					Expression expr = exprF.Create(passwordChangeExpression);
					newPassword = (string)expr.evaluate(Context);
				}

				if (!string.IsNullOrEmpty(newPassword))
				{
					service.ChangeMemberPassword(lwmember, newPassword, false);
				}
				result.NewPassword = newPassword;
				lwmember.PasswordChangeRequired = pwdChangeRequired;

				if (Context.Mode == RuleExecutionMode.Real)
				{
					service.SaveMember(lwmember);
					if (!string.IsNullOrEmpty(emailName))
					{
						Dictionary<string, string> fields = new Dictionary<string, string>();
						fields.Add("Username", lwmember.Username);
						fields.Add("Password", newPassword);
						try
						{
							using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailName))
							{
								if (fields != null)
								{
									email.SendAsync(lwmember, fields);
								}
								else
								{
									email.SendAsync(lwmember);
								}
							}
							result.EmailSent = true;
						}
						catch (Exception ex)
						{
							string msg = string.Format("Error sending TriggeredEmail using mailing {0} for member with ipcode {1}",
								emailName, lwmember.MyKey);
							logger.Error(className, methodName, msg, ex);
							result.EmailSent = false;
						}
					}
				}
			}

            return;
        }

        #region Migrartion Helpers

        public override List<string> GetBscriptsToMove()
        {
            List<string> bscriptList = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.PasswordExpression) && ExpressionUtil.IsLibraryExpression(this.PasswordExpression))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.PasswordExpression));
            }            
            return bscriptList;
        }

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            logger.Trace(className, methodName, "Migrating Issuereward rule.");

            ChangePassword src = (ChangePassword)source;

            pwdChangeRequired = src.pwdChangeRequired;
            passwordChangeExpression = src.passwordChangeExpression;
            emailName = src.emailName;
                        
            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
        #endregion
    }
}
