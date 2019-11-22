﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.RewardsHistory.Components.Grid
{
    internal class CancelRewardCommand : ICustomGridAction
    {
        #region Fields
        private string _name = "Cancel Reward";
        private string _cssClass = "cancel";
        private string _ctrId = "cancel_reward";
        private string _text;
        private DefaultGridProvider _grdProvider = null;
        #endregion

        internal CancelRewardCommand(DefaultGridProvider grdProvider)
		{
			_grdProvider = grdProvider;
            _text = ResourceUtils.GetLocalWebResource(_grdProvider.ParentControl, "CommandCancelReward.Text", "Cancel Reward");
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