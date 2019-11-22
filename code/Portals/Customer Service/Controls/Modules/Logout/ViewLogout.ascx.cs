using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Configuration;

namespace Brierley.LWModules.Logout
{
    public partial class ViewLogout : ModuleControlBase
    {
        protected LogoutModuleConfig _config = null;
		private const string _modulePath = "~/Controls/Modules/Logout/ViewLogout.ascx";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
				if (Request.IsAuthenticated) {
					InitializeConfig();

					LWLogoutControl logoutControl = new LWLogoutControl();
					logoutControl.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.LinkTextResourceKey);
					Controls.Add(logoutControl);
				}
            }
            catch (Exception ex)
            {
				OnException(ex);
            }
        }

        private void InitializeConfig()
        {
            if (_config == null)
            {
				_config = ConfigurationUtil.GetConfiguration<LogoutModuleConfig>(ConfigurationKey);
				if (_config == null)
                {
					_config = new LogoutModuleConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
                }
            }
        }
    }
}
