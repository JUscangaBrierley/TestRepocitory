using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.LoyaltyCardsGrid.Components
{
    internal class CardPrimaryCommand : ICustomGridAction
    {
        #region Fields
        private string _name = "Primary";
        private string _cssClass = "primary";
        private string _ctrId = "primary_card";
        private string _text;
        private DefaultGridProvider _grdProvider = null;
        #endregion

        internal CardPrimaryCommand(DefaultGridProvider grdProvider)
		{
			_grdProvider = grdProvider;
            _text = ResourceUtils.GetLocalWebResource(_grdProvider.ParentControl, "CommandPrimary.Text", "Primary");
		}

        #region Properties
        public string CommandName
        {
            get { return _name; }
        }
        public string CssClass
        {
            get { return _cssClass; }
        }
        public string ControlId
        {
            get { return _ctrId; }
        }
        public string Text
        {
            get { return _text; }
        }
        #endregion

        #region Methods
        public void HandleCommand(System.Web.UI.Page page, object key)
        {
            if (_grdProvider.Member != null)
            {
            }
            else
            {
                
            }
        }
        #endregion

    }
}