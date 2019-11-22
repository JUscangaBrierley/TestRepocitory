//-----------------------------------------------------------------------
// <copyright file="RewardHistoryGridProvider.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Brierley.AEModules.RewardHistory
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Brierley.WebFrameWork.Controls.Grid;
    
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.ClientDevUtilities.LWGateway;

    #endregion

    /// <summary>
    /// Class definition for custom grid provider for Reward History page
    /// </summary>
    public class RewardHistoryGridProvider : AspGridProviderBase
    {
        #region Private variables
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("RewardHistory");
        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        /// <summary>
        /// MemberReward instance
        /// </summary>
        private IList<MemberReward> memberReward = new List<MemberReward>();

        /// <summary>
        /// RewardList to set MemberRewards
        /// </summary>
        private IList<MemberReward> rewardList = null;
        #endregion

        #region Public Member

        #region Grid Properties
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Method to get Grid Name.
        /// </summary>
        /// <returns>empty</returns>
        protected override string GetGridName()
        {
            return string.Empty;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        /// <summary>
        /// Method to set whether grid is selectable or not.
        /// </summary>
        /// <returns>Grid Row Selectable or not</returns>
        public override bool IsGridRowSelectable()
        {
            return false;
        }

        /// <summary>
        /// Method to set whether grid is editable or not.
        /// </summary>
        /// <returns>Grid Row Editable or not</returns>
        public override bool IsGridEditable()
        {
            return false;
        }

        /// <summary>
        /// Method to set which type of buttons will be display.
        /// </summary>
        /// <param name="commandName">string commandName</param>
        /// <returns>Button visible or not</returns>
        public override bool IsButtonVisible(string commandName)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if (commandName == AspDynamicGrid.EDIT_ROW_COMMAND ||
                commandName == AspDynamicGrid.DELETE_ROW_COMMAND ||
                commandName == AspDynamicGrid.ADDNEW_COMMAND)
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                return false;
            }
            else
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                return true;
            }
        }

        /// <summary>
        /// Returns Action header text of grid
        /// </summary>
        /// <returns>Get Grid Actions Header Text</returns>
        public override string GetGridActionsHeaderText()
        {
            return string.Empty;
        }

        /// <summary>
        /// Method for columns specification
        /// </summary>
        /// <returns>Column Specification</returns>
        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            ////Array of grid columns, AEO-2076 + 1 COLUMN
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[10];


            //// Create new object for DynamicGridColumnSpec
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "Certificate";
            c.DisplayText = "Certificate Number";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            c.MaxLength = 25;
            
            columns[0] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "Description";
            c.DisplayText = "Description";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[1] = c;

            // AEO-806 BEGIN
            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "Date";
            c.DisplayText = "Issue Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[2] = c;

            //AEO-2076
            c = new DynamicGridColumnSpec();
            c.Name = "Fulfillmentdate";
            c.DisplayText = "Start Date";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[3] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "ExpirationDate";
            c.DisplayText = "Expiration </br> Date";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[4] = c;


            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "Status";
            c.DisplayText = "Status";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[5] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "RedemptionDate";
            c.DisplayText = "Redemption </br> Date";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[6] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "StoreName";
            c.DisplayText = "Store Info";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            c.IsVisible = true;
            c.IsKey = true;
            columns[7] = c;


            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "TxnNumber";
            c.DisplayText = "Trans#/Order# </br> for Redemption";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            c.IsVisible = true;
            c.IsKey = true;
            columns[8] = c;
            // AEO-806 END

            //// Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "Csr";
            c.DisplayText = "CSR";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = Type.GetType("System.String");
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[9] = c;

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return columns;
        }
        #endregion

        #region Data Source
        /// <summary>
        /// Set the list of search parameter
        /// </summary>
        /// <param name="parmName">string parmName</param>
        /// <param name="parmValue">object parmValue</param>
        public override void SetSearchParm(string parmName, object parmValue)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if (parmName == "RewardList")
            {
                this.rewardList = (IList<MemberReward>)parmValue;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Method to load data for grid.
        /// </summary>
        public override void LoadGridData()
        {
            this.LoadMembersReward();
        }

        /// <summary>
        ///  Abstract method for saving of grid data.
        /// </summary>
        /// <param name="columns">DynamicGridColumnSpec[] columns</param>
        /// <param name="update">Is grid updatable or not</param>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
        }

        /// <summary>
        /// Returns count of rows
        /// </summary>
        /// <returns>Number of rows</returns>
        public override int GetNumberOfRows()
        {
            return this.memberReward.Count;
        }

        /// <summary>
        /// Get value for particular column
        /// </summary>
        /// <param name="rowIndex">integer rowIndex</param>
        /// <param name="column">DynamicGridColumnSpec column</param>
        /// <returns>Value of column</returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            object value = null;

            try
            {
                ////Stores single member reward information
                MemberReward mbrReward = this.memberReward[rowIndex] as MemberReward;
                //Product product = _ContentService.GetProduct(mbrReward.ProductId);
                switch (column.Name)
                {
                    case "Certificate":
                        // AEO-580 BEGIN
                        // AEO-1589 Begin
                        if (mbrReward.CertificateNmbr != null)
                        {
                            value = (mbrReward.CertificateNmbr == null ? "" : mbrReward.CertificateNmbr) + "-" +
                                    (mbrReward.OfferCode == null ? "" : mbrReward.OfferCode);
                        }
                        // AEO-1589 End
                        // AEO-580 END

                        break;
                    case "Csr":
                        if (null != mbrReward)
                        {
                            value = mbrReward.ChangedBy != null ? mbrReward.ChangedBy : string.Empty;
                        }

                        break;
                    case "Date":
                        if (null != mbrReward && null != mbrReward.DateIssued)
                        {
                            DateTime tempDate = Convert.ToDateTime(mbrReward.DateIssued);
                            value = tempDate != null ? tempDate.ToShortDateString() : null;
                        }

                        break;
                    case "Fulfillmentdate":
                        if (null != mbrReward)
                        {
                            if (null != mbrReward.FulfillmentDate)
                            {
                                DateTime tmpdate = Convert.ToDateTime(mbrReward.FulfillmentDate);
                                value = value = tmpdate != null ? tmpdate.ToShortDateString() : null;

                            }
                        }
                        break;

                    case "Description":
                        using (IContentService contentService = _dataUtil.ContentServiceInstance())
                        {
                            RewardDef rewardDef = contentService.GetRewardDef(mbrReward.RewardDefId);
                            if (null != rewardDef)
                            {
                                if (rewardDef.Name.Contains("Bra"))
                                {
                                    value = "Bra Reward";
                                }
                                else
                                {
                                    value = rewardDef.Name != null ? rewardDef.Name : string.Empty;
                                }

                                if (rewardDef.Name.Equals("Free Bra - Appeasement") || rewardDef.Name.Equals("B5G1 Bra Appeasement"))
                                {
                                    value = "Bra Appeasement";
                                }
                            }
                        }
                        break;
                    // AEO-806 BEGIN
                    case "ExpirationDate":
                        value = mbrReward.Expiration != null ? ((DateTime)mbrReward.Expiration).ToString("MM/dd/yyyy") : string.Empty;

                        break;

                    case "Status":
                        if (null != mbrReward)
                        {
                            value = "Issued";
                            if (mbrReward.RedemptionDate != null)
                            {
                                value = "Redeemed";
                            }
                        }

                        break;

                    case "RedemptionDate":
                        if (mbrReward.RedemptionDate != null)
                        {
                            value = ((DateTime)mbrReward.RedemptionDate).ToString("MM/dd/yyyy");
                        }


                        break;
                    case "StoreName":
                        value = string.Empty;

                        if (null != mbrReward)
                        {
                            using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                            using (IContentService contentService = _dataUtil.ContentServiceInstance())
                            {
                                // first we retirve the member
                                Member lmember = dataService.LoadMemberFromIPCode(mbrReward.MemberId);

                                if (lmember == null)
                                {
                                    return value;
                                }

                                long[] cards = lmember.GetLoyaltyCardIds();
                                int index = 0;
                                bool found = false;


                                while (cards != null && index < cards.Length && !found)
                                {

                                    LWCriterion criteria = new LWCriterion("TxnRewardRedeem");
                                    criteria.Add(LWCriterion.OperatorType.AND, "CertificateCode", mbrReward.CertificateNmbr, LWCriterion.Predicate.Eq);
                                    IList<IClientDataObject> tmp = dataService.GetAttributeSetObjects(null, "TxnRewardRedeem", criteria, new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 }, true, false);


                                    if (tmp != null && tmp.Count > 0)
                                    {



                                        foreach (IClientDataObject tmpobj in tmp)
                                        {

                                            TxnRewardRedeem txn = (TxnRewardRedeem)tmp[0];
                                            if (txn.VcKey != cards[index])
                                            {
                                                continue;
                                            }

                                            found = true;

                                            LWCriterion criteriaTxnHead = new LWCriterion("TxnHeader");
                                            criteriaTxnHead.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txn.TxnHeaderId, LWCriterion.Predicate.Eq);

                                            IList<IClientDataObject> tmp2 = dataService.GetAttributeSetObjects(null, "TxnHeader", criteriaTxnHead, new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 }, true, false);
                                            if (tmp2 != null && tmp2.Count > 0)
                                            {

                                                TxnHeader txnHead = (TxnHeader)tmp2[0];

                                                StoreDef varStore = contentService.GetStoreDef(txnHead.TxnStoreId);

                                                if (varStore != null)
                                                {
                                                    value = varStore.StoreNumber + "-" + varStore.StoreName;
                                                }

                                            }

                                            if (found)
                                            {
                                                break;
                                            }

                                        }



                                    }

                                    index++;

                                    if (found)
                                    {
                                        break;
                                    }

                                }
                            }

                        }

                        break;

                    case "TxnNumber":
                        if (null != mbrReward)
                        {
                            using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                            {
                                // first we retirve the member
                                Member lmember = dataService.LoadMemberFromIPCode(mbrReward.MemberId);

                                if (lmember == null)
                                {
                                    return value;
                                }

                                long[] cards = lmember.GetLoyaltyCardIds();
                                int index = 0;
                                bool found = false;

                                while (cards != null && index < cards.Length && !found)
                                {

                                    LWCriterion criteria = new LWCriterion("TxnRewardRedeem");
                                    criteria.Add(LWCriterion.OperatorType.AND, "CertificateCode", mbrReward.CertificateNmbr, LWCriterion.Predicate.Eq);

                                    IList<IClientDataObject> tmp = dataService.GetAttributeSetObjects(null, "TxnRewardRedeem", criteria, null, true, false);

                                    if (tmp != null && tmp.Count > 0)
                                    {

                                        foreach (IClientDataObject tmpobj in tmp)
                                        {

                                            TxnRewardRedeem txn = (TxnRewardRedeem)tmp[0];
                                            if (txn.VcKey != cards[index])
                                            {
                                                continue;
                                            }

                                            found = true;
                                            LWCriterion criteriaTxnHead = new LWCriterion("TxnHeader");
                                            criteriaTxnHead.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txn.TxnHeaderId, LWCriterion.Predicate.Eq);

                                            IList<IClientDataObject> tmp2 = dataService.GetAttributeSetObjects(null, "TxnHeader", criteriaTxnHead, null, true, false);
                                            if (tmp2 != null && tmp2.Count > 0)
                                            {

                                                TxnHeader txnHead = (TxnHeader)tmp2[0];

                                                value = txnHead.TxnNumber;

                                            }

                                            if (found)
                                            {
                                                break;
                                            }

                                        }

                                    }

                                    index++;

                                }

                            }
                        }

                        break;
                    // AEO-806 END
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,MethodBase.GetCurrentMethod().Name,ex.Message);
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");

            return value;
        }

        /// <summary>
        /// Returns default for particular column
        /// </summary>
        /// <param name="column">DynamicGridColumnSpec column</param>
        /// <returns>Default value of column</returns>
        public override object GetDefaultValues(DynamicGridColumnSpec column)
        {
            return null;
        }

        #endregion
        #endregion

        #region Protected Members
        #region Helpers
        /// <summary>
        /// Get member rewards
        /// </summary>
        protected void LoadMembersReward()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if (this.rewardList != null && this.rewardList.Count > 0)
            {
                this.memberReward.Clear();
                this.memberReward = (IList<MemberReward>)this.rewardList;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        #endregion
        #endregion

        #region Private Member
        #endregion
    }
}
