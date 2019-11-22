using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Util;

namespace Brierley.LWModules.CFRewardsHistory.Components.List
{
	public class RewardsHistoryListProvider : AspListProviderBase
    {
        #region Fields
        private const string _className = "RewardsHistoryListProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private List<Dictionary<string, object>> _parms = null;

		private IList<MemberReward> _rewards = null;
		private List<RewardDef> _definitions = new List<RewardDef>();
        private Dictionary<string, MemberOrder> _orderMap = null;
        private Dictionary<long, MemberReward> _rewardMap = null;
		private string _statusFilter = string.Empty;
		private string _rewardFilter = string.Empty;
        private Member _m = null;
        private CFRewardsHistoryConfig _config = null;
        
        #endregion

        #region Helpers

        protected void LoadRewards()
        {
            string method = "LoadRewards";
            if (_m != null)
            {
                DateTime? startDate = null, endDate = null;

                if (_parms != null)
                {
                    foreach (var dict in _parms)
                    {
                        if (dict.ContainsKey("FromDate"))
                        {
                            startDate = DateTime.Parse(dict["FromDate"].ToString());
                        }
                        if (dict.ContainsKey("ToDate"))
                        {
                            endDate = DateTime.Parse(dict["ToDate"].ToString());
                        }
                    }
                }
                List<long> rewardIds = LoyaltyService.GetMemberRewardIds(_m, null, startDate, endDate, null, false, false);
                if (rewardIds.Count > 0)
                {
                    _rewards = LoyaltyService.GetMemberRewardByIds(rewardIds.ToArray<long>());
                }
                if (_rewards == null || _rewards.Count == 0)
                {
                    _logger.Trace(_className, method, "No rewards found for member with Ipcode = " + _m.IpCode);
                }
                else
                {
                    if (!string.IsNullOrEmpty(_rewardFilter))
                    {
                        long rewardId = 0;
                        if (long.TryParse(_rewardFilter, out rewardId))
                        {
                            _rewards = _rewards.Where(o => o.RewardDefId == rewardId).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(_statusFilter))
                    {
                        switch (_statusFilter)
                        {
                            case "Active":
                                _rewards = _rewards.Where(o => o.Expiration.GetValueOrDefault(DateTime.MaxValue) >= DateTime.Now).ToList();
                                break;
                            case "Expired":
                                _rewards = _rewards.Where(o => o.Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.Now).ToList();
                                break;
                        }
                    }
                    MemberRewardsUtil.GetMemberRewardsStatus(_rewards, out _rewardMap, out _orderMap);
                }
            }
            else
            {
                _logger.Trace(_className, method, "No member has been selected.");
            }
        }

        private ConfigurationItem GetConfigItem(string columnName)
        {
            foreach (ConfigurationItem item in _config.ListFieldsToShow)
            {
                if (item.AttributeName == columnName)
                {
                    return item;
                }
            }

			foreach (ConfigurationItem item in _config.ViewFieldsToShow)
			{
				if (item.AttributeName == columnName)
				{
					return item;
				}
			}

            return null;
        }

		private object GetData(MemberReward reward, DynamicListColumnSpec column)
		{
			object value = null;
            if (column.Name == "Id")
            {
                value = reward.Id;
            }
            else
            {
                ConfigurationItem item = GetConfigItem(column.Name);
                if (item.NodeCategory == ItemTypes.RewardDef)
                {
                    long id = reward.RewardDefId;
					RewardDef rdef = GetRewardDef(id);
                    if (item.AttributeName == "CertificateTypeCode")
                    {
                        value = rdef.CertificateTypeCode;
                    }
                    else if (item.AttributeName == "Name")
                    {
                        value = rdef.Name;
                    }
                    else if (item.AttributeName == "ShortDescription")
                    {
                        value = rdef.ShortDescription;
                    }
                    else if (item.AttributeName == "HowManyPointsToEarn")
                    {
                        value = rdef.HowManyPointsToEarn.ToString();
                    }
                    else if (item.AttributeName == "PointType")
                    {
                        value = rdef.PointType;
                    }
                    else if (item.AttributeName == "PointEvent")
                    {
                        value = rdef.PointEvent;
                    }
                    else if (item.AttributeName == "Product")
                    {
                        Product p = ContentService.GetProduct(rdef.ProductId);
                        value = p.Name;
                    }
                    else if (item.AttributeName == "TierId" && rdef.TierId != null)
                    {
                        TierDef t = LoyaltyService.GetTierDef((long)rdef.TierId);
                        value = t.Name;
                    }
                    else if (item.AttributeName == "Threshold")
                    {
                        value = rdef.Threshold.ToString();
                    }
                    else if (item.AttributeName == "LogoFileName")
                    {
                        value = ContentManagementUtils.GetValidWebAddres(rdef.MediumImageFile, PortalState.GetImageUrl(string.Empty));
                    }
                    else if (item.AttributeName == "RewardType")
                    {
                        value = rdef.RewardType.ToString();
                    }
                    else if (item.AttributeName == "ConversionRate")
                    {
                        value = rdef.ConversionRate;
                    }
                }
                else if (item.NodeCategory == ItemTypes.MemberReward)
                {
                    if (item.AttributeName == "CertificateNmbr")
                    {
                        value = reward.CertificateNmbr;
                    }
                    else if (item.AttributeName == "OfferCode")
                    {
                        value = reward.OfferCode;
                    }
                    else if (item.AttributeName == "AvailableBalance")
                    {
                        value = reward.AvailableBalance.ToString();
                    }
                    else if (item.AttributeName == "FulfillmentOption")
                    {
                        value = reward.FulfillmentOption.ToString();
                    }
                    else if (item.AttributeName == "DateIssued")
                    {
                        value = reward.DateIssued.ToShortDateString();
                    }
                    else if (item.AttributeName == "Expiration" && reward.Expiration != null)
                    {
                        value = reward.Expiration.Value.ToShortDateString();
                    }
                    else if (item.AttributeName == "FulfillmentDate" && reward.FulfillmentDate != null)
                    {
                        value = reward.FulfillmentDate;
                    }
                    else if (item.AttributeName == "RedemptionDate" && reward.RedemptionDate != null)
                    {
                        value = reward.RedemptionDate;
                    }
                    else if (item.AttributeName == "OrderNumber")
                    {
                        value = reward.LWOrderNumber;
                    }
                    else if (item.AttributeName == "CancellationNumber")
                    {
                        value = reward.LWCancellationNumber;
                    }
                    else if (item.AttributeName == "OrderStatus")
                    {
                        string status = "Unknown";
                        if (_rewardMap != null && _rewardMap.ContainsKey(reward.Id))
                        {
                            if (_rewardMap[reward.Id].OrderStatus != null)
                            {
                                status = _rewardMap[reward.Id].OrderStatus.ToString();
                            }
                        }
                        value = status;
                    }
                    else if (item.AttributeName == "OrderStatus")
                    {
                        string status = "Unknown";
                        if (_rewardMap != null && _rewardMap.ContainsKey(reward.Id))
                        {
                            if (_rewardMap[reward.Id].OrderStatus != null)
                            {
                                status = _rewardMap[reward.Id].OrderStatus.ToString();
                            }
                        }
                        value = status;
                    }
                    else if (item.AttributeName == "RewardStatus")
                    {
                        string status = string.Empty;
                        if (reward.Expiration != null)
                        {
                            DateTime expDate = (DateTime)reward.Expiration;
                            if (DateTimeUtil.LessThan(expDate, DateTime.Now))
                            {
                                // it has expired.
                                status = "Expired on " + expDate.ToShortDateString();
                            }
                            else
                            {
                                // not yet.
                                status = "Expires on " + expDate.ToShortDateString();
                            }
                        }
                        else if (reward.RedemptionDate != null)
                        {
                            status = "Used on " + ((DateTime)reward.RedemptionDate).ToShortDateString();
                        }
                        value = status;
                    }
                    else if (item.AttributeName == "PointsConsumed")
                    {
                        value = reward.PointsConsumed;
                    }
                    else if (item.AttributeName == "FromCurrency")
                    {
                        value = reward.FromCurrency;
                    }
                    else if (item.AttributeName == "ToCurrency")
                    {
                        value = reward.ToCurrency;
                    }
                    else if (item.AttributeName == "PointConversionRate")
                    {
                        value = reward.PointConversionRate;
                    }
                    else if (item.AttributeName == "ExchangeRate")
                    {
                        value = reward.ExchangeRate;
                    }
                    else if (item.AttributeName == "MonetaryValue")
                    {
                        value = reward.MonetaryValue;
                    }
                    else if (item.AttributeName == "CartTotalMonetaryValue")
                    {
                        value = reward.CartTotalMonetaryValue;
                    }
                }
                else if (item.NodeCategory == ItemTypes.MemberOrder)
                {
                    if (_orderMap.ContainsKey(reward.LWOrderNumber))
                    {
                        MemberOrder order = _orderMap[reward.LWOrderNumber];
                        if (item.AttributeName == "FirstName")
                        {
                            value = order.FirstName;
                        }
                        else if (item.AttributeName == "LastName")
                        {
                            value = order.LastName;
                        }
                        else if (item.AttributeName == "EmailAddress")
                        {
                            value = order.EmailAddress;
                        }
                        else if (item.AttributeName == "ShippingAddress")
                        {
                            value = order.ShippingAddress;
                        }
                        else if (item.AttributeName == "Channel")
                        {
                            value = order.Channel;
                        }
                    }
                }
                else
                {
                    if (item.AttributeName == "ShippingAddress")
                    {
                        if (!string.IsNullOrEmpty(_config.BarcodeAssemblyName) && !string.IsNullOrEmpty(_config.BarcodeFactoryName) && !string.IsNullOrWhiteSpace(reward.CertificateNmbr))
                        {
                            var link = new BarcodeLink(_config.BarcodeAssemblyName, _config.BarcodeFactoryName, _config.BarcodeSymbology, reward.CertificateNmbr);
                            string imageName = string.Format("~/Barcode.aspx?barcode={0}", link.ToString());
                            value = imageName;
                        }
                    }
                }
            }            
			return value;
		}

		private RewardDef GetRewardDef(long id)
		{
			var ret = _definitions.FirstOrDefault(o => o.Id == id);
			if(ret == null)
			{
				ret = ContentService.GetRewardDef(id);
				if (ret != null)
				{
					_definitions.Add(ret);
				}
			}
			return ret;
		}


		#endregion

		#region Grid Properties

        public override string Id
        {
            get { return "grdRewardsHistory"; }
        }


		public override IEnumerable<DynamicListItem> GetListItemSpecs()
		{
			if (_config == null)
			{
				return GetItemSpecs(null);
			}
			return GetItemSpecs(_config.ListFieldsToShow);
		}


		public override IEnumerable<DynamicListItem> GetViewItemSpecs()
		{
			if (_config == null)
			{
				return GetItemSpecs(null);
			}
			return GetItemSpecs(_config.ViewFieldsToShow);
		}


		private IEnumerable<DynamicListItem> GetItemSpecs(List<ConfigurationItem> itemsToShow)
		{
            IList<DynamicListItem> items = new List<DynamicListItem>();

            DynamicListColumnSpec id = new DynamicListColumnSpec();
            id.Name = "Id";
            id.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Id.Text", "Id");
            id.DataType = typeof(long);
            id.IsKey = true;
            id.IsEditable = false;
            id.IsVisible = false;
            items.Add(id);

            foreach (ConfigurationItem item in itemsToShow)
            {
                if (item.AttributeType == ItemTypes.DynamicListCommandButton)
                {
                    DynamicListCommandSpec command = new DynamicListCommandSpec(new ListCommand(item.AttributeName), null) { BeginHtml = item.BeginHtml, EndHtml = item.EndHtml };
                    if (!string.IsNullOrEmpty(item.DisplayText))
                    {
                        command.Text = item.DisplayText;
                    }
                    if (!string.IsNullOrEmpty(item.ResourceKey))
                    {
                        command.Text = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, command.Text);
                    }
                    if (!string.IsNullOrEmpty(item.ControlCSSClass))
                    {
                        command.CssClass = item.ControlCSSClass;
                    }

					if (command.CommandName == ListCommand.Share.CommandName)
					{
						string socialShareUrl = string.Empty;
						if (_config.UseExternalSharedUrl)
						{
							socialShareUrl = StringUtils.FriendlyString(_config.ExternalSharedUrl);
						}
						else
						{
							if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.Url != null)
							{
								socialShareUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).ToString();
							}
							socialShareUrl += "/SocialShare.aspx";
						}
						SocialShareConfig socialShareConfig = new SocialShareConfig()
						{
							IsFacebookEnabled = _config.IsFacebookEnabled,
							Facebook = new FacebookShareParams()
							{
								AppID = _config.FacebookAppID,
								SharedUrl = socialShareUrl,
								Layout = FacebookShareLayoutEnum.Button
							},
							IsTwitterEnabled = _config.IsTwitterEnabled,
							Twitter = new TwitterShareParams()
							{
								SharedUrl = socialShareUrl,
								TextToTweet = _config.TwitterTweetText,
								HashTag = _config.TwitterHashTag,
								ViaTwitterUser = string.Empty,
								RecommendTwitterUser = string.Empty,
								UseLargeButton = false,
								ShowShareCount = false,
								DoNotTailor = true
							},
							IsGooglePlusEnabled = _config.IsGooglePlusEnabled,
							GooglePlus = new GooglePlusShareParams()
							{
								SharedUrl = socialShareUrl
							}
						};
						command.SocialShareConfig = socialShareConfig;
					}

                    items.Add(command);
                }
                else
                {
					string displayText = item.DisplayText;
                    if (!string.IsNullOrEmpty(item.ResourceKey))
                    {
                        displayText = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, item.DisplayText);
                    }

                    var c = new DynamicListColumnSpec(item.AttributeName, displayText, string.IsNullOrEmpty(item.LabelCssClass) ? item.ControlCSSClass : item.LabelCssClass);
                    c.BeginHtml = item.BeginHtml;
                    c.EndHtml = item.EndHtml;

                    if (item.AttributeName == "LogoFileName" || item.AttributeName == "SmallImageFile" || item.AttributeName == "BarCode")
                    {
                        c.DataType = typeof(HtmlImage);
                    }
                    items.Add(c);
                }
            }
                        
            return items;
		}


        public override bool IsButtonEnabled(ListCommand command, object key)
        {
			if (command == ListCommand.Delete || command == ListCommand.Create)
			{
				return false;
			}
			return true;
        }

        public override string GetAppPanelTotalText(int totalRecords)
        {
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoRewardsMessage.Text", "No Rewads");
            }
            else
            {
                return string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "Total.Text", "Total") + " {0} ", totalRecords);
            }
        }

        #endregion

        #region Data Source

        public override void SetSearchParm(string parmName, object parmValue)
		{
			if (parmName == "Member")
			{
				_m = (Member)parmValue;
			}
            else if (parmName == "Config")
            {
                _config = (CFRewardsHistoryConfig)parmValue;
            }
		}

        public override void SetSearchParm(List<Dictionary<string, object>> parms)
        {
            _parms = parms;
            base.SetSearchParm(parms);
        }

        public override List<DynamicGridFilter> GetFilters()
        {
            var filters = new List<DynamicGridFilter>();
            string defaultSelect = ResourceUtils.GetLocalWebResource(ParentControl, "DefaultSelect.Text", "-- Select --");
            filters.Add(new DynamicGridFilter(ResourceUtils.GetLocalWebResource(ParentControl, "StatusFilterLabel.Text", "Reward Status:"),
                FilterDisplayTypes.DropDownList, defaultSelect,
                ResourceUtils.GetLocalWebResource(ParentControl, "ActiveFilter.Text", "Active"),
                ResourceUtils.GetLocalWebResource(ParentControl, "ExpiredFilter.Text", "Expired")));
            var rewards = ContentService.GetAllRewardDefs();
            if (rewards != null && rewards.Count > 0)
            {
                var rewardFilter = new DynamicGridFilter(ResourceUtils.GetLocalWebResource(ParentControl, "RewardFilterLabel.Text", "Reward:"), FilterDisplayTypes.DropDownList);
                rewardFilter.FilterValues.Add(string.Empty, defaultSelect);
                foreach (var reward in rewards.OrderBy(o => o.Name))
                {
                    rewardFilter.FilterValues.Add(reward.Id.ToString(), reward.Name);
                }
                filters.Add(rewardFilter);
            }
            return filters;
        }

        public override void SetFilter(string filterName, string filterValue)
        {
            if (filterName == ResourceUtils.GetLocalWebResource(ParentControl, "StatusFilterLabel.Text", "Reward Status:"))
                _statusFilter = filterValue;
            else if (filterName == ResourceUtils.GetLocalWebResource(ParentControl, "RewardFilterLabel.Text", "Reward:"))
                _rewardFilter = filterValue;
        }

		public override void LoadListData()
		{
			LoadRewards();
		}

        public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
		{

		}

		public override int GetNumberOfRows()
		{
			return (_rewards != null ? _rewards.Count : 0);
		}

		public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
		{
			MemberReward r = _rewards[rowIndex];
			return GetData(r, column);
		}

		public override object GetColumnData(object keyVal, DynamicListColumnSpec column)
		{
			long id = long.Parse(keyVal.ToString());
			MemberReward r = null;
			if (_rewards != null)
			{
				r = _rewards.Where(o => o.Id == id).FirstOrDefault();
			}
            if (r == null)
            {
                r = LoyaltyService.GetMemberReward(id);
            }
			
			return GetData(r, column);
		}

		public override string GetSocialSharedUrlArgument(object key)
		{
			string result = string.Empty;
			if (key != null)
			{
				string args = string.Format("command=reward&rewardID={0}", key.ToString());

				string encryptionKey = CryptoUtil.EncodeUTF8("sOc14l5hAr3");
				result = "?id=" + CryptoUtil.Encrypt(encryptionKey, args);
			}
			return result;
		}

		#endregion        
	}
}
