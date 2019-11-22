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
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Configuration;

namespace Brierley.LWModules.SelectLanguage
{
	public partial class ViewSelectLanguage : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewSelectLanguage";
		private const string _modulePath = "~/Controls/Modules/SelectLanguage/ViewSelectLanguage.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		protected SelectLanguageConfig _config = null;
		#endregion

		#region page life cycle
		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			
			try
			{
				_config = ConfigurationUtil.GetConfiguration<SelectLanguageConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new SelectLanguageConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}

				if (!Page.IsPostBack)
				{
					lblLanguagePrompt.Text = ResourceUtils.GetLocalWebResource(_modulePath, "lblLanguagePrompt.Text");

					if (drpLanguages.Items.Count < 1)
					{
						string selectPrompt = ResourceUtils.GetLocalWebResource(_modulePath, "SelectPrompt.Text");
						drpLanguages.Items.Add(new ListItem(selectPrompt, string.Empty));

						List<string> languages = _config.GetLanguagesToShow();
						if (languages != null && languages.Count > 0)
						{
							char[] _delimiter = new char[] { ':' };
							foreach (string language in languages)
							{
								// culture:language
								string[] tokens = language.Split(_delimiter);
								if (tokens.Length > 1)
								{
									drpLanguages.Items.Add(new ListItem(tokens[1], tokens[0]));
								}
							}
						}

						HttpCookie cookie = HttpContext.Current.Request.Cookies["SelectedLanguageCookie"];
						if (cookie != null)
						{
							drpLanguages.SelectedValue = cookie["SelectedLanguage"];
						}
					}
				}
				drpLanguages.SelectedIndexChanged += new EventHandler(drpLanguages_SelectedIndexChanged);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}
		#endregion

		#region event handlers
		void drpLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			const string methodName = "drpLanguages_SelectedIndexChanged";
			try
			{
				SetLanguageCookie(drpLanguages.SelectedValue);

				if (_config.RefreshPageWhenLanguageChanged) 
				{
					Response.Redirect(Request.RawUrl); 
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}
		#endregion

		#region private methods
		private void SetLanguageCookie(string culture)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies["SelectedLanguageCookie"];
			if (cookie == null)
			{
				cookie = new HttpCookie("SelectedLanguageCookie");
			}
			cookie["SelectedLanguage"] = culture;
			HttpContext.Current.Response.Cookies.Add(cookie);
		}
		#endregion
	}
}
