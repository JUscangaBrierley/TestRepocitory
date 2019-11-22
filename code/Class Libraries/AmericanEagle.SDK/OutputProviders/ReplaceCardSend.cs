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
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;

namespace AmericanEagle.SDK.OutputProviders
{
    class ReplaceCardSend : IDAPOutputProvider
    {

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private string fileName = "JJ001_yyyMMdd_hhmmss_nnnnn.txt";
        private string path = string.Empty;
        private int recordCount = 00000;
        private string RowID = string.Empty;
        private string LoyaltyIDNumber = string.Empty;
        private string AlternateID = string.Empty;
        private string NamePrefix = string.Empty;
        private string FirstName = string.Empty;
        private string LastName = string.Empty;
        private string NameSuffix = string.Empty;
        private string AddressLineOne = string.Empty;
        private string AddressLineTwo = string.Empty;
        private string Company = string.Empty;
        private string City = string.Empty;
        private string StateOrProvince = string.Empty;
        private string ZipOrPostalCode = string.Empty;
        private string Country = string.Empty;
        private string Filler1 = string.Empty;
        private string Program = string.Empty;
        private string OfferCode = string.Empty;
        private string RewardLevel = string.Empty;
        private string StartingPoints = string.Empty;
        private string BasePoints = string.Empty;
        private string BonusPoints = string.Empty;
        private string RemainingPoints = string.Empty;
        private string TotalPoints = string.Empty;
        private string AuthCode = string.Empty;
        private string BaseLoyaltyBrand = string.Empty;
        private string LanguagePreference = string.Empty;
        private string Filler3 = string.Empty;
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

            fileName = path + @"\" + fileName.Replace("yyyMMdd", DateTime.Today.ToString("yyyMMdd"));
            fileName = fileName.Replace("hhmmss", DateTime.Today.ToString("hhmmss"));
            if (null != ConfigurationManager.AppSettings["ProcessDate"])
            {
                string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                DateTime.TryParse(strProcessDate, out processDate);
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");


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

                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/CardReplacement");



                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "RowID":
                            RowID = string.Format("{0,-10}", node.InnerText);
                            break;
                        case "LoyaltyIDNumber":
                            LoyaltyIDNumber = string.Format("{0,-14}", node.InnerText);
                            break;
                        case "AlternateID": // Check for valid hhkey/alternateid value and reformat data
                            AlternateID = node.InnerText;
                            int nDigit = 0, nTotal = 0;
                            for (int nX = AlternateID.Length - 1; nX >= 0; --nX)
                            {
                                char ch = AlternateID[nX];
                                if (Char.IsDigit(ch))
                                    nDigit = Convert.ToInt32(ch - '0');
                                else if (Char.IsLetter(ch))
                                    nDigit = ch - '0';
                                else
                                    throw new Exception("Value contains invalid characters.  Only letters and numbers are allowed.");

                                nTotal += nDigit;
                            }
                            int nCheckDigit = nTotal % 10;
                            AlternateID += nCheckDigit.ToString();
                            AlternateID = "00" + AlternateID;
                            AlternateID = string.Format("{0,-16}", AlternateID);
                            break;
                        case "NamePrefix":
                            NamePrefix = string.Format("{0,-17}", node.InnerText);
                            break;
                        case "FirstName":
                            FirstName = string.Format("{0,-30}", node.InnerText);
                            break;
                        case "LastName":
                            LastName = string.Format("{0,-30}", node.InnerText);
                            break;
                        case "NameSuffix":
                            NameSuffix = string.Format("{0,-10}", node.InnerText);
                            break;
                        case "AddressLineOne":
                            AddressLineOne = string.Format("{0,-50}", node.InnerText);
                            break;
                        case "AddressLineTwo":
                            AddressLineTwo = string.Format("{0,-50}", node.InnerText);
                            break;
                        case "Company":
                            Company = string.Format("{0,-40}", node.InnerText);
                            break;
                        case "City":
                            City = string.Format("{0,-30}", node.InnerText);
                            break;
                        case "StateOrProvince":
                            StateOrProvince = string.Format("{0,-2}", node.InnerText);
                            break;
                        case "ZipOrPostalCode":
                            ZipOrPostalCode = string.Format("{0,-10}", node.InnerText);
                            break;
                        case "Country":
                            Country = string.Format("{0,-25}", node.InnerText);
                            break;
                        case "Filler1":
                            Filler1 = string.Format("{0,-25}", node.InnerText);
                            break;
                        case "Program":
                            Program = string.Format("{0,-20}", node.InnerText);
                            break;
                        case "OfferCode":
                            OfferCode = string.Format("{0,-20}", node.InnerText);
                            break;
                        case "RewardLevel":
                            RewardLevel = string.Format("{0,-10}", node.InnerText);
                            break;
                        case "StartingPoints":
                            StartingPoints = string.Format("{0,-6}", node.InnerText);
                            break;
                        case "BasePoints":
                            BasePoints = string.Format("{0,-6}", node.InnerText);
                            break;
                        case "BonusPoints":
                            BonusPoints = string.Format("{0,-6}", node.InnerText);
                            break;
                        case "RemainingPoints":
                            RemainingPoints = string.Format("{0,-6}", node.InnerText);
                            break;
                        case "TotalPoints":
                            TotalPoints = string.Format("{0,-10}", node.InnerText);
                            break;
                        case "AuthCode":
                            AuthCode = string.Format("{0,-2}", node.InnerText);
                            break;
                        case "BaseLoyaltyBrand":
                            BaseLoyaltyBrand = string.Format("{0,-2}", node.InnerText);
                            break;
                        case "LanguagePreference":
                            LanguagePreference = string.Format("{0,-1}", node.InnerText);
                            break;
                        case "Filler3":
                            Filler3 = string.Format("{0,-87}", node.InnerText);
                            break;
                        default:
                            Filler3 = string.Format("{0,-87}", node.InnerText);
                            break;
                    }
                }

                if (null != xmlNode)
                {
                    Member member = null;
                    MemberCardReplacements memberCR = null;
                    IList<IClientDataObject> mbrDtlObjs = null;
                    
                    if (LoyaltyIDNumber.Length > 0)
                    {
                        //  bool addressMailable = (strAddressMailable == "1");
                         member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(LoyaltyIDNumber);
                        if (null == member)
                        {
                            // Log error when member not found
                            this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyNumber"].Value);
                        }
                        else
                        {
                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Getting card for replacement");
                            if (Program.Length > 5)
                            {
                                fileName = fileName.Replace("JJ001", Program.Substring(0, 5));  // change the file name J1003 or J1001
                            }
                           //  return strValue;
                            //PI26966- Non primary Replacemnt cards sent changes--- begin
                            mbrDtlObjs = member.GetChildAttributeSets("MemberCardReplacements");
                          
                            for(var i = 0; i < mbrDtlObjs.Count; i++)
                            {
                               var item = mbrDtlObjs[i];
                               memberCR = (MemberCardReplacements)mbrDtlObjs[i];
                               if (memberCR != null)
                               {
                                   if (memberCR.StatusCode == 2) // only cards set for replacement
                                   {
                                       if (memberCR.DATESENT == null) // Update Datesent only if the date not previously entered
                                       {
                                           memberCR.DATESENT = DateTime.Now;
                                       }
                                   }
                               }
                            }
                            LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                            
                           // memberCR1 = (MemberCardReplacements)mbrDtlObjs[1];     
                           ////-- if (memberCR != null)
                           // {

                           //     //if ((memberCr. .MemberSource != (int)MemberSource.OnlineAEEnrolled) && (memberDetails.MemberSource != (int)MemberSource.OnlineAERegistered) && (member.MemberStatus != MemberStatusEnum.Terminated))
                           //     {
                                      
                           //         memberCR.DATESENT = DateTime.Now;
                           //         // Save member Information to Database
                                  
                           //         LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                           //         Member member1 = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(LoyaltyIDNumber);
                           //         IList<IClientDataObject> mbrDtlObjs1 = member.GetChildAttributeSets("MemberCardReplacements");
                           //         MemberCardReplacements memberCR1 = (MemberCardReplacements)mbrDtlObjs1[0];
                           //         this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "loyaltynumber" + memberCR1.LoyaltyIDNumber + "rowkey:" + memberCR1.RowKey + "statuscode:" + memberCR1.StatusCode + "datesent " + memberCR1.DATESENT + "updatedate " + memberCR1.UpdateDate + "after");
                           //     }
                           // }

                            //PI26966- Non primary Replacemnt cards sent changes--- end
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append(RowID);
                        sb.Append(LoyaltyIDNumber);
                        sb.Append(AlternateID);
                        sb.Append(NamePrefix);
                        sb.Append(FirstName);
                        sb.Append(LastName);
                        sb.Append(NameSuffix);
                        sb.Append(AddressLineOne);
                        sb.Append(AddressLineTwo);
                        sb.Append(Company);
                        sb.Append(City);
                        sb.Append(StateOrProvince);
                        sb.Append(ZipOrPostalCode);
                        sb.Append(Country);
                        sb.Append(Filler1);
                        sb.Append(Program);
                        sb.Append(OfferCode);
                        sb.Append(RewardLevel);
                        sb.Append(StartingPoints);
                        sb.Append(BasePoints);
                        sb.Append(BonusPoints);
                        sb.Append(RemainingPoints);
                        sb.Append(TotalPoints);
                        sb.Append(AuthCode);
                        sb.Append(BaseLoyaltyBrand);
                        sb.Append(LanguagePreference);
                        sb.Append(Filler3);

                        StreamWriter sw = new StreamWriter(fileName, true);

                        sw.WriteLine(sb.ToString());
                        sw.Close();
                        ++recordCount;
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
            //  throw new System.NotImplementedException();
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

            if (File.Exists(newfileName))
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
