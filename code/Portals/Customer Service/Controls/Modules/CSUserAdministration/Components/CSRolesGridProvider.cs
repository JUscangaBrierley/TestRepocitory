using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;

namespace Brierley.LWModules.CSUserAdministration.Components
{
	public class CSRolesGridProvider : AspGridProviderBase
    {
		private const string _className = "CSRolesGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private IList<CSRole> _roles = null;
		private IList<CSFunction> _functions = null;

		public CSRolesGridProvider()
		{
            _functions = CSService.GetAllFunctions();
		}

        #region Helpers
        
        protected void LoadRoles()
        {
            _roles = CSService.GetAllRoles(true);
        }

		private CSFunction GetFunction(string fname)
		{
			foreach (CSFunction func in _functions)
			{
				if (func.Name == fname)
				{
					return func;
				}
			}
			return null;
		}

		private object GetData(CSRole role, DynamicGridColumnSpec column, bool editPanel)
		{
			string methodName = "GetData";

			object value = null;
			if (column.Name == "Id")
			{
				value = role.Id;
			}
			else if (column.Name == "Name")
			{
				value = role.Name;
			}
			else if (column.Name == "Description")
			{
				value = role.Description;
			}
			else if (column.Name == "PointAwardLimit")
			{
				value = role.PointAwardLimit;
			}
			else if (column.Name == "AssociatedFunctions")
			{
				bool first = true;
				string funcList = string.Empty;
				if (role != null && role.Functions != null)
				{
					foreach (CSFunction func in role.Functions)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							funcList += "; ";
						}

                        //This checks to see if we are populating the eidt panel or the grid since we have to take two different approaches to bind the data correctly
                        if (editPanel)
                        {
                            funcList += func.Name;
                        }
                        else
                        {
                            funcList += ResourceUtils.GetLocalWebResource(ParentControl, "csRole" + func.Name + ".Text", func.Name);
                        }
                    }
				}
				value = funcList;
			}						
			return value;
		}

        #endregion

        #region Grid Properties

		public override string Id
		{
			get { return "grdCSRoles"; }
		}

        protected override string GetGridName()
        {
            return "CSRoles";
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[5];

			int idx = 0;
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "Id";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Id.Text", "ID");
            c.DataType = typeof(long);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            columns[idx++] = c;

            c = new DynamicGridColumnSpec();
			c.Name = "Name";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Name.Text", "Name");
            c.DataType = typeof(string);
			c.IsEditable = true;
            c.IsRequired = true;
            c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "ProvideRoleName.Text", "Please provide a role name."), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
            c.MaxLength = 50;
            columns[idx++] = c;

			c = new DynamicGridColumnSpec();
			c.Name = "Description";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Description.Text", "Description");
			c.DataType = typeof(string);
			c.IsEditable = true;
			c.IsVisible = false;
			c.IsRequired = false;
			c.EditControlType = DynamicGridColumnSpec.TEXTAREA;
			c.Columns = 50;
			c.Rows = 5;
            c.MaxLength = 255;
			columns[idx++] = c;

			c = new DynamicGridColumnSpec();
			c.Name = "PointAwardLimit";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-PointAwardLimit.Text", "Point Award Limit");
			c.DataType = typeof(long);
			c.IsEditable = true;
			c.IsVisible = true;
			c.IsRequired = false;
			c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
            c.Validators.Add(new RangeValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "ValidNumberError.Text", "Enter a valid number between 0 and 2147483647."), MinimumValue = "0", MaximumValue = int.MaxValue.ToString(), Type = ValidationDataType.Integer, CssClass = "Validator", ValidationGroup = this.ValidationGroup });
			columns[idx++] = c;

			c = new DynamicGridColumnSpec();
			c.Name = "AssociatedFunctions";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-AssociatedFunctions.Text", "Functions");
			c.DataType = typeof(string);
            c.IsRequired = true;
			c.EditControlType = DynamicGridColumnSpec.LISTBOX;            
            c.Rows = 10;
			c.IsVisible = true;
            c.Validators.Add(new CustomValidator()
            {
                CssClass = "Validator",
                ClientValidationFunction = "ValidateCheckedFunctions",
                Display = ValidatorDisplay.Dynamic,
                ValidationGroup = this.ValidationGroup,
                ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, Id + "ProvideFunction.Text", "Please provide at least one function for this role.")
            });
			columns[idx++] = c;
            
            return columns;
        }

        public override bool IsGridEditable()
        {
            return true;            
        }

        public override string GetGridInsertLabel()
        {
            return ResourceUtils.GetLocalWebResource(ParentControl, "btnNewRole.Text", "Add Role");
        }

		public override void DeleteGridData(object keyData)
		{
			string methodName = "DeleteGridData";

			string errMsg = string.Empty;
            try
            {
                CSRole role = null;
                long id = long.Parse(keyData.ToString());
                role = CSService.GetRole(id, true);

                if (role == null)
                {
                    return;
                }

                var agents = CSService.GetCSAgents(null, null, null, role, null, null);
                if (agents != null && agents.Count > 0)
                {
                    var agentList = (from x in agents select x.Username).ToArray();
                    string errorCannotDelete = ResourceUtils.GetLocalWebResource(ParentControl, "CannotDeleteRole.Text", "The role cannot be deleted because it is assigned to the following agents:\r\n {0}");
                    errMsg = string.Format(errorCannotDelete.Replace("\\r\\n" , Environment.NewLine ) , string.Join(",\r\n", agentList));
                    OnValidationError(errMsg);
                    throw new LWValidationException(errMsg);
                }

                CSService.DeleteRole(id);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error deleting CSRole.", ex);
                throw;
            }
		}
       
        #endregion

        #region Data Source

        public override void LoadGridData()
        {
			LoadRoles();          
        }


		public override bool Validate(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{
			string methodName = "Validate";
			bool valid = true;

			try
			{
				foreach (DynamicGridColumnSpec column in columns)
				{
                    if (column.Name == "Name")
                    {
                        string nameInput = (string)column.Data;
                        if (!string.IsNullOrEmpty(nameInput) && RoleNameUsed(nameInput, columns, gridAction))
                        {
                            OnValidationError(ResourceUtils.GetLocalWebResource(ParentControl, "RoldNameUsed.Text", "Role name has been used."), column.Control);
                            valid = false;
                        }
                    }
                    else if (column.Name == "Description")
					{
						string desc = (string)column.Data;
						if (!string.IsNullOrEmpty(desc) && desc.Length > 255)
						{
							OnValidationError(ResourceUtils.GetLocalWebResource(ParentControl, "DescriptionLength.Text", "Description cannot be more than 255 characters."), column.Control);
							valid = false;
						}
					}
                    //else if (column.Name == "PointAwardLimit")
                    //{
                    //    int rpoints = 0;
                    //    string data = (string)column.Data;
                    //    if (!string.IsNullOrEmpty(data))
                    //    {
                    //        rpoints = int.Parse(data);
                    //        if (rpoints <= 0)
                    //        {
                    //            OnValidationError("Enter a valid number greater than or equal to zero.", column.Control);
                    //            valid = false;
                    //        }                            
                    //    }                        
                    //}
					else if (column.Name == "AssociatedFunctions")
					{
						string[] tokens = ((string)column.Data).Split(';');
						if (tokens == null || tokens.Length == 0 || (tokens.Length == 1 && string.IsNullOrEmpty(tokens[0])))
						{
                            OnValidationError(ResourceUtils.GetLocalWebResource(ParentControl, "ProvideFunction.Text", "Please provide at least one function for this role."), column.Control);
							valid = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error validating CSRole.", ex);
				throw;
			}
			return valid;
		}

        /// <summary>
        /// Check if the entered role name has been used.
        /// </summary>
        private bool RoleNameUsed(string nameInput, DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            CSRole theRole = null;
            if (_roles != null && _roles.Count > 0)
                theRole = _roles.SingleOrDefault(r => r.Name.ToLower() == nameInput.ToLower());

            if (theRole == null)  // selected name not used
                return false;

            // name has been used; if we are creating new role
            if (gridAction == AspDynamicGrid.GridAction.Create)
                return true;

            // if we are updating existing role
            if (theRole.Id == long.Parse(columns[0].Data.ToString()))
                return false;
            else
                return true;
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
			string methodName = "SaveGridData";

			List<CSFunction> toAdd = new List<CSFunction>();
			List<CSFunction> toRemove = new List<CSFunction>();

            try
            {
                CSRole role = null;
                if (gridAction != AspDynamicGrid.GridAction.Update)
                {
                    role = new CSRole();
                }
                else
                {
                    long id = long.Parse(columns[0].Data.ToString());
                    role = CSService.GetRole(id, true);
                }

                foreach (DynamicGridColumnSpec column in columns)
                {
                    if (column.Name == "Name")
                    {
                        role.Name = (string)column.Data;
                    }
                    else if (column.Name == "Description")
                    {
                        role.Description = (string)column.Data;
                    }
                    else if (column.Name == "PointAwardLimit")
                    {
                        string data = (string)column.Data;
                        if (string.IsNullOrEmpty(data))
                        {
                            role.PointAwardLimit = null;
                        }
                        else
                        {
                            role.PointAwardLimit = int.Parse(data);
                        }
                    }
                    else if (column.Name == "AssociatedFunctions")
                    {
                        string[] tokens = ((string)column.Data).Split(';');
                        // which roles have been removed.
                        foreach (CSFunction func in role.Functions)
                        {
                            if (tokens == null || tokens.Length == 0)
                            {
                                toRemove.Add(func);
                            }
                            else
                            {
                                bool hasit = false;
                                foreach (string t in tokens)
                                {
                                    if (!string.IsNullOrEmpty(t))
                                    {
                                        if (t == func.Name)
                                        {
                                            hasit = true;
                                            break;
                                        }
                                    }
                                }
                                if (!hasit)
                                {
                                    toRemove.Add(func);
                                }
                            }
                        }
                        // determine whichones to add
                        if (tokens != null && tokens.Length > 0)
                        {
                            IList<string> allowedFunctions = new List<string>();
                            foreach (string t in tokens)
                            {
                                if (!string.IsNullOrEmpty(t))
                                {
                                    CSFunction func = GetFunction(t);
                                    if (!role.HasFunction(func.Name))
                                    {
                                        toAdd.Add(func);
                                    }
                                }
                            }
                        }
                    }
                }

                if (gridAction == AspDynamicGrid.GridAction.Update)
                {
                    CSService.UpdateRole(role);
                }
                else
                {
                    CSService.CreateRole(role);
                }
                foreach (CSFunction func in toRemove)
                {
                    CSService.RemoveFunctionFromRole(role, func);
                }
                foreach (CSFunction func in toAdd)
                {
                    CSService.AddFunctionToRole(role, func);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error saving CSRole.", ex);
                throw;
            }
        }

        public override bool IsButtonVisible(string commandName)
        {
            //if (commandName == AspDynamicGrid.DELETE_ROW_COMMAND) return false;
            return base.IsButtonVisible(commandName);
        }

        public override int GetNumberOfRows()
        {
			return (_roles != null ? _roles.Count : 0);            
        }

        public override object GetColumnData(object keyVal, DynamicGridColumnSpec column)
        {
            long id = long.Parse(keyVal.ToString());
            CSRole role = CSService.GetRole(id, true);
            return GetData(role, column, true);
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
			CSRole role = _roles[rowIndex];
			return GetData(role, column, false);            
        }

		public override object GetDefaultValues(DynamicGridColumnSpec column)
		{
			List<ListItem> vals = new List<ListItem>();
			if (column.Name == "AssociatedFunctions")
			{				
				foreach (CSFunction f in _functions)
				{
					ListItem li = new ListItem();
					li.Text = ResourceUtils.GetLocalWebResource(ParentControl, "csRole" + f.Name + ".Text", f.Name);
					li.Value = f.Name;
					vals.Add(li);
				}
			}
			else
			{
				return null;
			}
			return vals;
		}
        #endregion
    }
}
