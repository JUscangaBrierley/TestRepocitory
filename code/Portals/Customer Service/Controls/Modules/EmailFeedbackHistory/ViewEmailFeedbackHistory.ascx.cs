using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.LWModules.EmailFeedbackHistory.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.EmailFeedbackHistory
{
    public partial class ViewEmailFeedbackHistory : ModuleControlBase, IIpcEventHandler
    {
        private const string _className = "ViewEmailFeedbackHistory";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private const string _modulePath = "~/Controls/Modules/EmailFeedbackHistory/ViewEmailFeedbackHistory.ascx";
        private EmailFeedbackHistoryConfig _config = null;
        protected AspDynamicGrid _feedbackGrid = null;
        private IDynamicGridProvider _grdProvider = null;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            IpcManager.RegisterEventHandler("MemberSelected", this, false);
            IpcManager.RegisterEventHandler("MemberUpdated", this, false);
            _config = ConfigurationUtil.GetConfiguration<EmailFeedbackHistoryConfig>(ConfigurationKey) ?? new EmailFeedbackHistoryConfig();
            _grdProvider = new DefaultGridProvider();

            _grdProvider.ParentControl = "~/Controls/Modules/EmailFeedbackHistory/ViewEmailFeedbackHistory.ascx";
            _grdProvider.SetSearchParm("Configuration", _config);
            var member = PortalState.CurrentMember;
            if (member != null)
            {
                _grdProvider.SetSearchParm("email", member.PrimaryEmailAddress);
            }
            _feedbackGrid = new AspDynamicGrid();
            _feedbackGrid.ShowPositive += delegate (object sndr, string message) { ShowPositive(message); };
            _feedbackGrid.ShowNegative += delegate (object sndr, string message) { ShowNegative(message); };
            _feedbackGrid.ShowWarning += delegate (object sndr, string message) { ShowWarning(message); };
            _feedbackGrid.Provider = _grdProvider;
            _feedbackGrid.CreateTopPanel = false;
            
            pchFeedback.Controls.Add(_feedbackGrid);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _feedbackGrid.GridActionClicked += _feedbackGrid_GridActionClicked;
            lnkClearAll.Click += LnkClearAll_Click;
            lnkTestEmail.Click += LnkTestEmail_Click;
        }

        private void LnkTestEmail_Click(object sender, EventArgs e)
        {
            using (var svc = LWDataServiceUtil.EmailServiceInstance())
            {
                var email = TriggeredEmailFactory.Create(_config.TestEmailName);
                email.SendAsync(PortalState.CurrentMember).Wait();
            }
        }

        private void LnkClearAll_Click(object sender, EventArgs e)
        {
            var agent = PortalState.GetLoggedInCSAgent();
            if (agent == null)
            {
                throw new Exception("Email feedback must be cleared by a customer service agent. Current logged in agent is null");
            }

            using (var svc = LWDataServiceUtil.EmailServiceInstance())
            {
                svc.ClearEmailFeedbacks(PortalState.CurrentMember.PrimaryEmailAddress, agent.Id);
            }

            _feedbackGrid.Rebind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            CheckForSuppression();
            lnkClearAll.Visible = _config.AllowClear && _feedbackGrid.Grid.Rows.Count > 0;
        }

        private void _feedbackGrid_GridActionClicked(object sender, GridActionClickedArg e)
        {
            _feedbackGrid.Rebind();
        }

        protected override bool ControlRequiresTelerikSkins()
        {
            return false;
        }

        public void HandleEvent(IpcEventInfo info)
        {
            const string methodName = "HandleEvent";
            if (info.EventName == "MemberSelected" || info.EventName == "MemberUpdated")
            {
                _logger.Trace(_className, methodName, string.Format("Event {0}, by module id {1}", info.EventName, info.PublishingModule));
                if (_feedbackGrid != null)
                {
                    _feedbackGrid.Rebind();
                    CheckForSuppression();
                }
            }
        }

        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }

        private void CheckForSuppression()
        {
            pchEmailSuppressed.Visible = false;
            lnkTestEmail.Visible = false;
            Member member = PortalState.CurrentMember;
            if (member != null && !string.IsNullOrEmpty(member.PrimaryEmailAddress))
            {
                bool suppressed = EmailService.IsEmailSuppressed(member.PrimaryEmailAddress);
                pchEmailSuppressed.Visible = suppressed;
                if (suppressed)
                {
                    litEmailSuppressed.Text = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "EmailIsSuppressed"), member.PrimaryEmailAddress);
                }
                lnkTestEmail.Visible = !suppressed && !string.IsNullOrEmpty(_config.TestEmailName);
            }
        }
    }
}