using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Threading;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal.Util;
using Brierley.WebFrameWork.Portal.Configuration.Modules.RequestCredit;


namespace Brierley.LWModules.RequestCredit.Components
{
    public class RequestCreditListProvider : AspListProviderBase
    {
        #region Fields
        private const string _className = "RequestCreditListProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private Member _member = null;

		private RequestCreditConfig _config = null;
		private IList<IClientDataObject> refCardTypes = null;

        protected IList<IClientDataObject> HistoryRecords { get; set; }
        protected TransactionType TransactionType { get; set; }
        protected Dictionary<String, String> SearchParm { get; set; }

        private IRequestCreditInterceptor _helper = null;
        private ModuleConfigurationKey _configKey = null;
        #endregion

        #region Helpers
        internal Member Member
        {
            get
            {
                if (_member == null)
                {
                    _member = PortalState.CurrentMember;
                }
                return _member;
            }
        }

        private IRequestCreditInterceptor GetHelper()
        {
            string methodName = "GetHelper";

            if (_helper == null)
            {
                if (_config == null)
                {
                    string msg = ResourceUtils.GetLocalWebResource(ParentControl, "NoHelperConfiguration.Text", "No configuration set for creating helper.");
                    _logger.Error(_className, methodName, "No configuration set for creating helper.");
                    throw new LWException(msg) { ErrorCode = 1 };
                }
                _helper = RequestCreditHelper.CreateRequestCreditInterceptor(_config.HelperClassName, _config.HelperAssemblyName);
            }
            return _helper;
        }

        internal decimal AddLoyaltyTransaction(Member member, string txnHeaderId)
        {
            return GetHelper().AddLoyaltyTransaction(member, string.Empty, txnHeaderId);
        }
        #endregion

        #region Public Properties
        public virtual ModuleConfigurationKey ConfigurationKey { get; set; }        
        #endregion

        #region List Properties
        public override string Id
		{
            get { return "lstRequestCredit"; }
		}

		public override bool IsListEditable()
		{
			return false;
		}

		public override IEnumerable<DynamicListItem> GetListItemSpecs()
		{
			if (_config == null)
			{
				return GetItemSpecs(null);
			}            
			return GetItemSpecs(_config.ResultsAttributes);
		}
		
		private IEnumerable<DynamicListItem> GetItemSpecs(List<ConfigurationItem> itemsToShow)
		{
			IList<DynamicListItem> colList = new List<DynamicListItem>()
			{
				new DynamicListColumnSpec("RowKey", null, typeof(long), null, false) { IsKey = true}
			};

			if (itemsToShow == null || itemsToShow.Count == 0)
			{
				return colList;
			}

			foreach (ConfigurationItem item in itemsToShow)
			{
				if (item.AttributeType == ItemTypes.DynamicListCommandButton)
				{
					DynamicListCommandSpec command = new DynamicListCommandSpec(new ListCommand(item.AttributeName), null) { BeginHtml = item.BeginHtml, EndHtml = item.EndHtml };
					switch (item.AttributeName)
					{						
                        case "RequestCredit":
                            command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RequestCredit.Text", "Request Credit");
                            command.CommandClicked += command_CommandClicked;
							break;						
					}
					if (!string.IsNullOrEmpty(item.DisplayText))
					{
						command.Text = item.DisplayText;
					}
					if (!string.IsNullOrEmpty(item.ResourceKey))
					{
						command.Text = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, command.Text);
					}
					command.CssClass = item.ControlCSSClass;
					colList.Add(command);
				}
                else
                {
					string displayText = item.DisplayText;
					if (!string.IsNullOrEmpty(item.ResourceKey))
					{
						displayText = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, item.DisplayText);
					}
					else
					{
						displayText = ResourceUtils.GetLocalWebResource(ParentControl, item.DisplayText, item.DisplayText);
					}


                    var c = new DynamicListColumnSpec(item.AttributeName, displayText, string.IsNullOrEmpty(item.LabelCssClass) ? item.ControlCSSClass : item.LabelCssClass);
                    c.BeginHtml = item.BeginHtml;
                    c.EndHtml = item.EndHtml;
					c.FormatString = item.Format;
                    colList.Add(c);
                }
			}
			return colList;
		}
        
		public override bool IsButtonEnabled(ListCommand command, object key)
		{
            bool isEnabled = command.CommandName == "RequestCredit";
            return isEnabled;
		}


		public override string GetEmptyListMessage()
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
			if (parmName == "Configuration")
			{
				_config = parmValue as RequestCreditConfig;
			}
            else if (parmName == "TxnType")
            {
                TransactionType = (TransactionType)parmValue;
            }
            else if (parmName == "SearchParms")
            {
                SearchParm = (Dictionary<String, String>)parmValue;
            }
		}

		public override void LoadListData()
		{
            _logger.Trace(_className, "LoadGrid", "Load grid data.");
            HistoryRecords = GetHelper().SearchTransaction(TransactionType, SearchParm, _config.ProcessIdSuppressionList, null);
		}

		public override int GetNumberOfRows()
		{
            return HistoryRecords.Count;
		}

		public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
		{
            IClientDataObject historyData = HistoryRecords[rowIndex];
            if (!string.IsNullOrWhiteSpace(column.FormatString))
            {
                return string.Format(column.FormatString, historyData.GetAttributeValue(column.Name));
            }
            else
            {
                return historyData.GetAttributeValue(column.Name);
            }            
		}
        
		public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
		{
			
		}

		public override string GetAppPanelTotalText(int totalRecords)
		{
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoTransactionsMessage.Text", "No Transactions");
            }
            else
            {
                return string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "Total.Text", "Total") + " {0} ", totalRecords);
            }
        }
        #endregion

        #region Custom Commands

        //private HiddenField _hdnId = null;
        //private Label _lbl1 = null;
        //private Label _lbl2 = null;
        public override void CreateCommandControls(ListCommand command, System.Web.UI.Control container)
        {
            if (command == "RequestCredit")
            {
                var div = new HtmlGenericControl("div");
                div.Attributes.Add("style", "padding: 20px;");

                //_lbl1 = new Label();
                //div.Controls.Add(_lbl1);
                //div.Controls.Add(new LiteralControl("</br>"));
                //_lbl2 = new Label();
                //div.Controls.Add(_lbl2);

                //div.Controls.Add(new LiteralControl("<div class=\"Buttons\">"));

                //var lnkSend = new LinkButton() { CssClass = "Button", Text = "Send" };
                //lnkSend.Click += lnkSend_Click;
                //div.Controls.Add(lnkSend);

                //var lnkCancel = new LinkButton() { CssClass = "Button", Text = "Cancel" };
                //lnkCancel.Click += delegate { OnCustomPanelFinished(); };
                //div.Controls.Add(lnkCancel);

                //div.Controls.Add(new LiteralControl("</div>"));

                //_hdnId = new HiddenField();
                //div.Controls.Add(_hdnId);

                container.Controls.Add(div);
            }            
        }

        void command_CommandClicked(object sender, ListCommand command, object keyVal)
        {
            //LoadListData();
            ////_hdnId.Value = keyVal.ToString();

            //try
            //{
            //    ApplyTransaction(keyVal.ToString());
            //}
            //catch (LWValidationException)
            //{
            //    throw;
            //}
            //catch (Exception)
            //{
            //    //_logger.Error(_className, methodName, ex.Message, ex, true);
            //    throw;
            //}
            
        }

        //private void ApplyTransaction(string rowKey)
        //{
        //    const string methodName = "ApplyTransaction";
        //    string defaultAmount = string.Empty;
        //    string TxnHeaderId = string.Empty;

        //    LWCriterion lwCriteria = new LWCriterion("HistoryTxnDetail");
        //    lwCriteria.Add(LWCriterion.OperatorType.AND, "RowKey", rowKey, LWCriterion.Predicate.Eq);
        //    IList<IClientDataObject> oHistoryRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, null, false);

        //    IClientDataObject historyTxnDetail = oHistoryRecords[0];
        //    TxnHeaderId = (string)historyTxnDetail.GetAttributeValue("TxnHeaderId");

        //    object processId = historyTxnDetail.GetAttributeValue("ProcessId");
        //    if (processId != null && processId is long && (long)processId == (long)ProcessCode.RequestCreditApplied)
        //    {   
        //        //TODO: Show positive message here
        //        //lblApliedMsg.Visible = true;
        //        return;
        //    }
            
        //    double pointsEarned = AddLoyaltyTransaction(_member, TxnHeaderId);

        //    if (_config.AddGlobalNotes && PortalState.Portal.PortalMode == PortalModes.CustomerService)
        //    {
        //        var note = new CSNote() { MemberId = _member.IpCode, CreateDate = DateTime.Now };
        //        if (string.IsNullOrEmpty(_config.GlobalNoteFormat))
        //        {
        //            note.Note = "Transaction in the amount of " + defaultAmount + " was requested";
        //        }
        //        else
        //        {
        //            note.Note = string.Format(_config.GlobalNoteFormat,
        //                historyTxnDetail.GetAttributeValue("TxnNumber"),
        //                defaultAmount,
        //                historyTxnDetail.GetAttributeValue("TxnStoreId"));
        //        }
        //        CSAgent agent = PortalState.GetLoggedInCSAgent();
        //        note.CreatedBy = agent.Id;
        //        ILWCSService inst = LWDataServiceUtil.CSServiceInstance();
        //        inst.CreateNote(note);
        //    }
        //    IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, _member);
        //    //TODO: SHow positive message here
        //    //lblSuccess.Text = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, _config.SuccessMessageResourceKey), pointsEarned);
        //    //lblSuccess.Visible = true;

        //}       
        #endregion
    }
}