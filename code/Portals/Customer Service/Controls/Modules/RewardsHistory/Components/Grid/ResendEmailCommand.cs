//-----------------------------------------------------------------------
//(C) 2013 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.RewardsHistory.Components.Grid
{
    internal class ResendEmailCommand : ICustomGridAction
    {
        #region Fields
        private string _name = "Resend Email";
        private string _cssClass = "resend";
        private string _ctrId = "resend_email";
        private string _text;
        private DefaultGridProvider _grdProvider = null;
        #endregion

        internal ResendEmailCommand(DefaultGridProvider grdProvider)
        {
            _grdProvider = grdProvider;
            _text = ResourceUtils.GetLocalWebResource(_grdProvider.ParentControl, "CommandResendEmail.Text", "Resend Email");
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
        }
        #endregion

    }
}