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
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.EmailFeedbackHistory.Components
{
    public class DefaultGridProvider : AspGridProviderBase
    {
        private const string _className = "DefaultGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private List<EmailFeedback> _feedback = null;
        private string _email = null;
        private Dictionary<long, string> _agents = new Dictionary<long, string>();
        private ICustomGridAction[] _actions = null;

        public override string Id
        {
            get { return "grdEmailFeedback"; }
        }

        protected void LoadFeedback()
        {
            _feedback = EmailService.EmailFeedbackDao.RetrieveAll(_email);
        }

        private object GetData(EmailFeedback feedback, DynamicGridColumnSpec column)
        {
            object ret = null;
            switch (column.Name)
            {
                case "Id":
                    ret = feedback.Id;
                    break;
                case "FeedbackDate":
                    ret = feedback.FeedbackDate;
                    break;
                case "FeedbackType":
                    ret = string.Format(
                        "{0} ({1})", 
                        ResourceUtils.GetLocalWebResource(ParentControl, "feedbacktype-" + feedback.FeedbackType.ToString(), feedback.FeedbackType.ToString()),
                        ResourceUtils.GetLocalWebResource(ParentControl, "feedbacksubtype-" + feedback.FeedbackSubtype.ToString(), feedback.FeedbackSubtype.ToString()));
                    break;
                case "ClearedBy":
                    if (feedback.ClearedBy.HasValue)
                    {
                        if (!_agents.ContainsKey(feedback.ClearedBy.Value))
                        {
                            using (var cs = LWDataServiceUtil.CSServiceInstance())
                            {
                                var agent = cs.GetCSAgentById(feedback.ClearedBy.Value);
                                if(agent == null)
                                {
                                    throw new Exception(string.Format("Could not locate CS Agent with id {0}", feedback.ClearedBy.Value.ToString()));
                                }
                                _agents.Add(feedback.ClearedBy.Value, string.Format("{0} {1}", agent.FirstName, agent.LastName));
                            }
                        }
                        ret = _agents[feedback.ClearedBy.Value];
                    }
                    break;
            }
            return ret;
        }

        protected override string GetGridName()
        {
            return "EmailFeedbackHistory";
        }

        public override bool IsButtonEnabled(string commandName, object key)
        {
            long id = long.Parse(key.ToString());
            if(_feedback != null)
            {
                var feedback = _feedback.FirstOrDefault(o => o.Id == id);
                if(feedback != null && feedback.ClearedBy.HasValue)
                {
                    return false;
                }
            }
            return true;
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            return new DynamicGridColumnSpec[4]
            {
                new DynamicGridColumnSpec("Id", "Id", typeof(long), true, false, false, null, false, false),
                new DynamicGridColumnSpec("FeedbackDate", ResourceUtils.GetLocalWebResource(ParentControl, "FeedbackDate.Text", "Feedback Date"), typeof(DateTime), false, false, false, null, true, true),
                new DynamicGridColumnSpec("FeedbackType", ResourceUtils.GetLocalWebResource(ParentControl, "FeedbackType.Text", "Feedback Type"), typeof(string), false, false, false, null, true, true),
                new DynamicGridColumnSpec("ClearedBy", ResourceUtils.GetLocalWebResource(ParentControl, "ClearedBy.Text", "Cleared By"), typeof(string), false, false, false, null, true, true)
            };
        }

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            throw new InvalidOperationException();
        }

        public override bool IsGridEditable()
        {
            return true;
        }

        public override ICustomGridAction[] GetCustomCommands()
        {
            if (_actions == null)
            {
                _actions = new ICustomGridAction[1];
                _actions[0] = new ClearFeedbackCommand(this);
            }
            return _actions;
        }

        public override bool IsButtonVisible(string commandName)
        {
            return commandName == "clear";
        }

        public override string GetEmptyGridMessage()
        {
            return ResourceUtils.GetLocalWebResource(ParentControl, "NoHistoryFound.Text", "No feedback history found.");
        }

        public override bool IsActionColumnVisible()
        {
            return true;
        }

        public override void SetSearchParm(string parmName, object parmValue)
        {
            if (parmName == "email")
            {
                _email = (string)parmValue;
            }
        }

        public override void LoadGridData()
        {
            LoadFeedback();
        }

        public override int GetNumberOfRows()
        {
            return _feedback != null ? _feedback.Count : 0;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            EmailFeedback feedback = _feedback[rowIndex];
            return GetData(feedback, column);
        }
    }
}