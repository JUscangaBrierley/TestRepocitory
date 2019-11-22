using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LWModules.RequestCredit.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.FixedView;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules.RequestCredit;
using Brierley.WebFrameWork.Portal.Validators;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.LWModules.RequestCredit
{
	public partial class ViewRequestCredit : ModuleControlBase
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private RequestCreditConfig _config = null;
		protected AspDynamicGrid _grid = null;
        protected AspDynamicList _list = null;
		private RequestCreditGridProvider _grdProvider = null;
        private RequestCreditListProvider _lstProvider = null;
		private Member _member = null;
        private const string _className = "ViewRequestCredit";
		private const string _modulePath = "~/Controls/Modules/RequestCredit/ViewRequestCredit.ascx";

		private TransactionType SelectedTransactionType
		{
			get
			{
				if (_config.TransactionTypes.Count == 1)
				{
					return _config.TransactionTypes[0];
				}
				string t = _config.DisplayType == DisplayTypes.DropDownList ? ddlTransactionType.SelectedValue : rdoTransactionType.SelectedValue;
				if (string.IsNullOrEmpty(t))
				{
					return TransactionType.Store;
				}
				else
				{
					return (TransactionType)Enum.Parse(typeof(TransactionType), t);
				}
			}
		}


		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";
			_logger.Debug(_className, methodName, "Begin");

			_member = PortalState.CurrentMember;
			if (_member == null)
			{
				divSearchForm.Visible = false;
				divTransactionType.Visible = false;
				_logger.Trace(_className, methodName, "No member selected.");
				return;
			}

            _config = ConfigurationUtil.GetConfiguration<RequestCreditConfig>(ConfigurationKey);
			if (_config == null)
			{
				divSearchForm.Visible = false;
				divTransactionType.Visible = false;
				_logger.Trace(_className, methodName, String.Format("Missing configuration for module {0}.", ConfigurationKey));
				return;
			}

			divSearchForm.Visible = _config.TransactionTypes != null && _config.TransactionTypes.Count > 0;
			divTransactionType.Visible = _config.TransactionTypes != null && _config.TransactionTypes.Count > 1;

            lblTransactionType.Text = ResourceUtils.GetLocalWebResource(_modulePath, "lblTransactionType.Text", "Transaction Type:"); 
            litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);
			lblSuccess.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.SuccessMessageResourceKey);
			lblNoResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);
			lblApliedMsg.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CreditAlreadyAppliedMessageResourceKey);

			rdoTransactionType.Visible = _config.DisplayType == DisplayTypes.RadioList;
			ddlTransactionType.Visible = _config.DisplayType == DisplayTypes.DropDownList;
			ListControl lc = _config.DisplayType == DisplayTypes.DropDownList ? (ListControl)ddlTransactionType : (ListControl)rdoTransactionType;
			foreach (var t in _config.TransactionTypes)
			{
                string resourceName = t.ToString() + "TT.Text";
                lc.Items.Add(new ListItem(ResourceUtils.GetLocalWebResource(_modulePath, resourceName, t.ToString()), t.ToString()));
			}
			foreach (ListItem item in rdoTransactionType.Items)
			{
				item.Attributes.Add("onclick", "javascript:SetTransactionType(this);");
			}
			if (lc.Items.Count > 0)
			{
				lc.Items[0].Selected = true;
			}
            
			if (!string.IsNullOrEmpty(_config.ProviderAssemblyName) && !string.IsNullOrEmpty(_config.ProviderClassName))
			{
                if (_config.ResultDisplayType == DataViewType.Grid)
                {
                    _grdProvider = (RequestCreditGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                }
                else
                {
                    _lstProvider = (RequestCreditListProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                }
			}
			else
			{
                if (_config.ResultDisplayType == DataViewType.Grid)
                {
                    _grdProvider = new RequestCreditGridProvider();
                }
                else
                {
                    _lstProvider = new RequestCreditListProvider();
                }
			}
            if (_config.ResultDisplayType == DataViewType.Grid)
            {
                _grdProvider.ParentControl = "~/Controls/Modules/RequestCredit/ViewRequestCredit.ascx";
                _grdProvider.Configuration = _config;
            }
            else
            {
                _lstProvider.ParentControl = "~/Controls/Modules/RequestCredit/ViewRequestCredit.ascx";
                _lstProvider.SetSearchParm("Configuration", _config);
            }
			_logger.Debug(_className, methodName, "End");
			base.OnInit(e);
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
				lblSuccess.Visible = false;
				pnlSearchResult.Visible = false;
				lblApliedMsg.Visible = false;
				lblNoResults.Visible = false;

				if (_config != null && _member != null)
				{
                    string resultTitle = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.ResultGridTitleResourceKey, "ResultGridTitle.Text"));
                    if (_config.ResultDisplayType == DataViewType.Grid)
                    {
                        _grid = new AspDynamicGrid() { AutoRebind = false, Title = resultTitle };
                        _grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                        _grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                        _grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                        _grid.Provider = _grdProvider;
                        _grid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(grid_GridActionClicked);
                        _grid.Title = ResourceUtils.GetLocalWebResource(_modulePath, _config.ResultGridTitleResourceKey);
                        pnlSearchResult.Controls.Add(_grid);
                    }
                    else
                    {
                        _list = new AspDynamicList() { AutoRebind = false, Title = resultTitle, WrapInApplicationPanel = true };
                        _list.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                        _list.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                        _list.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                        _list.Provider = _lstProvider;
                        _list.ListActionClicked += new AspDynamicList.ListActionClickedHandler(list_ListActionClicked);
                        _list.Title = ResourceUtils.GetLocalWebResource(_modulePath, _config.ResultGridTitleResourceKey);
                        pnlSearchResult.Controls.Add(_list);
                    }					
					CreateControls();
				}

				_logger.Debug(_className, methodName, "end");
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex, true);
				throw;
			}
		}


		protected void lnkSearch_Click(object sender, EventArgs e)
		{
			const string methodName = "lnkSearch_Click";
			_logger.Debug(_className, methodName, "Begin");
			lblApliedMsg.Visible = false;
			try
			{
				if (_config != null && _config.ResultsAttributes.Count > 0)
				{
                    int rowCount = 0;
                    if (_config.ResultDisplayType == DataViewType.Grid)
                    {
                        _grid.SetSearchParm("TxnType", SelectedTransactionType);
                        _grid.SetSearchParm("SearchParms", GetSearchValues());
                        _grid.Rebind();
                        rowCount = _grdProvider.GetNumberOfRows();
                    }
                    else
                    {
                        _list.SetSearchParm("TxnType", SelectedTransactionType);
                        _list.SetSearchParm("SearchParms", GetSearchValues());
                        _list.Rebind();
                        rowCount = _lstProvider.GetNumberOfRows();
                    }

					if (_config.AutoApplyTransaction && rowCount == 1)
					{
                        if (_config.ResultDisplayType == DataViewType.Grid)
                        {
                            var key = _grdProvider.GetColumnSpecs().Where(o => o.IsKey).FirstOrDefault();
                            if (key != null)
                            {
                                var rowKey = _grdProvider.GetColumnData(0, key);
                                if (rowKey != null)
                                {
                                    ApplyTransaction(rowKey.ToString());
                                }
                                else
                                {
                                    pnlSearchResult.Visible = true;
                                }
                            }
                            else
                            {
                                pnlSearchResult.Visible = true;
                            }
                        }
                        else
                        {
                            // get the row key column
                            DynamicListColumnSpec key = null;
                            foreach (DynamicListItem listItem in _lstProvider.GetListItemSpecs())
                            {
                                DynamicListColumnSpec spec = listItem as DynamicListColumnSpec;
                                if (spec != null && spec.IsKey)
                                {
                                    key = spec;
                                    break;
                                }                                
                            }
                            if (key != null)
                            {
                                var rowKey = _lstProvider.GetColumnData(0, key);
                                if (rowKey != null)
                                {
                                    ApplyTransaction(rowKey.ToString());
                                }
                                else
                                {
                                    pnlSearchResult.Visible = true;
                                }
                            }
                            else
                            {
                                pnlSearchResult.Visible = true;
                            }
                        }
					}
					else if (rowCount > 0)
					{
						pnlSearchResult.Visible = true;
					}
					else
					{
						lblNoResults.Visible = true;
					}
				}
			}
			catch (LWValidationException ex)
			{
				ShowNegative(ex.Message);
				return;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
			_logger.Debug(_className, methodName, "end");
		}


		protected void grid_GridActionClicked(object sender, GridActionClickedArg e)
		{
			const string methodName = "grid_GridActionClicked";
			_logger.Debug(_className, methodName, "Begin");
			if (e.CommandName == AspDynamicGrid.SELECT_COMMAND)
			{
				try
				{
					String rowKey = Convert.ToString(e.Key);
					ApplyTransaction(rowKey);
				}
				catch (LWValidationException ex)
				{
					ShowNegative(ex.Message);
					return;
				}                
			}
			_logger.Trace(_className, methodName, "end");
		}

        protected void list_ListActionClicked(object sender, ListActionClickedArg e)
        {
            const string methodName = "list_ListActionClicked";
            _logger.Debug(_className, methodName, "Begin");
            if (e.Command == "RequestCredit")
            {
                try
                {
                    String rowKey = Convert.ToString(e.Key);
                    ApplyTransaction(rowKey);
                }
                catch (LWValidationException ex)
                {
                    ShowNegative(ex.Message);
                    return;
                }                
            }
            _logger.Trace(_className, methodName, "end");
        }

		private void ApplyTransaction(string rowKey)
		{
            //const string methodName = "ApplyTransaction";
			string defaultAmount = string.Empty;
			string TxnHeaderId = string.Empty;

			LWCriterion lwCriteria = new LWCriterion("HistoryTxnDetail");
			lwCriteria.Add(LWCriterion.OperatorType.AND, "RowKey", rowKey, LWCriterion.Predicate.Eq);
			IList<IClientDataObject> oHistoryRecords = LoyaltyService.GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, null, false);

			IClientDataObject historyTxnDetail = oHistoryRecords[0];
			TxnHeaderId = (string)historyTxnDetail.GetAttributeValue("TxnHeaderId");

			object processId = historyTxnDetail.GetAttributeValue("ProcessId");
			if (processId != null && processId is long && (long)processId == (long)ProcessCode.RequestCreditApplied)
			{
				lblApliedMsg.Visible = true;
				return;
			}

            decimal pointsEarned = 0;
            if (_config.ResultDisplayType == DataViewType.Grid)
            {
                pointsEarned = _grdProvider.AddLoyaltyTransaction(_member, TxnHeaderId);
            }
            else
            {
                pointsEarned = _lstProvider.AddLoyaltyTransaction(_member, TxnHeaderId);
            }

			if (_config.AddGlobalNotes && PortalState.Portal.PortalMode == PortalModes.CustomerService)
			{
				var note = new CSNote() { MemberId = _member.IpCode, CreateDate = DateTime.Now };
				if (string.IsNullOrEmpty(_config.GlobalNoteFormat))
				{
					note.Note = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "TxnRequestMessage.Text", "Transaction in the amount of {0} was requested"), defaultAmount);
				}
				else
				{
					note.Note = string.Format(_config.GlobalNoteFormat,
						historyTxnDetail.GetAttributeValue("TxnNumber"),
						defaultAmount,
						historyTxnDetail.GetAttributeValue("TxnStoreId"));
				}
				CSAgent agent = PortalState.GetLoggedInCSAgent();
				note.CreatedBy = agent.Id;
				CSService.CreateNote(note);
			}
			IpcManager.PublishEvent("MemberUpdated", ConfigurationKey, _member);
			lblSuccess.Text = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, _config.SuccessMessageResourceKey), pointsEarned);
			lblSuccess.Visible = true;

		}

		private void CreateControls()
		{
			const string methodName = "CreateControls";
			_logger.Trace(_className, methodName, "Begin");

			foreach (var t in _config.TransactionTypes)
			{
				System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
				div.Attributes.Add("class", "SearchForm " + t.ToString());
				if (SelectedTransactionType != t)
				{
					div.Attributes.Add("style", "display:none;");
				}

				FixedLayoutManager layout = new FixedLayoutManager(_config.LayoutType, div);

				foreach (var attribute in _config.SearchAttributes.Where(o => o.TransactionType == t))
				{
					if (attribute.AttributeType == ItemTypes.HtmlBlock)
					{
						layout.AddHtmlBlock(attribute.DisplayText);
					}
					else if (attribute.AttributeType == ItemTypes.SwitchLayout)
					{
						layout.SwitchLayoutMode(attribute.LayoutType);
					}
					else
					{
						var lbl = new HtmlGenericControl("label") { InnerText = ResourceUtils.GetLocalWebResource(_modulePath, attribute.DisplayText, attribute.DisplayText) };
						lbl.Attributes.Add("class", attribute.LabelCssClass);
						lbl.Attributes.Add("class", "RequestCreditLabel");
						var txt = new TextBox() { ID = attribute.DataKey, CssClass = attribute.ControlCSSClass };
						txt.Attributes.Add("class", "RequestCreditControl");

						if (attribute.RangeType == RangeTypes.StartOfRange)
						{
							txt.ID += "_start";
						}
						else if (attribute.RangeType == RangeTypes.EndOfRange)
						{
							txt.ID += "_end";
						}
						attribute.Control = txt;

						List<BaseValidator> validators = new List<BaseValidator>();
						foreach (var validator in attribute.validators)
						{
							BaseValidator vldBase = null;

							switch (validator.ValidatorType)
							{
								case ValidatorTypes.Compare:
									CompareValidator vldCompare = new CompareValidator();
									vldCompare.ControlToCompare = validator.CompareToID;
									vldCompare.Type = validator.CompareType.GetValueOrDefault(ValidationDataType.String);
									vldBase = vldCompare;
									break;
								case ValidatorTypes.Range:
									RangeValidator vldRange = new RangeValidator();
									vldRange.MinimumValue = validator.MinValue;
									vldRange.MaximumValue = validator.MaxValue;
									vldRange.Type = validator.CompareType.GetValueOrDefault(ValidationDataType.String);
									vldBase = vldRange;
									break;
								case ValidatorTypes.RegularExpression:
									RegularExpressionValidator vldRegex = new RegularExpressionValidator();
									vldRegex.ValidationExpression = validator.RegularExpression;
									vldBase = vldRegex;
									break;
								case ValidatorTypes.RequiredField:
									RequiredFieldValidator vldReq = new RequiredFieldValidator();
									vldBase = vldReq;
									break;
								case ValidatorTypes.Custom:
									CustomValidator vldCustom = new CustomValidator();
									vldCustom.ClientValidationFunction = validator.ClientValidationFunction;
									vldCustom.ValidateEmptyText = true;
									vldBase = vldCustom;
									break;
							}

							vldBase.Display = ValidatorDisplay.Dynamic;
							vldBase.ControlToValidate = txt.ID;
							vldBase.ValidationGroup = "RequestCredit" + t.ToString();

							if (!string.IsNullOrEmpty(validator.ResourceKey))
							{
								vldBase.ErrorMessage = GetLocalResourceObject(validator.ResourceKey).ToString();
							}
							else
							{
								vldBase.ErrorMessage = validator.ErrorMessage;
							}

							vldBase.CssClass = validator.CssClass;
							validators.Add(vldBase);
						}
						layout.AddItem(txt, lbl, validators);
					}
				}

				var lnkSearch = new LinkButton() { Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.SearchLinkResourceKey), ValidationGroup = "RequestCredit" + t.ToString(), CssClass = "btn Button" };
				lnkSearch.Click += new EventHandler(lnkSearch_Click);

				HtmlGenericControl lnkClear = new HtmlGenericControl("a");
				lnkClear.Attributes.Add("href", "javascript: ClearSearch()");
				lnkClear.Attributes.Add("class", "btn Button");
				lnkClear.InnerText = ResourceUtils.GetLocalWebResource(_modulePath, _config.ClearLinkResourceKey);

				layout.AddActionButtons(new List<Control>() { lnkSearch, lnkClear });

				divSearchForm.Controls.AddAt(0, div);
			}
			_logger.Trace(_className, methodName, "end");
		}


		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}


		private Dictionary<string, string> GetSearchValues()
		{
			const string methodName = "GetSearchValues";
			_logger.Trace(_className, methodName, "Begin");
			Dictionary<string, string> parms = new Dictionary<string, string>();
			foreach (var attribute in _config.SearchAttributes.Where(o => o.TransactionType == SelectedTransactionType))
			{
				TextBox txt = attribute.Control as TextBox;
				if (txt != null && !string.IsNullOrWhiteSpace(txt.Text))
				{
					if (attribute.RangeType == RangeTypes.StartOfRange)
					{
						parms.Add(attribute.AttributeName + "_Start", txt.Text);
					}
					else if (attribute.RangeType == RangeTypes.EndOfRange)
					{
						parms.Add(attribute.AttributeName + "_End", txt.Text);
					}
					else
					{
						parms.Add(attribute.AttributeName, txt.Text);
					}
				}
			}
			_logger.Trace(_className, methodName, "end");
			return parms;
		}


	}
}
