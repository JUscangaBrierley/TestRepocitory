using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;

using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.RewardsHistory.Components.Grid
{
	public class DefaultGridProvider : AspGridProviderBase
    {
        #region Fields
        private const string _className = "DefaultGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private DateTime _startDate = DateTimeUtil.MinValue;
		private DateTime _endDate = DateTimeUtil.MaxValue;
		private IList<MemberReward> _rewards = null;
        private Dictionary<string, MemberOrder> _orderMap = null;
        private Dictionary<long, MemberReward> _rewardMap = null;
		private string _statusFilter = string.Empty;
		private string _rewardFilter = string.Empty;
        private Member _m = null;
        private RewardsHistoryConfig _config = null;
        private ICustomGridAction[] actions = null;
        #endregion

        #region Helpers

        protected void LoadRewards()
		{
			string method = "LoadRewards";
			if (_m != null)
			{
                IList<long> rewardIds = LoyaltyService.GetMemberRewardIds(_m, null, _startDate, _endDate, null, false, false);
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
						if(long.TryParse(_rewardFilter, out rewardId))
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

        private RHConfigurationItem GetConfigItem(string columnName)
        {
            foreach (RHConfigurationItem item in _config.AttributesToShow)
            {
                if (item.AttributeName == columnName)
                {
                    return item;
                }
            }
            return null;
        }

		private object GetData(MemberReward reward, DynamicGridColumnSpec column)
		{
			string method = "GetData";

			object value = null;
            if (column.Name == "Id")
            {
                value = reward.Id;
            }
            else
            {
                RHConfigurationItem item = GetConfigItem(column.Name);
                if (item.ItemType == ItemTypes.RewardDef)
                {
                    long id = reward.RewardDefId;
                    RewardDef rdef = ContentService.GetRewardDef(id);
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
                    else if (item.AttributeName == "RewardType")
                    {
                        value = rdef.RewardType.ToString();
                    }
                    else if (item.AttributeName == "ConversionRate")
                    {
                        value = rdef.ConversionRate;
                    }
                }
                else if (item.ItemType == ItemTypes.MemberReward)
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
                        value = string.IsNullOrEmpty(item.Format) ? reward.DateIssued.ToShortDateString() : reward.DateIssued.ToString(item.Format);
                    }
                    else if (item.AttributeName == "Expiration" && reward.Expiration != null)
                    {
                        value = string.IsNullOrEmpty(item.Format) ? reward.Expiration.Value.ToString() : reward.Expiration.Value.ToString(item.Format);
                    }
                    else if (item.AttributeName == "FulfillmentDate" && reward.FulfillmentDate != null)
                    {
                        value = string.IsNullOrEmpty(item.Format) ? reward.FulfillmentDate.Value.ToString() : reward.FulfillmentDate.Value.ToString(item.Format);
                    }
                    else if (item.AttributeName == "RedemptionDate" && reward.RedemptionDate != null)
                    {
                        value = string.IsNullOrEmpty(item.Format) ? reward.RedemptionDate.Value.ToString() : reward.RedemptionDate.Value.ToString(item.Format);
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
                                status = ResourceUtils.GetLocalWebResource(ParentControl, "lblExpiredOn", "Expired on") + " " + expDate.ToShortDateString();
                            }
                            else
                            {
                                // not yet.
                                status = ResourceUtils.GetLocalWebResource(ParentControl, "lblExpiresOn", "Expires on") + " " + expDate.ToShortDateString();
                            }
                        }
                        else if (reward.RedemptionDate != null)
                        {
                            status = ResourceUtils.GetLocalWebResource(ParentControl, "lblUsedOn", "Used on") + " " + ((DateTime)reward.RedemptionDate).ToShortDateString();
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
                else if (item.ItemType == ItemTypes.MemberOrder)
                {
                    if (!string.IsNullOrWhiteSpace(reward.LWOrderNumber) && _orderMap.ContainsKey(reward.LWOrderNumber))
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
            }            
			return value;
		}

		#endregion

		#region Grid Properties

        public override string Id
        {
            get { return "grdRewardsHistory"; }
        }

        protected override string GetGridName()
        {
            return "RewardsHistory";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
            IList<DynamicGridColumnSpec> specs = new List<DynamicGridColumnSpec>();

            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "Id";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Id.Text", "Id");
            c.DataType = typeof(long);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            specs.Add(c);

            foreach (RHConfigurationItem item in _config.AttributesToShow)
            {
                c = new DynamicGridColumnSpec();
                c.Name = item.AttributeName;
                if (!string.IsNullOrEmpty(item.ResourceFileKey))
                {
                    c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceFileKey, item.AttributeName);
                }
                else
                {
                    c.DisplayText = item.AttributeName;
                }
                //c.DataType = !string.IsNullOrEmpty(item.DataType) ? item.DataType : typeof(string);
                c.DataType = typeof(string);
                c.IsKey = true;
                c.IsEditable = false;
                c.IsVisible = true;
                specs.Add(c);
            }
                        
            return specs.ToArray<DynamicGridColumnSpec>();
		}

		public override bool IsActionColumnVisible()
		{
            return _config.AllowCancellation;
		}

        public override bool IsButtonVisible(string commandName)
        {
            return commandName == "Cancel Reward";
        }

        public override bool IsButtonEnabled(string commandName, object key)
        {
            bool enabled = false;
            if (commandName == "Cancel Reward" && _config.AllowCancellation && !string.IsNullOrEmpty(_config.CancelleableChannels))
            {
                string[] tokens = _config.CancelleableChannels.Split(';');
                var rewards = (from x in _rewards where x.Id == (long)key select x);
                if (rewards != null && rewards.Count() > 0)
                {
                    MemberReward reward = rewards.ElementAt<MemberReward>(0);
                    RewardOrderStatus status = RewardOrderStatus.InProcess;
                    if (_rewardMap != null && _rewardMap.ContainsKey(reward.Id))
                    {
                        if (_rewardMap[reward.Id].OrderStatus != null)
                        {
                            status = (RewardOrderStatus)_rewardMap[reward.Id].OrderStatus;
                        }
                    }
                    if (status == RewardOrderStatus.Created || status == RewardOrderStatus.Pending)
                    {
                        if (!string.IsNullOrEmpty(reward.LWOrderNumber) && _orderMap.ContainsKey(reward.LWOrderNumber))
                        {
                            MemberOrder order = _orderMap[reward.LWOrderNumber];
                            if (!string.IsNullOrEmpty(order.Channel))
                            {
                                enabled = (from x in tokens where x == order.Channel select 1).Count() > 0;
                            }
                        }
                    }
                } 
            }
            else if (commandName == "Resend Email" && _config.AllowEmailResend)
            {
                long rewardId = long.Parse(key.ToString());
                var rewards = (from x in _rewards where x.Id == (long)key select x);
                if (rewards != null && rewards.Count() > 0)
                {
                    MemberReward reward = rewards.ElementAt<MemberReward>(0);
                    IList<EmailAssociation> list = EmailService.GetEmailAssociations(PointTransactionOwnerType.Reward, reward.RewardDefId, reward.Id);
                    enabled = list.Count > 0 ? true : false;
                }
            }
            return enabled;
        }

		public override bool IsGridEditable()
		{
            return _config.AllowCancellation;
		}
						
		#endregion

		#region Data Source

		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (string.IsNullOrWhiteSpace(parmName))
			{
				_startDate = DateTimeUtil.MinValue;
				_endDate = DateTimeUtil.MaxValue;
			}
			else if (parmName == "FromDate")
			{
                _startDate = (DateTime?)parmValue ?? DateTimeUtil.MinValue;
			}
			else if (parmName == "ToDate")
			{
                _endDate = (DateTime?)parmValue ?? DateTimeUtil.MaxValue;
				_endDate = _endDate.AddDays(1).AddTicks(-1);
			}
			else if (parmName == "Member")
			{
				_m = (Member)parmValue;
			}
            else if (parmName == "Config")
            {
                _config = (RewardsHistoryConfig)parmValue;
            }
		}

        public override List<DynamicGridFilter> GetFilters()
        {
            var filters = new List<DynamicGridFilter>();
            string defaultSelect = ResourceUtils.GetLocalWebResource(ParentControl, "DefaultSelect.Text", "-- Select --");
            filters.Add(new DynamicGridFilter(ResourceUtils.GetLocalWebResource(ParentControl, "StatusFilterLabel.Text", "Reward Status:"),
                FilterDisplayTypes.DropDownList,
                defaultSelect,
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

		public override void LoadGridData()
		{
			LoadRewards();
		}

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{

		}

		public override int GetNumberOfRows()
		{
			return (_rewards != null ? _rewards.Count : 0);
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
			MemberReward r = _rewards[rowIndex];
			return GetData(r, column);
		}

		#endregion

        #region Command Handling
        public override ICustomGridAction[] GetCustomCommands()
        {
            if (actions == null)
            {
                if (_config == null || _config.AllowCancellation)
                {
                    actions = new ICustomGridAction[2];
                    actions[0] = new CancelRewardCommand(this);
                    actions[1] = new ResendEmailCommand(this);                    
                }
            }
            return actions;
        }
        #endregion
	}
}
