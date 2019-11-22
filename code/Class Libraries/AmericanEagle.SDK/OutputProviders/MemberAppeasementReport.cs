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
using Brierley.FrameWork;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

using AmericanEagle.SDK.Global;
using Brierley.Clients.AmericanEagle.DataModel;

namespace  AmericanEagle.SDK.OutputProviders
{
    class MemberAppeasementReport : IDAPOutputProvider
    {

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;
        private string LastMember = String.Empty;
        private string strMember = string.Empty;
        private long lineCount = 0;
        private bool printNewMember = true;
       
        
        private string outputFileName = "MemberAppeasementReport_MMM_YYYY.txt";
       
        
        #region IDAPOutputProvider Members

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
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
                path = path + "\\";
                if ( !Directory.Exists(path) )
                {
                    Directory.CreateDirectory(path);
                }
            }

            if ( null != ConfigurationManager.AppSettings["ProcessDate"] )
            {
                string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                DateTime.TryParse(strProcessDate, out processDate);
            }
            
            this.outputFileName = path + outputFileName.Replace("MMM_YYYY", DateTime.Now.ToString("MMM_yyyy"));
          
      
            
            string[] files = { outputFileName };

            foreach ( string filestr in files )
            {
                StreamWriter sw = new StreamWriter(filestr, false);

                sw.Write(string.Empty);
                sw.Flush();

                sw.Close();
            }
         

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }



        public void ProcessMessageBatch(List<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            string notes = string.Empty;
            string agFirstName = string.Empty;
            string agLastName = string.Empty;
            string mbrtFirstName = string.Empty;
            string mbrLastName = string.Empty;
            string pointName = string.Empty;
            string eventName = string.Empty;
       

            DateTime awardDate = new DateTime();
            double points = 0;

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
                XmlNode xmlNode = doc.SelectSingleNode("Members/Member");
               
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                                    
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NodeName:" + node.Name);

                  
                    switch (node.Name)
                    {
                        case "memberName":
                            mbrtFirstName = node.InnerText;
                            break;

                        case "memberLastName":
                            mbrLastName = node.InnerText;
                            strMember = mbrtFirstName + ',' + mbrLastName;
                              break;

                        case "date":
                            if (! DateTime.TryParse(node.InnerText, out awardDate)) {
                                throw new Exception("Invalid awardpoint date");
                            }
                              break;
                          
                        case "points":
                            if (! double.TryParse(node.InnerText, out points)) {
                                  throw new Exception("Invalid point amount");
                            }
                              break;

                        case "pointTypeName":
                             pointName = node.InnerText;
                              break;

                        case "pointEventName":
                           eventName = node.InnerText;
                              break;


                        case "agentID":
                            notes = node.InnerText;

                            if (notes.StartsWith("AgentID=", StringComparison.OrdinalIgnoreCase) ){
                                int pos = notes.IndexOf("=");
                                StringBuilder number = new StringBuilder();

                                pos++;
                                while ( pos < notes.Length && char.IsDigit(notes[pos]) )
                                {
                                    number.Append(notes[pos]);
                                    pos++;
                                }

                                long agentID = 0;
                                if (long.TryParse( number.ToString(), out agentID)) {

                                    using (var svc = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                                    {
                                        CSAgent agent = svc.GetCSAgentById(agentID);
                                        if (agent != null)
                                        {
                                            agFirstName = agent.FirstName;
                                            agLastName = agent.LastName;
                                        }
                                        else
                                        {
                                            throw new Exception("Agent Id number not found in DB");
                                        }
                                    }
                                }
                                else {
                                    throw new Exception("Invalid Agent Id number");
                                }
                                
                            }
                            else {
                                agFirstName = "No agent specified";
                                agLastName = string.Empty;                                   
                            }
                              break;
   
     
    
                    }


                }

                if (null != xmlNode)
                {

                    if ( (lineCount > 60)) {
                        this.printNewPage();
                        lineCount = 0;
                    }

                    if ( strMember != this.LastMember )
                    {
                        this.printNewMember = true;
                        this.LastMember = strMember;
                    }
                    else {
                        this.printNewMember = false;
                    }


                    if ( this.printNewMember )
                    {
                        lineCount += this.printMember( mbrtFirstName, mbrLastName);
                        this.printNewMember = false;
                    }

                    lineCount +=  this.printRow( awardDate, points, eventName+"/"+pointName, agFirstName +','+ agLastName);
                }

            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        private int printMember( string firstname, string lastname ) {

            StreamWriter sw = new StreamWriter(this.outputFileName, true);

            sw.WriteLine();
            sw.WriteLine();
            sw.WriteLine("Member:"+ firstname+","+lastname);
            sw.WriteLine("Date         Points           Point Type/Event                            Agent");
            sw.WriteLine("---------------------------------------------------------------------------------------------------------------------------");
            sw.Flush();
            sw.Close();

            return 5;

        }

        private int printRow ( DateTime awardDate, double points, string eventName, string agentName ) {
            
            StreamWriter sw = new StreamWriter(this.outputFileName, true);
                    
            sw.Write(awardDate.ToString("ddMMMyyyy").PadRight(11));
            sw.Write(points.ToString("###,###,##0").PadRight(13));
            sw.Write(eventName.PadRight(50));
            sw.Write(agentName.PadRight(55));
            sw.WriteLine();
            sw.Flush();
            sw.Close();

            return 1;
        }


        private void printNewPage ( )
        {

            StreamWriter sw = new StreamWriter(this.outputFileName, true);

            sw.Write( '\f' );
            sw.Flush();
            sw.Close();

        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion


        public void Dispose ( )
        {
            return;
        }

        private string getHeaderline ( DateTime processingDate)
        {
            //12345678901234567890123456789012345678901234567890123456789012345678901234567890
            //         1         2         3         4         5         6         7         8

            StringBuilder tmp = new StringBuilder();

          

            return tmp.ToString();
        }

       
    }


  
    

  
}
