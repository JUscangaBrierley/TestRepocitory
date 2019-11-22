using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CSUserAdministration.Components
{
	public class CSAgentsGridProvider : AspGridProviderBase
	{
		#region fields
		private const string _className = "CSAgentsGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private IList<CSAgent> _agents = null;
		private ICustomGridAction[] _actions = null;
        private enum CSSearchOptions { None, Role, Name, EmailAddress, PhoneNumber, Username, Status };
        private CSSearchOptions _searchOptions = CSSearchOptions.None;
        private string _searchValue = string.Empty;
        private int portalId = 0;
        private int userId = 0;
        private CSUserAdministrationConfig _config = null;
		#endregion

		#region properties
		public bool AllowNullPasswords { get; set; }

		private bool EnablePasswordExpiry
		{
			get
			{
				return !StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWPasswordExpiryDisabled"), false);
			}
		}
		#endregion

		#region Helpers

		protected void LoadCSAgents()
        {
            if (_agents == null)
            {
                _agents = new List<CSAgent>();
            }
            if (_searchOptions != CSSearchOptions.None && string.IsNullOrEmpty(_searchValue))
            {
                _agents.Clear();
                return;
            }
            switch (_searchOptions)
            {
                case CSSearchOptions.None:
                    _agents = CSService.GetAllCSAgents(null);
                    break;
                case CSSearchOptions.Name:

                    string[] nameParts = _searchValue.Split(' ');
                    string firstName = nameParts[0].Trim();
                    string lastName = nameParts.Length > 1 ? nameParts[1].Trim() : string.Empty;
                    _agents = CSService.GetCSAgents(firstName, lastName, null, null, null, null);
                    break;
                case CSSearchOptions.Role:
                    CSRole role = CSService.GetRole(_searchValue, false);
                    _agents = CSService.GetCSAgents(string.Empty, string.Empty, null, role, null, null);
                    break;
                case CSSearchOptions.EmailAddress:
                    _agents = CSService.GetCSAgents(string.Empty, string.Empty, _searchValue, null, null, null);
                    break;
                case CSSearchOptions.PhoneNumber:
                    _agents = CSService.GetCSAgents(string.Empty, string.Empty, null, null, _searchValue, null);
                    break;
                case CSSearchOptions.Username:
                    CSAgent agent = CSService.GetCSAgentByUserName(_searchValue, null);
                    _agents.Clear();
                    if (agent != null)
                    {
                        _agents.Add(agent);
                    }
                    break;
                case CSSearchOptions.Status:
                    AgentAccountStatus status = (AgentAccountStatus)Enum.Parse(typeof(AgentAccountStatus), _searchValue);
                    _agents = CSService.GetCSAgents(string.Empty, string.Empty, string.Empty, null, null, status);
                    break;
            }
        }

		private object GetData(CSAgent agent, DynamicGridColumnSpec column,bool forEditForm)
		{			
			object value = null;
			if (column.Name == "Id")
			{
				value = agent.Id;
			}
			else if (column.Name == "RoleId")
			{
                if (agent.RoleId != 0)
                {
                    CSRole role = CSService.GetRole(agent.RoleId, false);
                    if (role != null)
                    {
                        if (forEditForm)
                        {
                            value = role.Id.ToString();
                        }
                        else
                        {
                            value = role.Name;
                        }
                    }
                    else
                    {
                        value = string.Empty;
                    }
                }
			}
			else if (column.Name == "GroupId")
			{
				value = agent.GroupId;
			}
			else if (column.Name == "AgentNumber")
			{
				value = agent.AgentNumber;
			}
			else if (column.Name == "FirstName")
			{
				value = agent.FirstName;
			}
			else if (column.Name == "LastName")
			{
				value = agent.LastName;
			}
			else if (column.Name == "EmailAddress")
			{
				value = string.IsNullOrEmpty(agent.EmailAddress) ? string.Empty : agent.EmailAddress;
			}
			else if (column.Name == "PhoneNumber")
			{
				value = string.IsNullOrEmpty(agent.PhoneNumber) ? string.Empty : agent.PhoneNumber;				
			}
			else if (column.Name == "Extension")
			{
				value = string.IsNullOrEmpty(agent.Extension) ? string.Empty : agent.Extension;
			}
			else if (column.Name == "Username")
			{
				value = agent.Username;
			}
			else if (column.Name == "Password")
			{
                value = agent.Password;
			}
			else if (column.Name == "PasswordChangeRequired")
			{
				value = agent.PasswordChangeRequired;
			}
			else if (column.Name == "PasswordExpireDate")
			{
				if (!EnablePasswordExpiry || agent.PasswordExpireDate == null)
				{
					value = DateTimeUtil.MaxValue;
				}
				else
				{
					value = agent.PasswordExpireDate;
				}
			}
			else if (column.Name == "Status")
			{
				if (forEditForm)
				{
					// return the value.
					value = ((int)agent.Status).ToString();
				}
				else
				{
					value = Enum.GetName(typeof(AgentAccountStatus), agent.Status);
				}
			}
			return value;
		}

        #endregion

        #region Grid Properties

		public override string Id
		{
			get { return "grdCSUserAdministration"; }
		}

        protected override string GetGridName()
        {
            return "CSUserAdministration";
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
			int numColumns = (EnablePasswordExpiry ? 14 : 13);
			DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[numColumns];
            string errorMessage = string.Empty;

			int idx = 0;
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "Id";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Id.Text", "ID");
            c.DataType = typeof(long);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;            
            columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-AgentNumber.Text", "Agent Number") + " " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "NumberLengthError.Text", "should be a positive number with {0} to {1} digits"), 1, 18);
			c = new DynamicGridColumnSpec();
			c.Name = "AgentNumber";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-AgentNumber.Text", "Agent Number");
			c.DataType = typeof(long);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = false;
            c.IsSortable = true;
            c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^[0-9]{1,18}$", ErrorMessage = errorMessage, CssClass = "Validator", Display = ValidatorDisplay.Dynamic, ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-GroupId.Text", "Group Id") + " " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "NumberLengthError.Text", "should be a positive number with {0} to {1} digits"), 1, 18);
			c = new DynamicGridColumnSpec();
			c.Name = "GroupId";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-GroupId.Text", "Group Id");
			c.DataType = typeof(long);
			c.IsEditable = true;
			c.IsVisible = false;
			c.IsRequired = false;
            c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^[0-9]{1,18}$", ErrorMessage = errorMessage, CssClass = "Validator", Display = ValidatorDisplay.Dynamic, ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-FirstName.Text", "First Name") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 255);
			c = new DynamicGridColumnSpec();
			c.Name = "FirstName";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-FirstName.Text", "First Name");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = true;
            c.IsSortable = true;
            c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "reqEntryError.Text", "Please enter a") + " " + ResourceUtils.GetLocalWebResource(ParentControl, Id + "-FirstName.Text", "First Name"), CssClass = "Validator", Display = ValidatorDisplay.Dynamic, ValidationGroup = this.ValidationGroup });
			c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{1,255}$", ErrorMessage = errorMessage, CssClass = "Validator", Display = ValidatorDisplay.Dynamic, ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-LastName.Text", "Last Name") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 255);
			c = new DynamicGridColumnSpec();
			c.Name = "LastName";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-LastName.Text", "Last Name");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = true;
            c.IsSortable = true;
            c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "reqEntryError.Text", "Please enter a") + " " + ResourceUtils.GetLocalWebResource(ParentControl, Id + "-LastName.Text", "Last Name"), Display = ValidatorDisplay.Dynamic, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{1,255}$", ErrorMessage = errorMessage, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
            columns[idx++] = c;

			c = new DynamicGridColumnSpec();
			c.Name = "RoleId";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RoleId.Text", "Role");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = true;
            c.IsSortable = true;
			c.EditControlType = DynamicGridColumnSpec.DROPDOWNLIST;
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EmailAddress.Text", "Email Address") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 255);
			c = new DynamicGridColumnSpec();
			c.Name = "EmailAddress";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EmailAddress.Text", "Email Address");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = false;
			c.IsRequired = false;
			c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{0,255}$", ErrorMessage = errorMessage, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
            c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b", ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "InvalidEmailError.Text", "Invalid email address"), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-PhoneNumber.Text", "Phone Number") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 20);
			c = new DynamicGridColumnSpec();
			c.Name = "PhoneNumber";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-PhoneNumber.Text", "Phone Number");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = false;
			c.IsRequired = false;
			c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{0,20}$", ErrorMessage = errorMessage, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
            //c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = @"^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$", ErrorMessage = "Invalid phone number", CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Extension.Text", "Extension") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 10);
			c = new DynamicGridColumnSpec();
			c.Name = "Extension";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Extension.Text", "Extension");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = false;
			c.IsRequired = false;
			c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{0,10}$", ErrorMessage = errorMessage, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Username.Text", "Username") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 100);
			c = new DynamicGridColumnSpec();
			c.Name = "Username";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Username.Text", "Username");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = true;
            c.IsSortable = true;
            c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "reqEntryError.Text", "Please enter a") + " " + ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Username.Text", "Username"), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{1,100}$", ErrorMessage = errorMessage, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

            errorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Password.Text", "Password") + " - " + string.Format(ResourceUtils.GetLocalWebResource(ParentControl, Id + "CharLengthError.Text", "max {0} characters"), 200);
            c = new DynamicGridColumnSpec();
            c.Name = "Password";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Password.Text", "Password");
            c.DataType = typeof(string);
            c.EditControlType = DynamicGridColumnSpec.PASSWORDBOX;
            c.IsEditable = true;
            c.IsUpdateable = true;
            c.IsVisible = false;
			c.SuppressOnUpdate = true;
			if (AllowNullPasswords)
			{
				c.IsRequired = false;
			}
			else
			{
				c.IsRequired = true;
                c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "reqEntryError.Text", "Please enter a") + " " + ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Password.Text", "Password"), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
                c.Validators.Add(new RegularExpressionValidator() { ValidationExpression = "^.{1,200}$", ErrorMessage = errorMessage, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			}
            columns[idx++] = c;

			c = new DynamicGridColumnSpec();
			c.Name = "PasswordChangeRequired";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-PasswordChangeRequired.Text", "Password Change Required");
			c.DataType = typeof(bool);
			c.IsEditable = true;
			c.IsVisible = false;
			c.IsRequired = false;
			c.EditControlType = DynamicGridColumnSpec.CHECKBOX;
			columns[idx++] = c;

			if (EnablePasswordExpiry)
			{
				c = new DynamicGridColumnSpec();
				c.Name = "PasswordExpireDate";
				c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-PasswordExpireDate.Text", "Password Expire Date");
				c.DataType = typeof(DateTime);
				c.IsEditable = true;
				c.IsVisible = false;
				c.IsRequired = false;
				c.EditControlType = DynamicGridColumnSpec.DATE;
                c.Validators.Add(new CustomValidator()
                {
                    CssClass = "Validator",                    
                    Display = ValidatorDisplay.Dynamic,
                    ValidationGroup = this.ValidationGroup,
                    Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "ValidDateError.Text", "Please enter a valid date.")
                });
				columns[idx++] = c;
			}

			c = new DynamicGridColumnSpec();
			c.Name = "Status";
			c.DisplayText = "Status";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Status.Text", "Status");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = true;
            c.IsSortable = true;
			c.EditControlType = DynamicGridColumnSpec.DROPDOWNLIST;
			columns[idx++] = c;

            return columns;
        }

        public override bool IsGridEditable()
        {
            return true;            
        }

        public override string GetGridInsertLabel()
        {
            return ResourceUtils.GetLocalWebResource(ParentControl, "btnNewAgent.Text", "Create New Agent");
        }

        public override string GetEmptyGridMessage()
        {
            return _config != null ? _config.EmptySearchResultMessage : string.Empty;
        }

        #endregion

        #region Data Source

        public override void SetSearchParm(string parmName, object parmValue)
        {            
            if (string.IsNullOrWhiteSpace(parmName))
            {
                _searchOptions = CSSearchOptions.None;
                _searchValue = string.Empty;
            }
            else if (parmName == "Role")
            {
                _searchOptions = CSSearchOptions.Role;
                _searchValue = parmValue != null ? parmValue.ToString() : string.Empty;
            }
            else if (parmName == "Name")
            {
                _searchOptions = CSSearchOptions.Name;
                _searchValue = parmValue != null ? parmValue.ToString() : string.Empty;
            }
            else if (parmName == "Email Address")
            {
                _searchOptions = CSSearchOptions.EmailAddress;
                _searchValue = parmValue != null ? parmValue.ToString() : string.Empty;
            }
            else if (parmName == "Phone Number")
            {
                _searchOptions = CSSearchOptions.PhoneNumber;
                _searchValue = parmValue != null ? parmValue.ToString() : string.Empty;
            }
            else if (parmName == "Username")
            {
                _searchOptions = CSSearchOptions.Username;
                _searchValue = parmValue != null ? parmValue.ToString() : string.Empty;
            }
            else if (parmName == "Status")
            {
                _searchOptions = CSSearchOptions.Status;
                _searchValue = parmValue != null ? parmValue.ToString() : string.Empty;
            }
            else if (parmName == "PortalId")
            {
                portalId = (int)parmValue;
            }
            else if (parmName == "UserId")
            {
                userId = (int)parmValue;
            }
            else if (parmName == "Config")
            {
                _config = parmValue as CSUserAdministrationConfig;
            }
        }

        public override void LoadGridData()
        {
			LoadCSAgents();          
        }

		public override bool Validate(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{
			string methodName = "Validate";

            try
            {
                string uname = string.Empty;
                string pwd = string.Empty;
                CSAgent agent = null;
                if (gridAction != AspDynamicGrid.GridAction.Update)
                {
                    agent = new CSAgent();
                    CSAgent loggedInAgent = PortalState.GetLoggedInCSAgent();
                    if (loggedInAgent == null)
                    {
                        string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "NoCurrentAgent.Text", "Unable to get CSAGent.  PortalId = {0}, UserId = {1}"),
                            portalId, userId);
                        _logger.Error(_className, methodName, string.Format("Unable to get CSAGent.  PortalId = {0}, UserId = {1}",
                            portalId, userId));
                        throw new LWDynamicGridException(errMsg);
                    }
                    if (!EnablePasswordExpiry) agent.PasswordExpireDate = DateTimeUtil.MaxValue;
                }
                else
                {
                    long id = long.Parse(columns[0].Data.ToString());
                    agent = CSService.GetCSAgentById(id);
                }

                foreach (DynamicGridColumnSpec column in columns)
                {
                    if (column.Name == "Username")
                    {
                        uname = (string)column.Data;
                        CSAgent existing = CSService.GetCSAgentByUserName(uname, null);
                        if (existing != null)
                        {
                            string errMsg = ResourceUtils.GetLocalWebResource(ParentControl, "AgentAlreadyExists.Text", "An agent already exists with this username.");
                            if (gridAction != AspDynamicGrid.GridAction.Update)
                            {
                                _logger.Error(_className, methodName, "An agent already exists with this username.");
                                OnValidationError(errMsg, column);
                                return false;
                            }
                            else
                            {
                                if (existing.Id != agent.Id)
                                {
                                    _logger.Error(_className, methodName, "An agent already exists with this username.");
                                    OnValidationError(errMsg, column);
                                    return false;
                                }
                            }
                        }
                        agent.Username = uname;
                    }
                    else if (column.Name == "PhoneNumber")
                    {
                        string nmbr = (string)column.Data;
                        if (!string.IsNullOrWhiteSpace(nmbr))
                        {
                            foreach (char c in nmbr)
                            {
                                if (!char.IsDigit(c) && !char.IsWhiteSpace(c) && c != '-')
                                {
                                    string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "PhoneInvalidChar.Text", "Invalid character in phone number {0}."), c);
                                    _logger.Error(_className, methodName, errMsg);
                                    OnValidationError(string.Format("Invalid character in phone number {0}.", c), column);
                                    return false;
                                }
                            }
                        }
                    }
                    else if (column.Name == "Password" && gridAction != AspDynamicGrid.GridAction.Update)
                    {
                        pwd = (string)column.Data;
                    }
                    else if (EnablePasswordExpiry && column.Name == "PasswordExpireDate")
                    {
                        if (column.Data != null)
                        {
                            if (column.Data.GetType() == typeof(string))
                            {
                                if (!string.IsNullOrEmpty((string)column.Data))
                                {
                                    agent.PasswordExpireDate = DateTime.Parse((string)column.Data);
                                }
                            }
                            if (column.Data.GetType() == typeof(DateTime))
                            {
                                agent.PasswordExpireDate = (DateTime)column.Data;
                            }

                            int days = StringUtils.FriendlyInt32(LWConfigurationUtil.GetConfigurationValue("LWPasswordExpiryInterval"), 90);
                            if (days > 90) days = 90;
                            DateTime maxExpireDate = DateTime.Now.AddDays(days);
                            if (agent.PasswordExpireDate != null && agent.PasswordExpireDate > maxExpireDate)
                            {
                                string errMsg = ResourceUtils.GetLocalWebResource(ParentControl, "ExpireBeyondError.Text", "Cannot set password to expire beyond") + " " + maxExpireDate;
                                _logger.Error(_className, methodName, "Cannot set password to expire beyond");
                                OnValidationError(errMsg, column);
                                return false;
                            }
                        }
                    }
                }

                if (gridAction != AspDynamicGrid.GridAction.Update)
                {
                    if (string.IsNullOrEmpty(pwd) && !AllowNullPasswords)
                    {
                        string errMsg = ResourceUtils.GetLocalWebResource(ParentControl, "ExpireBeyondError.Text", "Password is required.");
                        _logger.Error(_className, methodName, "Password is required.");
                        OnValidationError(errMsg, columns.Where(o => o.Name == "Password").FirstOrDefault());
                        return false;
                    }

                    // This check is also done in ChangePassword() but we need to check the
                    // password validity before the agent is created.  This method will throw
                    // an exception if the password is not valid.
                    if (!string.IsNullOrEmpty(pwd) || !AllowNullPasswords)
                    {
                        try
                        {
                            LWPasswordUtil.ValidatePassword(agent.Username, pwd);
                        }
                        catch (Exception ex)
                        {
                            OnValidationError(ex.Message, columns.Where(o => o.Name == "Password").FirstOrDefault());
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error saving CSAgent.", ex);
                throw;
            }
			return true;
		}

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
			string methodName = "SaveGridData";

            try
            {
                string uname = string.Empty;
                string pwd = string.Empty;
                CSAgent agent = null;
                if (gridAction != AspDynamicGrid.GridAction.Update)
                {
                    agent = new CSAgent();
                    CSAgent loggedInAgent = PortalState.GetLoggedInCSAgent();
                    if (loggedInAgent == null)
                    {
                        string errMsg = string.Format("Unable to get CSAGent.  PortalId = {0}, UserId = {1}",
                            portalId, userId);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWDynamicGridException(errMsg);
                    }
                    else
                    {
                        agent.CreatedBy = loggedInAgent.Id;
                    }
                    if (!EnablePasswordExpiry) agent.PasswordExpireDate = DateTimeUtil.MaxValue;
                }
                else
                {
                    long id = long.Parse(columns[0].Data.ToString());
                    agent = CSService.GetCSAgentById(id);
                }

                foreach (DynamicGridColumnSpec column in columns)
                {
                    if (column.Name == "RoleId")
                    {
                        agent.RoleId = long.Parse(column.Data.ToString());
                    }
                    else if (column.Name == "GroupId" && column.Data != null)
                    {
                        if (!string.IsNullOrEmpty((string)column.Data))
                        {
                            agent.GroupId = long.Parse(column.Data.ToString());
                        }
                        else
                        {
                            agent.GroupId = null;
                        }
                    }
                    else if (column.Name == "AgentNumber" && column.Data != null)
                    {
                        if (!string.IsNullOrEmpty((string)column.Data))
                        {
                            agent.AgentNumber = long.Parse(column.Data.ToString());
                        }
                        else
                        {
                            agent.AgentNumber = null;
                        }
                    }
                    else if (column.Name == "FirstName")
                    {
                        agent.FirstName = (string)column.Data;
                    }
                    else if (column.Name == "LastName")
                    {
                        agent.LastName = (string)column.Data;
                    }
                    else if (column.Name == "EmailAddress")
                    {
                        agent.EmailAddress = (string)column.Data;
                    }
                    else if (column.Name == "PhoneNumber")
                    {
                        agent.PhoneNumber = (string)column.Data;
                    }
                    else if (column.Name == "Extension")
                    {
                        agent.Extension = (string)column.Data;
                    }
                    else if (column.Name == "Username")
                    {
                        uname = (string)column.Data;
                        CSAgent existing = CSService.GetCSAgentByUserName(uname, null);
                        if (existing != null)
                        {
                            string errMsg = ResourceUtils.GetLocalWebResource(ParentControl, "AgentAlreadyExists.Text", "An agent already exists with this username.");
                            if (gridAction != AspDynamicGrid.GridAction.Update)
                            {
                                _logger.Error(_className, methodName, "An agent already exists with this username.");
                                OnValidationError(errMsg, column);
                                return;
                            }
                            else
                            {
                                if (existing.Id != agent.Id)
                                {
                                    _logger.Error(_className, methodName, "An agent already exists with this username.");
                                    OnValidationError(errMsg, column);
                                    return;
                                }
                            }
                        }
                        agent.Username = uname;
                    }
                    else if (column.Name == "Password" && gridAction != AspDynamicGrid.GridAction.Update)
                    {
                        pwd = (string)column.Data;
                        agent.Password = pwd;
                    }
                    else if (column.Name == "PasswordChangeRequired")
                    {
                        agent.PasswordChangeRequired = (bool)column.Data;
                    }
                    else if (EnablePasswordExpiry && column.Name == "PasswordExpireDate")
                    {
                        if (column.Data != null)
                        {
                            if (column.Data.GetType() == typeof(string))
                            {
                                if (!string.IsNullOrEmpty((string)column.Data))
                                {
                                    agent.PasswordExpireDate = DateTime.Parse((string)column.Data);
                                }
                            }
                            if (column.Data.GetType() == typeof(DateTime))
                            {
                                agent.PasswordExpireDate = (DateTime)column.Data;
                            }

                            int days = StringUtils.FriendlyInt32(LWConfigurationUtil.GetConfigurationValue("PasswordExpiryInterval"), 90);
                            if (days > 90) days = 90;
                            DateTime maxExpireDate = DateTime.Now.AddDays(days);
                            if (agent.PasswordExpireDate != null && agent.PasswordExpireDate > maxExpireDate)
                            {
                                string errMsg = ResourceUtils.GetLocalWebResource(ParentControl, "ExpireBeyondError.Text", "Cannot set password to expire beyond") + " " + maxExpireDate;
                                _logger.Error(_className, methodName, "Cannot set password to expire beyond");
                                OnValidationError(errMsg, column);
                                return;
                            }
                        }
                        else
                        {
                            agent.PasswordExpireDate = null;
                        }
                    }
                    else if (column.Name == "Status")
                    {
                        AgentAccountStatus oldStatus = agent.Status;
                        agent.Status = (AgentAccountStatus)Enum.Parse(typeof(AgentAccountStatus), column.Data.ToString());
                        if (gridAction == AspDynamicGrid.GridAction.Update && oldStatus != AgentAccountStatus.Active && agent.Status == AgentAccountStatus.Active)
                        {
                            // agent made active, so reset failed password attempt count
                            agent.FailedPasswordAttemptCount = 0;
                        }
                    }
                }

                if (gridAction == AspDynamicGrid.GridAction.Update)
                {
                    CSService.UpdateCSAgent(agent);
                }
                else
                {
                    if (string.IsNullOrEmpty(pwd) && !AllowNullPasswords)
                    {
                        _logger.Error(_className, methodName, "Password is required.");
                        OnValidationError(ResourceUtils.GetLocalWebResource(ParentControl, "reqPassword.Text", "Password is required"), columns.Where(o => o.Name == "Password").FirstOrDefault());
                        return;
                    }

                    // This check is also done in ChangePassword() but we need to check the
                    // password validity before the agent is created.  This method will throw
                    // an exception if the password is not valid.
                    if (!string.IsNullOrEmpty(pwd) || !AllowNullPasswords)
                    {
                        try
                        {
                            LWPasswordUtil.ValidatePassword(agent.Username, pwd);
                        }
                        catch (Exception ex)
                        {
                            OnValidationError(ex.Message, columns.Where(o => o.Name == "Password").FirstOrDefault());
                            return;
                        }
                    }

                    CSService.CreateCSAgent(agent);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error saving CSAgent.", ex);
                throw;
            }
        }

        public override string GetSuccessfullSaveMessage()
        {
            return ResourceUtils.GetLocalWebResource(ParentControl, "AgentSaveSuccessful.Text", "CSR agent saved successfully.");
        }

        public override bool IsButtonVisible(string commandName)
        {
            if (commandName == AspDynamicGrid.DELETE_ROW_COMMAND) return false;
            return base.IsButtonVisible(commandName);
        }

        public override int GetNumberOfRows()
        {
			return (_agents != null ? _agents.Count : 0);            
        }

        public override object GetColumnData(object keyVal, DynamicGridColumnSpec column)
        {
            long id = long.Parse(keyVal.ToString());
            CSAgent agent = CSService.GetCSAgentById(id);
            return GetData(agent, column, true);
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            CSAgent agent = _agents[rowIndex];
			return GetData(agent, column, false);            
        }

		public override object GetDefaultValues(DynamicGridColumnSpec column)
		{
            object result = null;
            switch (column.Name)
            {
                case "RoleId":
                    List<ListItem> rolevals = new List<ListItem>();
                    IList<CSRole> roles = CSService.GetAllRoles(false);
                    foreach (CSRole role in roles)
                    {
                        ListItem li = new ListItem();
                        li.Text = role.Name;
                        li.Value = role.Id.ToString();
                        rolevals.Add(li);
                    }
                    result = rolevals;
                    break;

                case "Status":
                    List<ListItem> statusvals = new List<ListItem>();
                    foreach (int enumVal in Enum.GetValues(typeof(AgentAccountStatus)))
                    {
                        ListItem li = new ListItem();
                        li.Text = Enum.GetName(typeof(AgentAccountStatus), enumVal);
                        li.Value = enumVal.ToString();
                        statusvals.Add(li);
                    }
                    result = statusvals;
                    break;

                case "PasswordExpireDate":
                    if (EnablePasswordExpiry)
                    {
                        int days = StringUtils.FriendlyInt32(LWConfigurationUtil.GetConfigurationValue("PasswordExpiryInterval"), 90);
                        if (days > 90) days = 90;
                        result = DateTime.Now.AddDays((double)days);
                    }
                    else
                    {
                        result = DateTimeUtil.MaxValue;
                    }
                    break;

                default:
                    result = null;
                    break;
            }
            return result;
		}

        #endregion

		#region Command Handling
		public override ICustomGridAction[] GetCustomCommands()
		{
			if (_actions == null)
			{
				_actions = new ICustomGridAction[1];
				_actions[0] = new ChangePasswordCommand(this);				
			}
			return _actions;
		}
		#endregion
    }
}
