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
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Security;

namespace Brierley.LWModules.ChangePassword
{
	public partial class ViewChangePassword : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewChangePassword";
		private const string _modulePath = "~/Controls/Modules/ChangePassword/ViewChangePassword.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private ChangePasswordConfig _config;
		#endregion

		#region page life cycle
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			InitializeConfig();

			string validationGroup = "ChangePassword" + ConfigurationKey.ConfigName;
			rfOldPassword.ValidationGroup = validationGroup;
			rfNewPassword.ValidationGroup = validationGroup;
			rfConfirmPassword.ValidationGroup = validationGroup;
			cmpConfirmPassword.ValidationGroup = validationGroup;
			btnSave.ValidationGroup = validationGroup;

			litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);
			btnSave.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.SaveButtonResourceKey);
			btnCancel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CancelButtonResourceKey);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			phSuccessMessage.Visible = false;
			btnCancel.Visible = _config.EnableCancelButton;

			btnSave.Click += new EventHandler(btnSave_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
		}
		#endregion

		#region event handlers
		void btnSave_Click(object sender, EventArgs e)
		{
			const string methodName = "btnSave_Click";

			if (PortalState.Portal == null) return;

			try
			{
				switch (PortalState.Portal.PortalMode)
				{
					case PortalModes.CustomerService:
						SaveCSAgentPassword();
						break;

					case PortalModes.CustomerFacing:
						SaveMemberPassword();
						break;
				}
			}
			catch (LWException ex)
			{
				_logger.Error(_className, methodName, "LWException: " + ex.Message, ex);
				//ShowAlert(ex.Message, ex);
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				//ShowAlert(GetLocalResourceObject("ApplicationException.Text").ToString(), ex);
				throw;
			}
		}

		void btnCancel_Click(object sender, EventArgs e)
		{
			litSuccessMessage.Text = string.Empty;
			phSuccessMessage.Visible = false;

			tbOldPassword.Text = string.Empty;
			tbNewPassword.Text = string.Empty;
			tbConfirmPassword.Text = string.Empty;

			if (!string.IsNullOrEmpty(_config.CancelUrl))
			{
				Response.Redirect(_config.CancelUrl);
			}
		}
		#endregion

		#region private methods
		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<ChangePasswordConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new ChangePasswordConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}

		private void SaveCSAgentPassword()
		{
			const string methodName = "SaveCSAgentPassword";

			CSAgent csagent = PortalState.GetLoggedInCSAgent();
			if (csagent == null)
				throw new LWException("PortalState.GetLoggedInCSAgent() returns null");

			try
			{
				// validate old password
				if (LWPasswordUtil.IsHashingEnabled())
				{
					if (!string.IsNullOrEmpty(csagent.Salt))
					{
						string hash = LWPasswordUtil.HashPassword(csagent.Salt, tbOldPassword.Text);
						if (!hash.Equals(csagent.Password))
							throw new AuthenticationException("Invalid 'old password'");
					}
				}
				else
				{
					if (!tbOldPassword.Text.Equals(csagent.Password))
						throw new AuthenticationException("Invalid 'old password'");
				}

				CSService.ChangeCSAgentPassword(csagent.Username, tbOldPassword.Text, tbNewPassword.Text);

				SendChangedPasswordEmail(null, csagent);

				if (!string.IsNullOrEmpty(_config.SuccessUrl))
				{
					Response.Redirect(_config.SuccessUrl);
				}
				else
				{
					//litSuccessMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.SuccessMessageResourceKey);
					//phSuccessMessage.Visible = true;
					ShowPositive(ResourceUtils.GetLocalWebResource(_modulePath, _config.SuccessMessageResourceKey));
				}
			}
			catch (AuthenticationException ex)
			{
				_logger.Error(_className, methodName, "Error saving csagent password: " + ex.Message, ex);
				if (_config.UsePasswordValidationMessages)
				{
					//ShowAlert(ex.Message, ex);
					AddInvalidField(ex.Message, null);
				}
				else
				{
					AddInvalidField(GetLocalResourceObject("ApplicationException.Text").ToString(), null);
					//ShowAlert(GetLocalResourceObject("ApplicationException.Text").ToString(), ex);
				}
			}
		}

		private void SendChangedPasswordEmail(Member member, CSAgent csagent)
		{
			string methodName = "SendChangedPasswordEmail";

			string passwordChangedEmailName = _config.PasswordChangedEmailName;
			if (!string.IsNullOrEmpty(passwordChangedEmailName))
			{
				try
				{
					// send password changed email
					EmailDocument emailDoc = EmailService.GetEmail(passwordChangedEmailName);
					if (emailDoc != null)
					{
						ITriggeredEmail email = TriggeredEmailFactory.Create(emailDoc.Id);
						string emailAddress = string.Empty;
						if (email != null)
						{
							long emailSerialNum = -1;
							if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
							{
								emailAddress = member.PrimaryEmailAddress;
								email.SendAsync(member).Wait();
							}
							else
							{
								emailAddress = csagent.EmailAddress;
								email.SendAsync(csagent.EmailAddress, null).Wait();
							}
							_logger.Debug(_className, methodName, "Password change email was sent to: " + emailAddress);
						}
						else
						{
							_logger.Error(_className, methodName, "Unable to resolve email for ID: " + emailDoc.Id);
						}
					}
					else
					{
						_logger.Error(_className, methodName, "Unable to resolve emailDoc for name: " + passwordChangedEmailName);
					}
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Error sending password changed email:" + ex.Message, ex);
				}
			}
			else
			{
				_logger.Debug(_className, methodName, "No password change email is configured.");
			}
		}

		private void SaveMemberPassword()
		{
			const string methodName = "SaveMemberPassword";

			Member member = PortalState.GetLoggedInMember();
			if (member == null)
				throw new LWException("PortalState.GetLoggedInMember() returns null");

			try
			{
				// validate old password
				if (LWPasswordUtil.IsHashingEnabled())
				{
					if (!string.IsNullOrEmpty(member.Salt))
					{
						string hash = LWPasswordUtil.HashPassword(member.Salt, tbOldPassword.Text);
						if (!hash.Equals(member.Password))
							throw new AuthenticationException("Invalid 'old password'");
					}
				}
				else
				{
					if (!tbOldPassword.Text.Equals(member.Password))
						throw new AuthenticationException("Invalid 'old password'");
				}

				LoyaltyService.ChangeMemberPassword(member, tbNewPassword.Text);

				SendChangedPasswordEmail(member, null);

				if (!string.IsNullOrEmpty(_config.SuccessUrl))
				{
					Response.Redirect(_config.SuccessUrl);
				}
				else
				{
					litSuccessMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.SuccessMessageResourceKey);
					phSuccessMessage.Visible = true;
				}
			}
			catch (AuthenticationException ex)
			{
				_logger.Error(_className, methodName, "Error saving member password: " + ex.Message, ex);
				if (_config.UsePasswordValidationMessages)
				{
					//ShowAlert(ex.Message, ex);
					AddInvalidField(ex.Message, null);
				}
				else
				{
					//ShowAlert(GetLocalResourceObject("ApplicationException.Text").ToString(), ex);
					AddInvalidField(GetLocalResourceObject("ApplicationException.Text").ToString(), null);
				}
			}
		}
		#endregion
	}
}
