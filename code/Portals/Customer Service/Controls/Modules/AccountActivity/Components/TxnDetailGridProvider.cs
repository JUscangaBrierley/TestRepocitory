using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.AccountActivity.Components
{
	public class TxnDetailGridProvider : AspGridProviderBase, INestedGridProvider
	{
		private static string _className = "TxnDetailGridProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private long _hdrRowKey;
		private IList<TransactionDetail> _details = null;        
		private AccountActivityConfiguration _config = null;
        private bool _ruleResultRequired = false;

		public TxnDetailGridProvider(long keyVal)
		{
			_hdrRowKey = keyVal;
		}

		public long ParentKey
		{
			set { _hdrRowKey = value; }
		}

		#region Grid properties

		public override string Id
		{
			get { return "grdTxnDetail"; }
		}

        protected override string GetGridName()
        {
            return "TxnDetail";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "RowKey";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RowKey.Text", "RowKey");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

			foreach (ConfigurationItem attribute in _config.DetailFieldsToShow)
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

				if (attribute.AttributeName == "DtlSaleAmount")
				{
					c.DataType = typeof(double);
					c.SubtotalFormat = "Total Spend: ${0:0.00}";
				}
                if (attribute.AttributeName == "RuleExecution")
                {
                    _ruleResultRequired = true;
                }
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

		#region Child Grid
		public IDynamicGridProvider GetChildGridProvider(object keyVal)
		{
			if (keyVal == null || !(keyVal is long))
			{
				return null;
			}
			string methodName = "GetChildGridProvider";
			if (_config != null &&
				!string.IsNullOrEmpty(_config.DetailsPointTxnProviderAssemblyName) &&
				!string.IsNullOrEmpty(_config.DetailsPointTxnProviderClassName))
			{
				object childProvider = ClassLoaderUtil.CreateInstance(_config.DetailsPointTxnProviderAssemblyName, _config.DetailsPointTxnProviderClassName);
				if (childProvider != null)
				{
					PropertyInfo pinfo = childProvider.GetType().GetProperty("ParentKey");
					if (pinfo != null)
					{
						pinfo.SetValue(childProvider, keyVal, null);
						return (IDynamicGridProvider)childProvider;
					}
					else
					{
						string errMsg = string.Format("Child grid for TxnDetails does not define property ParentKey.");
						_logger.Error(_className, methodName, errMsg);
						throw new LWException(errMsg);
					}
				}
				else
				{
					string errMsg = string.Format("Error creating child grid for TxnHeaders.");
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
			}
			else
			{
				DetailPointTxnGridProvider childProvider = new DetailPointTxnGridProvider((long)keyVal);
				childProvider.Config = _config;
				childProvider.ParentControl = "~/Controls/Modules/AccountActivity/ViewAccountActivity.ascx";
				return childProvider;
			}
		}

		public NestingTypes NestingType
		{
			get { return NestingTypes.DataBound; }
		}

		public bool HasChildren(object keyVal)
		{
			if (_config.PointTxnFieldsToShow.Count == 0 || keyVal == null)
			{
				return false;
			}
			if (keyVal is long)
			{
				long rowkey = (long)keyVal;
				foreach (TransactionDetail detail in _details)
				{
					if (detail.RowKey == rowkey)
					{
                        if (detail.DetailRecord.HasTransientProperty("PointsHistory"))
                        {
                            IList<PointTransaction> txns = (IList<PointTransaction>)detail.DetailRecord.GetTransientProperty("PointsHistory");
                            return txns != null && txns.Count > 0 ? true : false;
                        }
                        else
                        {
                            return false;                            
                        }
					}
				}
			}
			return false;
		}
		#endregion

		#region Grid Data Source
		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (parmName == "Config")
			{
				_config = (AccountActivityConfiguration)parmValue;
			}
		}

		public override void LoadGridData()
		{
			IList<TransactionHeader> _headers = (IList<TransactionHeader>)PortalState.GetFromCache("Headers");
			if (_headers != null && _headers.Count > 0)
			{
				foreach (TransactionHeader hdr in _headers)
				{
					if (hdr.TxnHeader.RowKey == _hdrRowKey)
					{
						_details = hdr.Details;                        
						PortalState.PutInCache("Details", _details);
                        if (_ruleResultRequired && _details != null)
                        {
                            foreach (TransactionDetail detail in _details)
                            {
                                if (detail.DetailRecord != null)
                                {
                                    long[] rowkeys = new long[] { detail.DetailRecord.MyKey };
                                    VirtualCard vc = detail.DetailRecord.Parent.Parent as VirtualCard;
                                    Member member = vc.Member;
                                        IList<RuleExecutionLog> logs = LoyaltyService.GetRuleExecutionLogs(
                                            member.IpCode, null,
                                            PointTransactionOwnerType.AttributeSet, detail.DetailRecord.GetMetaData().ID,
                                            rowkeys, null, null);
                                        if (logs != null && logs.Count > 0)
                                        {
                                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                            foreach (RuleExecutionLog log in logs)
                                            {
                                                if (_config.DisplayRuleExecutionLog(log))
                                                {
                                                    if (sb.Length > 0)
                                                    {
                                                        sb.Append("\n");
                                                    }
                                                    sb.Append(log.Detail);
                                                }
                                            }
                                            detail.RuleResult = sb.ToString();
                                        }
                                }
                            }
                        }
						break;
					}
				}
			}
		}

		public override int GetNumberOfRows()
		{
			return _details != null ? _details.Count : 0;            
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
            TransactionDetail detail = _details[rowIndex];
            if (detail.PointTransaction != null)
            {
                return GetColumnData(detail.PointTransaction, column);
            }
            else
            {
                return GetColumnData(detail, column);
            }                        
		}

        //TransactionDetail detail
		private object GetColumnData(TransactionDetail detail, DynamicGridColumnSpec column)
		{
            string methodName = "GetColumnData";

            IClientDataObject txnDetail = detail.DetailRecord;
			if (column.Name == "RowKey")
			{
                return txnDetail.RowKey;
			}

			ConfigurationItem item = null;
			foreach (var i in _config.DetailFieldsToShow)
			{
				if (i.DataKey == column.Name)
				{
					item = i;
					break;
				}
			}

			if (item == null || item.AttributeType == ItemTypes.PointTransaction)
			{
				return null;
			}

            if (item.AttributeName == "RuleExecution")
            {
                return detail.RuleResult;
            }

            object val = null;
            if (item.AttributeType == ItemTypes.BScript)
            {
                if (!string.IsNullOrWhiteSpace(item.BScriptExpression))
                {
                    try
                    {
                        ContextObject ctx = new ContextObject() { Owner = txnDetail.Parent.Parent, InvokingRow = txnDetail };
                        ExpressionFactory exprF = new ExpressionFactory();
                        Expression expression = exprF.Create(item.BScriptExpression);
                        val = expression.evaluate(ctx);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName, string.Format("Error evaluating {0}.", item.AttributeName), ex);
                        throw;
                    }
                }
            }
            else
            {                
                try
                {
                    val = txnDetail.GetAttributeValue(item.AttributeName);
                }
                catch (Exception) { }

                if (item.AttributeName == "DtlProductId" && val != null)
                {
                    if (!string.IsNullOrEmpty(val.ToString()))
                    {
                        long productId = long.Parse(val.ToString());
                        Product p = ContentService.GetProduct(productId);
                        if (p != null)
                        {
                            val = p.ShortDescription;
                        }
                    }
                }
            }
            return val;           
		}


        private object GetColumnData(PointTransaction pt, DynamicGridColumnSpec column)
        {
            object val = null;

            ConfigurationItem item = null;
            foreach (var i in _config.DetailFieldsToShow)
            {
                if (i.DataKey == column.Name)
                {
                    item = i;
                    break;
                }
            }

            if (item == null || item.AttributeType != ItemTypes.PointTransaction)
            {
                return null;
            }

            switch (item.AttributeName)
            {
                case "PointTypeId":
                    PointType c = LoyaltyService.GetPointType(pt.PointTypeId);
                    val = c.Name;
                    break;
                case "PointEventId":
                    PointEvent pe2 = LoyaltyService.GetPointEvent(pt.PointEventId);
                    val = pe2.Name;
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
                case "RuleExecution":
                    // since there is no such attribute here but this may be called so just return empty string.
                    val = string.Empty;
                    break;
                default:
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
