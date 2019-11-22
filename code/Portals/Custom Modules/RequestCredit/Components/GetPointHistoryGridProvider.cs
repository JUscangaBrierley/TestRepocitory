namespace Brierley.AEModules.RequestCredit
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    //using Brierley.DNNModules.PortalModuleSDK.Controls.Grid;
    using Brierley.WebFrameWork.Controls.Grid;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ

    using Brierley.FrameWork.Common.Logging;
    #endregion

    #region Class definition for custom grid provider for member lookup page
    /// <summary>
    /// Class defination for custom grid provider for member lookup page
    /// </summary>
    public class GetPointHistoryGridProvider : AspGridProviderBase
    {
        #region Variables

        private static LWLogger logger = LWLoggerManager.GetLogger("GetPointHistory");

        /// <summary>
        /// Holds member receipts
        /// </summary>
        private List<MemberReceipts> memberReceipts = new List<MemberReceipts>();
        private List<MemberReceipts> lstMemberReceipts = new List<MemberReceipts>();
        #endregion

        #region Grid Properties

        /// <summary>
        /// Set whether the particular grid row is selectable or not
        /// </summary>
        /// <returns>false: dont want to make grid selectable</returns>
        public override bool IsGridRowSelectable()
        {
            return false;
        }

        /// <summary>
        /// Load the Grid Data
        /// </summary>
        public override void LoadGridData()
        {
            if (null != this.memberReceipts && this.memberReceipts.Count > 0)
            {
                this.lstMemberReceipts = this.memberReceipts;
            }
        }

        /// <summary>
        /// Returns value for perticular column
        /// </summary>
        /// <param name="column">culumn: grid column</param>
        /// <param name="dataValue">object: cell value</param>
        /// <param name="rowIndex">Grid's selected row number</param>
        /// <returns>string: cell value to display</returns>
        public override string GetValueToDisplay(DynamicGridColumnSpec column, object dataValue, int rowIndex = 0)
        {
            return dataValue.ToString();
        }

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
        /// Set whether the grid is editable or not
        /// </summary>
        /// <returns>false:Dont want to make grid editable</returns>
        public override bool IsGridEditable()
        {
            return false;
        }

        /// <summary>
        /// Set what action buttons are visible or not. By default all button are visible
        /// </summary>
        /// <param name="commandName">Required to capture the command for further processing</param>
        /// <returns>bool: dont visible</returns>
        public override bool IsButtonVisible(string commandName)
        {
            return false;
        }

        /// <summary>
        /// Returns the columns name needed to show in grid
        /// </summary>
        /// <returns>Returns arry of columns</returns>
        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[7];

            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "TxnNumber";
            c.DisplayText = "Trans/Order #";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsKey = false;
            c.IsEditable = false;
            c.IsVisible = true;
            columns[0] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "TxnDate";
            c.DisplayText = "Trans Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[1] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "TenderAmount";
            c.DisplayText = "Total Amount";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[2] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "StoreNumber";
            c.DisplayText = "Store #";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[3] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Register";
            c.DisplayText = "Register #";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[4] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "CreateDate";
            c.DisplayText = "Get Points Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[5] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "StatusCode";
            c.DisplayText = "Status";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[6] = c;
            return columns;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //code commented since there are no suitable methods to overide
        /// <summary>
        /// Returns row editor form location
        /// </summary>
        /// <returns>Editor form location</returns>
        //public override string GetRowEditorFormLocation()
        //{
        //    return string.Empty;
        //}

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        #endregion

        #region Data Source

        /// <summary>
        /// Sets search param
        /// </summary>
        /// <param name="parmName">string: Param name</param>
        /// <param name="parmValue">Object: Param value</param>
        public override void SetSearchParm(string parmName, object parmValue)
        {
            this.memberReceipts = (List<MemberReceipts>)parmValue;
        }

        
        /// <summary>
        /// Save grid data
        /// </summary>
        /// <param name="columns">arry of grid columns</param>
        /// <param name="update">boolean: update</param>
        ///  // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
        }

        /// <summary>
        /// Returns number of rows 
        /// </summary>
        /// <returns>int: Record count</returns>
        public override int GetNumberOfRows()
        {
            return this.memberReceipts.Count;
        }

        /// <summary>
        /// Show the different child attributes value of the seleted member
        /// </summary>
        /// <param name="rowIndex">int: Row index</param>
        /// <param name="column">column: Grid column</param>
        /// <returns>object: column data</returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            logger.Trace("GetPointHistoryGridProvider", "GetColumnData", "Begin");
            MemberReceipts memberReceipts = this.memberReceipts[rowIndex];
 
            object value = null;
            switch (column.Name)
            {
                case "TxnNumber":
                    if (memberReceipts.ReceiptType == (int)ReceiptTypes.Web)
                    {
                        value = memberReceipts.OrderNumber;
                    }
                    else
                    {
                        value = memberReceipts.TxnNumber;
                    }
                    break;
                case "TxnDate":
                    if (memberReceipts.ReceiptType == (int)ReceiptTypes.Web)
                    {
                        value = string.Format("{0:MM/dd/yyyy}", memberReceipts.TxnDate);
                    }
                    else
                    {
                        value = string.Format("{0:MM/dd/yyyy}", memberReceipts.TxnDate);
                    }
                    break;
                case "TenderAmount":
                    value = string.Format("{0:C}", memberReceipts.TenderAmount);
                    break;
                case "StoreNumber":   //"TxnStoreId":
                     if (memberReceipts.ReceiptType == (int)ReceiptTypes.Web)
                    {
                        value = "Web";
                    }
                    else
                    {
                        value = memberReceipts.TxnStoreID;
                    }
                    break;
                case "Register":
                    if (memberReceipts.ReceiptType == (int)ReceiptTypes.Web)
                    {
                        value = "-";
                    }
                    else
                    {
                        value = memberReceipts.TxnRegisterNumber;
                    }
                    break;
                case "CreateDate":
                    value = string.Format("{0:MM/dd/yyyy}", memberReceipts.CreateDate);
                    break;
                case "StatusCode":
                    value = Enum.GetName(typeof(ReceiptStatus), memberReceipts.StatusCode);
                    break;
                default:
                    break;
            }
            logger.Trace("GetPointHistoryGridProvider", "GetColumnData", "End");
            return value;
        }
        #endregion
    }
    #endregion
}