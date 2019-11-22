namespace Brierley.AEModules.RequestCredit
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Brierley.Clients.AmericanEagle.DataModel;
    //using Brierley.DNNModules.PortalModuleSDK.Controls.Grid;
    using Brierley.WebFrameWork.Controls.Grid;

    using Brierley.AEModules.RequestCredit.Components;
    using Brierley.FrameWork.Data;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    using Brierley.FrameWork.Common.Logging;
    #endregion

    /// <summary>
    /// Default grid provider for search results
    /// </summary>
    public class DefaultGridProvider : AspGridProviderBase
    {
        #region Member variables

        /// <summary>
        /// Holds Request Credit module
        /// </summary>
        public static int RequestCreditModuleId;

        /// <summary>
        /// Holds the configuration settings for request credit
        /// </summary>
        //private RequestCreditConfig configuration = null;

        /// <summary>
        /// Holds history record
        /// </summary>
        private IList<IClientDataObject> historyRecord = new List<IClientDataObject>();

        /// <summary>
        /// Holds history record
        /// </summary>
        private IList<IClientDataObject> historyRecordList = null;

        private static LWLogger logger = LWLoggerManager.GetLogger("RequestCredit");

        #endregion member variables

        #region Data Source

        /// <summary>
        /// Set the list of search parameter
        /// </summary>
        /// <param name="parmName">string: Param name</param>
        /// <param name="parmValue">Object: Param value</param>
        public override void SetSearchParm(string parmName, object parmValue)
        {
            if (parmName == "HistoryRecordList")
            {
                this.historyRecordList = (IList<IClientDataObject>)parmValue;
            }
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
        /// Method to load data for grid.
        /// </summary>
        public override void LoadGridData()
        {
            this.LoadHistoryRecord();
        }

        /// <summary>
        /// Abstract method for saving of grid data.
        /// </summary>
        /// <param name="columns">arry of columns</param>
        /// <param name="update">boolean: update</param>
        ///  // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ 
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        
        {
        }

        /// <summary>
        /// Returns count of rows
        /// </summary>
        /// <returns>int: Record count</returns>
        public override int GetNumberOfRows()
        {
            return this.historyRecord.Count;
        }

        /// <summary>
        /// Get value for perticular column
        /// </summary>
        /// <param name="rowIndex">int: Row index</param>
        /// <param name="column">column specification</param>
        /// <returns>object: cell value</returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            HistoryTxnDetail historyData = this.historyRecordList[rowIndex] as HistoryTxnDetail;
            // Checking format of column data

            string value = string.Empty;
            if (column.Name == "TxnType")
            {
                value = string.Format(column.FormatString != string.Empty ? column.FormatString : "{0}", historyData.GetAttributeValue(column.Name));
                if (value == "1")
                {
                    value = "P";
                }
                else
                {
                    value = "R";
                }
                logger.Trace("DefaultGridProvider", "GetColumnData", "TxnType: " + value);
            }
            else
            {
                if (historyData.GetAttributeValue(column.Name) != null)
                {
                    string columnValue = historyData.GetAttributeValue(column.Name).ToString();

                    if (column.Name == "TxnStoreID")
                    {
                        value = historyData.GetAttributeValue("StoreNumber").ToString();
                    }
                    else
                    {
                        string format = column.FormatString;
                        //if (column.FormatString.Length > 0)
                        //{
                        //value = string.Format(format, columnValue);
                        //value = string.Format(column.FormatString != string.Empty ? column.FormatString : "{0}", historyData.GetAttributeValue(column.Name));
                        value = columnValue;
                        //}
                    }
                }
                else
                {
                    value = "-";
                }
                logger.Trace("DefaultGridProvider", "GetColumnData", "Column value: " + value);
            }
            return value;
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

        /// <summary>
        /// Returns default for perticular column
        /// </summary>
        /// <param name="column">column: grid column</param>
        /// <returns>object: defult cell value</returns>
        public override object GetDefaultValues(DynamicGridColumnSpec column)
        {
            return null;
        }

        #endregion

        #region Grid Properties

        /// <summary>
        /// Method to set whether grid is selectable or not. Make it selectable 
        /// </summary>
        /// <returns>true:selectable, false: not selectable</returns>
        public override bool IsGridRowSelectable()
        {
            return true;
        }

        /// <summary>
        /// Method to set whether grid is editable or not. Just make it not editable
        /// </summary>
        /// <returns>True: editable and false: non editable</returns>
        public override bool IsGridEditable()
        {
            return false;
        }

        /// <summary>
        /// Method to set which type of buttons will be displayed.
        /// </summary>
        /// <param name="commandName">Command name to process further</param>
        /// <returns>bool: command button should visible or not</returns>
        public override bool IsButtonVisible(string commandName)
        {
            if (commandName == AspDynamicGrid.EDIT_ROW_COMMAND ||
                commandName == AspDynamicGrid.DELETE_ROW_COMMAND ||
                commandName == AspDynamicGrid.ADDNEW_COMMAND)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns Action header text of grid
        /// </summary>
        /// <returns>string: Button text</returns>
        public override string GetGridActionsHeaderText()
        {
            return "Actions";
        }

        /// <summary>
        /// Method for columns specification
        /// </summary>
        /// <returns>Arry of DynamicGridColumnSpec</returns>
        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            char quote = '"';

            //var config = Brierley.DNNModules.RequestCredit.Components.ModuleConfiguration.GetConfiguration(RequestCreditModuleId);
            //this.configuration = new RequestCreditConfig();
            //XmlSerializer serializer = new XmlSerializer(this.configuration.GetType());
            //this.configuration = (RequestCreditConfig)serializer.Deserialize(new System.IO.StringReader(config.Content));

            //int intLength = this.configuration.AttributesResultfields.Count + 1;
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[7];
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "RowKey";
            c.DisplayText = "RowKey";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            c.IsSortable = false;
            columns[0] = c;
            int count = 1;

            columns[count] = BuildColumn("TxnStoreID", "<span style=" + quote + "color: red;" + quote + ">*</span>Store Number", @"^\d{5}$");
            count++;
            columns[count] = BuildColumn("TxnRegisterNumber", "Register Number", @"^\d{3}$");
            count++;
            columns[count] = BuildColumn("TenderAmount", "Total Payment", @"^\d+(\.\d{1,2})?$");
            count++;
            columns[count] = BuildColumn("TxnNumber", "Transaction Number", @"^\d{3}[0-9]?[0-9]?[0-9]?$");
            count++;
            columns[count] = BuildColumn("TxnDate", "<span style=" + quote + "color: red;" + quote + ">*</span>Transaction Date", @"^(([1-9])|(0[1-9])|(1[0-2]))\/(([0-9])|([0-2][0-9])|(3[0-1]))\/(([0-9][0-9])|([1-2][0,9][0-9][0-9]))$");
            count++;

            columns[count] = BuildColumn("TxnLoyaltyID", "Loyalty Number", @"^\d{14}$");
            count++;
 
 
            //foreach (ConfigurationItem attribute in this.configuration.AttributesResultfields)
            //{
            //    c = new DynamicGridColumnSpec();
            //    c.Name = attribute.AttributeName;
            //    c.DisplayText = attribute.DisplayText;
            //    c.FormatString = attribute.Format;
            //    c.DataType = "System.String";
            //    c.IsKey = false;
            //    c.IsEditable = false;
            //    c.IsVisible = true;
            //    c.IsSortable = false;
            //    columns[count] = c;
            //    count++;
            //}

            return columns;
        }

        private DynamicGridColumnSpec BuildColumn(string attributeName, string displayText, string format)
        {
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = attributeName;
            c.DisplayText = displayText;
            c.FormatString = format;
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsKey = false;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            return c;
        }
        /// <summary>
        /// Method to set insert label of grid.
        /// </summary>
        /// <returns>Grid insert label</returns>
        public override string GetGridInsertLabel()
        {
            return string.Empty;
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Load history record list to history record to populate grid
        /// </summary>
        protected void LoadHistoryRecord()
        {
            if (this.historyRecordList != null && this.historyRecordList.Count > 0)
            {
                this.historyRecord.Clear();
                this.historyRecord = (IList<IClientDataObject>)this.historyRecordList;
            }
        }
        #endregion
    }
}