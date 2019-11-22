//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;

using Brierley.FrameWork.Rules.UIDesign;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Rules
{
	public class CertificateUtil
	{
		private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string className = "CertificateUtil";

		private string offerCode = string.Empty;
		private bool assignLWcertificate = false;
		private string lowThresholdEmail = string.Empty;

        private Dictionary<ContentObjType, Dictionary<string, DateTime>> lastCheckDate = new Dictionary<ContentObjType, Dictionary<string, DateTime>>();

		#region Helper Methods

		public long GetCertificateCount(ContentObjType objectType, string certificateTypeCode)
		{
			string methodName = "GetCertificateCount";

			try
			{
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					return content.HowManyPromoCertificates(objectType, certificateTypeCode, DateTime.Now, DateTime.Now, true);
				}
			}
			catch (Exception ex)
			{
				logger.Error(className, methodName, "Error getting certificate count.", ex);
				throw;
			}
		}

		public void CheckLowCertificateThreshold(Member member, ContentObjType objectType, string certificateTypeCode, long threshold)
		{
			string methodName = "CheckLowCertificateThreshold";

            if (!string.IsNullOrEmpty(lowThresholdEmail) && !string.IsNullOrEmpty(certificateTypeCode) && !string.IsNullOrEmpty(LowThresholdEmailRecepient))
            {
                // Check that we haven't checked in the last 5 minutes
                DateTime lastCheckDate = GetLastCheckDate(objectType, certificateTypeCode);
                if (lastCheckDate > DateTime.Now.AddMinutes(-5))
                {
                    logger.Trace(className, methodName, 
                        string.Format("Low threshold check for object type {0} and certificate type code {1} last happened less than 5 minutes ago at {2}. Skipping check.",
                        objectType, certificateTypeCode, lastCheckDate));
                    return;
                }

                var ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
                SetLastCheckDate(objectType, certificateTypeCode, DateTime.Now);

                Task t = new Task(() =>
                {
                    LWConfigurationUtil.SetCurrentEnvironmentContext(ctx.Organization, ctx.Environment);
                    long count = GetCertificateCount(objectType, certificateTypeCode);
                    logger.Trace(className, methodName,
                        string.Format("{0} reward certificates are left.  Threshold is set at {1}", count, threshold));
                    if (count <= threshold)
                    {
                        // send email.                    
                        logger.Trace(className, methodName,
                            "Sending email for low certificate count.");
                        Dictionary<string, string> fields = new Dictionary<string, string>();
                        fields.Add("CertificateType", certificateTypeCode);
                        fields.Add("CertificateCount", count.ToString());
                        SendTriggeredEmail(LowThresholdEmailRecepient, lowThresholdEmail, fields);
                    }
                });

                t.ContinueWith((task) =>
                {
                    try
                    {
                        if (task.Exception != null)
                        {
                            foreach (var exception in task.Exception.InnerExceptions)
                            {
                                logger.Error(className, "CheckLowCertificateThreshold", "Error checking low cert threshold", exception);
                            }
                        }
                    }
                    catch { }
                }, TaskContinuationOptions.OnlyOnFaulted);

                t.Start();
            }
		}

		public void SendTriggeredEmail(string recepientEmail, string emailToSend, Dictionary<string, string> fields)
		{
			string method = "SendTriggeredEmail";

			try
			{
				using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailToSend))
				{
					if (fields != null)
					{
						email.SendAsync(recepientEmail, fields).Wait();
					}
				}
			}
			catch (Exception ex)
			{
				string msg = string.Format("Error sending TriggeredEmail using mailing {0} for {1}",
					emailToSend, recepientEmail);
				logger.Error(className, method, msg, ex);
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
				logger.Error(className, method, msg, ex);
			}
		}

		public PromotionCertificate IssueCertificate(ContentObjType objectType, string certificateTypeCode)
		{
			string methodName = "IssueRewardCertificate";

			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				PromotionCertificate cert = null;
				if (!string.IsNullOrEmpty(certificateTypeCode) && AssignLWCertificate)
				{
					cert = content.RetrieveFirstAvailablePromoCertificate(objectType, certificateTypeCode, DateTime.Now, DateTime.Now);
					if (cert == null)
					{
						string msg = string.Format("No more certificates of type {0} left ", certificateTypeCode);
						logger.Error(className, methodName, msg);
						throw new LWRulesException(msg);
					}
				}
				else
				{
					logger.Debug(className, methodName, "No certificate required for issuing.");
				}

				return cert;
			}
		}

        private DateTime GetLastCheckDate(ContentObjType objectType, string certificateTypeCode)
        {
            lock (lastCheckDate)
            {
                if (lastCheckDate.ContainsKey(objectType) && lastCheckDate[objectType].ContainsKey(certificateTypeCode))
                    return lastCheckDate[objectType][certificateTypeCode];
                return DateTime.MinValue;
            }
        }

        private void SetLastCheckDate(ContentObjType objectType, string certificateTypeCode, DateTime date)
        {
            lock (lastCheckDate)
            {
                if (!lastCheckDate.ContainsKey(objectType))
                    lastCheckDate.Add(objectType, new Dictionary<string, DateTime>());

                if (!lastCheckDate[objectType].ContainsKey(certificateTypeCode))
                    lastCheckDate[objectType].Add(certificateTypeCode, date);
                else
                    lastCheckDate[objectType][certificateTypeCode] = date;
            }
        }

		#endregion

		#region Properties

		//public PointsConsumptionOnIssueReward PointsConsumption
		//{
		//    get
		//    {
		//        return pointsConsumption;
		//    }
		//    set
		//    {
		//        pointsConsumption = value;
		//    }
		//}

		//public string ChangedByExpression
		//{
		//    get { return changedByExpression; }
		//    set { changedByExpression = value; }
		//}

		//public string TriggeredEmailName
		//{
		//    get
		//    {
		//        if (emailName != null)
		//        {
		//            return emailName;
		//        }
		//        else
		//        {
		//            return string.Empty;
		//        }
		//    }
		//    set
		//    {
		//        emailName = value;
		//    }
		//}

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

		public bool AssignLWCertificate
		{
			get
			{
				return assignLWcertificate;
			}
			set
			{
				assignLWcertificate = value;
			}
		}

		//public string CertificateBucket
		//{
		//    get
		//    {
		//        return certificateBucket;
		//    }
		//    set
		//    {
		//        certificateBucket = value;
		//    }
		//}

		//public string CertificateTypeCodeAttribute
		//{
		//    get
		//    {
		//        return certificateTypeCodeAttribute;
		//    }
		//    set
		//    {
		//        certificateTypeCodeAttribute = value;
		//    }
		//}

		public string OfferCode
		{
			get { return offerCode; }
			set { offerCode = value; }
		}

		//public string CertificateNmbrAttribute
		//{
		//    get
		//    {
		//        return certificateNmbrAttribute;
		//    }
		//    set
		//    {
		//        certificateNmbrAttribute = value;
		//    }
		//}

		//public string CertificateStatusAttribute
		//{
		//    get
		//    {
		//        return certificateStatusAttribute;
		//    }
		//    set
		//    {
		//        certificateStatusAttribute = value;
		//    }
		//}

		//public string StartDateAttribute
		//{
		//    get
		//    {
		//        return startDateAttribute;
		//    }
		//    set
		//    {
		//        startDateAttribute = value;
		//    }
		//}

		//public string EndDateAttribute
		//{
		//    get
		//    {
		//        return endDateAttribute;
		//    }
		//    set
		//    {
		//        endDateAttribute = value;
		//    }
		//}

		public string LowThresholdEmailName
		{
			get
			{
				if (!string.IsNullOrEmpty(lowThresholdEmail))
				{
					return lowThresholdEmail;
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				lowThresholdEmail = value;
			}
		}

		public string LowThresholdEmailRecepient { get; set; }

		#endregion

	}
}
