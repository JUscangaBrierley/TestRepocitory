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
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public class AEEmail
    {
        private const string _appName = "AmericanEagleCSPortal";
        private const string _UpdateProfileRule = "TriggeredEmail_UpdateProfile";
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private static LWLogger _logger = LWLoggerManager.GetLogger(_appName);
        
        public AEEmail()
        {

        }
        private static string GetRuleName(EmailType emailTypes)
        {
            string ruleName = string.Empty;
            using (var ldService = _dataUtil.DataServiceInstance())
            {
                switch (emailTypes)
                {
                    case EmailType.OnlineAppeasementReward:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.OnlineAppeasementReward.ToString()).Value;
                        break;
                    case EmailType.UpdateProfile:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.UpdateProfile.ToString()).Value;
                        break;
                    //PI31161 changes for new Dollarreward appeasement begin            -scj
                    case EmailType.DollarRewardAppeasementReward:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.DollarRewardAppeasementReward.ToString()).Value;
                        break;
                    //PI31161 changes for new Dollarreward appeasement end            -scj

                    // AEO-Redesign-2015 Begin
                    case EmailType.MergeAccounts:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.MergeAccounts.ToString()).Value;
                        break;
                    // AEO-Redesign-2015 End


                    // AEO-218 Begin
                    case EmailType.PointsNoReward:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.PointsNoReward.ToString()).Value;
                        break;

                    case EmailType.PointsReward:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.PointsReward.ToString()).Value;
                        break;
                    // AEO-218 End



                    // AEO-219 Begin
                    case EmailType.B5G1NoReward:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.B5G1NoReward.ToString()).Value;
                        break;

                    case EmailType.B5G1Reward:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.B5G1Reward.ToString()).Value;
                        break;
                    // AEO-219 End
                    /*AEO-489 Begin------------SCJ*/
                    case EmailType.B5G1JeanAppeasementEmail:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.B5G1JeanAppeasementEmail.ToString()).Value;
                        break;
                    case EmailType.FiveDollarRwdAppeasementEmail:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.FiveDollarRwdAppeasementEmail.ToString()).Value;
                        break;
                    case EmailType.FreeBraAppeasementEmail:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.FreeBraAppeasementEmail.ToString()).Value;
                        break;
                    /*AEO-489 End------------SCJ*/
                    case EmailType.RequestCreditReceived:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.RequestCreditReceived.ToString()).Value;
                        break;
                    case EmailType.RequestCreditExpiredWeb:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.RequestCreditExpiredWeb.ToString()).Value;
                        break;

                    case EmailType.RequestCreditReceiptFound:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.RequestCreditReceiptFound.ToString()).Value;
                        break;

                    case EmailType.RequestCreditExpiredInStore:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.RequestCreditExpiredInStore.ToString()).Value;
                        break;
                    case EmailType.BirthdayAppeasementEmail:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.BirthdayAppeasementEmail.ToString()).Value;
                        break;
                    case EmailType.LegacyRewardEmail:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.LegacyRewardEmail.ToString()).Value;
                        break;
					//AEO-2389 BEGIN
                    case EmailType.ChooseYourOwnSaleDayAppeasement20Email:
                        ruleName = ldService.GetClientConfiguration("TriggeredEmail_" + EmailType.ChooseYourOwnSaleDayAppeasement20Email.ToString()).Value;
                        break;
                    //AEO-2389 END
                    default:
                        break;
                }
            }
            return ruleName;
        }


        public static void SendEmail(Member member, EmailType emailType, Dictionary<string, string> additionalFields, string emailAddress)
        {
            string ruleName = string.Empty;

          //  ruleName = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("TriggeredEmail_" + Enum.GetName(typeof(EmailType), emailType)).Value;

            ruleName = GetRuleName(emailType);
            if ( ruleName != null && ruleName != string.Empty ) {
                member.PrimaryEmailAddress = emailAddress;
                ExecuteRule(member, additionalFields, ruleName);
                member.PrimaryEmailAddress = string.Empty; //AEO-1307
            }
           
        }
        public static void ExecuteRule(Member member, Dictionary<string, string> additionalFields, string ruleName)
        {
            try
            {

                _logger.Error("AEEmail", "ExecuteRule", "Send Email " + ruleName + " to " + member.PrimaryEmailAddress);
                foreach (string value in additionalFields.Keys) {
                    _logger.Trace("AEEmail", "ExecuteRule", value + ": " + additionalFields[value]);
                }
                

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 
                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                if (additionalFields != null)
                {
                   
                    //cobj.Environment =  additionalFields;
                    cobj.Environment = additionalFields.ToDictionary(pair => pair.Key, pair => (object)pair.Value);

                }
                using (var dataService = Brierley.FrameWork.Data.LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    RuleTrigger ruleTrigger = dataService.GetRuleByName(ruleName);
                    if (ruleTrigger == null)
                    {
                        _logger.Error("AEEmail", "ExecuteRule", ruleName + " Rule Not Defined");
                        throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                    }
                    dataService.Execute(ruleTrigger, cobj);
                }
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
                    using (var ldService = _dataUtil.DataServiceInstance())
                    {
                        smtpEmailClient = ldService.GetClientConfiguration("SMTPEmailClient").Value;
                    }
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
        public static void SendRewardEmail(Member member, string rewardType, string couponCode, EmailType emailType, string emailAddress, string rewardIndicator)
        {
            _logger.Trace("AEEmail", "SendRewardEmail", "Send Email RewardType: " + rewardType + " to " + member.PrimaryEmailAddress + " with reward Indicator: " + rewardIndicator);
            
            Dictionary<string, string> additionalFields = new Dictionary<string, string>();
            DateTime effectiveDate = DateTime.Now.AddMonths(1);

            if (rewardType.Length > 0)
            {
                if (rewardIndicator == "$")
                {
                    additionalFields.Add("RewardAmount", rewardType);
                }
                else
                {
                    additionalFields.Add("rewardtype", rewardType);
                }
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
