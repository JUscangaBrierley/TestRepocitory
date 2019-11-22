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

using Brierley.LWModules.CouponsList.Components;

namespace Brierley.LWModules.CouponsList
{
    public partial class ViewCouponsList : ModuleControlBase
    {
        #region fields
        private const string _className = "ViewCouponsList";
        private const string _modulePath = "~/Controls/Modules/CouponsList/ViewCouponsList.ascx";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private CouponsListConfig _config = null;

        private AspDynamicList _list = null;
		private AspListProviderBase _listProvider = null;

        private Member _member = null;

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

                InitializeConfig();

                string listTitle = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleLabelResourceKey);

                if (string.IsNullOrEmpty(_config.ProviderAssemblyName) || string.IsNullOrEmpty(_config.ProviderClassName))
                {
                    _listProvider = new CouponsListProvider();
                }
                else
                {
					_listProvider = (AspListProviderBase)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                }
                _listProvider.ValidationGroup = ValidationGroup;
                _listProvider.ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
                _listProvider.ParentControl = _modulePath;
                _listProvider.SetSearchParm("Config", _config);
                _listProvider.SetSearchParm("MemberIpCode", _member.IpCode);
                _listProvider.FilteringEnabled = _config.EnableFiltering;
                
                _list = new AspDynamicList() { Provider = _listProvider, WrapInApplicationPanel = true, Title = listTitle };
                _list.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                _list.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                _list.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };

                _list.DateDisplayType = _config.DateDisplayType;
                _list.PriorDatesToDisplay = _config.DatesToDisplay;
                _list.MinimumDateRange = _config.MinimumDateRange;
                

                phCouponsList.Controls.Add(_list);
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
                _config = ConfigurationUtil.GetConfiguration<CouponsListConfig>(ConfigurationKey);
                if (_config == null)
                {
                    _config = new CouponsListConfig();
                    ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
                }
            }
        }
        #endregion
    }
}
