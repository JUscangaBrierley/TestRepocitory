using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;

using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CouponsGrid.Components
{
    public class CouponsGridProvider : AspGridProviderBase
    {
        #region Fields

        private const string _className = "CouponsGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private CouponsGridConfig _config = null;
        
        private ICustomGridAction[] actions = null;

        private DateTime _startDate = DateTimeUtil.MinValue;
        private DateTime _endDate = DateTimeUtil.MaxValue;
        private IList<MemberCoupon> _coupons = null;
        private string _couponFilter = string.Empty;
        private Member _m = null;
        //Dictionary<string, string> columnMap = new Dictionary<string, string>();
        #endregion

        #region Private Helpers
		
        protected void LoadCoupons()
        {
            string method = "LoadCoupons";
            if (_m != null)
            {
                IList<long> couponIds = LoyaltyService.GetMemberCouponIds(_m, _startDate, _endDate, false);
                if (couponIds.Count > 0)
                {
                    _coupons = LoyaltyService.GetMemberCoupons(couponIds.ToArray<long>());
                }
                if (_coupons == null || _coupons.Count == 0)
                {
                    _logger.Trace(_className, method, "No coupons found for member with Ipcode = " + _m.IpCode);
                }
                else
                {
                    if (!string.IsNullOrEmpty(_couponFilter))
                    {
                        switch (_couponFilter)
                        {
                            case "Active":
                                _coupons = _coupons.Where(o => o.IsActive()).ToList();
                                break;
                            case "Redeemed":
                                _coupons = _coupons.Where(o => o.Status != null && o.Status.Value == CouponStatus.Redeemed).ToList();
                                break;
                            case "Expired":
                                _coupons = _coupons.Where(o => o.ExpiryDate.GetValueOrDefault(DateTime.MaxValue) < DateTime.Now).ToList();
                                break;
                        }
                    }
                }
            }
            else
            {
                _logger.Trace(_className, method, "No member has been selected.");
            }
        }

        private object GetColumnData(MemberCoupon mc, DynamicGridColumnSpec column, string language, string channel)
        {
            object value = null;
            CouponDef def = ContentService.GetCouponDef(mc.CouponDefId);

            if (column.Name == "ID" || column.Name == "RowKey" )
            {
                value = mc.ID;
                return value;
            }

            string columnName = column.Name;
            foreach (var i in _config.ListFieldsToShow)
            {
                if (i.DataKey == column.Name)
                {
                    columnName = i.AttributeName;
                    break;
                }
            }

            if (columnName == "CouponName")
            {
                value = def.Name;
            }
            else if (columnName == "TypeCode")
            {
                value = def.CouponTypeCode;
            }
            else if (columnName == "Logo")
            {
                value = def.LogoFileName;
            }
            else if (columnName == "CertificateNmbr")
            {
                value = mc.CertificateNmbr;
            }            
            else if (columnName == "ShortDescription")
            {
                value = def.GetShortDescription(language, channel);
            }
            else if (columnName == "Description")
            {
                value = def.GetDescription(language, channel);
            }
            else if (columnName == "DateIssued")
            {
                value = mc.DateIssued;
            }
            else if (columnName == "DateRedeemed")
            {
                value = mc.DateRedeemed;
            }
            else if (columnName == "ExpiryDate")
            {
                value = mc.ExpiryDate;
            }
            else if (columnName == "Status")
            {
                value = mc.Status.GetValueOrDefault().ToString();
            }
            else if (columnName == "UsesAllowed")
            {
                value = def.UsesAllowed;
            }
            else if (columnName == "TimesUsed")
            {
                value = mc.TimesUsed;
            }
            else if (columnName == "UsesLeft")
            {
                value = def.UsesAllowed - mc.TimesUsed;
            }
            return value;
        }
        #endregion

        #region Grid Properties
        public override string Id
        {
            get{ return "grdCoupons"; }
        }

        protected override string GetGridName()
        {
            return "Coupons";
        }

        public override bool IsGridEditable()
        {
            return _config.AllowEdits;
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            //columnMap.Clear();

            if (_config == null || _config.ListFieldsToShow == null || _config.ListFieldsToShow.Count == 0)
            {
                IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();

                DynamicGridColumnSpec c = new DynamicGridColumnSpec();
                c.Name = "ID";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-ID.Text", "Id");
                c.DataType = typeof(long);
                c.IsKey = true;
                c.IsEditable = false;
                c.IsVisible = false;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);
                
                c = new DynamicGridColumnSpec();
                c.Name = "CouponName";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-CouponName.Text", "CouponName");
                c.DataType = typeof(string);
                c.IsEditable = false;
                c.IsVisible = true;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);

                c = new DynamicGridColumnSpec();
                c.Name = "CertificateNmbr";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-CertificateNmbr.Text", "CertificateNmbr");
                c.DataType = typeof(string);
                c.IsEditable = false;
                c.IsVisible = false;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);
                
                c = new DynamicGridColumnSpec();
                c.Name = "DateIssued";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-DateIssued.Text", "DateIssued");
                c.DataType = typeof(DateTime);
                c.IsEditable = false;
                c.EditControlType = DynamicGridColumnSpec.DATE;
                c.IsVisible = true;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);

                c = new DynamicGridColumnSpec();
                c.Name = "DateRedeemed";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-DateRedeemed.Text", "DateRedeemed");
                c.DataType = typeof(DateTime);
                c.EditControlType = DynamicGridColumnSpec.DATE;
                c.IsVisible = true;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);

                c = new DynamicGridColumnSpec();
                c.Name = "ExpiryDate";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-ExpiryDate.Text", "ExpiryDate");
                c.DataType = typeof(DateTime);
                c.EditControlType = DynamicGridColumnSpec.DATE;
                c.IsVisible = true;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);

                c = new DynamicGridColumnSpec();
                c.Name = "Status";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Status.Text", "Status");
                c.DataType = typeof(string);
                c.IsVisible = true;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);
                
                c = new DynamicGridColumnSpec();
                c.Name = "UsesLeft";
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-UsesLeft.Text", "TimesUsed");
                c.DataType = typeof(long);
                c.IsVisible = true;
                c.IsEditable = false;
                colList.Add(c);
                //columnMap.Add(c.Name, c.Name);

                return colList.ToArray<DynamicGridColumnSpec>();
            }
            else
            {
                int intLength = _config.ListFieldsToShow.Count + 1;
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
                //columnMap.Add(c.Name, c.Name);

                foreach (ConfigurationItem attribute in _config.ListFieldsToShow)
                {
                    string displayText = string.Empty;
                    if (!string.IsNullOrEmpty(attribute.ResourceKey))
                    {
                        displayText = ResourceUtils.GetLocalWebResource(ParentControl, attribute.ResourceKey, attribute.DisplayText);
                    }
                    if (string.IsNullOrEmpty(displayText))
                    {
                        displayText = string.IsNullOrEmpty(attribute.DisplayText) ? attribute.AttributeName : attribute.DisplayText;
                    }

                    c = new DynamicGridColumnSpec();
                    //c.Name = attribute.DataKey;
                    c.Name = attribute.AttributeName;
                    c.DisplayText = displayText;                    
                    c.DataType = typeof(string);
                    c.IsKey = false;
                    if (attribute.AttributeName == "DateRedeemed" ||
                        attribute.AttributeName == "ExpiryDate" ||
                        attribute.AttributeName == "TimesUsed")
                    {
                        c.IsEditable = true;
                        if (attribute.AttributeName == "DateRedeemed" ||
                            attribute.AttributeName == "ExpiryDate")
                        {
                            c.DataType = typeof(DateTime);
                            c.EditControlType = DynamicGridColumnSpec.DATE;
                        }
                    }
                    else
                    {
                        c.IsEditable = false;
                    }
                    c.IsVisible = true;
                    c.IsSortable = true;
                    columns[count] = c;
                    count++;
                    //columnMap.Add(attribute.DataKey, attribute.AttributeName);
                }
                return columns;
            }
        }

        public override bool IsButtonVisible(string commandName)
        {
            if (commandName == "AddNew" || commandName == "DeleteRow")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsButtonEnabled(string commandName, object key)
        {
            if (commandName == AspDynamicGrid.DELETE_ROW_COMMAND || 
                commandName == AspDynamicGrid.ADDNEW_COMMAND ||
                commandName == AspDynamicGrid.SELECT_COMMAND )
            {
                return false;
            }
            else if (commandName == AspDynamicGrid.EDIT_ROW_COMMAND)
            {
                long id = long.Parse(key.ToString());
                MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
                return mc.IsActive() ? _config.EnableEditCard : false;
            }
            else if (commandName == "Redeem")
            {
                if (_config.EnableRedeemCard)
                {
                    long id = long.Parse(key.ToString());
                    MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
                    return mc.IsActive();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }            
        }

        public override string GetEmptyGridMessage()
        {
            if (_config != null && !string.IsNullOrEmpty(_config.EmptyResultMessageResourceKey))
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, _config.EmptyResultMessageResourceKey);
            }
            else
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EmptyResultMessage.Text");
            }
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
                _startDate = (DateTime)parmValue;
            }
            else if (parmName == "ToDate")
            {
                _endDate = (DateTime)parmValue;
                _endDate = _endDate.AddDays(1).AddTicks(-1);
            }
            else if (parmName == "Member")
            {
                _m = (Member)parmValue;
            }
            else if (parmName == "Config")
            {
                _config = (CouponsGridConfig)parmValue;
            }            
        }

        public override List<DynamicGridFilter> GetFilters()
        {
            var filters = new List<DynamicGridFilter>();
            filters.Add(new DynamicGridFilter(ResourceUtils.GetLocalWebResource(ParentControl, "CouponStatusLabel.Text", "Coupon Status:"),
                FilterDisplayTypes.DropDownList,
                ResourceUtils.GetLocalWebResource(ParentControl, "DefaultSelect.Text", "-- Select --"),
                ResourceUtils.GetLocalWebResource(ParentControl, "AllFilter.Text", "All"),
                ResourceUtils.GetLocalWebResource(ParentControl, "ActiveFilter.Text", "Active"),
                ResourceUtils.GetLocalWebResource(ParentControl, "RedeemedFilter.Text", "Redeemed"),
                ResourceUtils.GetLocalWebResource(ParentControl, "ExpiredFilter.Text", "Expired")));            
            return filters;
        }

        public override void SetFilter(string filterName, string filterValue)
        {
            if (filterName == ResourceUtils.GetLocalWebResource(ParentControl, "CouponStatusLabel.Text", "Coupon Status:"))
                _couponFilter = filterValue;
        }

        public override void LoadGridData()
        {            
            LoadCoupons();
        }

        public override int GetNumberOfRows()
        {
            return _coupons != null ? _coupons.Count() : 0;            
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            MemberCoupon mc = _coupons.ElementAt<MemberCoupon>(rowIndex);
            return GetColumnData(mc, column, LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel());            
        }

        public override object GetColumnData(object keyVal, DynamicGridColumnSpec column)
        {
            long id = long.Parse(keyVal.ToString());
            MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
            return GetColumnData(mc, column, LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel());
        }

        public override bool Validate(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            string methodName = "Validate";
            bool valid = true;

            if (gridAction != AspDynamicGrid.GridAction.Update)
            {
                string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "MemberCouponNotCreatedMessage.Text", "Member coupon cannot be created."));
                _logger.Error(_className, methodName, errMsg);
                throw new LWValidationException(errMsg);
            }

            MemberCoupon coupon = null;
            long couponId = 0;
            if (columns[0].Data != null)
            {
                couponId = long.Parse(columns[0].Data.ToString());
            }
            if (couponId != 0)
            {
                coupon = LoyaltyService.GetMemberCoupon(couponId);
            }
            CouponDef cdef = ContentService.GetCouponDef(coupon.CouponDefId);

            DateTime? redeemDate = null;
            DateTime? expiryDate = coupon.ExpiryDate;
            long timesUsed = coupon.TimesUsed;

            DynamicGridColumnSpec redeemColumn = null;
            DynamicGridColumnSpec expiryColumn = null;
            DynamicGridColumnSpec timesColumn = null;

            foreach (DynamicGridColumnSpec column in columns)
            {
                //string columnName = columnMap[column.Name];
                string columnName = column.Name;
                if (columnName == "DateRedeemed")
                {
                    redeemColumn = column;
                    if (column.Data != null)
                    {
                        if (column.Data.GetType() == typeof(string))
                        {
                            if (!string.IsNullOrEmpty((string)column.Data))
                            {
                                redeemDate = DateTime.Parse((string)column.Data);
                            }
                        }
                        if (column.Data.GetType() == typeof(DateTime))
                        {
                            redeemDate = (DateTime)column.Data;
                        }
                        redeemDate = DateTimeUtil.GetBeginningOfDay((DateTime)redeemDate);
                    }
                    else
                    {
                        redeemDate = DateTime.Today;
                    }                                        
                }
                else if (columnName == "ExpiryDate")
                {
                    expiryColumn = column;
                    if (column.Data != null)
                    {
                        if (column.Data.GetType() == typeof(string))
                        {
                            if (!string.IsNullOrEmpty((string)column.Data))
                            {
                                expiryDate = DateTime.Parse((string)column.Data);
                            }
                        }
                        if (column.Data.GetType() == typeof(DateTime))
                        {
                            expiryDate = (DateTime)column.Data;
                        }
                        if (expiryDate.HasValue)
                        {
                            expiryDate = DateTimeUtil.GetEndOfDay(expiryDate.Value);
                        }
                    }
                }
                else if (columnName == "TimesUsed" && column.Data != null)
                {
                    timesColumn = column;
                    try
                    {
                        timesUsed = long.Parse(column.Data.ToString());                                                
                    }
                    catch (FormatException)
                    {
                        string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "TimesUsedNotNumeric.Text", "Please provide a valid numeric value for TimesUsed."));
                        _logger.Error(_className, methodName, errMsg);
                        OnValidationError(errMsg, column);
                        valid = false;
                    }
                }
            }

            if (redeemDate != null && expiryDate != null &&
                        DateTimeUtil.LessThan(expiryDate.Value, (DateTime)redeemDate))
            {
                string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "InvalidRedeemDate.Text", "Redeem date cannot be greater than expiriation date."));
                _logger.Error(_className, methodName, errMsg);
                OnValidationError(errMsg, redeemColumn);
                valid = false;
            }

            if (timesUsed < 0)
            {
                string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "InvalidTimesUsed.Text", "The TimesUsed cannot be < 0."));
                _logger.Error(_className, methodName, errMsg);
                OnValidationError(errMsg, timesColumn);
                valid = false;
            }

            if (timesUsed > cdef.UsesAllowed)
            {
                string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "TimesUsedLeft.Text", "Only {0} uses are left."), cdef.UsesAllowed - coupon.TimesUsed);
                _logger.Error(_className, methodName, errMsg);
                OnValidationError(errMsg, timesColumn);
                valid = false;
            }

            return valid;
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            MemberCoupon coupon = null;
            long couponId = 0;
            if (columns[0].Data != null)
            {
                couponId = long.Parse(columns[0].Data.ToString());
            }
            if (couponId != 0)
            {
                coupon = LoyaltyService.GetMemberCoupon(couponId);
            }

            CouponDef cdef = ContentService.GetCouponDef(coupon.CouponDefId);
            
            foreach (DynamicGridColumnSpec column in columns)
            {
                //string columnName = columnMap[column.Name];
                string columnName = column.Name;
                if (columnName == "CertificateNmbr")
                {
                    coupon.CertificateNmbr = (string)column.Data;
                }                
                else if (columnName == "DateRedeemed")
                {
                    DateTime? redeemDate = null;
                    if (column.Data != null)
                    {
                        if (column.Data.GetType() == typeof(string))
                        {
                            if (!string.IsNullOrEmpty((string)column.Data))
                            {
                                redeemDate = DateTime.Parse((string)column.Data);
                            }
                        }
                        if (column.Data.GetType() == typeof(DateTime))
                        {
                            redeemDate = (DateTime)column.Data;
                        }
                        redeemDate = DateTimeUtil.GetBeginningOfDay((DateTime)redeemDate);
                    }
                    else
                    {
                        redeemDate = DateTime.Today;
                    }
                    
                    if (redeemDate != null)
                    {
                        coupon.DateRedeemed = redeemDate.Value;
                    }
                }
                else if (columnName == "ExpiryDate")
                {
                    if (column.Data != null)
                    {
                        if (column.Data.GetType() == typeof(string))
                        {
                            if (!string.IsNullOrEmpty((string)column.Data))
                            {
                                coupon.ExpiryDate = DateTime.Parse((string)column.Data);
                            }
                        }
                        if (column.Data.GetType() == typeof(DateTime))
                        {
                            coupon.ExpiryDate = (DateTime)column.Data;
                        }
                        if (coupon.ExpiryDate.HasValue)
                        {
                            coupon.ExpiryDate = DateTimeUtil.GetEndOfDay(coupon.ExpiryDate.Value);
                        }
                    }
                }
                else if (columnName == "TimesUsed" && column.Data != null)
                {
                    coupon.TimesUsed = long.Parse(column.Data.ToString());                    
                    if (coupon.TimesUsed >= cdef.UsesAllowed)
                    {
                        coupon.Status = CouponStatus.Redeemed;
                    }
                }                                
            }

            LoyaltyService.UpdateMemberCoupon(coupon);
        }
        #endregion

        #region Custom Actions
        public override ICustomGridAction[] GetCustomCommands()
        {
            if (actions == null)
            {
                if (_config == null || _config.AllowEdits)
                {
                    //actions = new ICustomGridAction[1];
                    //actions[0] = new RedeemCouponCommand(this);                    
                }
            }
            return actions;
        }
        #endregion

    }
}