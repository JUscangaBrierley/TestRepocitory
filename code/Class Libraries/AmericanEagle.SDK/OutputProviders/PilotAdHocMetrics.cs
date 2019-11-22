// ----------------------------------------------------------------------------------
// <copyright file="TierUpgrade.cs" company="Brierley and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Configuration;
using System.IO;

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

namespace AmericanEagle.SDK.OutputProviders
{
    public class PilotAdHocMetrics : IDAPOutputProvider
    {
       

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        /// 

        private string fileName = "AE_PILOT_METRICS_AD_HOC_MMM_dd_YYYY.CSV";
        private string path = string.Empty;
        private string lastSection = string.Empty;


        public void Initialize (NameValueCollection globals, NameValueCollection args, long jobId, 
            DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil )
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if ( null == ConfigurationManager.AppSettings["FilePath"] )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }
            else
            {
                path = ConfigurationManager.AppSettings["FilePath"];
                if ( !Directory.Exists(path) )
                {
                    Directory.CreateDirectory(path);
                }
            }

            fileName = path + @"\" + fileName.Replace("MMM_dd_YYYY", DateTime.Today.ToString("MMM_dd_yyyy"));


            StreamWriter sw = new StreamWriter(fileName, false);
            sw.Close();
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                // Tracing for starting of method
                this.logger.Trace(this.className, methodName, "Starts");
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }
                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("Metrics/Metric");
                // check for valid xml data
                if ( null != xmlNode )
                {

                    String tableNumber = xmlNode.ChildNodes[0].InnerText;

                    int howManyColumns = xmlNode.ChildNodes.Count;
                    int i = 1;
                    foreach ( XmlNode loTmp in xmlNode.ChildNodes ) {

                        if ( i != 1 )
                        {

                            StreamWriter sw = new StreamWriter(fileName, true);

                            Decimal ldTmp = decimal.Zero;
                            if (! decimal.TryParse(loTmp.InnerText, out ldTmp) ) {
                                ldTmp = decimal.Zero;
                            }

                            if ( i == 2 )
                            {
                                sw.Write(loTmp.InnerText);
                            }
                            else {
                                if ( tableNumber == "1" )
                                {
                                    sw.Write(ldTmp.ToString("$#,###,###,##0.00"));
                                }
                                else
                                {
                                    if(i == 25)
                                        sw.Write(ldTmp.ToString("#,###,###,##0.000"));
                                    else
                                        sw.Write(ldTmp.ToString("###,###,##,##0"));
                                }

                            }

                           
                            
                            if ( howManyColumns != i )
                            {
                                sw.Write(",");
                            }
                            else {
                                sw.WriteLine();
                            }

                            sw.Close();
                        }
                        else {

                            if ( loTmp.InnerText != this.lastSection ) {
                                this.lastSection = loTmp.InnerText;
                                this.printHeader();
                            }
                        }

                        i++;
                        
                    }
                } 

                // Logging for ending of method
                this.logger.Trace(this.className, methodName, "Ends");
            }
            catch (Exception ex)
            {
                // Logging for exception
                this.logger.Error(this.className, methodName, ex.Message);
            }
        }

       private void  printHeader ( ) {

           StreamWriter sw = new StreamWriter(fileName, true);

           sw.WriteLine("Date,Online,107 GREENWOOD MALL,140 CHERRYVALE MALL,180 MILLCREEK MALL,209 RIVER HILLS MALL" +
               ",225 GATEWAY CENTER,228 VALLEY VIEW MALL,260 RIVER VALLEY MALL,269 GOVERNORS SQUARE	" +
               ",282 VALDOSTA COLONIAL MALL,291 OAKWOOD MALL,300 FINDLAY VILLAGE MALL,312 WEST PARK MALL	" +
                ",366 BOISE TOWNE SQUARE,418 THE GALLERIA,482 HAYWOOD MALL,728 SOUTHLAKE MALL- WESTFIELD	" +
                ",825 SOUTHPARK MALL,880 THE CROSSROADS,882 HONEY CREEK MALL,2082 THE SHOPS AT CENTERRA,TOTAL, PILOT STORE AVERAGE, BALANCE OF CHAIN");
           sw.Close();
                   
       }

       public int Shutdown()
        {
            return 0;
            //        throw new System.NotImplementedException();
        }
     
       
        public void Dispose()
        {
            
        }
       
    }
}
