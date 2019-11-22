
#region Namespace
using System;
using System.Text;
using System.Collections.Generic;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
//using Brierley.DNNModules.PortalModuleSDK.Controls.Grid;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Common.Logging;

#endregion


namespace AmericanEagle.SDK.GridProvider
{

    
    #region Class definition for custom grid provider for member lookup page
    /// <summary>
    /// Class defination for custom grid provider for member lookup page
    /// </summary>
    public class MemberLookupGridProvider : AspGridProviderBase
    {
        #region Variables
        private IList<Member> _members = new List<Member>();
        private long[] _ipCodeList = null;
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        private static LWLogger _logger = LWLoggerManager.GetLogger("MemberLookup");
        #endregion

        #region Helpers

        /// <summary>
        /// Load all members based on the search criteria provided from front end
        /// </summary>
        protected void LoadMembers()
        {
            try
            {
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();

                if (_ipCodeList != null && _ipCodeList.Length > 0)
                {
                    _members.Clear();
                    _members = _LoyaltyData.GetAllMembers(_ipCodeList, true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("MemberLookupGridProvider", "LoadMembers", ex.Message);
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
        /// Set whether the particular grid row is selectable or not
        /// </summary>
        /// <returns></returns>
        public override bool IsGridRowSelectable()
        {
            return true;
        }

        /// <summary>
        /// Set whether the grid is editable or not
        /// </summary>
        /// <returns></returns>
        public override bool IsGridEditable()
        {
            return false;
        }

        /// <summary>
        /// Set what action buttons are visible or not. By default all button are visible
        /// 
        /// </summary>
        /// <param name="commandName"></param>
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
        /// Returns the columns name needed to show in grid
        /// </summary>
        /// <returns></returns>
        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[7];

            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "IpCode";
            c.DisplayText = "IpCode";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.Int64";
            c.DataType = typeof(System.Int64); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            columns[0] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "CardNumber";
            c.DisplayText = "<center><strong>AEO </br>Connected </br>Number</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[1] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Name";
            c.DisplayText = "<center><strong>Name</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[2] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Email";
            c.DisplayText = "<center><strong>Email</br>Address</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[3] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Mobile";
            c.DisplayText = "<center><strong>Mobile</br>Number</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[4] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Address";
            c.DisplayText = "<center><strong>Address</br>City</br>State</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[5] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Status";
            c.DisplayText = "<center><strong>Status</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[6] = c;

      /*
            c = new DynamicGridColumnSpec();
            c.Name = "State";
            c.DisplayText = "<center><strong>State</center></strong>";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsSortable = true;
            columns[7] = c;*/
            
         


            return columns;
        }

        public override string GetGridInsertLabel()
        {
            return "";
        }

        //AEO-1714 BEGIN
        /// <summary>
        /// Set whether the particular grid row is enabled or not
        /// </summary>
        /// <returns></returns>
        public override bool IsButtonEnabled(string commandName, object key)
        {
            Boolean bResult = true;            
            long iKey = 0;
            long.TryParse((key.ToString()), out iKey);

            if (commandName == AspDynamicGrid.SELECT_COMMAND)
            {
                foreach (Member Result in _members)
                {
                    if (Result.IpCode == iKey)
                    {
                        MemberStatusEnum MemStatus = (MemberStatusEnum)Enum.Parse(typeof(MemberStatusEnum), Result.MemberStatus.ToString());

                        if ((int)MemStatus == (int)MemberStatusAE.Closed)
                        {
                            bResult = false;
                        }
                        else
                        {
                            bResult = true;
                        }
                        break;
                    }
                }                
            }
            //AEO-1714 END

            return bResult;
        }
        #endregion

        #region Data Source

        public override void SetSearchParm(string parmName, object parmValue)
        {
            if (parmName == "IpCodeList")
            {
                _ipCodeList = (long[])parmValue;
            }
            else if (parmName == "MemberList")
            {
                _members = (IList<Member>)parmValue;
                _ipCodeList = null;
            }
        }


        /// <summary>
        /// Load the Grid Data
        /// </summary>
        public override void LoadGridData()
        {
            LoadMembers();
        }



        public override int GetNumberOfRows()
        {
            return _members.Count;
        }
        //public override string GetValueToDisplay(DynamicGridColumnSpec column, object dataValue)
        //{
        //    return dataValue.ToString();
        //}

        public override object GetDefaultValues(DynamicGridColumnSpec column)
        {
            return null;
        }
         //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Show the different child attributes value of the seleted member
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            Member member = _members[rowIndex];
            object value = null;
            IList<IClientDataObject> returnList = member.GetChildAttributeSets("MemberDetails");
            MemberDetails mbrDetails = new MemberDetails();

            if (returnList.Count > 0)
            {
                mbrDetails = returnList[0] as MemberDetails;
            }

            //Showing the value of different columns
            
            switch (column.Name)
            {
                case "IpCode":
                    value = member.IpCode;
                    break;
                case "CardNumber":
                    value = "<small>" + GetLoyaltyId(member)+"</small>" ;
                    break;
                case "Name":
                    value = "<small>" +member.FirstName + " " + member.LastName+ "</small>";
                    break;
                case "Email":
                    value = mbrDetails != null ? "<small>"+mbrDetails.EmailAddress+"</small>":"";
                    break;
                case "Mobile":
                    value = mbrDetails != null && mbrDetails.MobilePhone != null ? mbrDetails.MobilePhone.Replace("-", "").Trim() : "";
                    StringBuilder tmp = new StringBuilder();
                    String tmpval = new String( value.ToString().ToCharArray());

                    if ( tmpval.Length > 6 ) {
                        tmp.Append(tmpval,0,3);
                        tmp.Append('-');
                        tmp.Append(tmpval, 3,3);
                        tmp.Append('-');
                        tmp.Append(tmpval, 6, tmpval.Length-6);
                        value = "<small>" + tmp.ToString()+ "</small>";

                    }
                    else if ( tmpval.Length > 3 ) {
                        tmp.Append(tmpval, 0, 3);
                        tmp.Append('-');
                        tmp.Append(tmpval, 3, tmpval.Length - 3);
                        value = "<small>" + tmp.ToString() + "</small>";

                    } /* AEO-1481 begin */
                    else {
                        value = string.Empty;
                    } /* AEO-1481 end  */
                   
                    break;

                case "Address":
                    if (null != mbrDetails)
                    {
                        value = "<small>" + mbrDetails.AddressLineOne + "<br>"  +mbrDetails.City +
                                "<br>" + mbrDetails.StateOrProvince + "</small>";
                    }
                    break;
                case "Status":
                    string strValue = string.Empty;
                    MemberStatusEnum MemStatus = (MemberStatusEnum)Enum.Parse(typeof(MemberStatusEnum), member.MemberStatus.ToString());
                    strValue = Enum.GetName(typeof(MemberStatusAE), (int)MemStatus);
                    if (null != mbrDetails)
                    {
                        if (mbrDetails.MemberStatusCode == 1 && strValue == Enum.GetName(typeof(MemberStatusAE), MemberStatusAE.Active))
                            //Account is inactive, set value as inactive
                            strValue = Enum.GetName(typeof(MemberStatusAE), MemberStatusAE.Inactive);
                    }
                    value = strValue;
                    break;
                /* 
                case "State":
                    if (null != mbrDetails)
                    {
                        value = "<small>" + mbrDetails.StateOrProvince + "</small>";
                    }
                    break;*/
                default:
                    break;
            }
            return value;
        }

        #endregion

        #region Private Methods

                /// <summary>
        /// Retruns Loyalty Id of a member
        /// </summary>
        /// <param name="pVCL">List of Virtual Card of a Member</param>
        /// <returns>LoyalTyIdNumber</returns>
        private String GetLoyaltyId(Member pMember)
        {
            //Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromIPCode(pMember.IpCode);
            foreach (VirtualCard vc in pMember.LoyaltyCards)
            {
                if (vc.IsPrimary)
                {
                    return vc.LoyaltyIdNumber;
                }
            }
            return String.Empty;
        }
        #endregion Private Methods
    }
    #endregion
}
