using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using AmericanEagle.SDK.Global;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
using Brierley.Clients.AmericanEagle.DataModel;
using System.IO;
using System.Configuration;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{
    public class TierNotification : IDAPOutputProvider
    {

        private string mobileFileName = "AEORW_SM_TierUpg_YYYYMMDDHHMMSS.txt";
        private string emailFileName = "AEORW_EM_TierUpg_YYYYMMDDHHMMSS.txt";

        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            string fileDirectory = ConfigurationManager.AppSettings["FilePath"];

            if (null == fileDirectory)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }

            fileDirectory = fileDirectory + "\\";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            mobileFileName = Path.Combine(fileDirectory, mobileFileName.Replace("YYYYMMDDHHMMSS", DateTime.Now.ToString("yyyyMMddHHmmss")));
            emailFileName = Path.Combine(fileDirectory,emailFileName.Replace("YYYYMMDDHHMMSS", DateTime.Now.ToString("yyyyMMddHHmmss")));

            // we allways create the  2 possible output files
            StreamWriter sw = new StreamWriter(mobileFileName, false);
            sw.Close();


            sw = new StreamWriter(emailFileName, false);
            sw.Close();

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                using (var lwSevice = _dataUtil.LoyaltyDataServiceInstance())
                {
                    XmlDocument doc = new XmlDocument();
                    foreach (string str in messageBatch)
                    {
                        // Loding XML
                        doc.LoadXml(str);
                    }

                    // Get XML Node
                    XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global");

                    //We create the headers if are not created
                    CreateHeadersIfNotCreated(xmlNode);

                    Member member = lwSevice.LoadMemberFromLoyaltyID(xmlNode.Attributes["LOYALTY_NUMBER"].Value);
                    IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");

                    MemberDetails details = (loDetails == null || loDetails.Count == 0 ? null : loDetails[0]) as MemberDetails;

                    if (details.PendingCellVerification == 0 && details.MobilePhone != null && details.MobilePhone.Trim().Length > 0) // AEO-367 Begin & End
                    {
                        StreamWriter sw = new StreamWriter(mobileFileName, true);
                        for (int i = 0; i < xmlNode.Attributes.Count; i++)
                        {
                            // AEO-323 Begin
                            if (i == 0)
                            {
                                sw.Write("2");
                                sw.Write("|");
                            }
                            else
                            {
                                // AEO 251 Begin
                                sw.Write(xmlNode.Attributes[i].Value);
                                if (i != xmlNode.Attributes.Count - 1)
                                {
                                    sw.Write("|");
                                }

                                // AEO-251 End

                            }
                            // AEO-323 End
                        }
                        sw.WriteLine();
                        sw.Flush();
                        sw.Close();
                    }

                    if (details.PendingEmailVerification == 0 && details.EmailAddress != null && details.EmailAddress.Trim().Length > 0) // AEO-367 Begin & End)
                    {
                        StreamWriter sw = new StreamWriter(emailFileName, true);
                        for (int i = 0; i < xmlNode.Attributes.Count; i++)
                        {
                            // AEO-323 Begin
                            if (i == 0)
                            {
                                sw.Write("1");
                                sw.Write("|");
                            }
                            else
                            {
                                // AEO 251 Begin
                                sw.Write(xmlNode.Attributes[i].Value);
                                if (i != xmlNode.Attributes.Count - 1)
                                {
                                    sw.Write("|");
                                }

                                // AEO-251 End
                            }
                            // AEO-323 End
                        }
                        sw.WriteLine();
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception ex){
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw new Exception(ex.Message, new Exception("System Error"));
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "END");
        }

        private void CreateHeadersIfNotCreated(XmlNode xmlInfo)
        {
            FileInfo mobileFile = new FileInfo(mobileFileName);
            FileInfo emailFile = new FileInfo(emailFileName);

            if (mobileFile.Length == 0)
            {
                StreamWriter sw = new StreamWriter(mobileFileName, true);
                for (int i = 0; i < xmlInfo.Attributes.Count; i++)
                {
                    sw.Write(xmlInfo.Attributes[i].Name);
                    sw.Write("|");
                }
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }

            if (emailFile.Length == 0)
            {
                StreamWriter sw = new StreamWriter(emailFileName, true);
                for (int i = 0; i < xmlInfo.Attributes.Count; i++)
                {
                    
                    // AEO 251 Begin
                    sw.Write(xmlInfo.Attributes[i].Name);
                    if ( i != xmlInfo.Attributes.Count - 1 )
                    {
                        sw.Write("|");
                    }

                    // AEO-251 End
                }
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }

        public int Shutdown()
        {
            return 0;
        }

        public void Dispose()
        {
        }
    }
}
