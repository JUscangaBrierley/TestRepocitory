﻿// <copyright file="MergeAccountGridProvider.cs" company="B+P">
// Copyright (c) B+P. All rights reserved.
// </copyright>
namespace Brierley.AEModules.MergeAccount.Components
{
    #region Using Statements
    using System; //AEO-74 Upgrade 4.5 changes  here -----------SCJ
    using System.Collections.Generic;
    using Brierley.Clients.AmericanEagle.DataModel;
    //using Brierley.DNNModules.PortalModuleSDK.Controls.Grid;
    using Brierley.WebFrameWork.Controls.Grid;

    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    
    #endregion

    #region Class definition for custom grid provider for member lookup page
    /// <summary>
    /// Class definition for custom grid provider for member lookup page
    /// </summary>
    public class MergeAccountGridProvider : AspGridProviderBase
    {
        #region Variables

        /// <summary>
        /// Holds member receipts
        /// </summary>
        private List<MemberMergeHistory> memberMergeHistory = null;

        /// <summary>
        /// Holds member merge history
        /// </summary>
        private List<MemberMergeHistory> lstMemberMergeHistory = null;

        /// <summary>
        /// Holds the data service instance
        /// </summary>
        //private ILWDataService service = LWDataServiceUtil.DataServiceInstance(true);
        #endregion

        #region Grid Properties

        /// <summary>
        /// Set whether the particular grid row is selectable or not
        /// </summary>
        /// <returns>false: don't want to make grid selectable</returns>
        public override bool IsGridRowSelectable()
        {
            return false;
        }

        /// <summary>
        /// Load the Grid Data
        /// </summary>
        public override void LoadGridData()
        {
            if (null != this.memberMergeHistory && this.memberMergeHistory.Count > 0)
            {
                this.lstMemberMergeHistory = this.memberMergeHistory;
            }
        }

        /// <summary>
        /// Set whether the grid is editable or not
        /// </summary>
        /// <returns>false:Don't want to make grid editable</returns>
        public override bool IsGridEditable()
        {
            return false;
        }

        /// <summary>
        /// Set what action buttons are visible or not. By default all button are visible
        /// </summary>
        /// <param name="commandName">Required to capture the command for further processing</param>
        /// <returns>bool: don't visible</returns>
        public override bool IsButtonVisible(string commandName)
        {
            return false;
        }

        /// <summary>
        /// Returns the columns name needed to show in grid
        /// </summary>
        /// <returns>Returns array of columns</returns>
        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[2];

            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "AEREWARDSNumber";
            c.DisplayText = "<STRONG>AEO Connected Number</STRONG>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsKey = false;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[0] = c;

         /*   c = new DynamicGridColumnSpec();
            c.Name = "CSR";
            c.DisplayText = "CSR";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[1] = c;*/

            c = new DynamicGridColumnSpec();
            c.Name = "DateMerged";
            c.DisplayText = "<STRONG>Date Merged</STRONG>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[1] = c;

            return columns;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Returns row editor form location
        /// </summary>
        /// <returns>Editor form location</returns>
       // public override string GetRowEditorFormLocation()
       // {
       //     return string.Empty;
       // }

        protected  override string GetGridName()
        {
            return string.Empty;
        }

        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion

        #region Data Source

        /// <summary>
        /// Sets search parameter
        /// </summary>
        /// <param name="parmName">string: parameter name</param>
        /// <param name="parmValue">Object: parameter value</param>
        public override void SetSearchParm(string parmName, object parmValue)
        {
            this.memberMergeHistory = (List<MemberMergeHistory>)parmValue;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Save grid data
        /// </summary>
        /// <param name="columns">array of grid columns</param>
        /// <param name="update">Boolean: update</param>
        // public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
            public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        {
        }
        // AEO-74 Upgrade 4.5 changes End here -----------SCJ
        /// <summary>
        /// Returns number of rows 
        /// </summary>
        /// <returns>int: Record count</returns>
        public override int GetNumberOfRows()
        {
            return this.memberMergeHistory.Count;
        }

        /// <summary>
        /// Show the different child attributes value of the selected member
        /// </summary>
        /// <param name="rowIndex">int: Row index</param>
        /// <param name="column">column: Grid column</param>
        /// <returns>object: column data</returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            MemberMergeHistory memberMergeHistory = this.memberMergeHistory[rowIndex];
            object value = null;
            switch (column.Name)
            {
                case "AEREWARDSNumber":
                    //PI15929 - Display from loyaltyIdNumber in the merge history
                    value = memberMergeHistory.FromLoyaltyID;
                    break;
                    /* AEO -1341 begin */
              /*  case "CSR":
                    value = memberMergeHistory.ChangedBy;
                    break;*/
                /* AEO -1341 end */
                case "DateMerged":
                    value = string.Format("{0:g}", memberMergeHistory.CreateDate);
                    break;
                default:
                    break;
            }

            return value;
        }
        #endregion
    }
    #endregion
}