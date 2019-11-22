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
    class BraResetEmployee : IDAPOutputProvider
    {

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private string strLoyaltyNumber = string.Empty;
        private string strOwnerId = string.Empty;
        private string strOwnerType = string.Empty;
        private string strParentTransactionId = string.Empty;
        private string strPointAwardDate = string.Empty;
        private string strPoints = string.Empty;
        private string strTransactionType = string.Empty;
        private string strRowKey = string.Empty;
        private string strTransactionDate = string.Empty;
        private string strPointTransactionId = string.Empty;
        private const string strBraPoints_Type = "Bra Points";
        private const string strBraEmployeeReturn_Event = "Bra Employee Return";
        private const string strBraEmployee_Event = "Bra Employee";
        private const string strBraReturn_Event = "Bra Return";
        private const string strBraPurchase15_Event = "Bra Purchase - 15";
        private const string strBraPurchase1_Event = "Bra Purchase - 1";

        private long _BraReturn_Event = 0;
        private long _BraPurchase15_Event = 0;
        private long _BraPurchase1_Event = 0;

        #region IDAPOutputProvider Members

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
        }

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            DateTime processDate = DateTime.MaxValue;
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

                if (null != ConfigurationManager.AppSettings["ResetEmployeeProcessDate"])
                {
                    string strProcessDate = ConfigurationManager.AppSettings["ResetEmployeeProcessDate"];
                    DateTime.TryParse(strProcessDate, out processDate);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("BraPoints/BraPoint");
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "LoyaltyNumber":
                            strLoyaltyNumber = node.InnerText;
                            break;
                        case "OwnerId":
                            strOwnerId = node.InnerText;
                            break;
                        case "OwnerType":
                            strOwnerType = node.InnerText;
                            break;
                        case "ParentTransactionId":
                            strParentTransactionId = node.InnerText;
                            break;
                        case "PointAwardDate":
                            strPointAwardDate = node.InnerText;
                            break;
                        case "Points":
                            strPoints = node.InnerText;
                            break;
                        case "TransactionType":
                            strTransactionType = node.InnerText;
                            break;
                        case "RowKey":
                            strRowKey = node.InnerText;
                            break;
                        case "TransactionDate":
                            strTransactionDate = node.InnerText;
                            break;
                        case "PointTransactionId":
                            strPointTransactionId = node.InnerText;
                            break;
                        default:
                            strLoyaltyNumber = node.InnerText;
                            break;
                    }
                }

                if (null != xmlNode)
                {
                    if (strLoyaltyNumber.Length > 0)
                    {
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            Member member = lwService.LoadMemberFromLoyaltyID(strLoyaltyNumber);
                            if (null == member)
                            {
                                // Log error when member not found
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyNumber"].Value);
                            }
                            else
                            {
                                long txnId = 0;
                                long.TryParse(strPointTransactionId, out txnId);
                                long pointTypeId = 0;

                                IList<PointEvent> pointEvents = lwService.GetAllPointEvents();
                                IList<PointType> pointTypes = lwService.GetAllPointTypes();

                                foreach (PointEvent e in pointEvents)
                                {
                                    if (e.Name == strBraPurchase15_Event)
                                    {
                                        _BraPurchase15_Event = e.ID;
                                    }
                                    if (e.Name == strBraPurchase1_Event)
                                    {
                                        _BraPurchase1_Event = e.ID;
                                    }
                                    if (e.Name == strBraReturn_Event)
                                    {
                                        _BraReturn_Event = e.ID;
                                    }
                                }

                                foreach (PointType e in pointTypes)
                                {
                                    if (e.Name == strBraPoints_Type)
                                    {
                                        pointTypeId = e.ID;
                                    }
                                }

                                //Now go through the bra rewards and see if there are any that are ready to be sent.
                                PointTransaction pointTxn = lwService.GetPointTransaction(txnId);


                                PointTransaction newPointTxn = new PointTransaction();

                                newPointTxn.ChangedBy = newPointTxn.ChangedBy;
                                newPointTxn.ExpirationDate = pointTxn.ExpirationDate;
                                newPointTxn.Notes = "Bra Employee Reset";
                                newPointTxn.OwnerId = pointTxn.OwnerId;
                                newPointTxn.OwnerType = pointTxn.OwnerType;
                                newPointTxn.ParentTransactionId = pointTxn.ParentTransactionId;
                                newPointTxn.PointAwardDate = pointTxn.PointAwardDate;

                                //Set the PointEvent based on the date we are processing for.  If the processDate is not set in the web.config then we will assume it is run for the 1st
                                //bra fulfillment after the 1st of the quarter which is the 15th.  if not then check the day that is in the process date and that is the point event we will use.
                                if (pointTxn.TransactionType == PointBankTransactionType.Credit)
                                {
                                    if (processDate != DateTime.MaxValue)
                                    {
                                        if (processDate.Day == 1)
                                        {
                                            newPointTxn.PointEventId = _BraPurchase1_Event;
                                        }
                                        else
                                        {
                                            newPointTxn.PointEventId = _BraPurchase15_Event;
                                        }
                                    }
                                    else
                                    {
                                        newPointTxn.PointEventId = _BraPurchase15_Event;
                                    }
                                }
                                else
                                {
                                    newPointTxn.PointEventId = _BraReturn_Event;
                                }

                                newPointTxn.Points = pointTxn.Points;
                                newPointTxn.PointTypeId = pointTypeId;
                                newPointTxn.RowKey = pointTxn.RowKey;
                                newPointTxn.TransactionDate = pointTxn.TransactionDate;
                                newPointTxn.TransactionType = pointTxn.TransactionType;
                                newPointTxn.VcKey = pointTxn.VcKey;

                                lwService.CreatePointTransaction(newPointTxn);

                                //Update the current Employee Txn and set the expiration date to today.
                                pointTxn.ExpirationDate = DateTime.Today;
                                lwService.UpdatePointTransaction(pointTxn);

                            }
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
            //       throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
