using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.FrameWork.Pdf;

namespace Brierley.LWModules.AccountActivity.Components
{    
    public class OrphanTxnGridProvider : AspGridProviderBase
    {
        #region Fields
        private const string _className = "OrphanTxnGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private DateTime _startDate = DateTimeUtil.MinValue;
        private DateTime _endDate = DateTimeUtil.MaxValue;

        private IList<PointTransaction> _orphans = null;
        private AccountActivityConfiguration _config = null;
        #endregion

        #region Private helpers
        private object GetPointTransactionData(PointTransaction pt, string columnKey)
        {
            object val = null;

            ConfigurationItem item = null;
            foreach (var i in _config.OrphanTxnFieldsToShow)
            {
                if (i.DataKey == columnKey)
                {
                    item = i;
                    break;
                }
            }

            if (item == null || item.AttributeType != ItemTypes.PointTransaction)
            {
                return null;
            }

            if (item.AttributeName == "PromoName")
            {
                if (!string.IsNullOrEmpty(pt.PromoCode))
                {
                    Promotion p = LoyaltyService.GetPromotionByCode(pt.PromoCode);
                    if (p != null)
                    {
                        val = p.Name;
                    }
                }
            }
            else
            {
                PropertyInfo prop = typeof(PointTransaction).GetProperty(item.AttributeName);
                object pval = prop.GetValue(pt, null);
                if (pval is DateTime)
                {
                    val = ((DateTime)pval).ToShortDateString();
                }
                else if (pval != null)
                {
                    val = pval.ToString();
                }
            }
            return val;
        }        
        #endregion

        #region Grid properties

        public override string Id
        {
            get { return "grdOrphanPointTxn"; }
        }

        protected override string GetGridName()
        {
            return "OrphanPointTxn";
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "Id";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Id.Text", "Id");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

			foreach (ConfigurationItem attribute in _config.OrphanTxnFieldsToShow)
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
                		
				c.Name = attribute.DataKey;
				c.DisplayText = displayText;
				c.FormatString = attribute.Format;
				c.IsKey = false;
				c.IsEditable = false;
				c.IsVisible = true;
				c.IsSortable = attribute.IsSortable;
				colList.Add(c);
			}

			return colList.ToArray<DynamicGridColumnSpec>();
        }

        public override bool IsGridEditable()
        {
            return false;
        }

        public override string GetEmptyGridMessage()
        {
			return ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EmptyResultMessage.Text");
        }
        #endregion

        #region Grid Data Source
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
                _config = (AccountActivityConfiguration)parmValue;
            }
        }

        public override void LoadGridData()
        {
            string methodName = "LoadGridData";

            Member member = PortalState.CurrentMember;
			if (member != null && member.LoyaltyCards != null && member.LoyaltyCards.Count > 0)
			{
                try
                {
                    _orphans = AccountActivityUtil.GetOtherPointsHistory(member, _startDate, _endDate, _config.OrphanTxnTypesFilter, _config.OrphanPointTypesFilter, _config.OrphanPointEventsFilter, _config.ShowExpiredTransactions, false,new LWQueryBatchInfo() { BatchSize = 1000, StartIndex = 0 });
                }
                catch (LWException ex)
                {
                    if (ex.ErrorCode != 3230)
                    {
                        _logger.Error(_className, methodName, "Error loading grid data.", ex);
                        throw;
                    }
                    else
                    {
                        _logger.Error(_className, methodName, ex.Message);
                    }
                }
			}
			else
			{
				_orphans = null;
			}
        }

        public override int GetNumberOfRows()
        {
            return _orphans != null ? _orphans.Count : 0;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            object val = null;
            PointTransaction pt = _orphans[rowIndex];

			if (column.Name == "Id")
			{
				return pt.Id;
			}

			ConfigurationItem item = null;
			foreach (var i in _config.OrphanTxnFieldsToShow)
			{
				if (i.DataKey == column.Name)
				{
					item = i;
					break;
				}
			}

			if (item == null)
			{
				return null;
			}

            switch (item.AttributeName)
            {
                case "VcKey":
                    // get the loyalty id
                    Member member = PortalState.CurrentMember;
                    VirtualCard vc = member.GetLoyaltyCard(pt.VcKey);
                    val = vc.LoyaltyIdNumber;
                    break;
                case "PointTypeId":
                    PointType c = LoyaltyService.GetPointType(pt.PointTypeId);
                    val = c.Name;
                    break;
                case "PointEventId":
                    PointEvent pe = LoyaltyService.GetPointEvent(pt.PointEventId);
                    val = pe.Name;
                    break;
                case "PromoName":
                    if (!string.IsNullOrEmpty(pt.PromoCode))
                    {
                        Promotion p = LoyaltyService.GetPromotionByCode(pt.PromoCode);
                        if (p != null)
                        {
                            val = p.Name;
                        }
                    }
                    break;
                default:
                    PropertyInfo prop = pt.GetType().GetProperty(item.AttributeName);
                    object pval = prop.GetValue(pt, null);
                    val = pval;
                    //if (pval is DateTime)
                    //{
                    //    //val = ((DateTime)pval).ToShortDateString();
                    //    val = pval;
                    //}
                    //else if ( pval is Decimal)
                    //{
                    //    val = pval;
                    //}
                    //else if(pval != null)
                    //{
                    //    val = pval.ToString();
                    //}
                    break;
            }
            return val;
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            throw new NotImplementedException();
        }
        #endregion        

        #region Printing
        public override bool GeneratePdfFromData(string fileName)
        {
            bool success = true;

            string pageTitle = "Appeasements & Bonuses";
            if (!string.IsNullOrEmpty(_config.OrphansLabelResourceKey))
            {
                pageTitle = ResourceUtils.GetLocalWebResource(ParentControl, _config.OrphansLabelResourceKey, pageTitle);
            }

            PdfPageHeader pageHeader = new PdfPageHeader();
            pageHeader.HeaderContent = new List<string>();
            pageHeader.HeaderContent.Add(pageTitle);

            pageHeader.HeaderFontAlignment = PdfDocument.PdfAlignment.Center;
            pageHeader.HeaderFontFamily = PdfDocument.PdfFontFamily.Helvetica;
            pageHeader.HeaderFontSize = 20;
            pageHeader.HeaderFontStyle = PdfDocument.PdfFontStyle.Italic;
            
            if (_orphans != null && _orphans.Count > 0)
            {
                PdfDocument doc = (PdfDocument)PortalState.GetFromCache("AccountActivityPdf");
                doc.NewPage();
                doc.SetPageHeader(pageHeader);
                doc.NewPage();
                if ( doc != null )
                {                                        
                    bool first = true;
                    foreach (PointTransaction txn in _orphans)
                    {
                        if (first)
                        {
                            doc.EmptyLine();
                            first = false;
                        }
                        doc.WriteLine("Txn Id: " + txn.Id.ToString());
                        doc.LineSeparator();
                        foreach (ConfigurationItem attribute in _config.OrphanTxnFieldsToShow)
                        {                            
                            object value = GetPointTransactionData(txn, attribute.DataKey);
                            if (value != null)
                            {
                                string displayText = string.IsNullOrEmpty(attribute.DisplayText) ? attribute.AttributeName : attribute.DisplayText; ;
                                if (!string.IsNullOrEmpty(attribute.ResourceKey))
                                {
                                    displayText = ResourceUtils.GetLocalWebResource(ParentControl, attribute.ResourceKey, attribute.DisplayText);
                                }
                                doc.WriteLine("    " + displayText + " : " + value.ToString());
                            }
                        }                        
                        doc.NewLine();
                    }
                }
            }
            return success;
        }
        #endregion
    }
}
