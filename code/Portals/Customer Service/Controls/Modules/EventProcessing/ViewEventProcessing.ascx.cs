using System;
using System.Collections.Generic;
using System.Web.UI;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.EventProcessing
{
	public partial class ViewEventProcessing : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewEventProcessing";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private EventProcessingConfig _config = null;
		private Member _member = null;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			const string methodName = "OnInit";

			_config = ConfigurationUtil.GetConfiguration<EventProcessingConfig>(ConfigurationKey);

			if (_config == null)
			{
				return;
			}

			_member = PortalState.CurrentMember;

			if (_config.AllowEventTriggering)
			{
				IpcManager.RegisterEventHandler("TriggerEvent", this, false);
			}

			if (IsAjaxPostback())
			{
				try
				{
					_logger.Trace(_className, methodName, "AJAX post begin");

					string eventName = Request.Form["eventName"];
					if (_config.AllowEventTriggering && _config.ListenForEvents.Contains(eventName))
					{
						var context = new Dictionary<string, object>();

						foreach (var key in Request.Form.AllKeys)
						{
							if (key.StartsWith("context[", StringComparison.OrdinalIgnoreCase) && key.EndsWith("]"))
							{
								context.Add(key.Substring(8, key.Length - 9), Request.Form[key]);
							}
						}
						ProcessEvent(eventName, context);
					}
					else
					{
						_logger.Trace(_className, methodName, string.Format("Cannot process event {0} because configuration does not allow it.", eventName));
					}

					Response.ContentType = "application/json; charset=utf-8";
					Response.Clear();

					//todo: list rules that executed?
					Response.Write("success");

					_logger.Trace(_className, methodName, "AJAX post end");
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Failed to process events for AJAX post", ex);
				}
				finally
				{
					Response.Flush();
					Response.End();
				}
				return;
			}

			if (!IsPostBack)
			{
				try
				{
					if (_config.AutomaticEvents != null && _config.AutomaticEvents.Count > 0)
					{
						foreach (string eventName in _config.AutomaticEvents)
						{
							ProcessEvent(eventName);
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, ex.Message, ex);
				}
			}
		}

		private bool IsAjaxPostback()
		{
			return Page.Request.Form["eventName"] != null && Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}


        private void ProcessEvent(long id)
        {
            LWEvent e = LoyaltyService.GetLWEvent(id);
            ProcessEvent(e.Name);
        }

		private void ProcessEvent(string name, Dictionary<string, object> context = null)
		{
			ContextObject co = new ContextObject() { Owner = _member };
			co.Environment.Add(EnvironmentKeys.Channel, PortalState.UserChannel);
			co.Environment.Add(EnvironmentKeys.Language, LanguageChannelUtil.GetDefaultCulture());

			if (context != null)
			{
				foreach (var c in context)
				{
					if (co.Environment.ContainsKey(c.Key))
					{
						co.Environment[c.Key] = c.Value;
					}
					else
					{
						co.Environment.Add(c.Key, c.Value);
					}
				}
			}
            LoyaltyService.ExecuteEventRules(co, name, Brierley.FrameWork.Common.RuleInvocationType.Manual);
		}


		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Trace(_className, methodName, "Begin");
			try
			{
				if (
					info.PublishingModule != base.ConfigurationKey &&
					(info.EventName == "TriggerEvent") &&
					_config != null
					)
				{
					if (info.EventData is TriggerEventData)
					{
						TriggerEventData data = (TriggerEventData)info.EventData;
						ProcessEvent(data.EventName, data.Context);
					}
					else
					{
						_logger.Error(_className, methodName, "Cannot process event - IpcEventInfo.EventData is not of type Brierley.WebFrameWork.Ipc.TriggerEventData");
					}
				}

				//fire MemberUpdated event so that other modules can refresh (e.g., the event executed a campaign that granted a bonus/coupon/promotion 
				//and some other module may need to update its list to reflect the addition).
				IpcManager.PublishEvent("MemberUpdated", this.ConfigurationKey, _member);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Empty, ex);
			}
			_logger.Trace(_className, methodName, "End");
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return base.ConfigurationKey;
		}


	}
}