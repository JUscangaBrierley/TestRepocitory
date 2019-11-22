using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CSDataHook
{
	public partial class ViewCSDataHook : ModuleControlBase
	{
		private const string _className = "ViewCSDataHook";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private CSDataHookConfig _config = null;
		private NameValueCollection _dataHooks = null;

		protected bool AllowPostbacks
		{
			get
			{
				return _config != null && _config.AllowPostbacks;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			try
			{
				_config = ConfigurationUtil.GetConfiguration<CSDataHookConfig>(ConfigurationKey) ?? new CSDataHookConfig();

				string cookie = PortalState.GetCookie(CSDataHookConfig.DataHookCookieName, true);
				if (string.IsNullOrEmpty(cookie))
				{
					return;
				}

				_dataHooks = System.Web.HttpUtility.ParseQueryString(cookie);

				if (PortalState.Portal.PortalMode == PortalModes.CustomerService)
				{
					//check for selected member and put in cache if found
					foreach (string key in _dataHooks.Keys)
					{
						if (_dataHooks[key].ToLower() == CSDataHookConfig.SelectedMemberKey)
						{
							string val = _dataHooks[key];
							if (val.Contains(CSDataHookConfig.SelectedMemberDelimiter))
							{
								Member m = null;
								var split = val.Split((char)CSDataHookConfig.SelectedMemberDelimiter);
                                switch (split[0].ToLower())
                                {
                                    case "ipcode":
                                        long ipcode = -1;
                                        if (long.TryParse(split[1], out ipcode))
                                        {
                                            m = LoyaltyService.LoadMemberFromIPCode(ipcode);
                                        }
                                        break;
                                    case "primaryemail":
                                        m = LoyaltyService.LoadMemberFromEmailAddress(split[1]);
                                        break;
                                    case "alternateid":
                                        m = LoyaltyService.LoadMemberFromAlternateID(split[1]);
                                        break;
                                    case "loyaltyid":
                                        m = LoyaltyService.LoadMemberFromLoyaltyID(split[1]);
                                        break;
                                }

								if (m != null)
								{
									PortalState.CurrentMember = m;
									IpcManager.PublishEvent("MemberSelected", ConfigurationKey, m);
								}
							}
						}
					}
				}

				var hooks = new Dictionary<string, string>();
				foreach (string key in _dataHooks.Keys)
				{
					if (
						key == null || 
						key.ToLower() == CSDataHookConfig.SelectedMemberKey || 
						string.IsNullOrEmpty(_dataHooks[key]) ||
						!CheckPage(key)
						)
					{
						continue;
					}
					string page = RemovePage(key);
					if (!hooks.ContainsKey(page))
					{
						hooks.Add(page, _dataHooks[key]);
					}
				}

				if (hooks.Count > 0)
				{
					js.Visible = true;
					var joined = string.Join(", ", (from x in hooks.Keys select "['" + x.Replace("'", "\\'") + "', '" + hooks[x].Replace("'", "\\'") + "']").ToList());
					string hookScript = "<script type=\"text/javascript\">_hooks = [" + joined + "];</script>";
					Page.ClientScript.RegisterStartupScript(this.GetType(), "hooks", hookScript);
				}
			}
			catch (Exception ex)
			{
				//This should be considered non-fatal. We'll allow the page to continue on by not re-throwing. Worst case is that some data is missing from the page.
				_logger.Error("ViewCSDataHook.ascx.cs", "OnInit", "Unexpected error processing data hooks.", ex);
			}
		}

		private bool CheckPage(string key)
		{
			if (key.Contains("|"))
			{
				string url = key.Substring(0, key.IndexOf("|"));
				return (Page.Request.RawUrl.ToLower().EndsWith(url.ToLower()));
			}
			return true;
		}

		private string RemovePage(string key)
		{
			if (key.Contains("|"))
			{
				return key.Substring(key.IndexOf("|") + 1);
			}
			return key;
		}

		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}

	}
}
