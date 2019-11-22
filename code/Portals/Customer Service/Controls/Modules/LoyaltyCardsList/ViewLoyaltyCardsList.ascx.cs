using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Controls.List;

using Brierley.LWModules.LoyaltyCardsList.Components;

namespace Brierley.LWModules.LoyaltyCardsList
{
    public partial class ViewLoyaltyCardsList : ModuleControlBase
    {
        #region fields
        private const string _className = "ViewLoyaltyCardsList";
        private const string _modulePath = "~/Controls/Modules/LoyaltyCardsList/ViewLoyaltyCardsList.ascx";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private LoyaltyCardsListConfig _config = null;

		private AspListProviderBase _listProvider = null;        
        private AspDynamicList _list = null;        
        #endregion

        #region properties

        #endregion

        #region page life cycle
        protected override void OnInit(EventArgs e)
        {
            const string methodName = "OnLoad";
            try
            {
                base.OnInit(e);

                InitializeConfig();

                if (PortalState.CurrentMember == null)
                {
                    this.Visible = false;
                    return;
                }

                string title = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.ModuleTitleLabelResourceKey, "ModuleTitleLabel.Text"));

                //List Details
                if (string.IsNullOrEmpty(_config.ProviderClassName) || string.IsNullOrEmpty(_config.ProviderAssemblyName))
                {
                    _logger.Trace(_className, methodName, "No provider has been set. Using default");
                    _listProvider = new LoyaltyCardListProvider() { ParentControl = _modulePath };
                }
                else
                {
					_listProvider = (AspListProviderBase)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                    _listProvider.ParentControl = _modulePath;
                }
                _listProvider.FilteringEnabled = _config.EnableListFiltering;

                _list = new AspDynamicList() { WrapInApplicationPanel = true, Title = title };
                _list.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                _list.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                _list.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                _list.Provider = _listProvider;                
                _list.SetSearchParm("Configuration", _config);
                _listProvider.FilteringEnabled = _config.EnableListFiltering;
                
                phLoyaltyCards.Controls.Add(_list);
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
               
                lblModuleTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath,
                    StringUtils.FriendlyString(_config.ModuleTitleLabelResourceKey, "ModuleTitleLabel.Text")
                );

                if (!string.IsNullOrEmpty(_config.ModuleTitleLabelCssClass)) lblModuleTitle.CssClass = _config.ModuleTitleLabelCssClass;

                if (!Page.IsPostBack)
                {
					_list.Rebind();
                    _list.CollapsePanel(_config.IsCollapsed);
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
                _config = ConfigurationUtil.GetConfiguration<LoyaltyCardsListConfig>(ConfigurationKey);
                if (_config == null)
                {
                    _config = new LoyaltyCardsListConfig();
                    ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
                }
            }
        }
        #endregion
    }
}
