using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.FavoriteStores.Components
{
	public class DefaultListProvider : AspListProviderBase
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private enum ColumnName
		{
			ID = 0, Content
		}

		private int _numColumns = Enum.GetNames(typeof(ColumnName)).Length;
		private IList<StoreDef> _favoriteStores = null;
		private FavoriteStoresConfig _config = null;
        private const string parentControl = "~/Controls/Modules/FavoriteStores/ViewFavoriteStores.ascx";


        public override string Id
		{
			get { return "lstFavoriteStores"; }
		}

		public override IEnumerable<DynamicListItem> GetListItemSpecs()
		{
			var colList = new List<DynamicListItem>();


			DynamicListCommandSpec deleteButton = new DynamicListCommandSpec(
				new ListCommand("Delete"),
				ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkDelete.Text", "Delete")
			);
			deleteButton.CssClass = "btn btn-sm";
			colList.Add(deleteButton);

			DynamicListCommandSpec mapItButton = new DynamicListCommandSpec(
				new ListCommand("MapIt"),
				ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkMapIt.Text", "Map It")
			);
			mapItButton.CssClass = "btn-sm";
			colList.Add(mapItButton);



			DynamicListColumnSpec c = new DynamicListColumnSpec()
			{
				Name = ColumnName.ID.ToString(),
				DisplayText = null,
				DataType = typeof(long),
				IsKey = true,
				IsEditable = false,
				IsVisible = false
			};
			colList.Add(c);

			c = new DynamicListColumnSpec()
			{
				Name = ColumnName.Content.ToString(),
				DisplayText = null,
				DataType = typeof(string),
				IsEditable = false,
				IsVisible = true
			};
			colList.Add(c);

                    
			return colList;
		}

		public override bool IsButtonVisible(ListCommand commandName)
		{
			if (commandName == ListCommand.View 
				|| commandName == ListCommand.Print
				|| commandName == ListCommand.Delete
				//|| commandName == "MapIt"
				)
			{
				return true;
			}
			else
			{
				return false;
			};
		}

		public override string GetEmptyListMessage()
		{
			return ResourceUtils.GetLocalWebResource(parentControl, _config.EmptyResultMessageResourceKey);
		}

        public override string GetAppPanelTotalText(int totalRecords)
        {
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(parentControl, "NoFavoriteStoresMessage.Text", "No Favorite Stores.");
            }
            else
            {
                return string.Format(ResourceUtils.GetLocalWebResource(parentControl, "Total.Text", "Total") + " {0} ", totalRecords);
            }
        }

		public override void SetSearchParm(string parmName, object parmValue)
		{
			switch (parmName)
			{
				case "Config":
					_config = (FavoriteStoresConfig)parmValue;
					break;
			}
		}

        public override void LoadListData()
        {
            Member member = PortalState.CurrentMember;
            if (member == null) return;
            IList<MemberStore> memberStores = LoyaltyService.GetMemberStoresByMember(member.IpCode);
            if (memberStores != null && memberStores.Count > 0)
            {
                long[] sortedMemberStoreIDs = (from x in memberStores orderby x.PreferenceOrder ascending select x.StoreDefId).ToArray<long>();
                if (sortedMemberStoreIDs.Length > 0)
                {
                    _favoriteStores = ContentService.GetAllStoreDefs(sortedMemberStoreIDs);
                }
            }
        }

		public override int GetNumberOfRows()
		{
			return _favoriteStores != null ? _favoriteStores.Count : 0;
		}

		public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
		{
			object val = null;
			var favoriteStore = _favoriteStores[rowIndex];

			switch ((ColumnName)Enum.Parse(typeof(ColumnName), column.Name))
			{
				case ColumnName.ID:
					val = favoriteStore.StoreId;
					break;

				case ColumnName.Content:
					StringBuilder markup = new StringBuilder();
					markup.Append("<div class=\"col-md-6 col-sm-12\">");

					markup.Append("<div class=\"store-buttons\">");
					if (favoriteStore.Latitude.HasValue && favoriteStore.Longitude.HasValue)
					{
						markup.Append(
							string.Format(
								@"<a class=""btn btn-sm map-button"" data-toggle=""modal"" data-target=""#mapModal"" onclick=""mapfavstore('{0}', '{1}', '{2}'); return false;"">{3}</a>",
								favoriteStore.StoreName, 
								favoriteStore.Latitude, 
								favoriteStore.Longitude, 
								ResourceUtils.GetLocalWebResource(parentControl, "btnMapIt.Text"))
						);
					}
					markup.Append("</div>");


					markup.Append("<h3 class=\"name\">").Append(StringUtils.FriendlyString(favoriteStore.StoreName)).Append("</h3>");
					markup.Append("<p>");
					markup.Append("<span class=\"address1\">").Append(StringUtils.FriendlyString(favoriteStore.AddressLineOne)).Append("</span>");
					if (!string.IsNullOrWhiteSpace(favoriteStore.AddressLineTwo)) 
					{
						markup.Append("<span class=\"address2\">").Append(favoriteStore.AddressLineTwo).Append("</span>");
					}
					markup.Append("<span class=\"citystatezip\">").Append(StringUtils.FriendlyString(favoriteStore.City)).Append(",&nbsp;</span>");
					markup.Append(StringUtils.FriendlyString(favoriteStore.StateOrProvince));
					markup.Append(StringUtils.FriendlyString(favoriteStore.ZipOrPostalCode)).Append("</span>");
					markup.Append("<span class=\"country\">").Append(StringUtils.FriendlyString(favoriteStore.Country)).Append("</span>");
					markup.Append("</div>");
					val = markup.ToString();
					break;
			}
			return val;
		}

		public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
		{
			throw new NotImplementedException();
		}

		public override void DeleteListData(object keyData)
		{
			if (keyData == null) return;

			long storeId = StringUtils.FriendlyInt64(keyData);
			if (storeId > -1)
			{
				IList<MemberStore> memberStores = LoyaltyService.GetMemberStoresByMember(PortalState.CurrentMember.IpCode);
				if (memberStores != null && memberStores.Count > 0)
				{
					foreach (MemberStore memberStore in memberStores)
					{
						if (memberStore.StoreDefId == storeId)
						{
							LoyaltyService.DeleteMemberStore(memberStore.Id);
						}
					}
				}

				StoreDef existing = (from x in _favoriteStores where x.StoreId == storeId select x).FirstOrDefault<StoreDef>();
				if (existing != null)
				{
					_favoriteStores.Remove(existing);
				}
			}
		}
	}
}