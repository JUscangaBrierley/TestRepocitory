using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Authentication;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Security;
using Brierley.WebFrameWork.SocialNetwork;
using Brierley.WebFrameWork.SocialNetwork.Controls;
using Mobile = Brierley.WebFrameWork.Controls.Mobile;
using System.Web.UI.HtmlControls;

namespace Brierley.LWModules.Login
{
	public partial class ViewLogin : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewLogin";
		private const string _modulePath = "~/Controls/Modules/Login/ViewLogin.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		protected string delimiter = "##";
		protected HtmlGenericControl lblUsername;
		protected TextBox tbUsername;
		protected HtmlGenericControl lblPassword;
		protected TextBox tbPassword;
		protected IButtonControl btnLogin;
		protected IButtonControl btnForgotPassword;
		protected Mobile.CheckBox cbRememberMe;
		protected PlaceHolder pchError;
		protected LoginModuleConfig _config = null;
		protected Panel _pnlLogin = new Panel();
		protected PlaceHolder _phLoginPanelContent = new PlaceHolder();
		protected ChangePasswordPanel _pnlChangePassword = new ChangePasswordPanel();
		protected ILoginModuleProvider _provider = null;
		private string _rememberIdentityCookieName = "Login_" + PortalState.Portal.Name;
		#endregion

		#region page life cycle
		protected void Page_Init(object sender, EventArgs e)
		{
			const string methodName = "Page_Init";
            LoadResourceStrings();

			try
			{
				if (_config == null)
				{
					_config = ConfigurationUtil.GetConfiguration<LoginModuleConfig>(ConfigurationKey);
					if (_config == null)
					{
						_logger.Error(_className, methodName, string.Format("Configuration not found for key '{0}'", ConfigurationKey));
						this.Visible = false;
						return;
					}
				}

				_pnlLogin.Controls.Add(_phLoginPanelContent);
				Controls.Add(_pnlLogin);

				_pnlChangePassword.Visible = false;
				_pnlChangePassword.ChangePasswordClicked += new ChangePasswordPanel.ChangePasswordClickedEventHandler(_pnlChangePassword_ChangePasswordClicked);
				Controls.Add(_pnlChangePassword);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";

			try
			{
				if (!string.IsNullOrEmpty(_config.ProviderClass) && !string.IsNullOrEmpty(_config.ProviderAssembly))
				{
					_provider = (ILoginModuleProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssembly, _config.ProviderClass);
					_logger.Trace(_className, methodName, string.Format("Loaded login provider: assembly={0}, class={1}", _config.ProviderAssembly, _config.ProviderClass));
				}

				if (IsNotAuthenticated())
				{
					var portal = PortalState.Portal;
					if (portal != null && portal.SAMLForcePartnerSSOLogin)
					{
						Response.Redirect("~/SAML/RequestPartnerLogin.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl), false);
						return;
					}

					if (!Page.IsPostBack)
					{
						if (IsSocialNetworkLogin())
						{
							if (DoSocialNetworkLogin()) return;
						}
						else if (_config.AllowMTouchLogin && IsMTouchLogin())
						{
							if (DoMTouchLogin()) return;
						}
					}

					if (!Page.IsPostBack)
					{
						_pnlLogin.Visible = true;
					}

					// Get dynamic content to prompt for login credentials
					string contentKeyName = _config.SocialNetworkProvider == "None" ? "DefaultContentNoSocial" : "DefaultContentSocial";
					string contentTemplate = ResourceUtils.GetLocalWebResource(_modulePath, _config.GetResourceKeyName(contentKeyName));
					LoadDynamicContent(_phLoginPanelContent, contentTemplate);
				}

				if (btnLogin != null)
				{
					_pnlLogin.DefaultButton = ((WebControl)btnLogin).ID;
				}

				bool nativeSocialLoginEnabled = StringUtils.FriendlyString(_config.SocialNetworkProvider) == "Native"
					&& (_config.NativeFacebookEnabled || _config.NativeTwitterEnabled || _config.NativeGooglePlusEnabled);

				if (nativeSocialLoginEnabled)
				{
					try
					{
						if (!string.IsNullOrEmpty(Request.Form["hfSLIdentity"]) && !string.IsNullOrEmpty(Request.Form["hfSLPassword"]))
						{
							DoLinkAndLogin(Request.Form["hfSLIdentity"], Request.Form["hfSLPassword"]);
						}
						else if (!string.IsNullOrEmpty(Request.Form["hfSLJoin"]) && StringUtils.FriendlyBool(Request.Form["hfSLJoin"], false))
						{
							DoJoin();
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error attempting native login: " + ex.Message, ex);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}
		#endregion

		#region event handlers
		void btnLogin_Click(object sender, EventArgs e)
		{
			const string methodName = "btnLogin_Click";

			try
			{
				// Handle case where identity has been remembered and user unchecks remember me checkbox then we delete the cookie
				if (HasUncheckedRememberMe() && HasRememberIdentityCookie())
				{
					HttpCookie cookie = new HttpCookie(_rememberIdentityCookieName);
					cookie.Expires = DateTime.Now.AddDays(-1);
					Response.Cookies.Add(cookie);
					_logger.Debug(_className, methodName, string.Format("Deleting identity cookie '{0}'", _rememberIdentityCookieName));
				}

				bool rememberMe = false;
				if (cbRememberMe != null && cbRememberMe.Visible && cbRememberMe.Checked) rememberMe = true;

				DoLogin(PortalState.Portal.AuthenticationField, tbUsername.Text, tbPassword.Text, rememberMe);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		void btnForgotPassword_Click(object sender, EventArgs e)
		{
			string url = _config.ForgotPasswordUrl;
			if (_provider != null) url = _provider.GetForgotPasswordURL(_config);
			if (!string.IsNullOrEmpty(url))
			{
				if (!string.IsNullOrEmpty(tbUsername.Text))
				{
					if (url.Contains("?")) url += "&";
					else url += "?";
					url += "u=" + tbUsername.Text;
				}
				Response.Clear();
				Response.Redirect(url);
			}
		}

		void _pnlChangePassword_ChangePasswordClicked(object sender, ChangePasswordPanel.ChangePasswordClickedArgs args)
		{
			try
			{
				switch (PortalState.Portal.PortalMode)
				{
					case PortalModes.CustomerFacing:
						LoyaltyService.ChangeMemberPassword(PortalState.Portal.AuthenticationField, tbUsername.Text, args.OldPassword, args.NewPassword, false);
						break;

					case PortalModes.CustomerService:
						CSService.ChangeCSAgentPassword(tbUsername.Text, args.OldPassword, args.NewPassword);
						break;
				}
				DoLogin(PortalState.Portal.AuthenticationField, tbUsername.Text, args.NewPassword, false);
			}
			catch (AuthenticationException ex)
			{
				ShowChangePasswordError(ex.Message);
			}
			catch (Exception ex)
			{
				ShowChangePasswordError(ex.Message, ex);
			}
		}
		#endregion

		#region private methods
		void DoLinkAndLogin(string identity, string password)
		{
			const string methodName = "DoLinkAndLogin";

			LoginStatusEnum loginStatus = LoginStatusEnum.Failure;
			try
			{
				_logger.Debug(_className, methodName, "identity=" + identity);
				if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
				{
					string socialName = Session["LinkSocialAccount"] as string;
					Session["LinkSocialAccount"] = null;
					string socialUID = "<unknown>";
					string properties = string.Empty;
					SocialNetworkProviderType providerType = SocialNetworkProviderType.Facebook;
					switch (socialName)
					{
						case "facebook":
							providerType = SocialNetworkProviderType.Facebook;
							FacebookProvider fbProvider = new FacebookProvider(_config.Facebook);
							FBUser fbuser = fbProvider.GetUser();
							socialUID = fbuser.id;
							properties = JsonConvert.SerializeObject(fbuser);
							break;

						case "twitter":
							providerType = SocialNetworkProviderType.Twitter;
							TwitterProvider twProvider = new TwitterProvider(_config.Twitter);
							TWUser twuser = twProvider.GetUser();
							properties = JsonConvert.SerializeObject(twProvider.AccessToken);
							socialUID = twuser.id_str;
							break;

						case "googleplus":
							providerType = SocialNetworkProviderType.Google;
							GooglePlusProvider gpProvider = new GooglePlusProvider(_config.GooglePlus);
							GPUser gpuser = gpProvider.GetUser();
							socialUID = gpuser.id;
							break;
					}

					Member member = null;
					try
					{
						member = LoyaltyService.LoginMember(PortalState.Portal.AuthenticationField, identity, password, null, ref loginStatus);
						if (loginStatus == LoginStatusEnum.Success)
						{
							LoyaltyService.CreateMemberSocNet(member.IpCode, providerType, socialUID, properties);
							_logger.Trace(_className, methodName, string.Format("Linked {0} account '{1}' with member {2} ({3})", socialName, socialUID, member.Username, member.IpCode));
						}
						else
						{
							if (member != null)
							{
								_logger.Trace(_className, methodName, string.Format("Unable to link {0} account '{1}' with member {2} ({3}), status={4}", socialName, socialUID, member.Username, member.IpCode, loginStatus.ToString()));
							}
							else
							{
								_logger.Trace(_className, methodName, string.Format("Unable to link {0} account '{1}' with member {2}, status={3}", socialName, socialUID, identity, loginStatus.ToString()));
							}
						}
					}
					catch (Exception ex)
					{
						if (member != null)
						{
							_logger.Trace(_className, methodName, string.Format("Unable to link {0} account '{1}' with member {2} ({3}) due to exception: {4}", socialName, socialUID, member.Username, member.IpCode, ex.Message));
						}
						else
						{
							_logger.Trace(_className, methodName, string.Format("Unable to link {0} account '{1}' with member due to exception: {2}", socialName, socialUID, ex.Message));
						}
					}
				}
				else
				{
					_logger.Error(_className, methodName, "Social login is not supported on customer service site");
				}

				switch (loginStatus)
				{
					case LoginStatusEnum.Success:
						bool rememberMe = false;
						DoLogin(PortalState.Portal.AuthenticationField, identity, password, rememberMe);
						break;

					case LoginStatusEnum.Failure:
						ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "InvalidUserPass.Text", "Invalid user name or password."));
						break;

					case LoginStatusEnum.LockedOut:
						ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "AccountLocked.Text", "That loyalty account is locked."));
						break;

					case LoginStatusEnum.PasswordResetRequired:
						ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "ChangePassBeforeLonking.Text", "Please change your loyalty account password before linking the account with your social account."));
						break;

                    case LoginStatusEnum.Disabled:
                        ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "AccountDisabled.Text", "That loyalty account is disabled."));
                        break;
                    default:
                        ShowLoginError(loginStatus.ToString());
                        break;
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		void DoJoin()
		{
			const string methodName = "DoJoin";

			try
			{
				string socialName = Session["LinkSocialAccount"] as string;
				Session["LinkSocialAccount"] = null;
				if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
				{
					string cookie = string.Empty;
					switch (socialName)
					{
						case "facebook":
							FacebookProvider fbProvider = new FacebookProvider(_config.Facebook);
							FBUser fbuser = fbProvider.GetUser();
							cookie = string.Format("First+Name={0}&Last+Name={1}",
								StringUtils.FriendlyString(fbuser.first_name),
								StringUtils.FriendlyString(fbuser.last_name));
							cookie = AddGender(cookie, fbuser.gender);
							if (_provider != null)
							{
								cookie = _provider.SetRegistrationDatahookCookie(cookie, socialName, fbuser);
							}
							Session["AfterRegistrationLinkSocialAccount"] = socialName;
							Session["AfterRegistrationLinkSocialUser"] = fbuser;
							break;

						case "twitter":
							TwitterProvider twProvider = new TwitterProvider(_config.Twitter);
							TWUser twuser = twProvider.GetUser();
							if (!string.IsNullOrWhiteSpace(twuser.name))
							{
								string[] names = twuser.name.Split(' ');
								string first_name = names.Length > 0 ? names[0] : string.Empty;
								string last_name = names.Length > 1 ? names[1] : string.Empty;
								cookie = string.Format("First+Name={0}&Last+Name={1}", first_name, last_name);
							}
							if (_provider != null)
							{
								cookie = _provider.SetRegistrationDatahookCookie(cookie, socialName, twuser);
							}
							Session["AfterRegistrationLinkSocialAccount"] = socialName;
							Session["AfterRegistrationLinkSocialUser"] = twuser;
							Session["AfterRegistrationLinkSocialUserProperties"] = twProvider.AccessToken;
							break;

						case "googleplus":
							GooglePlusProvider gpProvider = new GooglePlusProvider(_config.GooglePlus);
							GPUser gpuser = gpProvider.GetUser();
							if (gpuser.name != null)
							{
								cookie = string.Format("First+Name={0}&Last+Name={1}",
									StringUtils.FriendlyString(gpuser.name.givenName),
									StringUtils.FriendlyString(gpuser.name.familyName));
							}
							else if (!string.IsNullOrWhiteSpace(gpuser.displayName))
							{
								string[] names = gpuser.displayName.Split(' ');
								string first_name = names.Length > 0 ? names[0] : string.Empty;
								string last_name = names.Length > 1 ? names[1] : string.Empty;
								cookie = string.Format("First+Name={0}&Last+Name={1}",
									first_name, last_name);
							}
							cookie = AddGender(cookie, gpuser.gender);
							if (_provider != null)
							{
								cookie = _provider.SetRegistrationDatahookCookie(cookie, socialName, gpuser);
							}
							Session["AfterRegistrationLinkSocialAccount"] = socialName;
							Session["AfterRegistrationLinkSocialUser"] = gpuser;
							break;
					}
					if (!string.IsNullOrEmpty(cookie))
					{
						PortalState.SetCookie(CSDataHookConfig.DataHookCookieName, cookie, true);
					}
				}
				else
				{
					_logger.Error(_className, methodName, "Social login is not supported on customer service site");
				}
				Response.Redirect(_config.RegistrationUrl, false);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		private string AddGender(string cookie, string gender)
		{
			switch (gender.ToLower())
			{
				case "female":
					if (!string.IsNullOrWhiteSpace(cookie))
						cookie += "&";
					cookie += "Gender=Female";
					break;

				case "male":
					if (!string.IsNullOrWhiteSpace(cookie))
						cookie += "&";
					cookie += "Gender=Male";
					break;
			}
			return cookie;
		}

		private bool HasUncheckedRememberMe()
		{
			bool result = cbRememberMe != null && cbRememberMe.Visible && !cbRememberMe.Checked;
			return result;
		}

		private bool HasRememberIdentityCookie()
		{
			bool result = Request.Cookies[_rememberIdentityCookieName] != null;
			return result;
		}

		private bool IsNotAuthenticated()
		{
			const string methodName = "IsNotAuthenticated";
			if (Request.IsAuthenticated)
			{
				_pnlLogin.Visible = false;
				if (_provider != null)
				{
					string url = _provider.GetSuccessURL(_config);
					_logger.Debug(_className, methodName, string.Format("Redirecting user '{0}' to '{1}' since already authenticated.", Page.User.Identity.Name, url));
					Response.Clear();
					Response.Redirect(url, false);
				}
				else if (!string.IsNullOrEmpty(_config.SuccessUrl) && _config.SuccessUrl != PortalState.CurrentPage.RouteUrl)
				{
					// We're already logged in and success url is on a different page, so redirect
					_logger.Debug(_className, methodName, string.Format("Redirecting user '{0}' to '{1}' since already authenticated.", Page.User.Identity.Name, _config.SuccessUrl));
					Response.Clear();
					Response.Redirect(_config.SuccessUrl, false);
				}
				return false;
			}
			return true;
		}

		private bool IsSocialNetworkLogin()
		{
			bool result = false;
			string socialNetworkProvider = StringUtils.FriendlyString(_config.SocialNetworkProvider, "None");
			switch (socialNetworkProvider)
			{
				case "None":
					break;
				case "Native":
					if (_config.NativeFacebookEnabled && Request.QueryString["code"] != null)
					{
						result = true;
					}
                    else if (_config.NativeTwitterEnabled && Request.QueryString["oauth_token"] != null && Request.QueryString["oauth_verifier"] != null)
					{
						result = true;
					}
                    else if (_config.NativeGooglePlusEnabled && Request.QueryString["googlepluscode"] != null)
					{
						result = true;
					}
					break;
			}
			return result;
		}

		private bool DoSocialNetworkLogin()
		{
			bool result = false;
			string socialNetworkProvider = StringUtils.FriendlyString(_config.SocialNetworkProvider, "None");
			switch (socialNetworkProvider)
			{
				case "None":
					break;
				case "Native":
                    if (_config.NativeFacebookEnabled && Request.QueryString["code"] != null)
					{
						result = DoFacebookLogin();
					}
                    else if (_config.NativeTwitterEnabled && Request.QueryString["oauth_token"] != null && Request.QueryString["oauth_verifier"] != null)
					{
						result = DoTwitterLogin();
					}
                    else if (_config.NativeGooglePlusEnabled && Request.QueryString["googlepluscode"] != null)
					{
						result = DoGooglePlusLogin();
					}
					break;
			}
			return result;
		}

		private void LoadLinkSocialAccount()
		{
			const string methodName = "LoadLinkSocialAccount";
			string socialName = Session["LinkSocialAccount"] as string;
			string socUserName = string.Empty;
			string socProfileImageURL = string.Empty;
			switch (socialName)
			{
				case "facebook":
					try
					{
						FacebookProvider fbProvider = new FacebookProvider(_config.Facebook);
						FBUser fbuser = fbProvider.GetUser();
						if (fbuser != null)
						{
							socUserName = !string.IsNullOrEmpty(fbuser.first_name) ? fbuser.first_name : fbuser.name;
							socProfileImageURL = "https://graph.facebook.com/" + fbuser.id + "/picture";
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error getting facebook user: " + ex.Message, ex);
					}
					break;

				case "twitter":
					try
					{
						TwitterProvider twProvider = new TwitterProvider(_config.Twitter);
						TWUser twuser = twProvider.GetUser();
						if (twuser != null)
						{
							socUserName = twuser.screen_name;
							socProfileImageURL = twuser.profile_image_url;
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error getting twitter user: " + ex.Message, ex);
					}
					break;

				case "googleplus":
					try
					{
						GooglePlusProvider gpProvider = new GooglePlusProvider(_config.GooglePlus);
						GPUser gpuser = gpProvider.GetUser();
						if (gpuser != null)
						{
							socUserName = gpuser.displayName;
							socProfileImageURL = gpuser.image.url;
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Error getting googleplus user: " + ex.Message, ex);
					}
					break;
			}
			if (!string.IsNullOrEmpty(socUserName))
			{
				RegisterResponseScript(string.Format("ShowLinkSocialAccountDialog('{0}', '{1}', '{2}');", socUserName, socProfileImageURL, PortalState.Portal.AuthenticationField.ToString()));
			}
		}

		private bool DoFacebookLogin()
		{
			const string methodName = "DoFacebookLogin";

			_logger.Debug(_className, methodName, "Attempting facebook login.");
			bool result = false;
			FacebookProvider fbProvider = new FacebookProvider(_config.Facebook);
            string authToken = fbProvider.GetAccessToken(Request.QueryString["code"]);
			if (authToken.Length > 0)
			{
				FBUser fbuser = fbProvider.GetUser();
				if (fbuser != null)
				{
					bool found = false;
					string userName = string.Empty;
					Member member = LoyaltyService.LoadMemberFromSocNet(SocialNetworkProviderType.Facebook, fbuser.id);
					if (member != null)
					{
						// member is linked to this social network, so update socnet record so we have the latest cookie
						MemberSocNet socNet = LoyaltyService.GetSocNetForMember(SocialNetworkProviderType.Facebook, member.IpCode, fbuser.id);
						socNet.Properties = JsonConvert.SerializeObject(fbuser);
						socNet.ProviderUID = fbuser.id;
						LoyaltyService.UpdateMemberSocNet(socNet);
						// member is linked to this social network, so login to site
						_logger.Debug(_className, methodName, string.Format("Found member '{0}' ({1}) linked with facebook user id '{2}'.", member.Username, member.IpCode, fbuser.id));
						userName = member.Username;
						found = true;
						bool createPersistentCookie = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWLoginPersistentCookie"), false);
						SecurityManager.LoginMember(member, createPersistentCookie);

						string url = _config.SuccessUrl;
						if (_provider != null)
						{
							url = _provider.GetSuccessURL(_config);
						}

						// redirect browser
						_logger.Debug(_className, methodName, "Redirecting to " + url);
						Response.Clear();
						Response.Redirect(url, false);
						result = true;
					}
					else
					{
						_logger.Debug(_className, methodName, string.Format("Could not find any member linked with facebook user id '{0}'.", fbuser.id));
					}

					if (!found)
					{
						Session["LinkSocialAccount"] = "facebook";
						LoadLinkSocialAccount();
					}
				}
				else
				{
					_logger.Warning(_className, methodName, "Provider did not return a user.");
				}
			}
			else
			{
				_logger.Warning(_className, methodName, "Provider did not return an access token.");
			}
			return result;
		}

		private bool DoTwitterLogin()
		{
			const string methodName = "DoTwitterLogin";

			_logger.Debug(_className, methodName, "Attempting twitter login.");
			bool result = false;
			TwitterProvider twProvider = new TwitterProvider(_config.Twitter);
            string authToken = twProvider.GetAccessToken(Request.QueryString["oauth_token"], Request.QueryString["oauth_verifier"]);
			if (authToken.Length > 0)
			{
				TWUser twuser = twProvider.GetUser();
				if (twuser != null)
				{
					bool found = false;
					string userName = string.Empty;
					Member member = LoyaltyService.LoadMemberFromSocNet(SocialNetworkProviderType.Twitter, twuser.id_str);
					if (member != null)
					{
						// member is linked to this social network, so update socnet record so we have the latest cookie
						MemberSocNet socNet = LoyaltyService.GetSocNetForMember(SocialNetworkProviderType.Twitter, member.IpCode, twuser.id_str);
						socNet.Properties = JsonConvert.SerializeObject(twProvider.AccessToken);
						socNet.ProviderUID = twuser.id_str;
						LoyaltyService.UpdateMemberSocNet(socNet);
						// member is linked to this social network, so login to site
						_logger.Debug(_className, methodName, string.Format("Found member '{0}' ({1}) linked with twitter user id '{2}'.", member.Username, member.IpCode, twuser.id_str));
						userName = member.Username;
						found = true;
						bool createPersistentCookie = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWLoginPersistentCookie"), false);
						SecurityManager.LoginMember(member, createPersistentCookie);

						string url = _config.SuccessUrl;
						if (_provider != null)
						{
							url = _provider.GetSuccessURL(_config);
						}

						// redirect browser
						_logger.Debug(_className, methodName, "Redirecting to " + url);
						Response.Clear();
						Response.Redirect(url, false);
						result = true;
					}
					else
					{
						_logger.Debug(_className, methodName, string.Format("Could not find any member linked with twitter user id '{0}'.", twuser.id_str));
					}

					if (!found)
					{
						Session["LinkSocialAccount"] = "twitter";
						LoadLinkSocialAccount();
					}
				}
				else
				{
					_logger.Warning(_className, methodName, "Provider did not return a user.");
				}
			}
			else
			{
				_logger.Warning(_className, methodName, "Provider did not return an access token.");
			}
			return result;
		}

		private bool DoGooglePlusLogin()
		{
			const string methodName = "DoGooglePlusLogin";

			_logger.Debug(_className, methodName, "Attempting googleplus login.");
			bool result = false;
			GooglePlusProvider gpProvider = new GooglePlusProvider(_config.GooglePlus);
            string authToken = gpProvider.GetAccessToken(Request.QueryString["googlepluscode"]);
			if (authToken != null)
			{
				GPUser gpuser = gpProvider.GetUser();
				if (gpuser != null)
				{
					bool found = false;
					string userName = string.Empty;
					Member member = LoyaltyService.LoadMemberFromSocNet(SocialNetworkProviderType.Google, gpuser.id);
					if (member != null)
					{
						// member is linked to this social network, so update socnet record so we have the latest cookie
						MemberSocNet socNet = LoyaltyService.GetSocNetForMember(SocialNetworkProviderType.Google, member.IpCode, gpuser.id);
						socNet.ProviderUID = gpuser.id;
						LoyaltyService.UpdateMemberSocNet(socNet);
						// member is linked to this social network, so login to site
						_logger.Debug(_className, methodName, string.Format("Found member '{0}' ({1}) linked with googleplus user id '{2}'.", member.Username, member.IpCode, gpuser.id));
						userName = member.Username;
						found = true;
						bool createPersistentCookie = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWLoginPersistentCookie"), false);
						SecurityManager.LoginMember(member, createPersistentCookie);

						string url = _config.SuccessUrl;
						if (_provider != null)
							url = _provider.GetSuccessURL(_config);

						// redirect browser
						_logger.Debug(_className, methodName, "Redirecting to " + url);
						Response.Clear();
						Response.Redirect(url, false);
						result = true;
					}
					else
					{
						_logger.Debug(_className, methodName, string.Format("Could not find any member linked with googleplus user id '{0}'.", gpuser.id));
					}

					if (!found)
					{
						Session["LinkSocialAccount"] = "googleplus";
						LoadLinkSocialAccount();
					}
				}
				else
				{
					_logger.Warning(_className, methodName, "Provider did not return a user.");
				}
			}
			else
			{
				_logger.Warning(_className, methodName, "Provider did not return an access token.");
			}
			return result;
		}

		private string ExtractMTouch()
		{
			if (!string.IsNullOrEmpty(Request.QueryString["MTouch"]))
			{
				return Request.QueryString["MTouch"];
			}

			string returnUrl = Request.QueryString["ReturnUrl"];
			if (string.IsNullOrEmpty(returnUrl))
			{
				returnUrl = Request.Form["ReturnUrl"];
			}
			if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("?"))
			{
				if (!returnUrl.Contains("/") && returnUrl.Contains("%"))
				{
					returnUrl = HttpUtility.UrlDecode(returnUrl);
				}

				if (returnUrl.ToLower().Contains("mtouch="))
				{
					NameValueCollection query = HttpUtility.ParseQueryString(returnUrl.Substring(returnUrl.IndexOf("?") + 1).ToLower());
					if (!string.IsNullOrEmpty(query["mtouch"]))
					{
						return query["mtouch"];
					}
				}
			}

			return null;
		}

		private bool IsMTouchLogin()
		{
			return PortalState.Portal.PortalMode == PortalModes.CustomerFacing && !string.IsNullOrEmpty(ExtractMTouch());
		}

		private bool DoMTouchLogin()
		{
			string mTouch = ExtractMTouch();
			if (string.IsNullOrEmpty(mTouch))
			{
				return false;
			}

			Member member = null;

			var mt = LoyaltyService.GetMTouch(mTouch);
			if (mt != null && !string.IsNullOrEmpty(mt.EntityId))
			{
				long ipCode = -1;
				if (long.TryParse(mt.EntityId, out ipCode))
				{
					member = LoyaltyService.LoadMemberFromIPCode(ipCode);
				}
				else
				{
					member = LoyaltyService.LoadMemberFromEmailAddress(mt.EntityId);
				}
			}

			if (member != null)
			{
				bool createPersistentCookie = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWLoginPersistentCookie"), false);
				SecurityManager.LoginMember(member, createPersistentCookie);

				string url = Request.QueryString["ReturnUrl"];
				if (string.IsNullOrEmpty(url))
				{
					url = _config.SuccessUrl;
				}

				if (_provider != null) url = _provider.GetSuccessURL(_config);

				// redirect browser
				Response.Clear();
				Response.Redirect(url, false);
				return true;
			}
			return false;
		}

		private bool ValidateFields(AuthenticationFields identityType, string identity, string password)
		{
			//List<string> errors = new List<string>();
			bool valid = true;

			if (string.IsNullOrEmpty(identity))
			{
				base.AddInvalidField(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "PleaseEnter.Text", "Please enter your {0}."), lblUsername.InnerText.Replace(":", string.Empty)), tbUsername);
				valid = false;
			}
			switch (identityType)
			{
				case AuthenticationFields.Username:
					int characterlimit = PortalState.Portal.PortalMode == PortalModes.CustomerService ? 100 : 254;
					if (identity.Length > characterlimit)
					{
						AddInvalidField(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "VariableMaxLength.Text", "Maximum {0} characters: {1}"), characterlimit, lblUsername.InnerText.Replace(":", string.Empty)), tbUsername);
						valid = false;
					}
					break;
				case AuthenticationFields.PrimaryEmailAddress:
					if (identity.Length > 254)
					{
						AddInvalidField(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "VariableMaxLength.Text", "Maximum {0} characters: {1}"), 254, lblUsername.InnerText.Replace(":", string.Empty)), tbUsername);
						valid = false;
					}
					break;
				case AuthenticationFields.LoyaltyIdNumber:
				case AuthenticationFields.AlternateId:
					if (identity.Length > 255)
					{
						AddInvalidField(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "VariableMaxLength.Text", "Maximum {0} characters: {1}"), 255, lblUsername.InnerText.Replace(":", string.Empty)), tbUsername);
						valid = false;
					}
					break;
			}
			if (string.IsNullOrEmpty(password))
			{
				base.AddInvalidField(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "PleaseEnter.Text", "Please enter your {0}."), lblPassword.InnerText.Replace(":", string.Empty)), tbPassword);
				valid = false;
			}
			if (password.Length > 50)
			{
				AddInvalidField(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "VariableMaxLength.Text", "Maximum {0} characters: {1}"), 50, lblPassword.InnerText.Replace(":", string.Empty)), tbPassword);
				valid = false;
			}
			return valid;
		}

		private void DoLogin(AuthenticationFields identityType, string identity, string password, bool rememberIdentity)
		{
			const string methodName = "DoLogin";

			// field validation
			if (!ValidateFields(identityType, identity, password)) return;

			if (rememberIdentity)
			{
				int expireDays = StringUtils.FriendlyInt32(LWConfigurationUtil.GetConfigurationValue("LWLoginRememberIdentityMaxDays"), 15);
				if (expireDays < 1) expireDays = 15;
				HttpCookie cookie = new HttpCookie(_rememberIdentityCookieName);
				cookie.Values.Add("identity", identity);
				cookie.Expires = DateTime.Now.AddDays(expireDays);
				Response.Cookies.Add(cookie);
				_logger.Debug(_className, methodName, string.Format("Wrote identity cookie '{0}' for identity '{1}' expiring in {2} days", _rememberIdentityCookieName, identity, expireDays));
			}

			LoginStatusEnum loginStatus = LoginStatusEnum.Failure;
			string userForSecurityCookie = null;
			try
			{
				switch (PortalState.Portal.PortalMode)
				{
					case PortalModes.CustomerFacing:
						Member member = LoyaltyService.LoginMember(identityType, identity, password, null, ref loginStatus);
						if (member != null)
						{
							userForSecurityCookie = member.IpCode.ToString();
							PortalState.CurrentMember = member;
						}
						break;

					case PortalModes.CustomerService:
						CSAgent csagent = CSService.LoginCSAgent(identity, password, null, ref loginStatus);
						if (csagent != null)
						{
							userForSecurityCookie = csagent.Username;
						}
						break;
				}

				switch (loginStatus)
				{
					case LoginStatusEnum.Success:
						bool rememberMe = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWLoginPersistentCookie"), false);
						SecurityManager.LoginUser(userForSecurityCookie, rememberMe);

						string url = _config.SuccessUrl;
						if (_provider != null) url = _provider.GetSuccessURL(_config);

						// redirect browser
						Response.Clear();
						Response.Redirect(url, false);
						break;

					case LoginStatusEnum.PasswordResetRequired:
						_pnlLogin.Visible = false;
						_pnlChangePassword.Visible = true;
						break;

					case LoginStatusEnum.Failure:
						ShowLoginError(ResourceUtils.GetLocalWebResource(_modulePath, "IncorrectPassword.Text", "Provided password is incorrect."));
						break;

                    case LoginStatusEnum.LockedOut:
                        ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "AccountLocked.Text", "That loyalty account is locked."));
                        break;

                    case LoginStatusEnum.Disabled:
                        ShowNegative(ResourceUtils.GetLocalWebResource(_modulePath, "AccountDisabled.Text", "That loyalty account is disabled."));
                        break;

                    default:
						ShowLoginError(loginStatus.ToString());
						break;
				}
			}
			catch (AuthenticationException ex)
			{
				ShowLoginError(ex.Message);
			}
		}

		private void ShowLoginError(string errorMessage, Exception exception = null)
		{
			const string methodName = "ShowLoginError";

			string msg = string.Format("Login failure: {0}", errorMessage);
			if (exception == null)
			{
				_logger.Error(_className, methodName, msg);
			}
			else
			{
				_logger.Error(_className, methodName, msg, exception);
			}

			//if (lblError != null)
			//{
			//    //user has placed an error label token in the module. Display login failure in the label
			//    lblError.Text = StringUtils.FriendlyString(errorMessage).Replace(Environment.NewLine, "<br/>");
			//    lblError.Visible = true;
			//    pchError.Visible = true;
			//}
			//else
			//{
			//    // Error url
			//    if (_provider != null || !string.IsNullOrEmpty(_config.ErrorUrl))
			//    {
			//        string errParamName = (_config.ShowPageError == 0 ? "errorMessage" : "error");
			//        string errParamValue = Server.UrlPathEncode(errorMessage).Replace("=", "%3d");
			//        string[] errParams = new string[] { errParamName + "=" + errParamValue };
			//        string errorUrl = _config.ErrorUrl + "?" + string.Join("&", errParams);
			//        if (_provider != null) errorUrl = _provider.GetErrorURL(_config, errParams);
			//        Response.Redirect(errorUrl, true);
			//    }
			//    else if (_config.ShowPageError != 0)
			//    {
			//        ShowNegative(errorMessage);
			//    }
			//}


			if (_provider != null || !string.IsNullOrEmpty(_config.ErrorUrl))
			{
				string errParamName = (_config.ShowPageError == 0 ? "errorMessage" : "error");
				string errParamValue = Server.UrlPathEncode(errorMessage).Replace("=", "%3d");
				string[] errParams = new string[] { errParamName + "=" + errParamValue };
				string errorUrl = _config.ErrorUrl + "?" + string.Join("&", errParams);
				if (_provider != null) errorUrl = _provider.GetErrorURL(_config, errParams);
				Response.Redirect(errorUrl, true);
			}
			else if (_config.ShowPageError != 0)
			{
				ShowNegative(errorMessage);
			}
		}

		private void ShowChangePasswordError(string errorMessage, Exception exception = null)
		{
			const string methodName = "ShowChangePasswordError";

			string msg = string.Format("Password change failure: {0}", errorMessage);
			if (exception == null)
			{
				_logger.Error(_className, methodName, msg);
			}
			else
			{
				_logger.Error(_className, methodName, msg, exception);
			}

			// Error url
			if (_provider != null || !string.IsNullOrEmpty(_config.ErrorUrl))
			{
				string errParamName = (_config.ShowPageError == 0 ? "errorMessage" : "error");
				string errParamValue = Server.UrlPathEncode(errorMessage).Replace("=", "%3d");
				string[] errParams = new string[] { errParamName + "=" + errParamValue };
				string errorUrl = _config.ErrorUrl + "?" + string.Join("&", errParams);
				if (_provider != null) errorUrl = _provider.GetErrorURL(_config, errParams);
				Response.Redirect(errorUrl, true);
			}
			else if (_config.ShowPageError != 0)
			{
				ShowNegative(errorMessage);
			}
		}

		private void LoadDynamicContent(Control placeHolder, string template)
		{
			string methodName = "LoadDynamicContent";

			bool hasRememberIdentityCookie = HasRememberIdentityCookie();
			string rememberIdentityCookieValue = string.Empty;
			if (hasRememberIdentityCookie)
			{
				rememberIdentityCookieValue = Request.Cookies[_rememberIdentityCookieName]["identity"];
				_logger.Debug(_className, methodName, string.Format("Found identity cookie '{0}' with identity '{1}'", _rememberIdentityCookieName, rememberIdentityCookieValue));
			}

			int contentBeginIndex = 0;
			while (contentBeginIndex < template.Length)
			{
				int tokenBeginIndex = template.IndexOf(delimiter, contentBeginIndex);
				if (tokenBeginIndex != -1)
				{
					int tokenEndIndex = template.IndexOf(delimiter, tokenBeginIndex + delimiter.Length);
					if (tokenEndIndex == -1)
					{
						_logger.Error(_className, methodName, "End delimiter missing from template token.");
						throw new FormatException("End delimiter missing from template token.");
					}

					string content = template.Substring(contentBeginIndex, tokenBeginIndex - contentBeginIndex);
					LiteralControl lc = new LiteralControl(content);
					placeHolder.Controls.Add(lc);

					string tokenName = template.Substring(tokenBeginIndex + 2, tokenEndIndex - tokenBeginIndex - 2);
					Hashtable tokenParams = new Hashtable();
					int index = tokenName.IndexOf("(");
					if (index != -1)
					{
						tokenParams = ExtractTokenParams(tokenName.Substring(index));
						tokenName = tokenName.Substring(0, index);
					}
					switch (tokenName)
					{
						case "LBL_USERNAME":
							lblUsername = new HtmlGenericControl("label");
							lblUsername.ID = "lblUsername";
							lblUsername.Attributes.Add("class", StringUtils.FriendlyString(tokenParams["cssclass"], "SubHead"));
							switch (PortalState.Portal.AuthenticationField)
							{
								default:
								case AuthenticationFields.Username:
									lblUsername.InnerText = GetLocalResourceObject(_config.GetResourceKeyName("Username")).ToString();
									break;
								case AuthenticationFields.LoyaltyIdNumber:
									lblUsername.InnerText = GetLocalResourceObject(_config.GetResourceKeyName("LoyaltyID")).ToString();
									break;
								case AuthenticationFields.AlternateId:
									lblUsername.InnerText = GetLocalResourceObject(_config.GetResourceKeyName("AlternateID")).ToString();
									break;
								case AuthenticationFields.PrimaryEmailAddress:
									lblUsername.InnerText = GetLocalResourceObject(_config.GetResourceKeyName("EmailAddress")).ToString();
									break;
							}
							placeHolder.Controls.Add(lblUsername);
							break;

						case "TB_USERNAME":
							tbUsername = new TextBox();
							tbUsername.ID = "tbUsername";
							if (!string.IsNullOrEmpty(Request.QueryString["username"]))
							{
								tbUsername.Text = Request.QueryString["username"];
							}
							else if (hasRememberIdentityCookie)
							{
								tbUsername.Text = rememberIdentityCookieValue;
							}
							tbUsername.CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "NormalTextBox");
							placeHolder.Controls.Add(tbUsername);
							if (tokenParams.Contains("autofocus"))
							{
								bool autoFocus = false;
								bool.TryParse(tokenParams["autofocus"].ToString(), out autoFocus);
								if (autoFocus)
								{
									Page.Form.DefaultFocus = tbUsername.ClientID;
								}
							}
							break;

						case "LBL_PASSWORD":
							lblPassword = new HtmlGenericControl("label");
							lblPassword.ID = "lblPassword";
							lblPassword.Attributes.Add("class", StringUtils.FriendlyString(tokenParams["cssclass"], "SubHead"));
							lblPassword.InnerText = GetLocalResourceObject(_config.GetResourceKeyName("Password")).ToString();
							placeHolder.Controls.Add(lblPassword);
							break;

						case "TB_PASSWORD":
							tbPassword = new TextBox();
							tbPassword.ID = "tbPassword";
							tbPassword.CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "NormalTextBox");
							tbPassword.TextMode = TextBoxMode.Password;
							tbPassword.Attributes.Add("value", tbPassword.Text);
							placeHolder.Controls.Add(tbPassword);
							break;

						case "BTN_LOGIN":
							switch (PortalState.Portal.ButtonStyle)
							{
								case PortalButtonStyle.Button:
									btnLogin = new Button() { ID = "btnLogin", CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "StandardButton"), EnableViewState = false };
									break;
								case PortalButtonStyle.LinkButton:
									btnLogin = new LinkButton() { ID = "btnLogin", CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "StandardButton"), EnableViewState = false };
									break;
							}
							btnLogin.Text = GetLocalResourceObject(_config.GetResourceKeyName("LoginButton")).ToString();
							btnLogin.ValidationGroup = ValidationGroup;
							btnLogin.Click += new EventHandler(btnLogin_Click);
							placeHolder.Controls.Add((WebControl)btnLogin);
							//ClientAPI.RegisterKeyCapture(Parent, btnLogin, 13);
							break;

						case "BTN_FORGOTPASSWORD":
							switch (PortalState.Portal.ButtonStyle)
							{
								case PortalButtonStyle.Button:
									btnForgotPassword = new Button() { ID = "btnForgotPassword", CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "StandardButton"), EnableViewState = false };
									break;
								case PortalButtonStyle.LinkButton:
									btnForgotPassword = new LinkButton() { ID = "btnForgotPassword", CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "StandardButton"), EnableViewState = false };
									break;
							}
							btnForgotPassword.Text = GetLocalResourceObject(_config.GetResourceKeyName("ForgotPassword")).ToString();
							btnForgotPassword.ValidationGroup = ValidationGroup;
							btnForgotPassword.Click += new EventHandler(btnForgotPassword_Click);
							placeHolder.Controls.Add((WebControl)btnForgotPassword);
							break;

						case "CB_REMEMBERME":
							cbRememberMe = new Mobile.CheckBox() { ID = "cbRememberMe", Checked = hasRememberIdentityCookie };
							cbRememberMe.Text = GetLocalResourceObject(_config.GetResourceKeyName("RememberMe")).ToString();
							cbRememberMe.CssClass = StringUtils.FriendlyString(tokenParams["cssclass"], "Normal");
							placeHolder.Controls.Add(cbRememberMe);
							break;

						case "CTL_SOCIALNETWORK":
							string socialNetworkProvider = "None";
							try
							{
								socialNetworkProvider = StringUtils.FriendlyString(_config.SocialNetworkProvider, "None");
								switch (socialNetworkProvider)
								{
									case "None":
										break;
									case "Native":
										{
											placeHolder.Controls.Add(new LiteralControl("<div id=\"native-social-login\">"));
											if (_config.NativeFacebookEnabled)
											{
												FacebookLoginButton facebookLoginButton = new FacebookLoginButton(_config.Facebook);
												placeHolder.Controls.Add(facebookLoginButton);
											}
											if (_config.NativeTwitterEnabled)
											{
												TwitterLoginButton twitterLoginButton = new TwitterLoginButton(_config.Twitter);
												placeHolder.Controls.Add(twitterLoginButton);
											}
											if (_config.NativeGooglePlusEnabled)
											{
												GooglePlusLoginButton googlePlusLoginButton = new GooglePlusLoginButton(_config.GooglePlus);
												placeHolder.Controls.Add(googlePlusLoginButton);
											}
											placeHolder.Controls.Add(new LiteralControl("</div>"));
										}
										break;
								}
							}
							catch (Exception ex)
							{
								string msg = string.Format("Unexpected exception creating social network provider '{0}': {1}", socialNetworkProvider, ex.Message);
								_logger.Error(_className, methodName, msg, ex);
							}
							break;

						case "LBL_ERROR":
							// old error label, so ignore
							break;

						default:
							_logger.Error(_className, methodName, "Invalid template token: " + tokenName);
							throw new FormatException("Invalid template token: " + tokenName);
					}

					contentBeginIndex = tokenEndIndex + 2;
				}
				else
				{
					string content = template.Substring(contentBeginIndex);
					LiteralControl lc = new LiteralControl(content);
					placeHolder.Controls.Add(lc);
					break;
				}
			}
		}

		private Hashtable ExtractTokenParams(string tokenParamsString)
		{
			Hashtable result = new Hashtable();
			tokenParamsString = tokenParamsString.Substring(1, tokenParamsString.Length - 2);
			string[] items = tokenParamsString.Split(new char[] { ',' });
			if (items != null && items.Length > 0)
			{
				foreach (string item in items)
				{
					if (item.IndexOf("=") > 0)
					{
						string itemName = item.Substring(0, item.IndexOf("="));
						string itemValue = item.Substring(item.IndexOf("=") + 1);
						result.Add(itemName, itemValue);
					}
				}
			}
			return result;
		}

        private void LoadResourceStrings()
        {
            pnlLinkSocialAccount.Attributes.Add("Title", ResourceUtils.GetLocalWebResource(_modulePath, "LinkSocialAccountTitle.Text", "Link Your Social Account to Your Loyalty Account"));
            litBtnYes.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnYes.Text", "Yes");
            litBtnNo.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnNo.Text", "No");
            litLinkSocialHeader.Text = ResourceUtils.GetLocalWebResource(_modulePath, "LinkSocialHeader.Text", "Accessing Client.com");
            litSocialWelcome.Text = ResourceUtils.GetLocalWebResource(_modulePath, "SocialWelcome.Text", "Welcome");
            litSocialQuestion.Text = ResourceUtils.GetLocalWebResource(_modulePath, "SocialQuestion.Text", "Do you have an existing loyalty account?");
            litLinkMemberHeader.Text = ResourceUtils.GetLocalWebResource(_modulePath, "LinkMemberHeader.Text", "Great! Thanks for being a loyalty member.");
            litLinkMemberContent.Text = ResourceUtils.GetLocalWebResource(_modulePath, "LinkMemberContent.Text", "For quick secure access, please link your loyalty account.");
            litLinkLogin.Text = ResourceUtils.GetLocalWebResource(_modulePath, "LinkLogin.Text", "Link Accounts and Login");
            litPassword_SL.Text = ResourceUtils.GetLocalWebResource(_modulePath, "Password_SL.Text", "Password:");
            litRegSocialHeader.Text = ResourceUtils.GetLocalWebResource(_modulePath, "RegSocialHeader.Text", "Not a Loyalty Member? Let's get you registered.");
            litRegSocialContent.Text = ResourceUtils.GetLocalWebResource(_modulePath, "RegSocialContent.Text", "Click <em>Join</em> to register as a loyalty member.  Once you become a loyalty member, you can start accruing points immediately and be on your way to earning the points necessary for added rewards.");
            litbtnJoin.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnJoin.Text", "Join");
        }
		#endregion
	}
}
