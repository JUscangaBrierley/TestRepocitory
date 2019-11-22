using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common;



namespace Brierley.LWModules.CSUserAdministration.Components
{
	public class ChangePasswordCommand : ICustomGridAction
	{
		#region Fields
		private string _name = "Change Password";
		private string _cssClass = "edit";
		private string _ctrId = "change_pwd";
        private string _text;
		CSAgentsGridProvider _grdProvider = null;
		#endregion

		internal ChangePasswordCommand(CSAgentsGridProvider grdProvider)
        {
            _grdProvider = grdProvider;
            _text = ResourceUtils.GetLocalWebResource(_grdProvider.ParentControl, "CommandChangePassword.Text", "Change Password");
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
            get { return _text; }
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
