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
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.WebDocumentViewer;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.TextBlockViewer
{
	public partial class ViewTextBlockViewer : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewTextBlockViewer";
		private const string _modulePath = "~/Controls/Modules/TextBlockViewer/ViewTextBlockViewer.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		protected TextBlockViewerConfig _config = null;
		#endregion

		#region properties

		#endregion

		#region page life cycle
		protected override void OnLoad(EventArgs e)
		{
			const string methodName = "OnLoad";
			try
			{
				base.OnLoad(e);

				InitializeConfig();

				if (!string.IsNullOrWhiteSpace(_config.TextBlockName))
				{
					TextBlock textBlock = ContentService.GetTextBlock(_config.TextBlockName);
					if (textBlock != null)
					{
						ContextObject contextObject = new ContextObject();
						contextObject.Owner = PortalState.CurrentMember;

						string rawContent = textBlock.GetContent();
						ContentEvaluator evaluator = new ContentEvaluator(rawContent, contextObject);
						string evaluatedContent = evaluator.Evaluate("##", 5);

						phTextBlock.Controls.Add(new LiteralControl(evaluatedContent));
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
		#endregion

		#region private methods
		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<TextBlockViewerConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new TextBlockViewerConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}
		#endregion
	}
}
