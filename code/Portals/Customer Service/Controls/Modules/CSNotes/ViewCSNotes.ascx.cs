using System;
using System.Reflection;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal;
using System.Web.UI;

namespace Brierley.LWModules.CSNotes
{
	public partial class ViewCSNotes : ModuleControlBase, IIpcEventHandler
	{
		#region Fields
		private const string _className = "ViewCSNotes";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private CSNotesConfig _config = null;
		protected AspDynamicGrid _csNotesGrid = null;
		private IDynamicGridProvider _grdProvider = null;
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
				IpcManager.RegisterEventHandler("MemberSelected", this, false);
				IpcManager.RegisterEventHandler("MemberUpdated", this, false);
				_config = ConfigurationUtil.GetConfiguration<CSNotesConfig>(ConfigurationKey) ?? new CSNotesConfig();
				_grdProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
				if (_grdProvider is AspGridProviderBase)
				{
					((AspGridProviderBase)_grdProvider).ValidationGroup = ValidationGroup;
					((AspGridProviderBase)_grdProvider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
				}

                _grdProvider.ParentControl = "~/Controls/Modules/CSNotes/ViewCSNotes.ascx";
                _grdProvider.SetSearchParm("Configuration", _config);
				var member = PortalState.CurrentMember;
				if (member == null)
				{
					_logger.Error(_className, methodName, "No member selected.");
				}
				else
				{
					_grdProvider.SetSearchParm("IpCode", member.IpCode);
				}
				_csNotesGrid = new AspDynamicGrid();
				_csNotesGrid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				_csNotesGrid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				_csNotesGrid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
				_csNotesGrid.Provider = _grdProvider;
				_csNotesGrid.CreateTopPanel = false;
				phCSNotes.Controls.Add(_csNotesGrid);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected Exception", ex);
				throw;
			}
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
				if (_csNotesGrid != null)
				{
					_csNotesGrid.Rebind();
				}
			}
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}
	}
}
