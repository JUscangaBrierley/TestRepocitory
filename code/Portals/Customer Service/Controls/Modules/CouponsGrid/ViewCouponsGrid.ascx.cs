using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;

using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;

using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.LWModules.CouponsGrid.Components;


namespace Brierley.LWModules.CouponsGrid
{
	public partial class ViewCouponsGrid : ModuleControlBase
    {
        #region Fields
        private const string _className = "ViewCouponsGrid";
		private const string _modulePath = "~/Controls/Modules/CouponsGrid/ViewCoupons.ascx";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private CouponsGridConfig _config = new CouponsGridConfig();
		private Member _member = null;

        private AspDynamicGrid _grid = null;
        private IDynamicGridProvider _gridProvider = null;

        
        private const int _maxDateRanges = 500;
        #endregion

		protected override void OnInit(EventArgs e)
		{
            string methodName = "OnInit";

            try
            {
                _member = PortalState.CurrentMember;
                if (_member == null)
                {
                    this.Visible = false;
                    _logger.Trace(_className, methodName, "No member selected");
                    return;
                }

                _config = ConfigurationUtil.GetConfiguration<CouponsGridConfig>(ConfigurationKey);
                if (_config == null)
                {
                    return;
                }

                string listTitle = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);

                if (string.IsNullOrEmpty(_config.ProviderAssemblyName) || string.IsNullOrEmpty(_config.ProviderClassName))
                {
                    _gridProvider = new CouponsGridProvider();
                }
                else
                {
                    _gridProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                }
                ((AspGridProviderBase)_gridProvider).ValidationGroup = ValidationGroup;
                ((AspGridProviderBase)_gridProvider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
                _gridProvider.SetSearchParm("Config", _config);
                _gridProvider.SetSearchParm("Member", _member);
                _gridProvider.FilteringEnabled = _config.EnableFiltering;
                _gridProvider.ParentControl = _modulePath;
                _grid = new AspDynamicGrid() { Provider = _gridProvider };

                _grid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(GridActionClickedHandler);

                phCouponsList.Controls.Add(_grid);

                if (!IsPostBack)
                {
                    //litTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.ModuleTitleResourceKey);
                }

                
                base.OnInit(e);
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Empty, ex);
				throw;
            }
		}
        
		protected void Page_Load(object sender, EventArgs e)
		{
			if (_grid != null)
			{
				_grid.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
				_grid.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
				_grid.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
			}
            //else if (_list != null)
            //{
            //    _list.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
            //    _list.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
            //    _list.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
            //    if (!IsPostBack)
            //    {
            //        _list.Rebind();
            //    }
            //}
		}

        //void ListActionClickedHandler(object sender, ListActionClickedArg e)
        //{
        //    if (e.Key != null && e.Command == "Redeem")
        //    {
        //        long id = long.Parse(e.Key.ToString());
        //        CouponUtil.RedeemCoupon(id, 1, null);
        //        _list.Rebind();
        //    }
        //}

        void GridActionClickedHandler(object sender, GridActionClickedArg e)
        {
            if (e.Key != null && e.CommandName == "Redeem")
            {
                long id = long.Parse(e.Key.ToString());
                CouponUtil.RedeemCoupon(id, 1, null);
                _grid.Rebind();
            }            
        }


	}
}
