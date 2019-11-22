using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Configuration;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{
    class BraFulfillment : IDAPOutputProvider
    {

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private string fileName = "AE_Promotions_ccyymmdd_hhxxss_nnnnn.csv";
        private string path = string.Empty;
        private int recordCount = 0;
        private string strLoyaltyNumber = string.Empty;
        private string strPromoCode = string.Empty;
        private string strFirstName = string.Empty;
        private string strLastName = string.Empty;
        private string strLine1 = string.Empty;
        private string strLine2 = string.Empty;
        private string strCity = string.Empty;
        private string strState = string.Empty;
        private string strPostalCode = string.Empty;
        private string strCountry = string.Empty;
        private string strAddressMailable = string.Empty;
        private string strReasonCode = string.Empty;
        private string strLanguagePreference = string.Empty;
        private string strUnder13 = string.Empty;
        private string strBrandFlag_AE = string.Empty;
        private string strBrandFlag_Aerie = string.Empty;
        private string strBrandFlag_77kids = string.Empty;
        private string strBaseBrand = string.Empty;
        private string strPromotionCount = string.Empty;
        private DateTime processDate = DateTime.Today;

        #region IDAPOutputProvider Members

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if (null == ConfigurationManager.AppSettings["FilePath"])
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }
            else
            {
                path = ConfigurationManager.AppSettings["FilePath"];
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            fileName = path + @"\" + fileName.Replace("ccyymmdd", DateTime.Today.ToString("yyyMMdd"));
            fileName = fileName.Replace("hhxxss", DateTime.Today.ToString("hhmmss"));

            if (null != ConfigurationManager.AppSettings["ProcessDate"])
            {
                string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                DateTime.TryParse(strProcessDate, out processDate);
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string rewardName = "Bra Reward-1";

            try
            {
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("BraRewards/BraReward");
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "LoyaltyNumber":
                            strLoyaltyNumber = node.InnerText;
                            break;
                        case "PromoCode":
                            strPromoCode = node.InnerText;
                            break;
                        case "FirstName":
                            strFirstName = node.InnerText;
                            break;
                        case "LastName":
                            strLastName = node.InnerText;
                            break;
                        case "Line1":
                            strLine1 = node.InnerText;
                            break;
                        case "Line2":
                            strLine2 = node.InnerText;
                            break;
                        case "City":
                            strCity = node.InnerText;
                            break;
                        case "State":
                            strState = node.InnerText;
                            break;
                        case "PostalCode":
                            strPostalCode = node.InnerText;
                            break;
                        case "Country":
                            strCountry = node.InnerText;
                            break;
                        case "AddressMailable":
                            strAddressMailable = node.InnerText;
                            break;
                        case "ReasonCode":
                            strReasonCode = node.InnerText;
                            break;
                        case "LanguagePreference":
                            strLanguagePreference = node.InnerText;
                            break;
                        case "Under13":
                            strUnder13 = node.InnerText;
                            break;
                        case "BrandFlag_AE":
                            strBrandFlag_AE = node.InnerText;
                            break;
                        case "BrandFlag_Aerie":
                            strBrandFlag_Aerie = node.InnerText;
                            break;
                        case "BrandFlag_77kids":
                            strBrandFlag_77kids = node.InnerText;
                            break;
                        case "BaseBrand":
                            strBaseBrand = node.InnerText;
                            break;
                        case "PromotionCount":
                            strPromotionCount = node.InnerText;
                            break;
                        default:
                            strPromotionCount = node.InnerText;
                            break;
                    }
                }

                if (null != xmlNode)
                {
                    if (strLoyaltyNumber.Length > 0)
                    {
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            bool addressMailable = (strAddressMailable == "1");
                            Member member = lwService.LoadMemberFromLoyaltyID(strLoyaltyNumber);
                            if (null == member)
                            {
                                // Log error when member not found
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyNumber"].Value);
                            }
                            else
                            {

                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Getting rewards for " + rewardName);
                                //Now go through the bra rewards and see if there are any that are ready to be sent.
                                RewardDef reward;
                                using (var contService = _dataUtil.ContentServiceInstance())
                                {
                                    reward = contService.GetRewardDef(rewardName);
                                }
                                long rewardDefId = reward.Id;

                                IList<MemberReward> memberRewards = lwService.GetMemberRewardsByDefId(member, rewardDefId);

                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Found " + memberRewards.Count.ToString() + " member rewards for ipcode: " + member.IpCode.ToString());
                                foreach (MemberReward mbrReward in memberRewards)
                                {
                                    if (mbrReward.FulfillmentDate == null)
                                    {
                                        if (addressMailable)
                                        {
                                            mbrReward.FulfillmentDate = DateTime.Today;
                                            int rewardStatus = (int)RewardStatus.Mailed;
                                            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                            //mbrReward.OrderNumber = rewardStatus.ToString();
                                            mbrReward.LWOrderNumber = rewardStatus.ToString();
                                            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                                            lwService.UpdateMemberReward(mbrReward);
                                        }
                                    }
                                }

                            }
                            StringBuilder sb = new StringBuilder();
                            sb.Append(strLoyaltyNumber);
                            sb.Append("," + strPromoCode);
                            sb.Append("," + strFirstName);
                            sb.Append("," + strLastName);
                            sb.Append("," + strLine1);
                            sb.Append("," + strLine2);
                            sb.Append(","); //Company
                            sb.Append("," + strCity);
                            sb.Append("," + strState);
                            sb.Append("," + strPostalCode);
                            sb.Append("," + strCountry);
                            sb.Append("," + strReasonCode);
                            sb.Append("," + strPromotionCount);
                            sb.Append("," + strLanguagePreference);
                            sb.Append("," + strUnder13);
                            sb.Append("," + strBrandFlag_AE);
                            sb.Append("," + strBrandFlag_Aerie);
                            sb.Append("," + strBrandFlag_77kids);
                            sb.Append("," + strBaseBrand);

                            StreamWriter sw = new StreamWriter(fileName, true);

                            sw.WriteLine(sb.ToString());
                            sw.Close();
                            ++recordCount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //        throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            if (recordCount == 0)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No records processed.  No file to produce.");
                return;
            }
            string newfileName = fileName.Replace("nnnnn", recordCount.ToString());

            if(File.Exists(newfileName))
            {
                File.Delete(newfileName);
            }
            File.Copy(fileName, newfileName);
            File.Delete(fileName);
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        #endregion
    }
}
