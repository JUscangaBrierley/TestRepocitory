using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.LWModules.CSUserAdministration.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using System.Web.UI;

namespace Brierley.LWModules.CSUserAdministration
{
	public partial class ViewCSUserAdministration : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewCSUserAdministration";
        private const string _modulePath = "~/Controls/Modules/CSUserAdministration/ViewCSUserAdministration.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private CSUserAdministrationConfig _config = null;

		private AspDynamicGrid _agentsGrid = null;
		private IDynamicGridProvider _agentGrdProvider = null;
		private AspDynamicGrid _rolesGrid = null;
		private IDynamicGridProvider _rolesGrdProvider = null;
		#endregion

		#region properties
		protected bool ActiveViewSet
		{
			get
			{
				if (ViewState["CurrentActiveView"] != null)
				{ return (bool)ViewState["CurrentActiveView"]; }
				else
				{
					return false;
				}
			}
			set
			{
				ViewState["CurrentActiveView"] = value;
			}
		}

		protected long PwdChangeAgentId
		{
			get
			{
				if (ViewState["PwdChangeAgentId"] != null)
				{ return (long)ViewState["PwdChangeAgentId"]; }
				else
				{
					return 0;
				}
			}
			set
			{
				ViewState["PwdChangeAgentId"] = value;
			}
		}

		public string SearchParm
		{
			get
			{
				if (ViewState["SearchParm"] != null)
				{
					return (string)ViewState["SearchParm"];
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				ViewState["SearchParm"] = value;
			}
		}

		public string SearchValue
		{
			get
			{
				if (ViewState["SearchValue"] != null)
				{
					return (string)ViewState["SearchValue"];
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				ViewState["SearchValue"] = value;
			}
		}
		#endregion

		#region page life cycle
		protected void Page_Load(object sender, EventArgs e)
		{
			Page.Form.DefaultButton = btnSearch.UniqueID;
			if (!IsPostBack)
			{
				PortalState.CurrentMember = null;
				IpcManager.PublishEvent("MemberSelected", ConfigurationKey, null);

				var roles = CSService.GetAllRoles(false);
				foreach (var role in roles)
				{
					ddlRole.Items.Add(new ListItem(role.Name, role.Name));
				}

				foreach (var status in Enum.GetNames(typeof(AgentAccountStatus)))
				{
					ddlAccountStatus.Items.Add(new ListItem(status, status));
				}

				ddlRole.Attributes.Add("style", "display:none;");
				ddlAccountStatus.Attributes.Add("style", "display:none;");
				_agentsGrid.Rebind();
			}
			else
			{
				if (!string.IsNullOrEmpty(SearchParm) && !string.IsNullOrEmpty(SearchValue))
				{
					_agentsGrid.SetSearchParm(SearchParm, SearchValue);
					_agentsGrid.SetSearchParm("Config", _config);
				}
                pnlPwdChange.Visible = false;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";

			try
			{
				base.OnInit(e);

				string validationGroup = ValidationGroup;
				rqPassword1.ValidationGroup = validationGroup;
                rqPassword1.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, "reqNewPassword.Text", "Please enter a New Password");

                rePassword1.ValidationGroup = validationGroup;
                rePassword1.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, "vldNewPassword.Text", "New Password: max 200 characters");
                
                rqPassword2.ValidationGroup = validationGroup;
                rqPassword2.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, "reqConfirmPassword.Text", "Please Confirm the password.");

                rePassword2.ValidationGroup = validationGroup;
                rePassword2.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, "vldConfirmPassword.Text", "Confirm Password: max 200 characters");

                cmpPassword2.ValidationGroup = validationGroup;
                cmpPassword2.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, "PasswordsDoNotMatch.Text", "New Password and Confirm Password must match."); 

                btnPasswordSave.ValidationGroup = validationGroup;

				if (btnSearch != null)
				{
					btnSearch.Click += new EventHandler(btnSearch_Click);
				}

				_config = ConfigurationUtil.GetConfiguration<CSUserAdministrationConfig>(ConfigurationKey) ?? new CSUserAdministrationConfig();

				_agentsGrid = new AspDynamicGrid();
				_agentGrdProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.AgentProviderAssemblyName, _config.AgentProviderClassName);
				if (_agentGrdProvider is CSAgentsGridProvider)
				{
                    _agentGrdProvider.ParentControl = _modulePath;
					((CSAgentsGridProvider)_agentGrdProvider).AllowNullPasswords = _config.AllowNullPasswords;
				}

				((AspGridProviderBase)_agentGrdProvider).ValidationGroup = validationGroup;
				((AspGridProviderBase)_agentGrdProvider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };

				_agentsGrid.Provider = _agentGrdProvider;
				//_agentsGrid.ErrorPanel = pnlErrorPanel;
				_agentsGrid.CreateTopPanel = false;
				_agentsGrid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(_agentsGrid_GridActionClicked);
				_agentsGrid.ShowPositive += delegate(object sender, string message) { ShowPositive(message); };
				_agentsGrid.ShowNegative += delegate(object sender, string message) { ShowNegative(message); };
				_agentsGrid.ShowWarning += delegate(object sender, string message) { ShowWarning(message); };
				_agentsGrid.AutoRebind = false;
				phCSAgents.Controls.Add(_agentsGrid);

				_rolesGrid = new AspDynamicGrid();
				_rolesGrdProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.RoleProviderAssemblyName, _config.RoleProviderClassName);
				if (_rolesGrdProvider is AspGridProviderBase)
				{
                    ((AspGridProviderBase)_rolesGrdProvider).ParentControl = _modulePath;
				}

				((AspGridProviderBase)_rolesGrdProvider).ValidationGroup = ValidationGroup;
				((AspGridProviderBase)_rolesGrdProvider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };

				_rolesGrid.Provider = _rolesGrdProvider; // new CSRolesGridProvider() { ParentControl = "~/Controls/Modules/CSUserAdministration/ViewCSUserAdministration.ascx" };
				//_rolesGrid.ErrorPanel = pnlErrorPanel;
				_rolesGrid.CreateTopPanel = false;
				_rolesGrid.ShowPositive += delegate(object sender, string message) { ShowPositive(message); };
				_rolesGrid.ShowNegative += delegate(object sender, string message) { ShowNegative(message); };
				_rolesGrid.ShowWarning += delegate(object sender, string message) { ShowWarning(message); };
				phCSRoles.Controls.Add(_rolesGrid);

				pnlCSUserAdministration.TabStripClicked += new LWApplicationPanel.TabStripClickHandler(pnlCSUserAdministration_TabStripClicked);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: ", ex);
				//pnlErrorPanel.ShowException(GetLocalResourceObject("ApplicationException.Text").ToString(), ex);
				throw;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			const string methodName = "OnLoad";

			try
			{
				base.OnLoad(e);

				if (CSUserAdminView.GetActiveView() == null || CSUserAdminView.GetActiveView() == CSAgentsView)
				{
					//_agentsGrid.SetSearchParm("PortalId", PortalId);
					//_agentsGrid.SetSearchParm("UserId", UserId);
					litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, "AgentViewTitle.Text", "Agent Administration");
					DisplayView("CSAgentsView");
				}
				else
				{
					litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, "RoleViewTitle.Text","Role Administration");
					DisplayView("CSRolesView");
				}
				if (IsPostBack && !string.IsNullOrEmpty(SearchParm) && !string.IsNullOrEmpty(SearchValue))
				{
					_agentsGrid.SetSearchParm(SearchParm, SearchValue);
					_agentsGrid.SetSearchParm("Config", _config);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: ", ex);
				//pnlErrorPanel.ShowException(GetLocalResourceObject("ApplicationException.Text").ToString(), ex);
				throw;
			}
		}

		#endregion

		#region event handlers
		void btnSearch_Click(object sender, EventArgs e)
		{
			string searchValue = string.Empty;

			if (ddlSearchOptions.SelectedValue == "Role")
			{
				ddlRole.Attributes.Clear();
				txtSearchValue.Attributes.Add("style", "display:none;");
				ddlAccountStatus.Attributes.Add("style", "display:none");
				searchValue = ddlRole.SelectedValue;
			}
			else if (ddlSearchOptions.SelectedValue == "Status")
			{
				ddlAccountStatus.Attributes.Clear();
				ddlRole.Attributes.Add("style", "display:none");
				txtSearchValue.Attributes.Add("style", "display:none;");
				searchValue = ddlAccountStatus.SelectedValue;
			}
			else
			{
				txtSearchValue.Attributes.Clear();
				ddlAccountStatus.Attributes.Add("style", "display:none");
				ddlRole.Attributes.Add("style", "display:none");
				searchValue = txtSearchValue.Text;
			}

			SearchParm = ddlSearchOptions.SelectedItem.Value;
			SearchValue = searchValue;

			_agentsGrid.SetSearchParm(ddlSearchOptions.SelectedItem.Value, searchValue);
			_agentsGrid.SetSearchParm("Config", _config);
			_agentsGrid.Rebind();
            _agentsGrid.ClearGridEditPanel();
		}

		void _agentsGrid_GridActionClicked(object sender, GridActionClickedArg e)
		{
			if (e.CommandName == "Change Password")
			{
				long agentId = long.Parse(e.Key.ToString());
				PwdChangeAgentId = agentId;
				pnlPwdChange.Visible = true;
			}
		}

		void pnlCSUserAdministration_TabStripClicked(string CommandName, CancelEventArgs args)
		{
			const string methodName = "pnlCSUserAdministration_TabStripClicked";
			//pnlErrorPanel.Clear();
			try
			{
				switch (CommandName.ToLower())
				{
					case "csagents":
                        litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, "AgentViewTitle.Text", "Agent Administration");
						DisplayView("CSAgentsView");
						break;
					case "roles":
                        litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, "RoleViewTitle.Text", "Role Administration");
						DisplayView("CSRolesView");
						CSUserAdminView.SetActiveView(CSRolesView);
						break;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, null, ex);
				//pnlErrorPanel.ShowException(GetLocalResourceObject("ApplicationException.Text").ToString(), ex);
				throw;
			}
		}

		protected void btnPassword_Save(object sender, EventArgs e)
		{
			const string methodName = "btnPassword_Save";
			try
			{
                pnlPwdChange.Visible = true;
				if (string.IsNullOrEmpty(txtNewPassword.Text))
				{
					//pnlErrorPanel.ShowException("Value for 'New Password' is required.");
					AddInvalidField(GetLocalResourceObject("reqNewPassword.Text").ToString(), txtNewPassword);
				}
				else if (string.IsNullOrEmpty(txtConfirmPassword.Text))
				{
					//pnlErrorPanel.ShowException("Value for 'Confirm Password' is required.");
					AddInvalidField(GetLocalResourceObject("reqConfirmPassword.Text").ToString(), txtConfirmPassword);
				}
				else if (txtNewPassword.Text != txtConfirmPassword.Text)
				{
					//pnlErrorPanel.ShowException("Passwords do not match.");
					AddInvalidField(GetLocalResourceObject("PasswordsDoNotMatch.Text").ToString(), null);
				}
				else
				{
					CSAgent csagent = CSService.GetCSAgentById(PwdChangeAgentId);
					try
					{
						CSService.ChangeCSAgentPassword(csagent, txtNewPassword.Text);
						SendChangedPasswordEmail(csagent);
						pnlPwdChange.Visible = false;
						ShowPositive(GetLocalResourceObject("PasswordChangeSuccess.Text").ToString());
					}
					catch (AuthenticationException ex)
					{
						_logger.Error(_className, methodName, "Error setting password: " + ex.Message, ex);

						AddInvalidField(ex.Message, txtNewPassword);
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error setting password: " + ex.Message, ex);
						throw;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: ", ex);
				throw;
			}
		}

		protected void btnPassword_Cancel(object sender, EventArgs e)
		{
			const string methodName = "btnPassword_Cancel";
			try
			{
				pnlPwdChange.Visible = false;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: ", ex);
				throw;
			}
		}
		#endregion

		#region private methods
		private void DisplayView(string viewname)
		{
			switch (viewname)
			{
				case "CSAgentsView":
					CSUserAdminView.SetActiveView(CSAgentsView);
					phCSAgents.Visible = true;
					phCSRoles.Visible = false;
					ActiveViewSet = true;
					this.Page.Form.Attributes.Add("onsubmit", "return ValidateExpireDate();");
                    _agentsGrid.Rebind();
                    break;
				case "CSRolesView":
					CSUserAdminView.SetActiveView(CSRolesView);
					phCSAgents.Visible = false;
					phCSRoles.Visible = true;
					ActiveViewSet = true;
					break;
			}
		}

		private void SendChangedPasswordEmail(CSAgent csagent)
		{
			string methodName = "SendChangedPasswordEmail";

			string passwordChangedEmailName = _config.PasswordChangedEmailName;
			if (!string.IsNullOrEmpty(passwordChangedEmailName))
			{
				try
				{
					// send password changed email
					EmailDocument emailDoc = EmailService.GetEmail(passwordChangedEmailName);
					if (emailDoc != null)
					{
						using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailDoc.Id))
						{
							string emailAddress = csagent.EmailAddress;
							if (email != null)
							{
								email.SendAsync(csagent.EmailAddress, null).Wait();
								_logger.Debug(_className, methodName, "Password change email was sent to: " + emailAddress);
							}
							else
							{
								_logger.Error(_className, methodName, "Unable to resolve email for ID: " + emailDoc.Id);
							}
						}
					}
					else
					{
						_logger.Error(_className, methodName, "Unable to resolve emailDoc for name: " + passwordChangedEmailName);
					}
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Error sending password changed email:" + ex.Message, ex);
				}
			}
			else
			{
				_logger.Debug(_className, methodName, "No password change email is configured.");
			}
		}
		#endregion
	}
}
