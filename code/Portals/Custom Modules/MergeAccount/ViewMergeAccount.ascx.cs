// <copyright file="ViewMergeAccount.ascx.cs" company="B+P">
// Copyright (c) B+P. All rights reserved.
// </copyright>
namespace Brierley.AEModules.MergeAccount
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI.WebControls;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;


    using Brierley.AEModules.MergeAccount.Components;
    //LW 4.1.14 changes
    //using Brierley.DNNModules.PortalModuleSDK;
    //using Brierley.DNNModules.PortalModuleSDK.Controls;

    using Brierley.WebFrameWork.Controls;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.ClientDevUtilities.LWGateway;

    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //LW 4.1.14 changes
    //using DotNetNuke.Entities.Modules;
    //using DotNetNuke.Entities.Modules.Actions;
    //using DotNetNuke.Services.Localization;
    #endregion

    /// <summary>
    /// View merge account
    /// </summary>
    public partial class ViewMergeAccount : ModuleControlBase, IIpcEventHandler
    {
        #region Member Variables
        /// <summary>
        /// Logger for log trace, debug or error information
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("MergeAccount");

        /// <summary>
        /// Hold History grid 
        /// </summary>        
        private AspDynamicGrid historyGrid = null;

        /// <summary>
        /// Data service to deal with data
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// Merge account configuration
        /// </summary>
        //private MergeAccountConfig config = null;

        /// <summary>
        /// Member to merge
        /// </summary>
        private Member toMember = null;

        /// <summary>
        /// Member detail to merge
        /// </summary>
        private MemberDetails toMemberDetails = null;

        #endregion

        //LW 4.1.14 change
        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        public void HandleEvent(IpcEventInfo info)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Handling IPC event: " + info.EventName);
        }

        #region Protected Members

        /// <summary>
        /// Page load event to set control properties
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Page load event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                this.toMember = WebUtilities.GetLoyaltyMemberFromCache();
                // added for redesign project 
                this.toMemberDetails = Merge.GetMemberDetails(this.toMember);

                this.SetControlProperties();
                this.LoadMergeHistryGrid();

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Unexpected Exception", ex);
                //AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert(ex.Message);
                ShowNegative(ex.Message);
                //AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            }
        }

        #endregion

        #region Private Members


        /// <summary>
        /// Set control properties
        /// </summary>
        private void SetControlProperties()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                lblMergeHistoryBanner.Text = "Merge History";
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Get cached member's primary loyalty id
        /// </summary>
        /// <returns>primary loyalty id of member</returns>
        private string GetCachedMemberPrimaryLoyaltyID()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string loyaltyIdNumber = string.Empty;
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                if (this.toMember != null)
                {
                    VirtualCard virtualCard = this.toMember.GetLoyaltyCardByType(FrameWork.Common.VirtualCardSearchType.PrimaryCard);
                    if (null != virtualCard)
                    {
                        loyaltyIdNumber = virtualCard.LoyaltyIdNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return loyaltyIdNumber;
        }

        /// <summary>
        /// Check whether user is AE member or not
        /// </summary>
        /// <param name="memberDetails">Member details of member to check</param>
        /// <returns>true: AE member false: not a AE member</returns>
        private string IsAEMember(MemberDetails memberDetails)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string returnValue = "No";
            if (null != memberDetails && null != memberDetails.MemberSource && memberDetails.MemberSource.HasValue && ((memberDetails.MemberSource == (int)MemberSource.OnlineAEEnrolled) || (memberDetails.MemberSource == (int)MemberSource.OnlineAEEnrolled)))
            {
                returnValue = "Yes";
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return returnValue;
        }

        /// <summary>
        /// Load merge history grid with member merge history table
        /// </summary>
        private void LoadMergeHistryGrid()
        {
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                if (toMember != null)
                {
                    LWCriterion criteria = new LWCriterion("MemberMergeHistory");
                    criteria.Add(LWCriterion.OperatorType.AND, "IpCode", this.toMember.IpCode, LWCriterion.Predicate.Eq);
                    criteria.Add(LWCriterion.OperatorType.AND, "FromLoyaltyID", this.GetCachedMemberPrimaryLoyaltyID(), LWCriterion.Predicate.Ne); //AEO-1341
                                                                                                                                                   //List<MemberMergeHistory> lstHistoryRecords = this.service.GetAttributeSetObjects(null, "MemberMergeHistory", criteria, new LWQueryBatchInfo(), false).Cast<MemberMergeHistory>().ToList();  // AEO-74 Upgrade 4.5 here -----------SCJ
                    List<MemberMergeHistory> lstHistoryRecords = service.GetAttributeSetObjects(null, "MemberMergeHistory", criteria, null, false).Cast<MemberMergeHistory>().ToList();
                    this.phMergeHistory.Controls.Clear();
                    if (null != lstHistoryRecords && lstHistoryRecords.Count > 0)
                    {
                        this.historyGrid = new AspDynamicGrid();
                        this.historyGrid.Provider = (IDynamicGridProvider)(new MergeAccountGridProvider());
                        this.historyGrid.SetSearchParm(string.Empty, lstHistoryRecords);
                        this.phMergeHistory.Controls.Add(this.historyGrid);
                    }
                    else
                    {
                        Label lblNoMerge = new Label();
                        lblNoMerge.Text = "This account has never been merged.";
                        this.phMergeHistory.Controls.Add(lblNoMerge);
                    }
                }

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
        }
        #endregion
    }
}
