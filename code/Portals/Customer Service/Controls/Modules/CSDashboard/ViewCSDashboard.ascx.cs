using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CSDashboard
{
    public partial class ViewCSDashboard : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewCSDashboard";
        private const string _modulePath = "~/Controls/Modules/CSDashboard/ViewCSDashboard.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private CSDashboardConfig _config = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
                IpcManager.RegisterEventHandler("MemberSelected", this, false); 
				_config = Brierley.WebFrameWork.Portal.Configuration.ConfigurationUtil.GetConfiguration<CSDashboardConfig>(ConfigurationKey);
				if (_config == null)
				{
					_logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey.ToString()));
					ulLinks.Visible = false;
					return;
				}
				CheckSecurityPermissions();
				LoadDashBoardLinks();
			}
			catch (System.Threading.ThreadAbortException)
			{
				// it's a redirect so ignore
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected Exception", ex);
				throw;
			}
		}

		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}

		protected bool IsMemberSelected()
		{
			return PortalState.CurrentMember != null;
		}

		private void CheckSecurityPermissions()
		{
			const string methodName = "CheckSecurityPermissions";
			if (_config == null)
			{
				return;
			}
			if (_config.Pages == null || _config.Pages.Count == 0)
			{
				return;
			}
			if (_config.NoPermissionUrl == PortalState.CurrentPage.RouteUrl)
			{
				return;
			}

			CSAgent agent = PortalState.GetLoggedInCSAgent();
			if (agent == null)
			{
				return;
			}

			//get current page and check permissions
			CSPageConfig page = (from x in _config.Pages where x.RouteUrl == PortalState.CurrentPage.RouteUrl select x).FirstOrDefault();
			if (page == null)
			{
				return;
			}

			bool hasPermission = false;

			IList<string> allowedFunctions = page.AllowedFunctions;
			foreach (string f in page.AllowedFunctions)
			{
				if (agent.HasPermission(f))
				{
					hasPermission = true;
					break;
				}
			}

			if (hasPermission)
			{
				hasPermission = false;
				var member = PortalState.CurrentMember;
				if (member == null && (page.DisplayFlags & NavigationDisplayFlags.NoMember) != 0)
				{
					hasPermission = true;
				}
				else if (member != null)
				{
					if (member.MemberStatus == FrameWork.Common.MemberStatusEnum.NonMember && (page.DisplayFlags & NavigationDisplayFlags.NonMember) != 0)
					{
						hasPermission = true;
					}
					else if (member.MemberStatus != FrameWork.Common.MemberStatusEnum.NonMember && (page.DisplayFlags & NavigationDisplayFlags.Member) != 0)
					{
						hasPermission = true;
					}
				}
			}

			if (!hasPermission)
			{
				bool redirected = false;
				if (!string.IsNullOrEmpty(_config.NoPermissionUrl))
				{
					Uri noPermissionUrl = StringUtils.AbsoluteUri(Request.Url, _config.NoPermissionUrl);
					if (noPermissionUrl != null)
					{
						Uri loginUrl = StringUtils.AbsoluteUri(Request.Url, FormsAuthentication.LoginUrl);
						if (noPermissionUrl.ToString().ToLower() != loginUrl.ToString().ToLower())
						{
							redirected = true;
							Response.Redirect(_config.NoPermissionUrl, true);
						}
						else
						{
							_logger.Error(_className, methodName, "NoPermissionUrl is incorrectly configured to go to Login page");
						}
					}
					else
					{
						_logger.Error(_className, methodName, "configured NoPermissionUrl is invalid");
					}
				}

				if (!redirected)
				{
					bool foundPage = false;
					var member = PortalState.CurrentMember;
					foreach (CSPageConfig p in _config.Pages)
					{
						if (member == null && (p.DisplayFlags & NavigationDisplayFlags.NoMember) == 0) continue;
						if (member != null)
						{
							if (member.MemberStatus == MemberStatusEnum.NonMember && (p.DisplayFlags & NavigationDisplayFlags.NonMember) == 0) continue;
							if (member.MemberStatus != MemberStatusEnum.NonMember && (p.DisplayFlags & NavigationDisplayFlags.Member) == 0) continue;
						}

						foreach (string f in p.AllowedFunctions)
						{
							if (agent.HasPermission(f))
							{
								foundPage = true;
								_logger.Error(_className, methodName, "redirecting to: " + p.RouteUrl);
								Response.Redirect(p.RouteUrl, true);
								break;
							}
						}
					}

					if (!foundPage)
					{
						_logger.Error(_className, methodName, "unable to find a page for redirect, logging out user");
						Session.Abandon();
						FormsAuthentication.RedirectToLoginPage();
					}
				}
			}
		}

		protected void LoadDashBoardLinks()
		{
			string method = "LoadDashBoardLinks";
			CSAgent agent = PortalState.GetLoggedInCSAgent();
			var configuredPage = PortalState.CurrentPage;
			if (agent != null)
			{
				_logger.Trace(_className, method, "Creating menu links for CSAgent " + agent.Username);

				bool memberSelected = IsMemberSelected();
				IEnumerable<CSPageConfig> pages = null;
				if (memberSelected && _config.Pages.Where(o => o.DisplayOrderMember > 0).Count() > 0)
				{
					pages = _config.Pages.OrderBy(o => o.DisplayOrderMember);
				}
				else if (!memberSelected && _config.Pages.Where(o => o.DisplayOrderNoMember > 0).Count() > 0)
				{
					pages = _config.Pages.OrderBy(o => o.DisplayOrderNoMember);
				}
				else
				{
					pages = _config.Pages;
				}

				foreach (CSPageConfig page in pages)
				{
					var member = PortalState.CurrentMember;
					if (member == null)
					{
						if ((page.DisplayFlags & NavigationDisplayFlags.NoMember) == 0)
						{
							continue;
						}
					}
					else
					{
						if (member.MemberStatus == FrameWork.Common.MemberStatusEnum.NonMember && (page.DisplayFlags & NavigationDisplayFlags.NonMember) == 0)
						{
							continue;
						}
						else if (member.MemberStatus != FrameWork.Common.MemberStatusEnum.NonMember && (page.DisplayFlags & NavigationDisplayFlags.Member) == 0)
						{
							continue;
						}
					}

					_logger.Debug(_className, method, "Configuring page " + page.RouteUrl);
					IList<string> allowedFunctions = page.AllowedFunctions;
					foreach (string f in allowedFunctions)
					{
						if (agent.HasPermission(f))
						{
							HtmlGenericControl li = new HtmlGenericControl("li");
							string cssClass = (page.RouteUrl == configuredPage.RouteUrl) ? "DashboardLink Current" : "DashboardLink";
							
							if (page.ResetSelectedMemberOnClick)
							{
								LinkButton link = new LinkButton() { CausesValidation = false, CssClass = cssClass, ID = "lnkDashboard" + page.Id };
                                link.Controls.Add(new LiteralControl(ResourceUtils.GetLocalWebResource(_modulePath, page.DisplayText, page.DisplayText)));
								string url = page.RouteUrl;
								link.Click += delegate { RedirectLink(url); };
								li.Controls.Add(link);
							}
							else
							{
                                var link = new HtmlGenericControl("a") { InnerHtml = ResourceUtils.GetLocalWebResource(_modulePath, page.DisplayText, page.DisplayText) };
								link.Attributes.Add("href", page.RouteUrl);
								link.Attributes.Add("class", cssClass);
								li.Controls.Add(link);
							}

							ulLinks.Controls.Add(li);

							break;

						}
						else
						{
							_logger.Debug(_className, method, string.Format("User does not have access to {0} for page {1}", f, page.RouteUrl));
						}
					}
				}
			}
			else
			{
				_logger.Trace(_className, method, "No customer service agent logged in.");
			}
		}

		void RedirectLink(string url)
		{
			PortalState.CurrentMember = null;
			//IpcManager.PublishEvent("MemberSelected", ConfigurationKey, null);
			Response.Redirect(url);
		}

        public void HandleEvent(IpcEventInfo info)
        {
            if (info.EventName == "MemberSelected" && info.EventData == null)
            {
                ulLinks.Controls.Clear();
                LoadDashBoardLinks();
            }
        }

        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
	}
}
