using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LWModules.MemberSearch.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Security;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.LWModules.MemberSearch
{
	public partial class ViewMemberSearch : ModuleControlBase
    {
		#region Fields
		private const string _className = "ViewMemberSearch";
        private const string _modulePath = "~/Controls/Modules/MemberSearch/ViewMemberSearch.ascx";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		protected AspDynamicGrid _grid = null;
		private MemberSearchConfiguration _config = null;

		protected LWLinkButton _btnSearch = null;
		#endregion        

        #region Page Loading and Initialization

		protected override void OnInit(EventArgs e)
		{
			_config = ConfigurationUtil.GetConfiguration<MemberSearchConfiguration>(base.ConfigurationKey) ?? new MemberSearchConfiguration();
			base.OnInit(e);
		}

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LoadControls();

				if (_config != null && !IsPostBack)
                {
                    PortalState.CurrentMember = null;
                    IpcManager.PublishEvent("MemberSelected", base.ConfigurationKey, null);
                }

				_grid = new AspDynamicGrid() { AutoRebind = false };
				_grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				_grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				_grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
				IDynamicGridProvider provider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
				if (provider is DefaultGridProvider)
				{
					((DefaultGridProvider)provider).Config = _config;
				}
				if (provider is AspGridProviderBase)
				{
					((AspGridProviderBase)provider).ValidationGroup = ValidationGroup;
					((AspGridProviderBase)provider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
				}
                provider.ParentControl = _modulePath;
				_grid.Provider = provider;
				_grid.SetSearchParm("IpCodeList", IpCodeList);
				_grid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(grid_GridActionClicked);
				phCSMemberSearch.Controls.Add(_grid);
            }
            catch (Exception ex)
            {
				_logger.Error(_className, "Page_Load", "Error initialzing MemberSearch grid.", ex);
				throw;
            }
        }		

        private void LoadControls()
        {
            if (_config != null)
            {                
                TableRow row = null;
                int colidx = 0;
				int maxNumber = _config.NumberOfControlsPerRow * 2;
				foreach (MemberSearchConfiguration.SearchField field in _config.SearchFields)
                {
					if (colidx == maxNumber || row == null)
                    {
                        row = new TableRow();
                        pnlSearchFormTable.Rows.Add(row);
                        colidx = 0;
                    }
                    //Add label for this field
                    Label label = new Label();
                    label.ID = "lbl" + field.FieldLabel;
                    label.Text = ResourceUtils.GetLocalWebResource(_modulePath, field.FieldLabel, field.FieldLabel);  //field.FieldLabel;
                    TableCell cell = new TableCell();
                    cell.Controls.Add(label);
                    row.Cells.Add(cell);
                    WebControl ctrl = null;
                    if (field.FieldType == "Boolean")
                    {
                        CheckBox chkBox = new CheckBox();
                        chkBox.ID = "ctrl" + field.FieldName;
                        chkBox.EnableViewState = false;
                        ctrl = chkBox;
                    }
                    else
                    {
                        TextBox textBox = new TextBox();
                        textBox.ID = "ctrl" + field.FieldName;
                        textBox.Rows = 1;
                        ctrl = textBox;
                    }                    
                    cell = new TableCell();                    
                    cell.Controls.Add(ctrl);
                    row.Cells.Add(cell);
					if (!string.IsNullOrEmpty(field.ValidExpress))
					{
						RegularExpressionValidator validator = new RegularExpressionValidator();                        
                        validator.ControlToValidate = ctrl.ID;
						validator.Display = ValidatorDisplay.Dynamic;
						validator.ValidationExpression = field.ValidExpress;
						validator.ValidationGroup = ValidationGroup;
						validator.CssClass = "Validator";
						if (!string.IsNullOrEmpty(field.FailedValidMsg))
						{
							validator.ErrorMessage = field.FailedValidMsg;
						}
						else
						{
							validator.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, "FieldValidationFailed.Text", "Field validation failed.");
						}
						cell.Controls.Add(validator);
					}
                    colidx += 2;
                }
                pnlSearchFormTable.Visible = true;                                  
                row = new TableRow();
                tblSearchCtrls.Rows.Add(row);
                TableCell tcell = new TableCell();
				tcell.ColumnSpan = 3;
				tcell.HorizontalAlign = HorizontalAlign.Center;
                row.Cells.Add(tcell);				

				_btnSearch = new LWLinkButton();
				_btnSearch.ID = "cmdSearch";
				_btnSearch.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnSearch.Text", "Search");
				_btnSearch.ButtonType = LWLinkButton.ButtonTypes.Submit;
				_btnSearch.ValidationGroup = ValidationGroup;
				_btnSearch.Click += new EventHandler(cmdSearch_Click);
				tcell.Controls.Add(_btnSearch);

				Label spacer = new Label();
				spacer.Text = " ";
				tcell.Controls.Add(spacer);

				LWLinkButton btn = new LWLinkButton();
				btn.CausesValidation = false;
				btn.ID = "cmdCancel";				
				btn.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnClear.Text", "Clear");
				btn.ButtonType = LWLinkButton.ButtonTypes.Cancel;
				btn.Click += new EventHandler(cmdCancel_Click);
				btn.OnClientClick = "return ClearSearch();";
				tcell.Controls.Add(btn);
            }
        }

        #endregion

        #region Helper Methods

        private void ClearSearch()
        {
            foreach (Control ctrl in pnlSearchFormTable.Controls)
            {
                if (ctrl is TableRow)
                {
                    foreach (Control rctrl in ctrl.Controls)
                    {
                        if (rctrl is TableCell)
                        {
                            foreach (Control tdctrl in rctrl.Controls)
                            {
                                if (tdctrl is TextBox)
                                {
                                    ((TextBox)tdctrl).Text = string.Empty;
                                }
                                else if (tdctrl is CheckBox)
                                {
                                    ((CheckBox)tdctrl).Checked = false;
                                }
                            }
                        }
                    }
                }
            }
            phCSMemberSearch.Visible = false;
			IpCodeList = null;
        }

        private MemberSearchConfiguration.SearchField GetField(MemberSearchConfiguration config, string ctrlId)
        {
            foreach (MemberSearchConfiguration.SearchField field in config.SearchFields)
            {
                string fieldCtrlId = "ctrl" + field.FieldName;
                if (ctrlId == fieldCtrlId)
                {
                    return field;
                }
            }
            return null;
        }

		private MemberSearchConfiguration GetSearchValues(MemberSearchConfiguration config, ref bool emptyCriteria)
        {
			emptyCriteria = true;
            foreach (Control ctrl in pnlSearchFormTable.Controls)
            {
                if (ctrl is TableRow)
                {
                    foreach (Control rctrl in ctrl.Controls)
                    {
                        if (rctrl is TableCell)
                        {
                            foreach (Control tdctrl in rctrl.Controls)
                            {
                                if (tdctrl is TextBox)
                                {
                                    TextBox txtBox = tdctrl as TextBox;
                                    MemberSearchConfiguration.SearchField field = GetField(config, tdctrl.ID);
                                    field.FieldValue = txtBox.Text;
									if (!string.IsNullOrEmpty(txtBox.Text))
									{
										emptyCriteria = false;
									}
                                }
                                if (tdctrl is CheckBox)
                                {
                                    CheckBox chkBox = tdctrl as CheckBox;
                                    MemberSearchConfiguration.SearchField field = GetField(config, tdctrl.ID);
                                    field.FieldValue = chkBox.Checked;
                                    emptyCriteria = false;
                                }
                            }
                        }
                    }
                }
            }
            return config;
        }

		protected long[] IpCodeList
		{
			get
			{
				if (ViewState["IpCodeList"] != null)
				{ return (long[])ViewState["IpCodeList"]; }
				else
				{
					return null;
				}
			}
			set
			{
				ViewState["IpCodeList"] = value;
			}
		}
        #endregion

        #region Event Handlers

        

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
			string method = "cmdSearch_Click";
			try
			{
				if (_config != null)
				{
					PortalState.CurrentMember = null;
					IpcManager.PublishEvent("MemberSelected", base.ConfigurationKey, null);

					bool emptyCriteria = true;
					_config = GetSearchValues(_config, ref emptyCriteria);
					if (emptyCriteria && !string.IsNullOrEmpty(_config.EmptySearchCriteriaMessage))
					{
						_logger.Trace(_className, method, _config.EmptySearchCriteriaMessage);
						ShowNegative(_config.EmptySearchCriteriaMessage);
					}
					else
					{
						long[] ipCodes = null;
						long[] searchIpCodes = MemberSearchUtil.SearchMembers(_config);
						if (searchIpCodes != null && searchIpCodes.Length > 0)
						{
							if (searchIpCodes.Length == 1 && _config.AutoSelectMember)
							{
								SelectMember(searchIpCodes[0]);
							}
							else
							{
								if (searchIpCodes.Length > _config.MaxNumberOfMembers)
								{
									if (!string.IsNullOrEmpty(_config.VagueCriteriaMessage))
									{
										ShowNegative(_config.VagueCriteriaMessage);
									}
									ipCodes = new long[_config.MaxNumberOfMembers];
									Array.Copy(searchIpCodes, ipCodes, _config.MaxNumberOfMembers);
								}
								else
								{
									ipCodes = searchIpCodes;
								}
								_logger.Debug(_className, method, string.Format("Member search returned {0} members", ipCodes.Length));
								phCSMemberSearch.Visible = true;
								_grid.SetSearchParm("IpCodeList", ipCodes);
								IpCodeList = ipCodes;
								_grid.Rebind();
							}
						}
						else
						{
							_logger.Debug(_className, method, "No members returned.");
							phCSMemberSearch.Visible = false;
							if (!string.IsNullOrEmpty(_config.EmptyResultMessage))
							{
								_logger.Trace(_className, method, _config.EmptyResultMessage);
								ShowNegative(_config.EmptyResultMessage);
							}
						}
					}
				}
				else
				{
					_logger.Error(_className, method, "No configuration available for searching.");
				}
			}
			catch (LWValidationException ex)
			{
				base.AddInvalidField(ex.Message, null);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, method, "Error searching:", ex);
				throw;
			}
        }
        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ClearSearch();
            }
            catch (Exception ex)
            {
				_logger.Error(_className, "cmdCancel_Click", "Unexpected exception", ex);
				throw;
            }
        }

		protected void grid_GridActionClicked(object sender, GridActionClickedArg e)
		{
			if (e.CommandName == AspDynamicGrid.SELECT_COMMAND)
			{
				long ipcode = long.Parse(e.Key.ToString());
                SelectMember(ipcode);				
			}
		}

        protected void SelectMember(long ipcode)
        {
            string methodName = "SelectMember";

            _logger.Debug(_className, methodName, "Ipcode selected = " + ipcode);
            try
            {
                Member member = LoyaltyService.LoadMemberFromIPCode(ipcode);
                if (member == null)
                {
                    throw new Exception(ResourceUtils.GetLocalWebResource(_modulePath, "MemberNotLoaded.Text", "No member could be loaded."));
                }
                else
                {
                    _logger.Trace(_className, methodName, "Member selected: " + ipcode);
                }
                if (PortalState.Portal.PortalMode == PortalModes.CustomerService)
                {
                    PortalState.CurrentMember = member;
                    IpcManager.PublishEvent("MemberSelected", base.ConfigurationKey, member);
                }
                else
                {
                    // login the member here.
                    try
                    {
                        switch (member.MemberStatus)
                        {
                            case MemberStatusEnum.Disabled:
                            case MemberStatusEnum.Locked:
                            case MemberStatusEnum.Terminated:
                                string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "UnableToLoadMember.Text", "Unable to login member because the member status is {0}."), member.MemberStatus.ToString());
                                _logger.Error(_className, methodName, errMsg);
                                ShowNegative(errMsg);
                                break;
                            default:
                                SecurityManager.LoginMember(member, false);
                                break;
                        }                                                

                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName, "Unable to log in member: " + ex.Message, ex);
                    }
                }
                if (!string.IsNullOrEmpty(_config.RedirectUrl))
                {
                    Response.Redirect(_config.RedirectUrl, false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Unexpected exception", ex);
                throw;
            }
        }
        #endregion
    }
}
