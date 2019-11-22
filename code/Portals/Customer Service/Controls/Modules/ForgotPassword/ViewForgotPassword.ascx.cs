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
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.FrameWork.Sms;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using System.Threading;
using System.Globalization;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using System.Text;

namespace Brierley.LWModules.ForgotPassword
{
	public partial class ViewForgotPassword : ModuleControlBase
	{
		private const string _className = "ViewForgotPassword";
		private const string _modulePath = "~/Controls/Modules/ForgotPassword/ViewForgotPassword.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private string _validationGroupName;
		private ForgotPasswordConfig _config;
		private IButtonControl _btnFPButton;
        private IButtonControl _btnROButton;
        private IButtonControl _btnRCButton;
		private IButtonControl _btnCPButton;
		private IButtonControl _btnPCButton;

        private const string _connectionIssueMessage = "ConnectionIssueMessage";
        private const string _invalidResetCodeOptionMessage = "InvalidResetOptionMessage";
        private const string _sendFailureMessage = "ResetCodeSendFailureMessage";
        private const string _unexpectedErrorMessage = "UnexpectedErrorMessage";
        private const string _invalidResetCodeMessage = "InvalidResetCodeMessage";
        private const string _resetCodeExpiredMessage = "ResetCodeExpiredMessage";

        private const string _userIdentityViewstateName = "FPUserIdentity";
        private long UserIdentity
        {
            get
            {
                if (ViewState[_userIdentityViewstateName] != null) return (long)ViewState[_userIdentityViewstateName];
                return -1;
            }
            set
            {
                ViewState[_userIdentityViewstateName] = value;
            }
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
				InitializeConfig();

                _validationGroupName = "ForgotPassword" + ConfigurationKey.ConfigName;

                // Forgot password view setup
				rfFPIdentity.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPRequiredFieldMessageResourceKey);
				rfFPIdentity.ValidationGroup = _validationGroupName;
				lblFPIdentityLabel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPIdentityLabelResourceKey);
				lblFPErrorMessage.Text = string.Empty;
                lblFPResetProcessExplanation.Text = ResourceUtils.GetLocalWebResource(_modulePath, "FPResetProcessExplanation.Text");

                // Reset options view setup
                lblResetOptionsErrorMessage.Text = string.Empty;
                lblResetOptionsTitleMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, "ResetOptionsTitleMessage.Text");

                // Reset code view setup
                lblResetCodeErrorMessage.Text = string.Empty;
                lblResetCodeExplanation.Text = ResourceUtils.GetLocalWebResource(_modulePath, "ResetCodeExplanation.Text");
                lblResetCodeText.Text = ResourceUtils.GetLocalWebResource(_modulePath, "ResetCodeText.Text");
                rfResetCode.ValidationGroup = _validationGroupName;
                rfResetCode.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, _config.SUIDRequiredFieldMessageResourceKey);

                // Change password view setup
				rfCPPassword.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPRequiredFieldMessageResourceKey);
				rfCPPassword.ValidationGroup = _validationGroupName;
				rfCPConfirm.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPRequiredFieldMessageResourceKey);
				rfCPConfirm.ValidationGroup = _validationGroupName;
				cmpCPConfirm.ErrorMessage = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPPasswordsMustMatchMessageResourceKey);
				cmpCPConfirm.ValidationGroup = _validationGroupName;
				lblCPPassword.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPPasswordLabelResourceKey);
				lblCPConfirm.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPConfirmLabelResourceKey);
				lblCPErrorMessage.Text = string.Empty;
                lblCPExplanation.Text = ResourceUtils.GetLocalWebResource(_modulePath, "CPExplanation.Text");

                // Password changed view setup
                lblPCMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.PCMessageResourceKey);

				switch (PortalState.Portal.ButtonStyle)
				{
					case PortalButtonStyle.Button:
						{
							_btnFPButton = new Button() { ID = "btnFPButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPButtonTextResourceKey), ValidationGroup = _validationGroupName };
							phFPButton.Controls.Add((WebControl)_btnFPButton);

							_btnCPButton = new Button() { ID = "btnCPButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPButtonTextResourceKey), ValidationGroup = _validationGroupName };
							phCPButton.Controls.Add((WebControl)_btnCPButton);

							_btnPCButton = new Button() { ID = "btnPCButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.PCButtonTextResourceKey), ValidationGroup = _validationGroupName };
							phPCButton.Controls.Add((WebControl)_btnPCButton);

                            _btnROButton = new Button() { ID = "btnROButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, "ROButtonText.Text"), ValidationGroup = _validationGroupName };
                            phResetOptionsButton.Controls.Add((WebControl)_btnROButton);

                            _btnRCButton = new Button() { ID = "btnRCButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, "RCButtonText.Text"), ValidationGroup = _validationGroupName };
                            phResetCodeButton.Controls.Add((WebControl)_btnRCButton);
						}
						break;
					case PortalButtonStyle.LinkButton:
						{
							_btnFPButton = new LinkButton() { ID = "btnFPButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPButtonTextResourceKey), ValidationGroup = _validationGroupName };
							phFPButton.Controls.Add((WebControl)_btnFPButton);

							_btnCPButton = new LinkButton() { ID = "btnCPButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPButtonTextResourceKey), ValidationGroup = _validationGroupName };
							phCPButton.Controls.Add((WebControl)_btnCPButton);

							_btnPCButton = new LinkButton() { ID = "btnPCButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.PCButtonTextResourceKey), ValidationGroup = _validationGroupName };
							phPCButton.Controls.Add((WebControl)_btnPCButton);

                            _btnROButton = new LinkButton() { ID = "btnROButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, "ROButtonText.Text"), ValidationGroup = _validationGroupName };
                            phResetOptionsButton.Controls.Add((WebControl)_btnROButton);

                            _btnRCButton = new LinkButton() { ID = "btnRCButton", Text = ResourceUtils.GetLocalWebResource(_modulePath, "RCButtonText.Text"), ValidationGroup = _validationGroupName };
                            phResetCodeButton.Controls.Add((WebControl)_btnRCButton);
						}
						break;
				}
				if (_btnFPButton != null) _btnFPButton.Click += new EventHandler(btnFPButton_Click);
				if (_btnCPButton != null) _btnCPButton.Click += new EventHandler(btnCPButton_Click);
				if (_btnPCButton != null) _btnPCButton.Click += new EventHandler(btnPCButton_Click);
                if (_btnROButton != null) _btnROButton.Click += new EventHandler(btnROButton_Click);
                if (_btnRCButton != null) _btnRCButton.Click += new EventHandler(btnRCButton_Click);

                if (!Page.IsPostBack)
                {
                    mvForgotPassword.SetActiveView(vwForgotPassword);
                    if (!string.IsNullOrEmpty(Request.Params["u"]))
                    {
                        tbFPIdentity.Text = Request.Params["u"];
                        btnFPButton_Click(sender, e);
                        if (!string.IsNullOrEmpty(Request.Params["r"]))
                        {
                            tbResetCode.Text = Request.Params["r"];
                            btnRCButton_Click(sender, e);
                        }
                    }
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				lblMessage.Text = "Unexpected error.";
				lblMessage.CssClass = "Validator";
				mvForgotPassword.SetActiveView(vwMessage);
				OnException(ex);
			}
		}

        // FP = Forgot Password. This event submits the user's identity and gets the user's reset options to be displayed
		void btnFPButton_Click(object sender, EventArgs e)
		{
			const string methodName = "btnFPButton_Click";
			try
			{
				if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
				{
					// resolve member
					if (DataService == null)
					{
						_logger.Error(_className, methodName, "Can't get instance of data service.");
						lblFPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
						return;
					}
					AuthenticationFields identityType = AuthenticationFields.Username;
					switch (_config.FPIdentityType)
					{
						case ForgotPasswordIdentityType.Username:
							identityType = AuthenticationFields.Username;
							break;
						case ForgotPasswordIdentityType.EmailAddress:
							identityType = AuthenticationFields.PrimaryEmailAddress;
							break;
						case ForgotPasswordIdentityType.AlternateID:
							identityType = AuthenticationFields.AlternateId;
							break;
						case ForgotPasswordIdentityType.LoyaltyID:
							identityType = AuthenticationFields.LoyaltyIdNumber;
							break;
					}
					Member member = null;
					try
					{
						member = LoyaltyService.LoadMemberFromIdentity(identityType, tbFPIdentity.Text);
					}
					catch(Exception ex)
					{
						string msg = string.Format("Unable to resolve member '{0}': {1}", tbFPIdentity.Text, ex.Message);
						_logger.Error(_className, methodName,  msg, ex);
						lblFPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
						return;
					}

                    UserIdentity = member.IpCode;

                    EmailDocument emailDoc = EmailService.GetEmail(_config.FPEmailID);
                    SmsDocument smsDoc = SmsService.GetSmsMessage(_config.FPSmsID);

                    Dictionary<string, string> resetOptions = LoyaltyService.GetPasswordResetOptions(member, emailDoc != null ? emailDoc.Name : null, smsDoc != null ? smsDoc.Name : null);
                    foreach (string key in resetOptions.Keys)
                    {
                        if (!string.IsNullOrEmpty(resetOptions[key]))
                        {
                            string displayText = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "ResetOption_" + key), resetOptions[key]);
                            if (string.IsNullOrEmpty(displayText)) displayText = string.Format("{0}: {1}", key, resetOptions[key]);

                            rblResetOptionsList.Items.Add(new ListItem(displayText, key));
                        }
                    }

                    string existingCodeText = ResourceUtils.GetLocalWebResource(_modulePath, "ResetOption_existing");
                    rblResetOptionsList.Items.Add(new ListItem(existingCodeText, "existing"));

                    mvForgotPassword.SetActiveView(vwResetOptions);
				}
				else
				{
					// resolve csagent					
					if (CSService == null)
					{
						_logger.Error(_className, methodName, "Can't get instance of cs service.");
						lblFPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
						return;
					}

					CSAgent csagent = CSService.GetCSAgentByUserName(tbFPIdentity.Text, AgentAccountStatus.Active);
					if (csagent == null)
					{
						_logger.Error(_className, methodName, "Unable to resolve csagent for: " + tbFPIdentity.Text);
						lblFPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
						return;
					}

                    UserIdentity = csagent.Id;

                    string maskedEmail = LoyaltyService.MaskEmail(csagent.EmailAddress);
                    string displayText = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "ResetOption_email"), maskedEmail);
                    if (string.IsNullOrEmpty(displayText)) displayText = string.Format("email: {0}", maskedEmail);
                    rblResetOptionsList.Items.Add(new ListItem(displayText, "email"));

                    string existingCodeText = ResourceUtils.GetLocalWebResource(_modulePath, "ResetOption_existing");
                    rblResetOptionsList.Items.Add(new ListItem(existingCodeText, "existing"));

                    mvForgotPassword.SetActiveView(vwResetOptions);
				}

                string emailSubmitText = ResourceUtils.GetLocalWebResource(_modulePath, "ROButtonText.Text");
                string existingSubmitText = ResourceUtils.GetLocalWebResource(_modulePath, "RCButtonText.Text");
                var sb = new StringBuilder("<script type=\"text/javascript\">\r\n$(document).ready(function() { ");
                sb.AppendLine("$(\"[id*='rblResetOptionsList']\").change(function() {");
                sb.AppendLine("if (this.value == 'email' || this.value == 'sms') { $(\"[id*='btnROButton']\").text('" + emailSubmitText + "') }");
                sb.AppendLine("else if (this.value == 'existing') { $(\"[id*='btnROButton']\").text('" + existingSubmitText + "') }");
                sb.AppendLine("});});");
                sb.AppendLine("</script>");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ResetOptionsListChange", sb.ToString());
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				lblFPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
				OnException(ex);
			}
		}

        // RO = Reset Options. This event submits the desired reset option (i.e. email or SMS) and displays the view to input the sent reset code
        void btnROButton_Click(object sender, EventArgs e)
        {
            const string methodName = "btnROButton_Click";

            try
            {
                if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
                {
                    // resolve member
                    if (DataService == null)
                    {
                        _logger.Error(_className, methodName, "Can't get instance of data service.");
                        lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _connectionIssueMessage);
                        return;
                    }

                    if (rblResetOptionsList.Items.Count == 0 || rblResetOptionsList.SelectedValue == null)
                    {
                        _logger.Error(_className, methodName, "No reset option selected.");
                        lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _invalidResetCodeOptionMessage);
                        return;
                    }

                    Member member = LoyaltyService.LoadMemberFromIPCode(UserIdentity);

                    switch (rblResetOptionsList.SelectedValue)
                    {
                        case "sms":
                            {
                                string resetCode = LoyaltyService.GenerateMemberResetCode(member, (int)_config.SUIDExpiryMinutes);
                                Dictionary<string, string> smsFields = new Dictionary<string, string>();
                                // send the sms message
                                try
                                {
                                    using (TriggeredSms sms = new TriggeredSms(LWConfigurationUtil.GetCurrentConfiguration(), _config.FPSmsID))
                                    {
                                        sms.Send(member, smsFields);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(_className, methodName, "Error occurred when sending the SMS message: " + _config.FPSmsID, ex);
                                    lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _sendFailureMessage);
                                    return;
                                }
                            }
                            break;

                        case "email":
                            {
                                string resetCode = LoyaltyService.GenerateMemberResetCode(member, (int)_config.SUIDExpiryMinutes);
                                Dictionary<string, string> emailFields = new Dictionary<string, string>();
                                // send the email
                                try
                                {
                                    using (ITriggeredEmail email = TriggeredEmailFactory.Create(_config.FPEmailID))
                                    {
                                        email.SendAsync(member, emailFields).Wait();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(_className, methodName, "Error occurred when sending the triggered email: " + _config.FPEmailID, ex);
                                    lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _sendFailureMessage);
                                    return;
                                }
                            }
                            break;

                        case "existing":
                            break;

                        default:
                            _logger.Error(_className, methodName, "Unknown reset option selected: " + rblResetOptionsList.SelectedValue);
                            lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _invalidResetCodeOptionMessage);
                            return;
                    }

                    mvForgotPassword.SetActiveView(vwResetCode);
                }
                else
                {
                    // resolve csagent					
                    if (CSService == null)
                    {
                        _logger.Error(_className, methodName, "Can't get instance of cs service.");
                        lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _connectionIssueMessage);
                        return;
                    }

                    CSAgent csagent = CSService.GetCSAgentById(UserIdentity);
                    if (csagent == null)
                    {
                        _logger.Error(_className, methodName, "Unable to resolve csagent for: " + UserIdentity);
                        lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
                        return;
                    }

                    switch (rblResetOptionsList.SelectedValue)
                    {
                        case "email":
                            {
                                string singleUseCode = CSService.GenerateCSAgentResetCode(csagent, (int)_config.SUIDExpiryMinutes);

                                Dictionary<string, string> emailFields = new Dictionary<string, string>();
                                emailFields.Add("ResetCode", singleUseCode);
                                emailFields.Add("AgentEmail", csagent.EmailAddress);
                                // send the email
                                try
                                {
									using (ITriggeredEmail email = TriggeredEmailFactory.Create(_config.FPEmailID))
                                    {
                                        email.SendAsync(csagent.EmailAddress, emailFields).Wait();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(_className, methodName, "Error occurred when sending the triggered email: " + _config.FPEmailID, ex);
                                    lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _sendFailureMessage);
                                    return;
                                }
                            }
                            break;

                        case "existing":
                            break;

                        default:
                            _logger.Error(_className, methodName, "Unknown reset option selected: " + rblResetOptionsList.SelectedValue);
                            lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _invalidResetCodeOptionMessage);
                            return;
                    }

                    mvForgotPassword.SetActiveView(vwResetCode);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
                lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _unexpectedErrorMessage);
                OnException(ex);
            }
        }

        // RC = Reset Code. This event submits the reset code and displays the change password screen
        void btnRCButton_Click(object sender, EventArgs e)
        {
            const string methodName = "btnRCButton_Click";

            try
            {
                if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
                {
                    // resolve member
                    if (DataService == null)
                    {
                        _logger.Error(_className, methodName, "Can't get instance of data service.");
                        lblResetCodeErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _connectionIssueMessage);
                        return;
                    }

                    Member member = LoyaltyService.LoadMemberFromIPCode(UserIdentity);

                    string resetCode = tbResetCode.Text;

                    if (member.ResetCode != resetCode)
                    {
                        _logger.Error(_className, methodName, "Invalid reset code entered: " + resetCode);
                        lblResetCodeErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _invalidResetCodeMessage);
                        return;
                    }

                    if (member.ResetCodeDate < DateTime.Now)
                    {
                        _logger.Error(_className, methodName, "Reset code has expired");
                        lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _resetCodeExpiredMessage);
                        mvForgotPassword.SetActiveView(vwResetOptions);
                        return;
                    }

                    mvForgotPassword.SetActiveView(vwChangePassword);
                }
                else
                {
                    // resolve csagent					
                    if (CSService == null)
                    {
                        _logger.Error(_className, methodName, "Can't get instance of cs service.");
                        lblResetCodeErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _connectionIssueMessage);
                        return;
                    }

                    CSAgent csagent = CSService.GetCSAgentById(UserIdentity);
                    if (csagent == null)
                    {
                        _logger.Error(_className, methodName, "Unable to resolve csagent for: " + UserIdentity);
                        lblResetCodeErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.FPErrorMessageResourceKey);
                        return;
                    }

                    string singleUseCode = tbResetCode.Text;

                    if (csagent.ResetCode != singleUseCode)
                    {
                        _logger.Error(_className, methodName, "Invalid reset code entered: " + singleUseCode);
                        lblResetCodeErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _invalidResetCodeMessage);
                        return;
                    }

                    if (csagent.ResetCodeDate < DateTime.Now)
                    {
                        _logger.Error(_className, methodName, "Reset code has expired");
                        lblResetOptionsErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _resetCodeExpiredMessage);
                        mvForgotPassword.SetActiveView(vwResetOptions);
                        return;
                    }

                    mvForgotPassword.SetActiveView(vwChangePassword);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
                lblResetCodeErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _unexpectedErrorMessage);
                OnException(ex);
            }
        }

        // CP = Change Password. This event submits the new password and displays the password changed confirmation
		void btnCPButton_Click(object sender, EventArgs e)
		{
			const string methodName = "btnCPButton_Click";

			try
			{
                if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing)
                {
                    // resolve member
                    if (DataService == null)
                    {
                        _logger.Error(_className, methodName, "Can't get instance of data service.");
                        lblCPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPErrorMessageResourceKey);
                        return;
                    }
                    
                    Member member = LoyaltyService.LoadMemberFromIPCode(UserIdentity);

                    if (_config.UnlockLockedMember && member.MemberStatus == MemberStatusEnum.Locked) member.NewStatus = MemberStatusEnum.Active;
                    LoyaltyService.ChangeMemberPassword(member, tbCPPassword.Text);
                    SendMemberConfirmationEmail(member);
                    UserIdentity = -1;
                    mvForgotPassword.SetActiveView(vwPasswordChanged);
                }
                else
                {
                    // resolve csagent					
                    if (CSService == null)
                    {
                        _logger.Error(_className, methodName, "Can't get instance of cs service.");
                        lblCPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPErrorMessageResourceKey);
                        return;
                    }

                    CSAgent csagent = CSService.GetCSAgentById(UserIdentity);
                    if (csagent == null)
                    {
                        _logger.Error(_className, methodName, "Unable to resolve csagent for: " + UserIdentity);
                        lblCPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPErrorMessageResourceKey);
                        return;
                    }

                    CSService.ChangeCSAgentPassword(csagent, tbCPPassword.Text);
                    SendCSAgentConfirmationEmail(csagent);
                    UserIdentity = -1;
                    mvForgotPassword.SetActiveView(vwPasswordChanged);
                }
			}
			catch (AuthenticationException ex)
			{
				_logger.Error(_className, methodName, "Invalid password: " + ex.Message, ex);
				if (_config.CPUseValidatorMessages)
				{
					lblCPErrorMessage.Text = ex.Message;
				}
				else
				{
					lblCPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPErrorMessageResourceKey);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				lblCPErrorMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.CPErrorMessageResourceKey);
				OnException(ex);
			}
		}

        // PC = Password Changed. This event simply forwards the user to the configured redirect URL
		void btnPCButton_Click(object sender, EventArgs e)
		{
			const string methodName = "btnPCButton_Click";

			try
			{
				if (!string.IsNullOrEmpty(_config.PCRedirectURL))
				{
					Response.Redirect(_config.PCRedirectURL);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				OnException(ex);
			}
		}


		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<ForgotPasswordConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new ForgotPasswordConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}

		private void SendMemberConfirmationEmail(Member member)
		{
			const string methodName = "SendMemberConfirmationEmail";
			try
			{
				if (_config.PCEmailOnChange && _config.PCEmailID > 0)
				{
					using (ITriggeredEmail email = TriggeredEmailFactory.Create(_config.PCEmailID))
					{
						email.SendAsync(member).Wait();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception sending notification email: " + ex.Message, ex);
			}
		}

		private void SendCSAgentConfirmationEmail(CSAgent csagent)
		{
			const string methodName = "SendCSAgentConfirmationEmail";
			try
			{
				if (_config.PCEmailOnChange && _config.PCEmailID > 0)
				{
					using (ITriggeredEmail email = TriggeredEmailFactory.Create(_config.PCEmailID))
                    {
                        email.SendAsync(csagent.EmailAddress, new Dictionary<string, string>()).Wait();
                    }
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception sending notification email: " + ex.Message, ex);
			}
		}
	}
}
