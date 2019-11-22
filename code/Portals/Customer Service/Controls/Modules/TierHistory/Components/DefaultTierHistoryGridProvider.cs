using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.LWModules.TierHistory.Components
{
    public class DefaultTierHistoryGridProvider : AspGridProviderBase
    {
        private DateTime _startDate = DateTimeUtil.MinValue;
        private DateTime _endDate = DateTimeUtil.MaxValue;
        private TierHistoryConfiguration _config = null;
        private List<MemberTier> _memberTiers = null;

        public override void LoadGridData()
        {
            Member member = PortalState.CurrentMember;
            _memberTiers = member.GetTiers() as List<MemberTier>;
            if(_memberTiers != null && (_startDate != DateTimeUtil.MinValue || _endDate != DateTimeUtil.MaxValue))
            {
                _memberTiers = (from a in _memberTiers where a.FromDate <= _endDate && a.ToDate >= _startDate select a).ToList();
            }
            _memberTiers.Sort((a, b) => a.FromDate.CompareTo(b.FromDate));
        }

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
            else if (parmName == "Config")
            {
                _config = (TierHistoryConfiguration)parmValue;
            }
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            var memberTier = _memberTiers[rowIndex];
            return GetColumnData(memberTier, column.Name);
        }

        private object GetColumnData(MemberTier memberTier, string columnName)
        {
            if (memberTier.TierDef == null)
                memberTier.TierDef = LoyaltyService.GetTierDef(memberTier.TierDefId);

            object val = null;

            if (columnName == "Id")
            {
                return memberTier.Id;
            }

            ConfigurationItem item = null;
            foreach (var i in _config.TierFieldsToShow)
            {
                if (i.DataKey == columnName)
                {
                    item = i;
                    break;
                }
            }

            switch (item.AttributeName)
            {
                // TierDef
                case "Name":
                    val = memberTier.TierDef.Name;
                    break;
                case "DisplayText":
                    val = memberTier.TierDef.DisplayText;
                    break;
                case "Tier Description":
                    val = memberTier.TierDef.Description;
                    break;

                // MemberTier
                case "FromDate":
                    val = memberTier.FromDate;
                    break;
                case "ToDate":
                    val = memberTier.ToDate;
                    break;
                case "Member Tier Description":
                    val = memberTier.Description;
                    break;
            }

            return val;
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            List<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "Id";
            c.DataType = typeof(long);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            c.IsSortable = false;
            colList.Add(c);

            if (_config != null)
            {
                foreach(var configItem in _config.TierFieldsToShow)
                {
                    string displayText = string.Empty;
                    if (!string.IsNullOrEmpty(configItem.ResourceKey))
                    {
                        displayText = ResourceUtils.GetLocalWebResource(ParentControl, configItem.ResourceKey, configItem.DisplayText);
                    }
                    if (string.IsNullOrEmpty(displayText))
                    {
                        displayText = string.IsNullOrEmpty(configItem.DisplayText) ? configItem.AttributeName : configItem.DisplayText;
                    }

                    c = new DynamicGridColumnSpec();
                    c.Name = configItem.DataKey;
                    c.DisplayText = displayText;
                    c.FormatString = configItem.Format;
                    c.IsKey = false;
                    c.IsEditable = false;
                    c.IsVisible = true;
                    c.IsSortable = configItem.IsSortable;
                    colList.Add(c);
                }
            }

            return colList.ToArray();
        }

        public override int GetNumberOfRows()
        {
            return _memberTiers != null ? _memberTiers.Count : 0;
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            throw new NotImplementedException();
        }

        protected override string GetGridName()
        {
            return "TierHistory";
        }

        public override bool IsGridEditable()
        {
            return false;
        }
    }
}