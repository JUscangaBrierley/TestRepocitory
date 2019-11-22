using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LWModules.MemberSearch.Components;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.MemberSearch
{
	public class DefaultGridProvider : AspGridProviderBase
    {
        private IList<Member> members = new List<Member>();
        private long[] ipCodeList = null;
		
		public MemberSearchConfiguration Config { get; set; }

        #region Helpers
        
        protected void LoadMembers()
        {
            if (ipCodeList != null && ipCodeList.Length > 0)
            {
                members.Clear();
                members = LoyaltyService.GetAllMembers(ipCodeList, true);
            }
        }        
        #endregion
        
        #region Grid Properties

        public override string Id
        {
            get { return "grdMemberSearch"; }
        }

        protected override string GetGridName()
        {
            return "MemberSearch";
        }

		public override bool IsGridRowSelectable()
		{
			return true;
		}

		public override bool IsGridEditable()
        {
            return false;              
        }

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

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
			if (Config == null || Config.ResultsAttributes == null || Config.ResultsAttributes.Count == 0)
			{
				//default results
				DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[4];

				DynamicGridColumnSpec c = new DynamicGridColumnSpec();
				c.Name = "IpCode";
				c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-IpCode.Text", "IpCode");
				c.DataType = typeof(long);
				c.IsKey = true;
				c.IsEditable = false;
				c.IsVisible = false;
				columns[0] = c;

				c = new DynamicGridColumnSpec();
				c.Name = "FirstName";
				c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-FirstName.Text", "First Name");
				c.DataType = typeof(string);
				c.IsEditable = false;
				c.IsSortable = true;
				columns[1] = c;

				c = new DynamicGridColumnSpec();
				c.Name = "LastName";
				c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-LastName.Text", "Last Name");
				c.DataType = typeof(string);
				c.IsEditable = false;
				c.IsSortable = true;
				columns[2] = c;

				c = new DynamicGridColumnSpec();
				c.Name = "Email";
				c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Email.Text", "Email");
				c.DataType = typeof(string);
				c.IsEditable = false;
				c.IsSortable = true;
				columns[3] = c;

				return columns;
			}
			else
			{
				int intLength = Config.ResultsAttributes.Count + 1;
				DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[intLength];
				DynamicGridColumnSpec c = new DynamicGridColumnSpec();
				c.Name = "RowKey";
				c.DisplayText = "RowKey";
				c.DataType = typeof(long);
				c.IsKey = true;
				c.IsEditable = false;
				c.IsVisible = false;
				c.IsSortable = false;
				columns[0] = c;
				int count = 1;

				foreach (ConfigurationItem attribute in Config.ResultsAttributes)
				{
					c = new DynamicGridColumnSpec();
					c.Name = attribute.DataKey;
					c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, attribute.ResourceKey, attribute.DisplayText);
					c.FormatString = attribute.Format;
					c.IsKey = false;
					c.IsEditable = false;
					c.IsVisible = true;
					c.IsSortable = true;
					columns[count] = c;
					count++;
				}
				return columns;
			}
        }

		public override string GetGridInsertLabel()
        {
            return "";
        }
		
        public override int NumberOfRowsPerPage
        {
            get
            {
                //return base.NumberOfRowsPerPage;
                return Config.ResultsPerPage;
            }
            set
            {
                Config.ResultsPerPage = value;
            }
        }

        #endregion

        #region Data Source

		public override void SetSearchParm(string parmName, object parmValue)
        {
            if (parmName == "IpCodeList")
            {
                ipCodeList = (long[])parmValue;
            }
            else if (parmName == "MemberList")
            {
                members = (IList<Member>)parmValue;
                ipCodeList = null;
            }
        }

        public override void LoadGridData()
        {
            LoadMembers();            
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {                        
        }

        public override int GetNumberOfRows()
        {
            return members.Count;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
			Member member = members[rowIndex];
			object value = null;

			if (Config == null || Config.ResultsAttributes == null || Config.ResultsAttributes.Count == 0)
			{

				if (column.Name == "IpCode")
				{
					value = member.IpCode;
				}
				else if (column.Name == "FirstName")
				{
					value = member.FirstName;
				}
				else if (column.Name == "LastName")
				{
					value = member.LastName;
				}
				else if (column.Name == "Email")
				{
					value = member.PrimaryEmailAddress;
				}
			}
			else
			{
				if (column.Name == "RowKey")
				{
					value = member.IpCode;
				}
				else
				{
					foreach (var item in Config.ResultsAttributes)
					{
						if (item.DataKey == column.Name)
						{

							value = GetMemberAttributeValue(item, member);
							break;
						}
					}
				}
			}
            return value;
        }

		//public override string GetValueToDisplay(DynamicGridColumnSpec column, object dataValue, int rowIndex = 0)
		//{
		//	return dataValue.ToString();
		//}

		public override object GetDefaultValues(DynamicGridColumnSpec column)
        {
            return null;
        }

        #endregion


		//todo: move this to a shared class somehwhere - it's copied from ViewMemberProfile.ascx.cs and only slightly modified
		private object GetMemberAttributeValue(ConfigurationItem item, Member member)
		{
			object value = null;
			if (member != null)
			{
				if (item.IsVirtualCard)
				{
					VirtualCard vc = null;
					if (member.LoyaltyCards != null && member.LoyaltyCards.Count > 0)
					{
						vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard) ?? member.GetFirstCard();
					}
					if (vc == null)
					{
						return value;
					}

					Type t = typeof(VirtualCard);
					System.Reflection.PropertyInfo[] properties = t.GetProperties();
					foreach (System.Reflection.PropertyInfo pi in properties)
					{
						if (pi.Name == item.AttributeName)
						{
							value = pi.GetValue(vc, null);
							break;
						}
					}
				}
				else if (item.AttributeSetID < 0)
				{
					Type t = typeof(Member);
					System.Reflection.PropertyInfo[] properties = t.GetProperties();
					foreach (System.Reflection.PropertyInfo pi in properties)
					{
						if (pi.Name == item.AttributeName)
						{
							value = pi.GetValue(member, null);
							break;
						}
					}
				}
				else
				{
					IList<IClientDataObject> atsList = member.GetChildAttributeSets(item.AttributeSetName);
					if (atsList.Count < 1)
					{
						//no rows exist
					}
					else if (atsList.Count == 1)
					{
						value = atsList[0].GetAttributeValue(item.AttributeName);
					}
					else
					{
						int rowIndex = -1;
						bool hasPrimary = false;

						try
						{
							object primaryTest = atsList[0].GetAttributeValue("isprimary");
							if (primaryTest != null)
							{
								hasPrimary = true;
							}
						}
						catch
						{
							//no "isprimary" exists.
						}

						if (!hasPrimary)
						{
							rowIndex = 0;
						}
						else
						{
							for (int i = 0; i < atsList.Count; i++)
							{
								if (atsList[i].GetAttributeValue("IsPrimary").ToString() != "1")
								{
									continue;
								}
								rowIndex = i;
							}
						}

						if (rowIndex > -1)
						{
							value = atsList[rowIndex].GetAttributeValue(item.AttributeName);
						}
					}
				}
			}
			return value;
		}
    }
}
