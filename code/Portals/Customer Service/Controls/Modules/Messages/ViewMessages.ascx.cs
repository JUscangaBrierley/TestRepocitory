using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.Messages
{
	public partial class ViewMessages : ModuleControlBase
    {
        private const string _className = "ViewMessages";
		private const string _modulePath = "~/Controls/Modules/Messages/ViewMessages.ascx";
		private MessagesConfig _config = null;
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private Member _member = null;

		protected MessagesConfig Config
		{
			get
			{
				return _config;
			}
		}

        protected override void OnInit(EventArgs e)
		{
            string methodName = "OnInit";

            try
            {
                _member = PortalState.CurrentMember;
                if (_member == null)
                {
                    this.Visible = false;
                    return;
                }

                _config = ConfigurationUtil.GetConfiguration<MessagesConfig>(ConfigurationKey);
                if (_config == null)
                {
					throw new Exception(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "ConfigurationNotFound.Text", "Messages module configuration not found for key {0}"), ConfigurationKey));
                }

				DateFilter.DateDisplayType = _config.DateDisplayType;
				DateFilter.PriorDatesToDisplay = _config.DatesToDisplay;
				DateFilter.MinimumDateRange = _config.MinimumDateRange;

				base.OnInit(e);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, string.Empty, ex);
				throw;
            }
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			if (_member == null || !this.Visible)
			{
				return;
			}
			/*
			//get initial set of messages instead of waiting
			var list = new List<dynamic>();

			var statuses = new List<MemberMessageStatus>();
			if ((_config.DefaultFilter & MessagesConfig.DefaultFilters.Unread) != 0)
			{
				statuses.Add(MemberMessageStatus.Unread);
			}
			if ((_config.DefaultFilter & MessagesConfig.DefaultFilters.Read) != 0)
			{
				statuses.Add(MemberMessageStatus.Read);
			}
			if (_config.AllowViewDeleted && (_config.DefaultFilter & MessagesConfig.DefaultFilters.Deleted) != 0)
			{
				statuses.Add(MemberMessageStatus.Deleted);
			}

			var context = new ContextObject() { Owner = PortalState.CurrentMember };

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				var page = loyalty.GetMemberMessages(PortalState.CurrentMember.IpCode, statuses, true, 1, _config.MessagesPerPage, null, null, MemberMessageOrder.Newest);
				foreach (var message in page.Items)
				{
					string description = null;
					
					var existing = list.FirstOrDefault(o => o.MessageDefId == message.MessageDefId);
					if (existing != null)
					{
						description = existing.Description;
					}
					else
					{
						var definition = content.GetMessageDef(message.MessageDefId);
						if (definition != null)
						{
							description = ParseExpressions(context, definition.Description);
						}
					}

					if (string.IsNullOrEmpty(description))
					{
						description = GetResource("EmptyMessageDescription");
					}

					var model = new { Id = message.Id, MessageDefId = message.MessageDefId, Status = message.Status, Description = description, DateIssued = message.DateIssued.ToShortDateString() };
					list.Add(model);
				}
			}

			lstMessages.DataSource = list;
			lstMessages.DataBind();
			
			/* 
			 * 
			 * 
				lstMessages.DataSource = messages;
				lstMessages.DataBind();
			}
			 * */
		}

		protected string GetResource(string key)
		{
			return ResourceUtils.GetLocalWebResource(_modulePath, key);
		}

		private string ParseExpressions(ContextObject co, string content)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				return content;
			}
			return ExpressionUtil.ParseExpressions(content, co);
		}
	}
}