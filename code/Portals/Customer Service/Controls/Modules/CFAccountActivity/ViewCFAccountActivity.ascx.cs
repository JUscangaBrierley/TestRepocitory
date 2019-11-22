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
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

//using Brierley.LWModules.Coupons.Components.Grid;
using Brierley.LWModules.CFAccountActivity.Components.List;


namespace Brierley.LWModules.CFAccountActivity
{
    public partial class ViewCFAccountActivity : ModuleControlBase
    {
        #region fields
        private const string _className = "ViewCFAccountActivity";
        private const string _modulePath = "~/Controls/Modules/CFAccountActivity/ViewCFAccountActivity.ascx";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        protected CFAccountActivityConfig _config = null;

        private Member _member = null;

        private AspDynamicGrid _grid = null;
        //private IDynamicGridProvider _gridProvider = null;

        private AspDynamicList _salesList = null;
		private AspListProviderBase _salesListProvider = null;
        private AspDynamicList _orphansList = null;
		private AspListProviderBase _orphansListProvider = null;

        #endregion

        #region properties

        #endregion

        #region page life cycle
        protected override void OnInit(EventArgs e)
        {
            const string methodName = "Page_Load";
            try
            {
                _member = PortalState.CurrentMember;
                if (_member == null)
                {
                    this.Visible = false;
                    _logger.Trace(_className, methodName, "No member selected");
                    return;
                }

                InitializeConfig();

                lblModuleTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.ModuleTitleLabelResourceKey, "ModuleTitleLabel.Text"));

                string salesTxnTitle = string.Empty;
                if (!string.IsNullOrEmpty(_config.SalesTxnLabelResourceKey))
                {
                    salesTxnTitle = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.SalesTxnLabelResourceKey, "SalesTxnLabel_SalesTransactions.Text"));
                }

                if (!string.IsNullOrEmpty(_config.ModuleTitleLabelCssClass)) lblModuleTitle.CssClass = _config.ModuleTitleLabelCssClass;

                #region Sales Txns
                if (string.IsNullOrEmpty(_config.ProviderAssemblyName) || string.IsNullOrEmpty(_config.ProviderClassName))
                {
                    _salesListProvider = new TxnHeaderListProvider();
                }
                else
                {
					_salesListProvider = (AspListProviderBase)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                }
                _salesListProvider.ValidationGroup = ValidationGroup;
                _salesListProvider.ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
                _salesListProvider.ParentControl = _modulePath;
                _salesListProvider.SetSearchParm("Config", _config);
                _salesList = new AspDynamicList() 
                { 
                    Provider = _salesListProvider, 
                    Title = salesTxnTitle,
                    DateDisplayType = _config.DateDisplayType,
                    MinimumDateRange = _config.MinimumDateRange,
                    PriorDatesToDisplay = _config.DatesToDisplay,
                    WrapInApplicationPanel = true 
                };

                pchTxnHeaderData.Controls.Add(_salesList);
                #endregion

                #region Orphans
                if (_config.ShowOrphanGrid)
                {
                    string orphansTitle = string.Empty;
                    if (!string.IsNullOrEmpty(_config.OrphansLabelResourceKey))
                    {
                        orphansTitle = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.OrphansLabelResourceKey, "OrphansLabel.Text"));
                    }

                    if (string.IsNullOrEmpty(_config.OrphansProviderAssemblyName) || string.IsNullOrEmpty(_config.OrphansProviderClassName))
                    {
                        _orphansListProvider = new OrphansTxnListProvider();
                    }
                    else
                    {
						_orphansListProvider = (AspListProviderBase)ClassLoaderUtil.CreateInstance(_config.OrphansProviderAssemblyName, _config.OrphansProviderClassName);
                    }
                    _orphansListProvider.ValidationGroup = ValidationGroup;
                    _orphansListProvider.ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
                    _orphansListProvider.ParentControl = _modulePath;
                    _orphansListProvider.SetSearchParm("Config", _config);
                    _orphansList = new AspDynamicList()
                    {
                        Provider = _orphansListProvider,
                        Title = orphansTitle,
                        DateDisplayType = DateDisplayTypes.None,                        
                        WrapInApplicationPanel = true
                    };

                    _salesList.List.DataBinding += _salesList_DataBinding;

                    pchOrphansDate.Controls.Add(_orphansList);
                }
                #endregion

                if (!Page.IsPostBack)
                {                    
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
            }

            base.OnInit(e);
        }
        
        void _salesList_DataBinding(object sender, EventArgs e)
        {
            if (_config.ShowOrphanGrid)
            {
                TxnHeaderListProvider txnProvider = _salesListProvider as TxnHeaderListProvider;
                if (txnProvider != null)
                {
                    _orphansListProvider.SetSearchParm("SearchParms", txnProvider.GetSearchParms());
                    _orphansList.Rebind();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (_grid != null)
            {
                _grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                _grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                _grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
            }
            else if (_salesList != null)
            {
                _salesList.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                _salesList.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                _salesList.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                if (!IsPostBack)
                {
                    _salesList.Rebind();                    
                }
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
                _config = ConfigurationUtil.GetConfiguration<CFAccountActivityConfig>(ConfigurationKey);
                if (_config == null)
                {
                    _config = new CFAccountActivityConfig();
                    ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
                }
            }
        }
        #endregion
    }
}
