#region | Namespace declaration |
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Portal;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.ClientDevUtilities.LWGateway;
#endregion

namespace AmericanEagle.SDK.GridProvider
{
    #region Class defination for LoyaltyCardRecord

    class LoyaltyCardRecord
    {
        public string LoyaltyId { get; set; }
        public bool Primary { get; set; }
        public string CSR { get; set; }
        public string IssueDate { get; set; }
    }
    #endregion

    #region Class defination for ReplaceCardProvider
    /// <summary>
    /// Class defination for LoyaltyCardsGridProvider class
    /// </summary>
    class LoyaltyCardsGridProvider : AspGridProviderBase
    {
        private IList<LoyaltyCardRecord> memberCards = new List<LoyaltyCardRecord>();
        private static LWLogger _logger = LWLoggerManager.GetLogger("LoyaltyCardsGridProvider");
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        #region Helpers
        /// <summary>
        /// Get cards from current Member
        /// </summary>
        protected void LoadMemberCards()
        {
            Member member = PortalState.CurrentMember;
            ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();

            if (member != null)
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Member Cards");

                LWCriterion criteria = new LWCriterion("MemberMergeHistory");
                criteria.Add(LWCriterion.OperatorType.AND, "IpCode", member.IpCode, LWCriterion.Predicate.Eq);
                List<MemberMergeHistory> lstHistoryRecords = _LoyaltyData.GetAttributeSetObjects(null, "MemberMergeHistory", criteria, null, false).Cast<MemberMergeHistory>().ToList();

                foreach (VirtualCard card in member.LoyaltyCards.OrderByDescending(c=>c.DateIssued).ToList())
                {
                    this.memberCards.Add(new LoyaltyCardRecord
                    {
                        LoyaltyId = card.LoyaltyIdNumber,
                        Primary = card.IsPrimary,
                        CSR = lstHistoryRecords.FirstOrDefault(hist => hist.FromLoyaltyID == card.LoyaltyIdNumber) != null ? lstHistoryRecords.FirstOrDefault(hist => hist.FromLoyaltyID == card.LoyaltyIdNumber).ChangedBy : string.Empty,
                        IssueDate = card.DateIssued.ToShortDateString()
                    });
                }
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
            c.Name = "LoyaltyId";
            c.DisplayText = "Loyalty Id";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsKey = false;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[0] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "Primary";
            c.DisplayText = "Primary";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.Boolean";
           c.DataType = typeof(System.Boolean); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            
            c.IsEditable = false;
            c.IsSortable = false;
            columns[1] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "CHANGEDBY";
            c.DisplayText = "CSR";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
            columns[2] = c;

            // Create new object for DynamicGridColumnSpec
            c = new DynamicGridColumnSpec();
            c.Name = "IssueDate";
            c.DisplayText = "Issue Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = false;
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
        /// // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
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
            return memberCards.Count;
        }

        /// <summary>
        /// Get value for perticular column
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            LoyaltyCardRecord card = memberCards[rowIndex];

            object value = null;
            switch (column.Name)
            {
                case "LoyaltyId":
                    value = card.LoyaltyId;
                    break;
                case "Primary":
                    value = card.Primary;
                    break;
                case "CHANGEDBY":
                    value = card.CSR;
                    break;
                case "IssueDate":
                    value = card.IssueDate;
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
