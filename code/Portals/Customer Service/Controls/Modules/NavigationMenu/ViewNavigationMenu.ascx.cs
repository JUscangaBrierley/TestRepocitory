using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Brierley.LWModules.NavigationMenu
{
	public partial class ViewNavigationMenu : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewNavigationMenu";
		private const string _modulePath = "~/Controls/Modules/NavigationMenu/ViewNavigationMenu.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private NavigationMenuConfig _config = null;
		Member _member = null;
		private ContextObject _context = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
				_config = Brierley.WebFrameWork.Portal.Configuration.ConfigurationUtil.GetConfiguration<NavigationMenuConfig>(ConfigurationKey);
				if (_config == null || _config.Pages == null || _config.Pages.Count == 0)
				{
					_logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey.ToString()));
					ulLinks.Visible = false;
					return;
				}

				_member = PortalState.CurrentMember;
				
				//get current page and check permissions
				var page = (from x in _config.Pages where x.RouteUrl == PortalState.CurrentPage.RouteUrl select x).FirstOrDefault();
				if (page != null)
				{
					if (!string.IsNullOrEmpty(page.NoPermissionUrl) && !IsVisible(page))
					{
						Response.Redirect(page.NoPermissionUrl, true);
					}
				}
				LoadLinks();
				IpcManager.RegisterEventHandler("MemberUpdated", this, false);
				IpcManager.RegisterEventHandler("MemberCreated", this, false);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected Exception", ex);
				throw;
			}
		}


		private bool IsVisible(NavigationMenuItem page)
		{
			if (_member == null)
			{
				if ((page.DisplayFlags & NavigationDisplayFlags.UnAuthenticated) == 0)
				{
					//member is not logged in and the page does not allow unauthenticated members
					return false;
				}
			}
			else
			{
				if (_member.MemberStatus == FrameWork.Common.MemberStatusEnum.NonMember && (page.DisplayFlags & NavigationDisplayFlags.NonMember) == 0)
				{
					//member is a non-member status and the page does not allow non-members
					return false;
				}
				if (_member.MemberStatus == FrameWork.Common.MemberStatusEnum.Active && (page.DisplayFlags & NavigationDisplayFlags.Member) == 0)
				{
					//member is an active loyalty member and the page does not allow members
					return false;
				}
			}

			if (!string.IsNullOrEmpty(page.VisibilityFilter))
			{
				if (_context == null)
				{
					_context = new ContextObject() { Owner = _member };
					var environment = new Dictionary<string, object>();
					environment.Add("routeurl", PortalState.CurrentPage.RouteUrl);
					_context.Environment = environment;
				}
				Expression e = new ExpressionFactory().Create(page.VisibilityFilter);
				object obj = e.evaluate(_context);
				if (obj == null || !(obj is bool) || !(bool)obj)
				{
					return false;
				}
			}
			return true;
		}

		protected void LoadLinks(HtmlGenericControl control = null, string parentId = null)
		{
			string method = "LoadLinks";
			var configuredPage = PortalState.CurrentPage;

			if (control == null ^ string.IsNullOrEmpty(parentId))
			{
				throw new ArgumentException(ResourceUtils.GetLocalWebResource(_modulePath, "NullMatchError.Text", "control and parentId must both be null or not null."));
			}

			if (control == null)
			{
				control = ulLinks;
			}

			var pages =
				from p in _config.Pages
				where
				(
					(parentId == null && string.IsNullOrEmpty(p.ParentId)) ||
					p.ParentId == parentId
				) &&
				IsVisible(p)
				orderby p.DisplayOrder, p.Name
				select p;

			if (pages.Count() > 0 && !string.IsNullOrEmpty(parentId))
			{
				HtmlGenericControl ul = new HtmlGenericControl("ul");
				control.Controls.Add(ul);
				control = ul;
			}

			foreach (var p in pages)
			{
				_logger.Debug(_className, method, "Configuring page " + p.Name);

				HtmlGenericControl li = new HtmlGenericControl("li");
				string cssClass = (p.RouteUrl == configuredPage.RouteUrl) ? "NavigationLink Current" : "NavigationLink";
				if (!string.IsNullOrEmpty(p.CssClass))
				{
					cssClass += " " + p.CssClass;
				}

				Control link = null;
				string linkText = StringUtils.FriendlyString(ResourceUtils.GetLocalWebResource(_modulePath, p.DisplayTextResourceKey), p.Name);
				if (p.LogoutOnClick)
				{
					link = new LinkButton() { CausesValidation = false, CssClass = cssClass };
					link.Controls.Add(new LiteralControl(linkText));
					string url = p.RouteUrl;
					((LinkButton)link).Click += delegate { RedirectLink(url, true); };
				}
				else
				{
					link = new HtmlGenericControl("a") { InnerHtml = linkText };
					((HtmlGenericControl)link).Attributes.Add("href", p.RouteUrl);
					((HtmlGenericControl)link).Attributes.Add("class", cssClass);
				}

				//if (string.IsNullOrEmpty(parentId))
				//{
				//	var h2 = new HtmlGenericControl("h2");
				//	h2.Controls.Add(link);
				//	li.Controls.Add(h2);
				//}
				//else
				//{
					li.Controls.Add(link);
				//}

				control.Controls.Add(li);
				LoadLinks(li, p.Id);
			}
		}


		void RedirectLink(string url, bool logout = false)
		{
			if (logout)
			{
				Session.Abandon();
				FormsAuthentication.SignOut();
			}
			Response.Redirect(url);
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Trace(_className, methodName, "Begin");
			if (
				info.PublishingModule != base.ConfigurationKey &&
				(info.EventName == "MemberUpdated" || info.EventName == "MemberCreated") &&
				_config != null
				)
			{
				_member = info.EventData as Member;
				ulLinks.Controls.Clear();
				LoadLinks();
			}
			_logger.Trace(_className, methodName, "End");
		}

	}
}