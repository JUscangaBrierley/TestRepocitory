namespace Brierley.AEModules.Redemptions
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Reflection;


    using Brierley.WebFrameWork.Controls.Grid;

    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    using AmericanEagle.SDK.Global;
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.ClientDevUtilities.LWGateway;

    #endregion
    public class RewardRedemption
    {
        public string RedemptionCode { get; set; }
        public string Description { get; set; }
        // AEO-580 Begin
        public string RedemptionDate { get; set; }
        public string TransactionDate { get; set; }
      
        // AEO-580 END
    }
    public class RedemptionsGridProvider : AspGridProviderBase          
    {

        #region Private variables
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("Redemptions");

        /// <summary>
        /// ILWDataService instance
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// RewardRedemption instance
        /// </summary>
        private IList<RewardRedemption> redemptions = new List<RewardRedemption>();

        /// <summary>
        /// RedemptionList to set MemberRedemptions
        /// </summary>
        private IList<RewardRedemption> redemptionList = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the RedemptionsGridProvider class
        /// </summary>
        public RedemptionsGridProvider()
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
          return  string.Empty;
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
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[4];

            //// Create new object for DynamicGridColumnSpec
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "RedemptionCode";
            c.DisplayText = "Redemption Code";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = Type.GetType( "System.String");
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[0] = c;

            //// Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "Description";
            c.DisplayText = "Description";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[1] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "RedemptionDate";
            c.DisplayText = "Redemption Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[2] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "TransactionDate";
            c.DisplayText = "Transaction Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[3] = c;

          

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
            if (parmName == "RedemptionList")
            {
                this.redemptionList = (IList<RewardRedemption>)parmValue;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Method to load data for grid.
        /// </summary>
        public override void LoadGridData()
        {
            this.LoadMembersRedemptions();
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

            return this.redemptionList.Count;
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
            RewardRedemption rewardRedemption = this.redemptions[rowIndex] as RewardRedemption;

            object value = null;
            switch (column.Name)
            {
                case "RedemptionCode":
                    value = rewardRedemption.RedemptionCode;
                    break;
                case "Description":
                    value = rewardRedemption.Description;
                    break;
                
                case "TransactionDate":
                    value = rewardRedemption.TransactionDate;
                    break;
                case "RedemptionDate":
                    value = rewardRedemption.RedemptionDate;
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
        /// Get member redemptions
        /// </summary>
        protected void LoadMembersRedemptions()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if (this.redemptionList != null && this.redemptionList.Count > 0)
            {
                this.redemptions.Clear();
                this.redemptions = (IList<RewardRedemption>)this.redemptionList;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        #endregion
        #endregion

        #region Private Member
        #endregion
    }
}