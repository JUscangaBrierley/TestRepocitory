//-----------------------------------------------------------------------
// <copyright file="ViewMemberTier.ascx.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Brierley.AEModules.MemberTier
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI.WebControls;
    using AmericanEagle.SDK.Global;
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.FrameWork.Common.Exceptions;
    

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
    //using Brierley.DNNModules.MemberTier.Components;
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

    /// <summary>
    /// View section of reward history module
    /// </summary>
    public partial class ViewMemberTier : ModuleControlBase, IIpcEventHandler
    {
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("MemberTier");

        /// <summary>
        /// MemberTierConfig object
        /// </summary>
        //private MemberTierConfig config = null;

        /// <summary>
        /// AspDynamicGrid object
        /// </summary>
        private AspDynamicGrid grid = null;

        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }

        /// <summary>
        /// List for member rewards
        /// </summary>
        private IList<MemberTier> lstMemberTiers = null;

        /// <summary>
        /// ILWDataService object
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// Member object
        /// </summary>
        private Member member = null;

        public void HandleEvent(IpcEventInfo info)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            if ((info.EventName == "RefreshMemberTiers"))
            {
                this.LoadData();
                this.grid.Rebind();
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        #region Public Members

        #endregion

        #region Protected Members

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

                //LW 4.1.14 change
                //IPCManager.RegisterEventHandler("RefreshRewards", this, false);
                IpcManager.RegisterEventHandler("RefreshMemberTiers", this, false);

                pnlMemberTierGrid.Visible = true;

                this.member = WebUtilities.GetLoyaltyMemberFromCache();

                this.lblBannerText.Text = "Customer Tier";
                this.lblInstructionCopy.Text = "Select member Tier from the list below";
                this.pnlBanner.Visible = true;

                this.CheckPermissions(); //AEO-1602

                if (!IsPostBack)
                {
                    this.StartDate.Text = DateTime.Now.ToShortDateString();
                    this.EndDate.Text = "1/1/" + DateTime.Now.AddYears(2).Year;
                    BindDropdown();
                    this.grid = new AspDynamicGrid();
                    this.grid.Provider = new MemberTierGridProvider();
                    LoadData();
                    pnlMemberTierGrid.Controls.Add(this.grid);
                    //this.grid.Rebind();
                }
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Unexpected Exception", ex);
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //ShowAlert("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                ShowNegative("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            }
        }

        /// <summary>
        /// Cancel click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ddlTiers.SelectedIndex = 0;
            txtNotes.Text = string.Empty;
            Response.Redirect(Request.RawUrl);
        }

        /// <summary>
        /// Save click
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin Save");
                    String strAgentNote = txtNotes.Text;
                    Member member = PortalState.GetFromCache("SelectedMember") as Member;

                    //Get Current Tier
                    MemberTier currenTier = member.GetTier(DateTime.Now);


                    //Get new Tier Definition
                    TierDef newTier = dataService.GetTierDef(ddlTiers.SelectedValue);

                    //Validate Start and End Dates
                    DateTime dtStartDate = DateTime.Now;
                    DateTime dtEndDate = DateTime.Now;
                    DateTime dtExpireDate = DateTime.Now;
                    if (!DateTime.TryParse(this.StartDate.Text, out dtStartDate))
                    {
                        throw new LWException("AEMessage|Invalid Start Date used for Tier Change");
                    }
                    if (!DateTime.TryParse(this.EndDate.Text, out dtEndDate))
                    {
                        throw new LWException("AEMessage|Invalid End Date used for Tier Change");
                    }
                    //dtEndDate = dtEndDate.AddHours(11).AddMinutes(59);
                    long MemberNetSpend = 0;
                    //Criteria for MemberDetails
                    LWCriterion critMemberDet = new LWCriterion("MemberDetails");
                    critMemberDet.Add(LWCriterion.OperatorType.AND, "IpCode", member.IpCode, LWCriterion.Predicate.Eq);  //Member's IpCode
                                                                                                                         //Object for Member Details
                    IList<IClientDataObject> objMemberDetails = dataService.GetAttributeSetObjects(null, "MemberDetails", critMemberDet, null, false);
                    MemberDetails memberdet = ((MemberDetails)objMemberDetails[0]); //AEO-1733
                    DateTime dtMaxDate = new DateTime(DateTime.Today.Year + 2, 1, 1);//, 11, 59, 0); 

                    if (dtStartDate < DateTime.Today)
                    {
                        throw new LWException("AEMessage|Invalid Start Date used for Tier Change");
                    }

                    if ((dtEndDate > dtMaxDate) || (dtEndDate <= dtStartDate))
                    {
                        throw new LWException("AEMessage|Invalid End Date used for Tier Change");
                    }
                    //After making validations we set the timestamp if the start date is today, if it is a future tier we retaint the 12 AM time
                    dtStartDate = dtStartDate == DateTime.Today ? DateTime.Now : dtStartDate.AddMinutes(1);

                    //We determine if 
                    if (objMemberDetails != null)
                    {
                        //MemberDetails memberdet = ( (MemberDetails)objMemberDetails[0] );

                        if (memberdet.NetSpend.HasValue)
                        {
                            MemberNetSpend = memberdet.NetSpend.Value;
                        }
                    }

                    LWCriterion crit = new LWCriterion("RefTierReason");
                    crit.Add(LWCriterion.OperatorType.AND, "ReasonCode", 5, LWCriterion.Predicate.Eq);  //Reason code for tier changes from CSPortal
                    IList<IClientDataObject> objRefTiersReason = dataService.GetAttributeSetObjects(null, "RefTierReason", crit, null, false);

                    if (objRefTiersReason != null)
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Adding Tier");
                        RefTierReason RefTierReason = (RefTierReason)objRefTiersReason[0];
                        if (objRefTiersReason.Count > 0)
                        {
                            long newTierId = member.AddTier(newTier.Name, dtStartDate, dtEndDate, RefTierReason.Description);

                            //Expire future tiers
                            IEnumerable<MemberTier> overrideList = member.GetTiers().Where(t => t.ToDate > dtStartDate);

                            foreach (var tierToExpire in overrideList)
                            {
                                if (tierToExpire.Id != newTierId)
                                {
                                    tierToExpire.ToDate = dtExpireDate;
                                    dataService.UpdateMemberTier(tierToExpire);
                                }
                            }

                            MemberNetSpend memberNetSpendToAdd = new MemberNetSpend();

                            //Add Member Net Spend record
                            memberNetSpendToAdd.ChangedBy = PortalState.GetLoggedInCSAgent().Username;
                            memberNetSpendToAdd.CreateDate = DateTime.Now;
                            memberNetSpendToAdd.UpdateDate = DateTime.Now;
                            memberNetSpendToAdd.NetSpend = MemberNetSpend;
                            memberNetSpendToAdd.MemberTierID = newTierId;
                            //memberNetSpendToAdd.IpCode = 
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Added Member NetSpend");
                            member.AddChildAttributeSet(memberNetSpendToAdd);

                            //AEO-1733 | Tier
                            memberdet.AITUpdate = true;
                            //AEO-4651 changedby not updated when tier is upgraded
                            memberdet.ChangedBy = "TierNomination";
                            member.AddChildAttributeSet(memberdet);

                            dataService.SaveMember(member);
                        }
                        //Create System text and attach note to csportal.
                        string strCurrentTier = currenTier != null ? currenTier.TierDef.Name : "None";
                        string strSystem = "Tier Change from " + strCurrentTier;
                        strSystem += " to " + newTier.Name;

                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Adding Note");

                        CreateStatusNote(member, strAgentNote, strSystem);
                    }
                    //**************************//
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End Save");
                    lblSuccess.Text = string.Empty;
                    ddlTiers.SelectedIndex = 0;
                    txtNotes.Text = string.Empty;

                    //Go to Previous page
                    Response.Redirect(Request.RawUrl);
                    //}
                }
            }
            catch(LWException ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                ShowNegative(ex.Message);
            }

            catch(Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                ShowNegative("An error has occured");
            }
        }

        private void CreateStatusNote(Member member, string strAgentNote, string strSystem)
        {
            string _note = strSystem + " - " + strAgentNote;
            using (var ilwcsservice = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                CSNote note = new CSNote();
                note.Note = _note;
                note.MemberId = member.IpCode;
                note.CreateDate = DateTime.Now;
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                ilwcsservice.CreateNote(note);
            }
        }

        #endregion

        #region Private Members

        /// <summary>1
        /// Bind dropdown with tiers
        /// </summary>
        private void BindDropdown()
        {
            using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                List<ListItem> lc = new List<ListItem>();
                // Display possible Tiers.
                MemberTier currenTier = member.GetTier(DateTime.Now);
                if (member.MemberStatus == MemberStatusEnum.Active)
                {
                    IList<TierDef> allTiers = new List<TierDef>();

                    if (currenTier != null)
                    {
                        if (currenTier.TierDef.Name == "Full Access")
                        {
                            allTiers.Add(dataService.GetTierDef("Extra Access"));
                        }
                        else
                        {
                            allTiers.Add(dataService.GetTierDef("Full Access"));
                        }
                    }
                    else
                    {
                        allTiers.Add(dataService.GetTierDef("Extra Access"));
                        allTiers.Add(dataService.GetTierDef("Full Access"));
                    }

                    lc.Add(new ListItem("--SELECT--", "0"));

                    foreach (var tier in allTiers)
                    {
                        lc.Add(new ListItem(tier.Name, tier.Id.ToString()));
                    }
                    //Add all valida tiers to the DropDown List
                    this.ddlTiers.DataSource = lc;
                    this.ddlTiers.DataBind();
                }
                else
                {
                    DisableControls();
                }

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
        }

        private void DisableControls()
        {
            ddlTiers.Enabled = false;
            this.StartDate.Enabled = false;
            this.EndDate.Enabled = false;
            this.txtNotes.Enabled = false;
        }

        /// <summary>
        /// Method for loading the data in Grid
        /// </summary>
        private void LoadData()
        {
            using (var dataService = _dataUtil.LoyaltyDataServiceInstance())
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Load Member Tiers History"));
                IList<MemberTier> memberTiers = dataService.GetMemberTiers(member);

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Order Member Tiers"));
                this.lstMemberTiers = memberTiers.OrderByDescending(x => x.CreateDate).ToList();
                if (null != this.lstMemberTiers && this.lstMemberTiers.Count > 0)
                {
                    logger.Debug(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Member Tiers returned {0} rewards", this.lstMemberTiers.Count));
                    this.grid.SetSearchParm("MemberTiers", this.lstMemberTiers);
                }
                else
                {
                    pnlMemberTierGrid.Visible = true;
                    logger.Debug(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No members tiers returned.");
                }

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
        }

        /// <summary>
        /// Method that Remove the ability to submit changes
        /// </summary>
        private void CheckPermissions()
        {            
            //AEO-1644 Begin
            Boolean AllowSubmit = WebUtilities.isMemberAllowedToSubmit();
            if (AllowSubmit)
            {
                this.btnSave.Visible = false;
                this.btnCancel.Visible = false;                
            }//AEO-1644 End
            else
            {
                //AEO-1602 Begin
                Boolean AllowOnlyView = WebUtilities.isRoleAllowedOnlyToView("ViewMemberTier");
                if (AllowOnlyView)
                {
                    this.btnSave.Visible = false;
                }
                //AEO-1602 End
            }            
        }

        #endregion
    }
}
