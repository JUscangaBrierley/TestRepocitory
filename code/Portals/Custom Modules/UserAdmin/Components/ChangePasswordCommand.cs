using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.WebFrameWork.Controls.Grid;


namespace Brierley.AEModules.UserAdmin.Components
{
	public class ChangePasswordCommand : ICustomGridAction
	{
		#region Fields
		private string _name = "Change Password";
		private string _cssClass = "edit";
		private string _ctrId = "change_pwd";
		CSAgentsGridProvider _grdProvider = null;
		#endregion

		internal ChangePasswordCommand(CSAgentsGridProvider grdProvider)
        {
            _grdProvider = grdProvider;
        }

		#region Properties
		public string CommandName
		{
			get { return _name; }
		}
		public string CssClass
		{
			get { return _cssClass; }
		}
		public string ControlId
		{
			get { return _ctrId; }
		}
		public string Text
		{
			get { return _name; }
		}
		#endregion

		#region Methods
		public void HandleCommand(System.Web.UI.Page page, object key)
		{
			//long agentId = long.Parse(key.ToString());			
			//_grdProvider.OnTemplateFiledsClicked(attCode);
		}
		#endregion
	}
}
