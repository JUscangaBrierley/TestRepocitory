//-----------------------------------------------------------------------
// <copyright file="ViewRewardHistory.ascx.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Brierley.AEModules.RewardHistory
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI.WebControls;    
    using Brierley.Clients.AmericanEagle.DataModel;    
    using Brierley.WebFrameWork.Controls;
    using Brierley.WebFrameWork.Controls.Grid;
    using Brierley.WebFrameWork.Portal.Configuration.Modules;
    using Brierley.WebFrameWork.Portal;
    using Brierley.WebFrameWork.Portal.Configuration;
    using Brierley.WebFrameWork.Ipc;
    using Brierley.FrameWork;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.FrameWork.Rules;
    using Brierley.FrameWork.bScript;
    using Brierley.ClientDevUtilities.LWGateway;
    using AmericanEagle.SDK.Global;
    #endregion

    /// <summary>
    /// View section of reward history module
    /// </summary>
    public partial class ViewRewardHistory : ModuleControlBase, IIpcEventHandler
    {
        //(Begin)[AEO-2372]->[Dev Jonatan Uscanga]
        protected string MemberRewardsPortalStateKey { get { return "MemberRewardsList"; } }

        //(End)[AEO-2372]

        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("RewardHistory");

        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630

        /// <summary>
        /// RewardHistoryConfig object
        /// </summary>
        //private RewardHistoryConfig config = null;

        /// <summary>
        /// AspDynamicGrid object
        /// </summary>
        private AspDynamicGrid grid = null;

        /// <summary>
        /// List for member rewards
        /// </summary>
        private IList<MemberReward> lstMemberRewards = null;
        
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
            if ((info.EventName == "RefreshRewards"))
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

                IpcManager.RegisterEventHandler("RefreshRewards", this, false);
                pnlRewardHistoryGrid.Visible = false;

                this.member = WebUtilities.GetLoyaltyMemberFromCache();

                this.lblBannerText.Text = "Reward History";
                this.pnlBanner.Visible = true;

                if (!IsPostBack)
                {
                    BindDropdown();
                }

                if (this.ddlDate.Items.Count > 0)
                {
                    SetSearchCriteria();
                    this.grid = new AspDynamicGrid();
                    this.grid.Provider = new RewardHistoryGridProvider();
                    this.grid.GridActionClicked += new AspDynamicGrid.GridActionClickedHandler(this.Grid_GridActionClicked);

                    if (IsPostBack && PortalState.GetFromCache(this.MemberRewardsPortalStateKey) != null)
                        this.ReLoadData();
                    else
                        this.LoadData(this.SearchStartDate, this.SearchEndDate);

                    pnlRewardHistoryGrid.Controls.Add(this.grid);
                    //(End)[AEO-2372]
                }
                else
                {
                    lblInstructionalReplacement.Visible = true;
                    this.lblInstructionalReplacement.Text = "No History Copy";
                }

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Unexpected Exception", ex);
                ShowNegative("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
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

        /// <summary>
        /// Method for replace reward
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">GridActionClickedArg e</param>
        protected void Grid_GridActionClicked(object sender, GridActionClickedArg e)
        {
            
            if (e.CommandName == AspDynamicGrid.SELECT_COMMAND)
            {
                long rewardID = 0;
                string memberRewardPartNumber = string.Empty;

                try
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

                    EmailType emailType = EmailType.OnlineReward;
                    string strRewardType = string.Empty;
                    string strCouponCode = string.Empty;
                    long memberQuarterlyRewardId =  0;
                    string key = string.Empty;

                    key = Convert.ToString(e.Key);

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "key: " + key);

                    memberQuarterlyRewardId = Int64.Parse(key);

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "memberQuarterlyRewardId: " + memberQuarterlyRewardId.ToString());

                    using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                    using (IContentService contentService = _dataUtil.ContentServiceInstance())
                    {
                        MemberReward memberReward = dataService.GetMemberReward(memberQuarterlyRewardId);

                        RewardDef reward = contentService.GetRewardDef(memberReward.RewardDefId);
                        if (reward == null)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward (" + memberReward.RewardDefId.ToString() + ") not defined");
                        }
                        else
                        {
                            rewardID = reward.Id;
                        }

                        ContextObject contextObj = new ContextObject();
                        contextObj.Owner = this.member;
                        string ruleName = reward.Name.Replace("Reward", "Replacement");
                        contextObj.InvokingRow = this.member.GetChildAttributeSets("MemberDetails")[0];
                        IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
                        MemberDetails mbrDetails = (MemberDetails)memberDetails[0];

                        RuleTrigger ruleTrigger = dataService.GetRuleByName(ruleName);
                        if (ruleTrigger == null)
                        {
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " Rule Not Defined");
                            throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                        }

                        string userName = string.Format("'{0}'", WebUtilities.GetCurrentUserName());

                        IssueReward issueRewardRule = (IssueReward)ruleTrigger.Rule;
                        issueRewardRule.ChangedByExpression = userName;

                        long memberRewardID = 0;
                        dataService.Execute(ruleTrigger, contextObj);

                        memberReward = dataService.GetMemberReward(memberRewardID);

                        if (memberReward != null)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Got Member Reward: " + memberRewardID.ToString());
                            if (memberReward.CertificateNmbr != null && memberReward.CertificateNmbr.Length > 0)
                            {
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward CertificateNmbr: " + memberReward.CertificateNmbr);
                                LWCriterion crit = new LWCriterion("RewardBarCodes");
                                crit.Add(LWCriterion.OperatorType.AND, "BarCode", memberReward.CertificateNmbr, LWCriterion.Predicate.Eq);
                                var rewardBarCodes = dataService.GetAttributeSetObjects(null, "RewardBarCodes", crit, null, true);

                                RewardBarCodes barcodes = (RewardBarCodes)rewardBarCodes[0];
                            }

                            strRewardType = ruleName.Replace(" - Replacement", null);
                            emailType = EmailType.OnlineReward;
                            Product product = contentService.GetProduct(memberReward.ProductId);

                            if (product != null)
                            {
                                memberRewardPartNumber = product.PartNumber;
                            }
                            strCouponCode = memberReward.CertificateNmbr;

                            AEEmail.SendRewardEmail(member, strRewardType, strCouponCode, emailType, mbrDetails.EmailAddress, string.Empty);

                            Utilities.AddMemberRewardFulfillment(member, memberRewardID, "Replacement", memberQuarterlyRewardId, strRewardType, memberRewardPartNumber);

                        }
                    }
                    ////**************************//
                    ////Adding entry in CSNotes table
                    CSNote note = new CSNote();
                    note.Note = "Reward Replaced";
                    note.MemberId = this.member.IpCode;
                    note.CreatedBy = WebUtilities.GetCurrentUserId();
                    using (CSService inst = FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                    {
                        inst.CreateNote(note);
                    }
                    ////**************************//

                    this.LoadData(this.SearchStartDate, this.SearchEndDate);
                    this.grid.Rebind();
                    ////Send a mail to member account
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                }
                catch (Exception ex)
                {
                    ShowNegative(ex.Message);
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
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

            // Number of months those will display in dropdown
            int numMonths = 13;
            DateTime baseDate, tempDate;
            DateTime.TryParse(string.Format("{0}/{1}/{2}", DateTime.Now.Month, 1, DateTime.Now.Year), out baseDate);

            for (int i = 0; i < numMonths; i++)
            {
                tempDate = baseDate.AddMonths(-i);
                lc.Add(new ListItem(tempDate.ToString("MMMM") + ' ' + tempDate.Year.ToString()));
            }

            this.ddlDate.DataSource = lc;
            this.ddlDate.DataBind();

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
                this.lstMemberRewards = new List<MemberReward>();
                if ((startDate != DateTime.MinValue) && (endDate > startDate) && (member != null))
                {
                    using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Load Member Rewards"));
                        IList<long> rewardIds = dataService.GetMemberRewardIds(member, null, SearchStartDate, SearchEndDate, null, false, false);
                        long[] rewardIDs = new long[rewardIds.Count];
                        int counter = 0;
                        foreach (long id in rewardIds)
                        {
                            rewardIDs[counter] = id;
                            ++counter;
                        }

                        IList<MemberReward> allreward = null;
                        if (rewardIds != null)
                        {

                            //AEO-2076
                            if (rewardIds.Count > 0)
                                allreward = dataService.GetMemberRewards(member, null);
                            if (allreward != null)
                                if (allreward.Count > 0)
                                    foreach (MemberReward mr in allreward)
                                    {
                                        foreach (long id in rewardIds)
                                        {
                                            if (id == mr.Id)
                                            {
                                                this.lstMemberRewards.Add(mr);
                                            }
                                        }
                                    }
                        }
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Order Member Rewards"));

                        //  aeo 2076            
                        if (this.lstMemberRewards != null)
                            if (this.lstMemberRewards.Count > 0)
                                this.lstMemberRewards = this.lstMemberRewards.OrderByDescending(x => x.DateIssued).ToList();
                        //(Begin)[AEO-2372]->[Dev Jonatan Uscanga]
                        PortalState.PutInCache(this.MemberRewardsPortalStateKey, this.lstMemberRewards);
                        lblInstructionalReplacement.Text = string.Empty;
                        lblInstructionalReplacement.Visible = false;
                        //(End)[AEO-2372]
                        if (null != this.lstMemberRewards && this.lstMemberRewards.Count > 0)
                        {
                            logger.Debug(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Member Reward returned {0} rewards", this.lstMemberRewards.Count));
                            this.grid.SetSearchParm("RewardList", this.lstMemberRewards);
                            pnlRewardHistoryGrid.Visible = true;

                            DateTime qtrStartDate;
                            DateTime qtrEndDate;
                            Utilities.GetProgramDates(member, out qtrStartDate, out qtrEndDate); // PI 30364 - Dollar reward program 

                            foreach (MemberReward mbrReward in this.lstMemberRewards)
                            {
                                if (mbrReward.DateIssued > qtrStartDate && mbrReward.DateIssued < qtrEndDate)
                                {
                                    if (mbrReward.LWOrderNumber != RewardStatus.Merged.ToString())
                                    {
                                        if (mbrReward.RedemptionDate != null)
                                        {
                                            if (WebUtilities.HasRights("ReplaceRewards"))
                                            {
                                                lblInstructionalReplacement.Visible = true;
                                                lblInstructionalReplacement.Text = "Click the 'Replace' button to send a replacement reward to the member’s email address.";
                                                break;
                                            }
                                            else
                                            {
                                                lblInstructionalReplacement.Visible = true;
                                                lblInstructionalReplacement.Text = "This member has already received an online Reward replacement during this quarter. A reward may only be replaced once per quarter";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lblInstructionalReplacement.Text = string.Empty;
                                        lblInstructionalReplacement.Visible = false;
                                    }
                                }
                                else
                                {
                                    lblInstructionalReplacement.Text = string.Empty;
                                    lblInstructionalReplacement.Visible = false;
                                }
                            }
                        }
                        else
                        {
                            pnlRewardHistoryGrid.Visible = false;
                            logger.Debug(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No members reward returned.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        //(Begin)[AEO-2372]->[Dev Jonatan Uscanga]
        private void ReLoadData()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            this.lstMemberRewards = (IList<MemberReward>)PortalState.GetFromCache(this.MemberRewardsPortalStateKey);
            if (null != this.lstMemberRewards && this.lstMemberRewards.Count > 0)
            {
                this.grid.SetSearchParm("RewardList", this.lstMemberRewards);
                pnlRewardHistoryGrid.Visible = true;
            }
            else
            {
                pnlRewardHistoryGrid.Visible = false;
                logger.Debug(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No members reward returned.");
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");

        }
        //(End)[AEO-2372]

        /// <summary>
        /// Method for setting search criteria
        /// </summary>
        private void SetSearchCriteria()
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            string[] selected = this.ddlDate.SelectedItem.Value.Split(' '); // {MonthName, Year}
            string dateString = string.Format("{0}/{1}/{2}", selected[0], 1, selected[1]);
            DateTime tempDate;
            DateTime.TryParse(dateString, out tempDate);
            this.SearchStartDate = tempDate;

            this.SearchEndDate = Convert.ToDateTime(string.Format("{0}/{1}/{2}", this.SearchStartDate.Month,
                                                                                 DateTime.DaysInMonth(this.SearchStartDate.Year,
                                                                                 this.SearchStartDate.Month), this.SearchStartDate.Year));
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        #endregion
    }
}
