using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Configuration;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

using AmericanEagle.SDK.Global;
using Brierley.Clients.AmericanEagle.DataModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Brierley.ClientDevUtilities.LWGateway;
namespace AmericanEagle.SDK.OutputProviders
{


    #region "RewardDefCrossReference"
public class RewardDefCrossReference
    {
      
        public string Auth_Cd { get; set; }
        public string Reward_Def { get; set; }

        public  string error { get; set; }

        public string toFormattedString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(this.Auth_Cd);
            s.Append("|");
            s.Append(this.Reward_Def);
          
            
            if(this.error!=null)
            {
             if(this.error.Length > 0 )
             {
                s.Append("| error : " + this.error);
               
             }
            }
            
            s.AppendLine();
            return s.ToString();
        }
       

       


    }
   #endregion
    
    public class AEOConnectedRewardCrossReference : IDAPOutputProvider
    {

        #region "Variables"
        
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private DateTime processDate = DateTime.Now;
        private string filenamecreatedexeption = String.Empty;
        private string filenamecreatedreward = String.Empty;


        private string path = string.Empty;
        //SERVICE TO ACCESS TO DATABASE
       // ILWDataService service = LWDataServiceUtil.DataServiceInstance(true);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        Dictionary<string, RewardDefCrossReference> RewardFails =new Dictionary<string, RewardDefCrossReference>();
        Dictionary<string, RewardDefCrossReference> Rewardsuccess = new Dictionary<string, RewardDefCrossReference>();

        #endregion


        #region "IMPLEMENTATION"

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            this.RewardFails.Clear();
            this.Rewardsuccess.Clear();
            try
            {
                RewardDefCrossReference classFields = new RewardDefCrossReference();

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    //// Loading XML
                    doc.LoadXml(str);
                }
                
                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("Rewards/Reward");
                RewardDefCrossReference Fields = new RewardDefCrossReference();
                

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    switch (node.Name.ToUpper())
                    {

                        case "AUTH_CD":
                            Fields.Auth_Cd = node.InnerText.Trim();
                            break;

                       case "REWARD_DEF":
                            Fields.Reward_Def = node.InnerText.Trim();
                            break;

                        default:
                            break;
                    }

                }
                if( !String.IsNullOrWhiteSpace(Fields.Auth_Cd) 
                    && !String.IsNullOrWhiteSpace(Fields.Reward_Def)
                     
                    )
                {
                    
                    using (var ldService = _dataUtil.ContentServiceInstance())
                    {
                        //  IList<RewardDef> RewardDefs = this.service.GetAllRewardDefs();
                        IList<RewardDef> RewardDefs = ldService.GetAllRewardDefs();

                        RewardDef reward;
                        var search = from r in RewardDefs
                                     where r.Name.ToUpper() == Fields.Reward_Def.ToUpper() && r.Active == true
                                     select r;
                        int count = search.Count();

                        if (count > 0)
                        {
                            try
                            {
                                reward = search.FirstOrDefault();
                                if (reward != null)
                                {
                                    reward.CertificateTypeCode = Fields.Auth_Cd;
                                    ldService.UpdateRewardDef(reward);
                                }


                            }
                            catch (Exception e)
                            {
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error" + e.Message);
                            }

                        }
                    }
                }
              
            }
            catch(Exception e)
            {
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error" + e.Message);
            }
            
            
           
        }

        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            this.filenamecreatedexeption = String.Empty;
            this.filenamecreatedreward = String.Empty;
        }
        public void Dispose()
        {

            //throw new NotImplementedException();
        }
        public int Shutdown()
        {
            // throw new NotImplementedException();
            return 0;
           

        }

        #endregion



       






    }
}
