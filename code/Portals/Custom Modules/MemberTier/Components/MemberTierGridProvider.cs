//-----------------------------------------------------------------------
// <copyright file="MemberTierGridProvider.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Brierley.AEModules.MemberTier
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
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    using AmericanEagle.SDK.Global;
    using Brierley.ClientDevUtilities.LWGateway;

    #endregion

    /// <summary>
    /// Class definition for custom grid provider for Reward History page
    /// </summary>
    public class MemberTierGridProvider : AspGridProviderBase
    {
        #region Private variables
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("MemberTier");

        /// <summary>
        /// ILWDataService instance
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// MemberReward instance
        /// </summary>
        private IList<MemberTier> memberTier = new List<MemberTier>();

        /// <summary>
        /// RewardList to set MemberRewards
        /// </summary>
        private IList<MemberTier> tierList = null;

        ///// <summary>
        ///// RewardStatus instance
        ///// </summary>
        //private RewardStatus rewardStatus = RewardStatus.Awarded;

        /// <summary>
        /// Store the week setting from the client configuration table.
        /// </summary>
        //private ClientConfiguration configWeek = null;
        #endregion

        /// <summary>
        /// Initializes a new instance of the MemberTierGridProvider class
        /// </summary>
        public MemberTierGridProvider()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

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
            ////Array of grid columns
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[5];
            
            //// Create new object for DynamicGridColumnSpec
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "TierStartDate";
            c.DisplayText = "Tier Start Date";
            c.DataType = typeof(System.String); 
            c.IsEditable = false;
            c.IsSortable = false;
            columns[0] = c;

            //// Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "ValidThrough";
            c.DisplayText = "Valid Through";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[1] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Tier";
            c.DisplayText = "Tier";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[2] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "ReasonCode";
            c.DisplayText = "Reason Code";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[3] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "NetSpend";
            c.DisplayText = "Net Spend";
            c.DataType = typeof(System.String);
            c.IsEditable = false;
            c.IsSortable = false;
            columns[4] = c;

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
            if (parmName == "MemberTiers")
            {
                this.tierList = (IList<MemberTier>)parmValue;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Method to load data for grid.
        /// </summary>
        public override void LoadGridData()
        {
            this.LoadMemberTiers();
        }

        /// <summary>
        ///  Abstract method for saving of grid data.
        /// </summary>
        /// <param name="columns">DynamicGridColumnSpec[] columns</param>
        /// <param name="update">Is grid updatable or not</param>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        {
        }

        /// <summary>
        /// Returns count of rows
        /// </summary>
        /// <returns>Number of rows</returns>
        public override int GetNumberOfRows()
        {
            return this.memberTier.Count;
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
            ////Stores single member reward information
            MemberTier mbrTier = this.memberTier[rowIndex] as MemberTier;
            object value = null;
            switch (column.Name)
            {
                case "TierStartDate":
                    if ( mbrTier.FromDate != null ) {
                        value = mbrTier.FromDate.ToShortDateString();
                    }
                    else
	                {
                        value = null;
	                }
                    break;
                case "ValidThrough":
                    if(mbrTier.ToDate != null)
                    {
                        value = mbrTier.ToDate.ToShortDateString();
                    }
                    else
                    {
                        value = null;
                    }
                    break;
                case "Tier":
                    if(null != mbrTier)
                    {
                        using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            value = dataService.GetTierDef(mbrTier.TierDefId).Name;
                        }
                    }
                    break;
                case "ReasonCode":
                    if(null != mbrTier.Description)
                    {
                        value = mbrTier.Description.Trim();
                    }

                    break;
                case "NetSpend":
                    LWCriterion criteriaMbrNetSpnd = new LWCriterion("MemberNetSpend");
                    criteriaMbrNetSpnd.Add(LWCriterion.OperatorType.AND, "MemberTierID", mbrTier.Id, LWCriterion.Predicate.Eq);
                    criteriaMbrNetSpnd.Add(LWCriterion.OperatorType.AND, "IpCode", mbrTier.MemberId, LWCriterion.Predicate.Eq);
                    using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        IList<IClientDataObject> tmp2 = dataService.GetAttributeSetObjects(null, "MemberNetSpend", criteriaMbrNetSpnd, new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 }, true, false);

                        if (tmp2 != null && tmp2.Count > 0)
                        {
                            value = ((MemberNetSpend)tmp2[0]).NetSpend.ToString("C");
                        }
                    }
                    break;
                default:
                    break;
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
        protected void LoadMemberTiers()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if (this.tierList != null && this.tierList.Count > 0)
            {
                this.memberTier.Clear();
                this.memberTier = (IList<MemberTier>)this.tierList;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        #endregion
        #endregion

        #region Private Member
        #endregion
    }
}
