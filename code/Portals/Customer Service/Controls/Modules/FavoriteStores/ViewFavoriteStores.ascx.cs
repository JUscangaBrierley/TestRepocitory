using System;
using System.Configuration;
using System.Linq;
using System.Web.UI;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LWModules.FavoriteStores.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.FavoriteStores
{
	public partial class ViewFavoriteStores : ModuleControlBase
	{
		private const string _className = "ViewFavoriteStores";
		private const string _modulePath = "~/Controls/Modules/FavoriteStores/ViewFavoriteStores.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private FavoriteStoresConfig _config = null;
		private AspListProviderBase _provider = null;
		private AspDynamicList _list = null;

		protected string LocationsUrl
		{
			get
			{
				return !string.IsNullOrEmpty(_config.LocationsPageUrl) ? _config.LocationsPageUrl : "locations";
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			const string methodName = "OnInit";
			try
			{
				InitializeConfig();

				if (!string.IsNullOrWhiteSpace(_config.HeaderContentTextBlockName))
				{
					TextBlock header = ContentService.GetTextBlock(_config.HeaderContentTextBlockName);
					if (header != null)
					{
						ContextObject contextObject = new ContextObject();
						contextObject.Owner = PortalState.CurrentMember;

						string rawContent = header.GetContent();
						ContentEvaluator evaluator = new ContentEvaluator(rawContent, contextObject);
						string evaluatedContent = evaluator.Evaluate("##", 5);

						phHeaderContent.Controls.Add(new LiteralControl(evaluatedContent));
					}
				}

                string listTitle = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleLabelResourceKey);
				pnlMain.Text = listTitle;
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
				Member member = PortalState.CurrentMember;
				if (member == null)
				{
					return;
				}
				var memberStores = LoyaltyService.GetMemberStoresByMember(member.IpCode);
				if (memberStores != null && memberStores.Count > 0)
				{
					long[] sortedMemberStoreIDs = (from x in memberStores orderby x.PreferenceOrder ascending select x.StoreDefId).ToArray<long>();
					if (sortedMemberStoreIDs.Length > 0)
					{
						var favoriteStores = ContentService.GetAllStoreDefs(sortedMemberStoreIDs);
						lstStores.DataSource = favoriteStores;
						lstStores.DataBind();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}


		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<FavoriteStoresConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new FavoriteStoresConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}
	}
}
