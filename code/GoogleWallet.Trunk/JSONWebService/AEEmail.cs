using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

using Brierley.FrameWork.Email;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;

namespace JSONWebService
{
    public class AEEmail
    {
        private const string _appName = "AmericanEagleCSPortal";
        private const string _UpdateProfileRule = "TriggeredEmail_UpdateProfile";

        private static LWLogger _logger = LWLoggerManager.GetLogger(_appName);
        
        public AEEmail()
        {

        }
        private string GetRuleName(EmailType emailTypes)
        {
            string ruleName = string.Empty;

            switch (emailTypes)
            {
                case EmailType.OnlineAppeasementReward:
                    ruleName = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("TriggeredEmail_" + EmailType.OnlineAppeasementReward.ToString()).Value;
                    break;
                case EmailType.UpdateProfile:
                    ruleName = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("TriggeredEmail_" + EmailType.UpdateProfile.ToString()).Value;
                    break;
                default:
                    break;
            }
            return ruleName;
        }
        public static void SendEmail(Member member, EmailType emailType, Dictionary<string, string> additionalFields, string emailAddress)
        {
            string ruleName = string.Empty;

            ruleName = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("TriggeredEmail_" + Enum.GetName(typeof(EmailType), emailType)).Value;
            member.PrimaryEmailAddress = emailAddress;
            ExecuteRule(member, additionalFields, ruleName);
        }
        public static void ExecuteRule(Member member, Dictionary<string, string> additionalFields, string ruleName)
        {
            try
            {

                _logger.Error("AEEmail", "ExecuteRule", "Send Email " + ruleName + " to " + member.PrimaryEmailAddress);

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 
                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                if (additionalFields != null)
                {
                    cobj.Environment = additionalFields;
                }
                RuleTrigger ruleTrigger = LWDataServiceUtil.DataServiceInstance(true).GetRuleByName(ruleName);
                if (ruleTrigger == null)
                {
                    _logger.Error("AEEmail", "ExecuteRule", ruleName + " Rule Not Defined");
                    throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                }

                //Execute the rule
                LWDataServiceUtil.DataServiceInstance(true).Execute(ruleTrigger, cobj, 0);

             }
            catch (Exception ex)
            {
                _logger.Error("AEEmail", "ExecuteRule", ruleName + " Rule Not Defined");
                throw new Exception(ex.Message, new Exception("System Error"));
            }

        }
        public static void SendEmail_SMTP(string from, string to, string subject, string body)
        {
            string smtpEmailClient = string.Empty;
            try
            {
                try
                {
                    smtpEmailClient = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("SMTPEmailClient").Value;
                }
                catch
                {
                    _logger.Error("AEEmail", "SendEmail_SMTP", "SMTPEmailClient not defined in global values");
                    throw new Exception("SendEmail_SMTP: " + "SMTPEmailClient not defined in global values");
                }
                
                MailMessage mail = new MailMessage(from, to, subject, body);
                mail.IsBodyHtml = false;

                mail.Priority = MailPriority.Normal;

                SmtpClient client = new SmtpClient(smtpEmailClient);
                client.Send(mail);
            }
            catch (Exception e)
            {
                _logger.Error("AEEmail", "SendEmail_SMTP", e.Message);
                throw new Exception("SendEmail_SMTP: " + e.Message);
            }

        }
        public static void SendRewardEmail(Member member, string rewardType, string couponCode, EmailType emailType, string emailAddress)
        {
            Dictionary<string, string> additionalFields = new Dictionary<string, string>();
            DateTime effectiveDate = DateTime.Now.AddMonths(1);

            if (rewardType.Length > 0)
            {
                additionalFields.Add("rewardtype", rewardType);
            }
            additionalFields.Add("couponcode", couponCode);
            additionalFields.Add("DDMonthYYYY", effectiveDate.ToShortDateString());
            foreach (KeyValuePair<string, string> entry in additionalFields)
            {
                _logger.Trace("AEEmail", "SendRewardEmail", "key: " + entry.Key + ", value: " + entry.Value);
            }
            SendEmail(member, emailType, additionalFields, emailAddress);
        }

    }
}
