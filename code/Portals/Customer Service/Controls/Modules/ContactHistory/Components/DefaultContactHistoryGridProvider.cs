using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.ContactHistory.Components
{
	public class DefaultContactHistoryGridProvider : AspGridProviderBase
	{
		#region fields
		private static string _className = "DefaultContactHistoryGridProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private DateTime _startDate = DateTimeUtil.MinValue;
		private DateTime _endDate = DateTimeUtil.MaxValue;
		private ContactHistoryConfiguration _config = null;
		private IList<Brierley.FrameWork.Data.DomainModel.ContactHistory> _contactHistory = null;
		private ContactStatusMap _contactStatusMap = null;
		#endregion

		#region Grid Properties
		public override string Id
		{
			get { return "grdContactHistory"; }
		}

        protected override string GetGridName()
        {
            return "ContactHistory";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();

			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "RowKey";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RowKey.Text", "Id");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

			var fields = _config.GetHeaderListToShow();
			if (fields != null && fields.Count > 0)
			{
				foreach (string field in fields)
				{
					c = new DynamicGridColumnSpec();
					c.Name = field;
					string resourceKey = string.Format("{0}-{1}.Text", Id, field);
					c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, resourceKey, field);
					c.DataType = typeof(string);
					c.IsEditable = false;
					c.IsVisible = true;
					c.IsSortable = true;
					colList.Add(c);
				}
			}
			return colList.ToArray<DynamicGridColumnSpec>();
		}

		public override bool IsGridEditable()
		{
			return false;
		}
		#endregion

		#region Data Sources
		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (string.IsNullOrWhiteSpace(parmName))
			{
				_startDate = DateTimeUtil.MinValue;
				_endDate = DateTimeUtil.MaxValue;
			}
			else if (parmName == "FromDate")
			{
				_startDate = parmValue != null ? (DateTime)parmValue : DateTimeUtil.MinValue;
			}
			else if (parmName == "ToDate")
			{
				_endDate = parmValue != null ? (DateTime)parmValue : DateTimeUtil.MaxValue;
				_endDate = _endDate.AddDays(1).AddTicks(-1);
			}
			else if (parmName == "Config")
			{
				_config = (ContactHistoryConfiguration)parmValue;
			}
		}

		public override void LoadGridData()
		{
			Member member = PortalState.CurrentMember;
            if (member != null && CSService != null)
            {
                _contactHistory = CSService.GetAllContactHistoryInDateRange(member.IpCode, _startDate, _endDate);
                if (_contactStatusMap == null) _contactStatusMap = CSService.GetContactStatusMap();
            }
		}

		public override int GetNumberOfRows()
		{
			return (_contactHistory != null ? _contactHistory.Count : 0);
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
			object val = null;
			var contactHistory = _contactHistory[rowIndex];

			switch (column.Name)
			{
				case "RowKey":
					val = contactHistory.ID;
					break;

				case "IPCode":
					val = contactHistory.IPCode;
					break;
					
				case "LoyaltyID":
					val = contactHistory.LoyaltyID;
					break;
					
				case "ContactStatus":
					val = _contactStatusMap.GetDescriptionForID(contactHistory.ContactStatusKey);
					break;

				case "ContactType":
					val = contactHistory.ContactType.ToString();
					break;

				case "ContactDate":
					val = contactHistory.ContactDate.ToString();
					break;

				case "MailingID":
					val = contactHistory.MailingID;
					break;
					
				case "EmailAddress":
					val = contactHistory.EmailAddress;
					break;
					
				case "MobileNumber":
					val = contactHistory.MobileNbr;
					break;

                case "CampaignName":
                    val = contactHistory.CampaignName;
                    break;

				case "CellCode":
					val = contactHistory.CellCode;
					break;

				case "Clicks":
					val = contactHistory.Clicks;
					break;

				case "Opens":
					val = contactHistory.Opens;
					break;

				case "Conversion":
					val = contactHistory.Conversion;
					break;

				case "EmailLink":
					if (contactHistory.ContactType == ContactTypeEnum.Email && !string.IsNullOrWhiteSpace(contactHistory.EmailLink))
					{
						string msg = ResourceUtils.GetLocalWebResource(ParentControl, _config.EmailLinkMessageResourceKey, "View");
						string cssclass = StringUtils.FriendlyString(_config.EmailLinkCSSClass);
						if (string.IsNullOrEmpty(cssclass))
						{
							val = string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", contactHistory.EmailLink, msg);
						}
						else
						{
							val = string.Format("<a href=\"{0}\" target=\"_blank\" class=\"{2}\">{1}</a>", contactHistory.EmailLink, msg, cssclass);
						}
					}
					else
					{
						val = string.Empty;
					}
					break;
			}
			return val;
		}

		public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}