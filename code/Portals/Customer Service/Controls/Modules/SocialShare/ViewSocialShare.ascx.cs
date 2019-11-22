using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.SocialNetwork.Controls;

namespace Brierley.LWModules.SocialShare
{
	public partial class ViewSocialShare : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewSocialShare";
		private const string _modulePath = "~/Controls/Modules/SocialShare/ViewSocialShare.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private SocialShareConfig _config = null;
		#endregion

		#region properties

		#endregion

		#region page life cycle
		protected override void OnLoad(EventArgs e)
		{
			const string methodName = "OnLoad";
			try
			{
				base.OnLoad(e);

				InitializeConfig();

				SocialShareControl socialShare = new SocialShareControl(_config);
				Controls.Add(socialShare);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				OnException(ex);
			}
		}
		#endregion

		#region event handlers
		#endregion

		#region private methods
		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<SocialShareConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new SocialShareConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}
		#endregion
	}
}
