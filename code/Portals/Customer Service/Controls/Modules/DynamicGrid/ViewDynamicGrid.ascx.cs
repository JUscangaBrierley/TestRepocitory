using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal.Configuration;

namespace Brierley.LWModules.DynamicGrid
{
	public partial class ViewDynamicGrid : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewDynamicGrid";
		private const string _modulePath = "~/Controls/Modules/DynamicGrid/ViewDynamicGrid.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private DynamicGridConfig _config = null;
		private AspDynamicGrid _grid = null;
		private IDynamicGridProvider _provider = null;
		

		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";
			try
			{
				_config = ConfigurationUtil.GetConfiguration<DynamicGridConfig>(ConfigurationKey);

				if (_config == null || string.IsNullOrEmpty(_config.ProviderAssembly) || string.IsNullOrEmpty(_config.ProviderClass))
				{
					_logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey.ToString()));
					this.Visible = false;
					return;
				}

				_provider= (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssembly, _config.ProviderClass);
				if (_provider== null)
				{
					string errMsg = string.Format("Unable to create the provider assembly {0} and class {1}", _config.ProviderAssembly, _config.ProviderClass);
					_logger.Error(_className, methodName, errMsg);
					throw new Exception(errMsg);
				}
				if(_provider is AspGridProviderBase)
				{
					((AspGridProviderBase)_provider).ValidationGroup = ValidationGroup;
					((AspGridProviderBase)_provider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
				}

				h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);

				_grid = new AspDynamicGrid() { CreateTopPanel = false };
				_grid.Provider = _provider;
				pchGrid.Controls.Add(_grid);

				IpcManager.RegisterEventHandler("MemberUpdated", this, false);
				IpcManager.RegisterEventHandler("MemberCreated", this, false);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected Exception", ex);
				throw;
			}
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (_grid != null)
			{
				_grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				_grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				_grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
			}

			base.OnLoad(e);
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Trace(_className, methodName, "Begin");
			if (
				info.PublishingModule != base.ConfigurationKey &&
				(info.EventName == "MemberUpdated" || info.EventName == "MemberCreated") &&
				_config != null && _grid != null && _provider != null
				)
			{
				_grid.Rebind();
			}
			_logger.Trace(_className, methodName, "End");
		}

	}
}
