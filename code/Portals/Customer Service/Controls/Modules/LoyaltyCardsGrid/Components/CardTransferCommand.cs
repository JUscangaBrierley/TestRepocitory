using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.LoyaltyCardsGrid.Components
{
    internal class CardTransferCommand : ICustomGridAction
    {
        #region Fields
        private string _name = "Transfer";
        private string _cssClass = "transfer";
        private string _ctrId = "transfer_card";
        private string _text;
        private DefaultGridProvider _grdProvider = null;
        #endregion

        internal CardTransferCommand(DefaultGridProvider grdProvider)
		{
			_grdProvider = grdProvider;
            _text = ResourceUtils.GetLocalWebResource(_grdProvider.ParentControl, "CommandTransfer.Text", "Transfer");
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
            //long attCode = long.Parse(key.ToString());
            //_grdProvider.OnTemplateFiledsClicked(attCode);
        }
        #endregion

    }
}