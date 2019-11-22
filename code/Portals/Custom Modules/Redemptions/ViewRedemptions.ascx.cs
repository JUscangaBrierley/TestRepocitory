namespace Brierley.AEModules.Redemptions
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI.WebControls;
    using AmericanEagle.SDK.Global;
    using Brierley.Clients.AmericanEagle.DataModel;


    //LW 4.1.14 change
    //using Brierley.DNNModules.PortalModuleSDK;
    //using Brierley.DNNModules.PortalModuleSDK.Controls;
    //using Brierley.DNNModules.PortalModuleSDK.ipc;

    using Brierley.WebFrameWork.Controls;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;

    //LW 4.1.14 change
    //using Brierley.DNNModules.RewardHistory.Components;
    using Brierley.FrameWork;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.FrameWork.Rules;
    using Brierley.FrameWork.bScript;
    using Brierley.ClientDevUtilities.LWGateway;

    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyNavigator.Controls.Grid.AspGrid;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    //LW 4.1.14 change
    //using DotNetNuke.Entities.Modules;
    //using DotNetNuke.Entities.Modules.Actions;
    //using DotNetNuke.Services.Localization;
    #endregion

    public partial class ViewRedemptions : ModuleControlBase, IIpcEventHandler
    {
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("Redemptions");

        /// <summary>
        /// RewardHistoryConfig object
        /// </summary>
        //private RewardHistoryConfig config = null;

        /// <summary>
        /// AspDynamicGrid object
        /// </summary>
        private AspDynamicGrid grid = null;

        /// <summary>
        /// List for reward redemptions
        /// </summary>
        private IList<RewardRedemption> lstRewardRedeems = null;

        /// <summary>
        /// ILWDataService object
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// Member object
        /// </summary>
        private Member member = null;

        //LW 4.1.14 change
        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        public void HandleEvent(IpcEventInfo info)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if ((info.EventName == "Redemptions"))
            {
                this.LoadData(this.SearchStartDate, this.SearchEndDate);
                this.grid.Rebind();
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        #region Public Members

        #endregion

        #region Protected Members

        #region Search Dates

        /// <summary>
        /// Gets or sets start date search criteria
        /// </summary>
        protected DateTime SearchStartDate
        {
            get
            {
                if (ViewState["SearchStartDate"] != null)
                {
                    return (DateTime)ViewState["SearchStartDate"];
                }
                else
                {
                    return DateTime.Now;
                }
            }

            set
            {
                ViewState["SearchStartDate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets end date search criteria
        /// </summary>
        protected DateTime SearchEndDate
        {
            get
            {
                if (ViewState["SearchEndDate"] != null)
                {
                    return (DateTime)ViewState["SearchEndDate"];
                }
                else
                {
                    return DateTime.Now;
                }
            }

            set
            {
                ViewState["SearchEndDate"] = value;
            }
        }

        #endregion

        /// <summary>
        /// Method for page loading
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                IpcManager.RegisterEventHandler("Redemptions", this, false);
                using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    lblInstructionalReplacement.Text = string.Empty;
                    pnlRedemptionsGrid.Visible = false;
                    this.member = WebUtilities.GetLoyaltyMemberFromCache();
                    this.lblBannerText.Text = "Redemption History";
                    this.lblInstructionCopy.Text = "Redemption activity during Quarter dates:";
                    this.pnlBanner.Visible = true;

                    if (!IsPostBack)
                    {
                        BindDropdown();
                    }

                    if (this.ddlDate.Items.Count > 0)
                    {
                        this.grid = new AspDynamicGrid();
                        this.grid.Provider = new RedemptionsGridProvider();
                        SetSearchCriteria();
                        LoadData(this.SearchStartDate, this.SearchEndDate);
                        pnlRedemptionsGrid.Controls.Add(this.grid);
                    }
                    else
                    {
                        this.lblInstructionCopy.Visible = false;
                    }
                    if (this.grid.Provider.GetNumberOfRows() == 0)
                    {
                        this.lblInstructionalReplacement.Text = "No Redemption History Found";
                    }

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Unexpected Exception", ex);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }

        private void LoadGrid(bool displayGrid)
        {

        }

        /// <summary>
        /// Event for dropdown list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void DdlDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            this.LoadData(this.SearchStartDate, this.SearchEndDate);
            this.grid.Rebind();
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        #endregion
        #region Private Members
        /// <summary>
        /// Bind dropdown with number of quarters 
        /// </summary>
        private void BindDropdown()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            List<ListItem> lc = new List<ListItem>();
            // Number of quarters those will display in dropdown
            int numQtrs = 13;
            if (numQtrs != 0)
            {
                for (int i = 0; i < numQtrs; i++)
                {
                    lc.Add(DropDownUtilities.GetListItem(DateTime.Now.AddMonths(-i * 3)));
                }

                this.ddlDate.DataSource = lc;
                this.ddlDate.DataBind();
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Method for loading the data in Grid
        /// </summary>
        /// <param name="startDate">DateTime startDate</param>
        /// <param name="endDate">DateTime endDate</param>
         private void LoadData(DateTime startDate, DateTime endDate)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            try
            {
                using (var dataService = _dataUtil.ContentServiceInstance())
                {
                    this.lstRewardRedeems = new List<RewardRedemption>();


                    IList<MemberReward> rewards = member.GetRewards();

                    foreach (MemberReward rew in rewards)
                    {
                        if (rew.LWOrderNumber == "3" && rew.RedemptionDate != null &&
                            DateTime.Compare(rew.DateIssued, startDate) > 0 &&
                            DateTime.Compare(rew.DateIssued, startDate) > 0)
                        {

                            RewardDef definition = dataService.GetRewardDef(rew.RewardDefId);
                            RewardRedemption tmp = new RewardRedemption();
                            tmp.Description = definition.Name;

                            tmp.RedemptionDate = rew.RedemptionDate.ToString();
                            tmp.RedemptionCode = rew.OfferCode + "-" + rew.CertificateNmbr;
                            tmp.TransactionDate = rew.DateIssued.ToString();

                            this.lstRewardRedeems.Add(tmp);
                        }
                    }



                    this.grid.SetSearchParm("RedemptionList", this.lstRewardRedeems);
                    pnlRedemptionsGrid.Visible = true;
                }
            }
            catch (Exception)
            {
                pnlRedemptionsGrid.Visible = false;
                logger.Debug(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No reward redemptions returned.");
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        
        /// <summary>
        /// Method for setting search criteria
        /// </summary>
        private void SetSearchCriteria()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string[] stringSeparators = new string[] { "to" };
            string qtr = this.ddlDate.SelectedItem.Value;
            string[] arrQtr = qtr.Split(stringSeparators, StringSplitOptions.None);
            this.SearchStartDate = Convert.ToDateTime(arrQtr[0].Trim());
            this.SearchEndDate = Convert.ToDateTime(arrQtr[1].Trim());
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        #endregion
    }
}