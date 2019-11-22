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
using System.Security.Cryptography.X509Certificates;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data;
//using TestPOSTWebService.Utils;
using WalletObjectsSample.Utils;
using System.Net;


namespace AmericanEagle.SDK.OutputProviders
{
    class GoogleWalletPtsUpdate : IDAPOutputProvider
    {
        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private int recordCount = 0000000;
        private string LoyaltyIDNumber = string.Empty;
        private string IPcode = string.Empty;
        private string GWVersion = string.Empty;
        private string TotalPoints = string.Empty;
        #region IDAPOutputProvider Members

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {          
        }
        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin Wallet Points Update");
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
                XmlNode xmlNode = doc.SelectSingleNode("GoogleWallet/PointsUpdate");
              
                foreach (XmlNode node in xmlNode.ChildNodes)
                {         
                    switch (node.Name)
                    {
                        case "LoyaltyIdNumber":
                           LoyaltyIDNumber = node.InnerText;
                  
                            break;
                        case "IPcode":
                           IPcode = node.InnerText;
                            
                            break;
                        case "GWVersion":                        
                            GWVersion = node.InnerText.Trim();
                            
                            break;
                        case "TotalPoints":
                            TotalPoints = node.InnerText.Trim();
                          
                            break;
                        default:
                         
                            TotalPoints = node.InnerText.Trim();
                           
                            break;
                    }
                }
                if (null != xmlNode)
                {
                    if (LoyaltyIDNumber.Length > 0)
                    {
                       
                        //Do batch requests here
                        Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(LoyaltyIDNumber);
                        if (null == member)
                        {    // Log error when member not found
                            this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyIdNumber"].Value);
                        }
                        else
                        {
                            WobCredentials credentials = new WobCredentials("649758045458@developer.gserviceaccount.com","GWprivatekey.p12","American Eagle","2966215529466925520");
                             X509Certificate2 certificate = null;
                            // OAuth

                            try
                            {    certificate = new X509Certificate2(@"E:\AmericanEagle\Processors\DAP\GWprivatekey.p12", "notasecret", X509KeyStorageFlags.Exportable);                               
                            }
                            catch (Exception ex)
                            {
                                string errmsg = ex.Message;
                            }
                            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(credentials.serviceAccountId){ Scopes = new[] { "https://www.googleapis.com/auth/wallet_object.issuer" } }.FromCertificate(certificate));
                            // WalletobjectsService
                            WalletobjectsService woService = new WalletobjectsService(new BaseClientService.Initializer() {HttpClientInitializer = credential, //ApplicationName = "WOBS Sample App"
                                  ApplicationName = "AEREWARDS Wallet Update"   });
                            // LoyaltyclassResource
                            LoyaltyclassResource lcResource = new LoyaltyclassResource(woService);
                            //UPDATYING Google Wallet object
                           // LoyaltyObject loyaltyobj = woService.Loyaltyobject.Get( credentials.IssuerId + "." + IPcode).Execute();
                            LoyaltyObject loyaltyobj = new LoyaltyObject();
                            LoyaltyPoints loyaltyPts = new LoyaltyPoints();
                            LoyaltyPointsBalance balance = new LoyaltyPointsBalance();
                            loyaltyPts.Balance = balance;
                            loyaltyobj.LoyaltyPoints = loyaltyPts;
                            loyaltyobj.LoyaltyPoints.Balance.String = TotalPoints;
                            loyaltyobj.Version = Convert.ToInt64(GWVersion) + 1;
                            loyaltyobj.ClassId = credentials.IssuerId + ".LoyaltyClassAE";
                            loyaltyobj.Id = credentials.IssuerId + "." + IPcode;                           
                           // loyaltyobj.Version = (Convert.ToInt64(loyaltyobj.Version) + 1).ToString(); 
                          //  loyaltyobj.LoyaltyPoints.Balance.String = TotalPoints;
                           // LoyaltyObject lcResponse = woService.Loyaltyobject.Update(loyaltyobj, credentials.IssuerId + "." + IPcode).Execute();
                            try
                            {
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "going to call Google API");
                                LoyaltyObject lcResponse = woService.Loyaltyobject.Patch(loyaltyobj, credentials.IssuerId + "." + IPcode).Execute();
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Done- Google API");
                                IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                                MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];
                                memberDetails.GwObjVer = Convert.ToInt32(loyaltyobj.Version);
                                LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                                ++recordCount;
                                
                            }                       
                            catch (Exception e)
                            {
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error1:" + loyaltyobj.AccountId + "  " + e.Message + " innerexception1 " + e.InnerException);
                                LoyaltyObject loyaltyobj1 = woService.Loyaltyobject.Get(credentials.IssuerId + "." + IPcode).Execute();
                                loyaltyobj1.Version = Convert.ToInt64(loyaltyobj1.Version) + 1;
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get object " + loyaltyobj.AccountId + "-  Google API called for correct GW version- " + loyaltyobj1.Version);
                                loyaltyobj1.LoyaltyPoints.Balance.String = TotalPoints;
                                LoyaltyObject lcResponse1 = woService.Loyaltyobject.Patch(loyaltyobj1, credentials.IssuerId + "." + IPcode).Execute();
                                loyaltyobj.Version = loyaltyobj1.Version;
                                IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                                MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];
                                memberDetails.GwObjVer = Convert.ToInt32(loyaltyobj.Version);
                                LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                                ++recordCount;
                            }
                            //Saving Google Wallet version to Brierley Db
                           
                            
                            //Patching a object
                          //  LoyaltyObject loyaltyObj1 = new LoyaltyObject();
                          //  loyaltyObj1.ClassId = credentials.IssuerId;
                          //  loyaltyObj1.Id = "LoyaltyObjectAE";
                          //  LoyaltyPoints loyaltyPts1 = new LoyaltyPoints();
                          //  LoyaltyPointsBalance balance = new LoyaltyPointsBalance();
                         //   balance.String = "300";
                         //   loyaltyPts1.Balance = balance;
                            //loyaltyObj1.LoyaltyPoints.Balance.String = "300";
                          //  loyaltyObj1.LoyaltyPoints = loyaltyPts1;
                           // loyaltyObj1.Version = loyaltyObj1.Version + 1;
                          //  LoyaltyObject lcResponse2 = woService.Loyaltyobject.Patch(loyaltyObj1, "2966215529466925520.LoyaltyObjectAE").Execute();
                           
                        }             
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message +  " innerexception " + ex.InnerException);
            }
            if (recordCount > 90000)
            {
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Record Count is greater than 90000 - " + recordCount);
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Total Member processed today" + recordCount);
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //         throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        #endregion
    }
}
