#region | Namespace declaration |
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
//LW 4.1.14 change
//using Brierley.DNNModules.PortalModuleSDK;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Portal;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.ClientDevUtilities.LWGateway;
#endregion

namespace AmericanEagle.SDK.GridProvider
{


    #region Class defination for ReplaceCardProvider
    /// <summary>
    /// Class defination for ReplaceCardProvider class
    /// </summary>
    class ReplaceCardProvider : AspGridProviderBase
    {
        //private IList<Member> members = new List<Member>();
        private IList<IClientDataObject> replaceCards = new List<IClientDataObject>();
        //private long[] ipCodeList = null;
        private static LWLogger _logger = LWLoggerManager.GetLogger("ReplaceCard");
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil();
        #region Helpers
        /// <summary>
        /// Get member details by IP Code
        /// </summary>
        protected void LoadMemberCards()
        {
            try
            {
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
                var member = PortalState.CurrentMember;
                if (member != null)
                {
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Card Replacements");
                    LWCriterion crit = new LWCriterion("MemberCardReplacements");

                    IList<IClientDataObject> objList = _LoyaltyData.GetAttributeSetObjects(member, "MemberCardReplacements", crit, null, true);
                    var v_replaceCards = objList.OrderByDescending(x => x.CreateDate);
                    replaceCards = v_replaceCards.ToList();
                }
            }
            catch(Exception ex)
            {
                _logger.Error("ReplaceCardProvider", "LoadMemberCards", ex.Message);
            }
        }
        #endregion

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
        /// <returns></returns>
        public override bool IsGridRowSelectable()
        {
            return false;
        }
        /// <summary>
        /// Method to set whether grid is editable or not.
        /// </summary>
        /// <returns></returns>
        public override bool IsGridEditable()
        {
            return false;
        }
        /// <summary>
        /// Method to set which type of buttons will be displayt.
        /// </summary>
        /// <returns></returns>
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
        /// Method for columns specification
        /// </summary>
        /// <returns></returns>
        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            //Array of grid columns
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[4];
            
            //Object to set properties of columns
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "LoyaltyIDNumber";
            c.DisplayText = "AEREWARD$Number";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = true;
            columns[0] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "CHANGEDBY";
            c.DisplayText = "CSR";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[1] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "CreateDate";
            c.DisplayText = "Date Changed";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[2] = c;


            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "StatusCode";
            c.DisplayText = "Status";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[3] = c;
            return columns;
        }
        /// <summary>
        /// Method to set insert label of grid.
        /// </summary>
        /// <returns></returns>
        public override string GetGridInsertLabel()
        {
            return "";
        }
      
       
        #endregion

        #region Data Source
        /// <summary>
        /// Set the list of search parameter
        /// </summary>
        /// <param name="parmName"></param>
        /// <param name="parmValue"></param>
        public override void SetSearchParm(string parmName, object parmValue)
        {

        }
        /// <summary>
        /// Method to load data for grid.
        /// </summary>
        /// <returns></returns>
        public override void LoadGridData()
        {
            LoadMemberCards();
        }

        /// <summary>
        /// Abstract method for saving of grid data.
        /// </summary>
        /// <returns></returns>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns count of rows
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfRows()
        {
            return replaceCards.Count;
        }
        /// <summary>
        /// Get value for perticular column
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {

            IClientDataObject obj = replaceCards[rowIndex];
            MemberCardReplacements replaceCard = (MemberCardReplacements)obj;
            
            object value = null;
            switch (column.Name)
            {
                case "LoyaltyIDNumber":
                    if (null != replaceCard.LoyaltyIDNumber)
                    {
                        value = replaceCard.LoyaltyIDNumber != null ? replaceCard.LoyaltyIDNumber : string.Empty;
                    }
                    break;
                case "CHANGEDBY":
                    if (null != replaceCard.CHANGEDBY)
                    {
                        value = replaceCard.CHANGEDBY != null ?  replaceCard.CHANGEDBY : string.Empty;
                    }
                    break;
                case "CreateDate":
                    if (null != replaceCard.CreateDate)
                    {
                        DateTime createDate = (DateTime)replaceCard.CreateDate;
                        value = replaceCard.CreateDate != null ? createDate.ToShortDateString() : string.Empty;
                    }
                    break;
                case "StatusCode":
                    if (0 != replaceCard.StatusCode)
                    {
                        value = replaceCard.StatusCode != 0 ? Definitions.GetCardReplaceStatus(replaceCard.StatusCode) : "0";
                    }
                    break;
                default:
                    break;
                
            }
            return value;
        }
     
        /// <summary>
        /// Returns default for perticular column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public override object GetDefaultValues(DynamicGridColumnSpec column)
        {
            return null;
        }

        #endregion
    }
    #endregion
}
