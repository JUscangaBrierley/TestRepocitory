using System;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal;

namespace Brierley.LWModules.DocumentViewer
{
    public partial class ViewDocumentViewer : ModuleControlBase
    {
		private const string _className = "ViewDocumentViewer";
		private static LWLogger _logger = LWLoggerManager.GetLogger("DocumentViewer");

		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";
			try
			{
				base.OnInit(e);

				LWConfiguration config = (LWConfiguration)HttpContext.Current.Session["LW_CONFIG"];
				if (config == null)
				{
					string orgName = ConfigurationManager.AppSettings["LWOrganization"];
					string envName = ConfigurationManager.AppSettings["LWEnvironment"];
					LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);

					config = LWConfigurationUtil.GetConfiguration(orgName, envName);
					HttpContext.Current.Session["LW_CONFIG"] = config;

					LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
        }

		protected override void OnLoad(EventArgs e)
		{
			const string methodName = "OnLoad";
            try
            {
				base.OnLoad(e);

                DocumentViewerConfiguration moduleInfo = ConfigurationUtil.GetConfiguration<DocumentViewerConfiguration>(ConfigurationKey);
                if (moduleInfo == null)
                {
                    moduleInfo = new DocumentViewerConfiguration();
                    string defaultDocId = GetLocalResourceObject("DefaultDocId.Text").ToString();
                    moduleInfo.DocId = Convert.ToInt64(defaultDocId);
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, moduleInfo);
                }
				WebDocumentViewer1.LoyaltyMember = PortalState.CurrentMember;
                WebDocumentViewer1.DocumentID = moduleInfo.DocId.ToString();
            }
            catch (Exception ex)
            {
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
            }
        }
    }
}
