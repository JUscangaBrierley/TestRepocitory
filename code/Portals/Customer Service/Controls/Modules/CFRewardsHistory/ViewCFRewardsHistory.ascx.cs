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
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.LWModules.CFRewardsHistory.Components.List;

namespace Brierley.LWModules.CFRewardsHistory
{
    public partial class ViewCFRewardsHistory : ModuleControlBase
    {
        #region fields
        private const string _className = "ViewCFRewardsHistory";
        private const string _modulePath = "~/Controls/Modules/CFRewardsHistory/ViewCFRewardsHistory.ascx";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private CFRewardsHistoryConfig _config = null;
        private Member _member = null;

        private AspDynamicList _list = null;
        private AspListProviderBase _listProvider = null;

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

                _member = PortalState.CurrentMember;
                if (_member == null)
                {
                    this.Visible = false;
                    _logger.Trace(_className, methodName, "No member selected");
                    return;
                }

                _config = ConfigurationUtil.GetConfiguration<CFRewardsHistoryConfig>(ConfigurationKey);
                if (_config == null)
                {
                    return;
                }

                //lblModuleTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.ModuleTitleLabelResourceKey, "ModuleTitleLabel.Text"));

                string title = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.ListTitleLabelResourceKey, "ModuleTitleLabel.Text")); ;

                // List type
                if (string.IsNullOrEmpty(_config.ProviderAssemblyName) || string.IsNullOrEmpty(_config.ProviderClassName))
                {
					_listProvider = new RewardsHistoryListProvider();
                }
                else
                {
					_listProvider = (AspListProviderBase)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                }
                _listProvider.ValidationGroup = ValidationGroup;
                _listProvider.ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
                _listProvider.ParentControl = _modulePath;
                _listProvider.SetSearchParm("Config", _config);
                _listProvider.SetSearchParm("Member", _member);
                _listProvider.FilteringEnabled = _config.EnableFiltering;
                _list = new AspDynamicList()
                {
                    Title = title,
                    WrapInApplicationPanel = true,
                    Provider = _listProvider,
                    DateDisplayType = _config.DateDisplayType,
                    MinimumDateRange = _config.MinimumDateRange,
                    PriorDatesToDisplay = _config.DatesToDisplay
                };

                _list.ListActionClicked += new AspDynamicList.ListActionClickedHandler(ListActionClickedHandler);

                phRewardsList.Controls.Add(_list);
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

                InitializeConfig();

                //lblModuleTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath,
                //    StringUtils.FriendlyString(_config.ModuleTitleLabelResourceKey, "ModuleTitleLabel.Text")
                //);

                //if (!string.IsNullOrEmpty(_config.ModuleTitleLabelCssClass)) lblModuleTitle.CssClass = _config.ModuleTitleLabelCssClass;

                if (!Page.IsPostBack)
                {
                    // TODO: populate components here
                    if (_list != null)
                    {
                        _list.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                        _list.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                        _list.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                        if (!IsPostBack)
                        {
                            _list.Rebind();
                            _list.CollapsePanel(_config.IsCollapsed);
                        }
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
        void ListActionClickedHandler(object sender, ListActionClickedArg e)
        {
            //if (e.Key == null || e.Command == "Redeem")
            //{
            //    long id = long.Parse(e.Key.ToString());
            //    CouponUtil.RedeemCoupon(id, 1, null);
            //    _list.Rebind();
            //}
        }
        #endregion

        #region private methods
        private void InitializeConfig()
        {
            if (_config == null)
            {
                _config = ConfigurationUtil.GetConfiguration<CFRewardsHistoryConfig>(ConfigurationKey);
                if (_config == null)
                {
                    _config = new CFRewardsHistoryConfig();
                    ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
                }
            }
        }
        #endregion
    }
}
