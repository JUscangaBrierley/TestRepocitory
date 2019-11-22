using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;

namespace Brierley.LWModules.EmailFeedbackHistory.Components
{
    public class ClearFeedbackCommand : ICustomGridAction
    {
        internal ClearFeedbackCommand(DefaultGridProvider provider)
        {
            Text = ResourceUtils.GetLocalWebResource(provider.ParentControl, "ClearFeedback.Text", "Clear");
            CommandName = "clear";
            CssClass = "delete";
            ControlId = "clearfeedback";
        }

        public string CommandName { get; private set; }

        public string CssClass { get; private set; }

        public string ControlId { get; private set; }

        public string Text { get; private set; }

        public void HandleCommand(System.Web.UI.Page page, object key)
        {
            long id = long.Parse((string)key);
            var agent = PortalState.GetLoggedInCSAgent();
            if (agent == null)
            {
                throw new Exception("Email feedback must be cleared by a customer service agent. Current logged in agent is null");
            }

            using (var svc = LWDataServiceUtil.EmailServiceInstance())
            {
                svc.ClearEmailFeedback(id, agent.Id);
            }
        }
    }
}