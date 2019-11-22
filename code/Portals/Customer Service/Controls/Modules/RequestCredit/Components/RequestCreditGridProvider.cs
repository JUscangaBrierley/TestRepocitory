using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules.RequestCredit;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.RequestCredit.Components
{
    public class RequestCreditGridProvider : AspGridProviderBase
    {
        #region Fields
        private const string _className = "RequestCreditGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private IRequestCreditInterceptor _helper = null;        
        #endregion

        #region General Properties
        protected IList<IClientDataObject> HistoryRecords { get; set; }
		protected TransactionType TransactionType { get; set; }
		protected Dictionary<String, String> SearchParm { get; set; }

        public RequestCreditConfig Configuration { get; set; }         
        #endregion

        #region Grid Properties
        public override string Id
        {
            get { return "grdRequestCredit"; }
        }

        protected override string GetGridName()
        {
            return "RequestCredit";
        }

        public override bool IsGridRowSelectable()
        {
            return true;
        }

        public override bool IsButtonVisible(String commandName)
        {
            return commandName == AspDynamicGrid.SELECT_COMMAND || commandName == AspDynamicGrid.EDIT_COMMAND;
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            int intLength = Configuration.ResultsAttributes.Count + 1;
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[intLength];
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "RowKey";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RowKey.Text", "RowKey");
            c.DataType = typeof(string);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            c.IsSortable = false;
            columns[0] = c;
            int count = 1;

            foreach (ConfigurationItem attribute in Configuration.ResultsAttributes)
            {
                c = new DynamicGridColumnSpec();
                c.Name = attribute.AttributeName;
                c.DisplayText = attribute.DisplayText;
                string resKey = string.Format("{0}-{1}.Text", Id, attribute.AttributeName);
                c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, resKey, attribute.DisplayText);
                c.FormatString = attribute.Format;
                c.DataType = typeof(string);
                c.IsKey = false;
                c.IsEditable = false;
                c.IsVisible = true;
                c.IsSortable = false;
                columns[count] = c;
                count++;
            }
            return columns;
        }

        /// <summary>
        /// Method to set insert label of grid.
        /// </summary>
        /// <returns></returns>
        public override String GetGridInsertLabel()
        {
            return string.Empty;
        }
        #endregion

        #region Construction
        public RequestCreditGridProvider()
		{
			TransactionType = FrameWork.Common.TransactionType.Store;            
		}
        #endregion

        #region Helpers
        protected virtual object GetColumnData(IClientDataObject dataObj,DynamicGridColumnSpec column)
        {
			if (!string.IsNullOrWhiteSpace(column.FormatString))
			{
				return string.Format(column.FormatString, dataObj.GetAttributeValue(column.Name));
			}
			else
			{
				return dataObj.GetAttributeValue(column.Name);
			}
        }

        private IRequestCreditInterceptor GetHelper()
        {
            string methodName = "GetHelper";

            if (_helper == null)
            {
                if (Configuration == null)
                {
                    string msg = ResourceUtils.GetLocalWebResource(ParentControl, "NoHelperConfiguration.Text", "No configuration set for creating helper.");
                    _logger.Error(_className, methodName, "No configuration set for creating helper.");
                    throw new LWException(msg) { ErrorCode = 1 };
                }
                _helper = RequestCreditHelper.CreateRequestCreditInterceptor(Configuration.HelperClassName, Configuration.HelperAssemblyName);
            }
            return _helper;
        }

        #endregion

        #region Public Methods
        public decimal AddLoyaltyTransaction(Member member, string txnHeaderId)
        {
            return GetHelper().AddLoyaltyTransaction(member, string.Empty, txnHeaderId);
        }        
        #endregion
                		
        #region Data Source

		public override void SetSearchParm(String parmName, object parmValue)
        {
            if (parmName == "TxnType")
            {
                TransactionType = (TransactionType)parmValue;
            }
            else if (parmName == "SearchParms")
            {
                SearchParm = (Dictionary<String, String>)parmValue;
            }            
        }

		public override void LoadGridData()
        {            
			_logger.Trace(_className, "LoadGrid", "Load grid data.");
            HistoryRecords = GetHelper().SearchTransaction(TransactionType, SearchParm, Configuration.ProcessIdSuppressionList, null);
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
        }

		public override int GetNumberOfRows()
        {
            return HistoryRecords.Count;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            IClientDataObject historyData = HistoryRecords[rowIndex];
            return GetColumnData(historyData, column);
        }

        #endregion
    }
}
